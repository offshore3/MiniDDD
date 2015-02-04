using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using MiniDDD.Domain;
using MiniDDD.Events;
using MiniDDD.Extensions;
using Newtonsoft.Json;

namespace MiniDDD.Storage
{
    public class SqlServerEventStorage : IEventStorage
    {
        private readonly string _connectionString;
        private const string EventSelectClause = "SELECT EventType, Event, AggregateId, AggregateVersion, EventId, TimeStamp FROM Events With(UPDLOCK,READCOMMITTED, ROWLOCK) ";
        
        private List<AggregateRoot> _pendingAggregateRoots;

        private static object _pendingAggregatelocker = new object();

        public SqlServerEventStorage(string connectionString)
        {
            _connectionString = connectionString;
            _pendingAggregateRoots = new List<AggregateRoot>();
        }

        public IEnumerable<IAggregateRootEvent> GetEvents(Guid aggregateId)
        {
            EnsureEventsTableExists();

            using (var connection = OpenSession(suppressTransactionWarning: true))
            {
                using (var loadCommand = connection.CreateCommand())
                {
                    loadCommand.CommandText = EventSelectClause + "WHERE AggregateId = @AggregateId";
                    loadCommand.Parameters.Add(new SqlParameter("AggregateId", aggregateId));

                    //TODO: add cache later
                    //loadCommand.CommandText += " AND AggregateVersion > @CachedVersion";
                    //loadCommand.Parameters.Add(new SqlParameter("CachedVersion", result.Last().AggregateRootVersion));

                    loadCommand.CommandText += " ORDER BY AggregateVersion ASC";

                    var result = new List<IAggregateRootEvent>();

                    using (var reader = loadCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(ReadEvent(reader));
                        }
                    }

                    return result;
                }
            }
        }

        public void Save(AggregateRoot aggregate)
        {
            List<IAggregateRootEvent> uncommittedChanges = aggregate.GetUncommittedChanges().ToList();
            
            var version = aggregate.Version;

            foreach (var @event in uncommittedChanges)
            {
                version++;
                if (version > 2)
                {
                    // TODO: think a good way for snapshot
                    //if (version%3 == 0)
                    //{
                    //    var originator = (IOriginator) aggregate;
                    //    var memento = originator.GetMemento();
                    //    memento.Version = version;
                    //    SaveMemento(memento);
                    //}
                }

                @event.AggregateRootVersion = version;
            }

            // No Unit of work
            if (Transaction.Current == null)
            {
                EnsureEventsTableExists();

                using (var connection = OpenSession())
                {
                    CommitAggreate(aggregate, connection);
                }

                aggregate.MarkChangesAsCommitted();

                return;
            }

            // in Unit of work

            lock (_pendingAggregatelocker)
            {
                var existAggregateRoot = _pendingAggregateRoots.SingleOrDefault(x => x.Id == aggregate.Id);
                if (existAggregateRoot == null)
                {
                    _pendingAggregateRoots.Add(aggregate);
                }
                else
                {
                    existAggregateRoot = aggregate;
                }
            }
        }

        public void Committing()
        {
            EnsureEventsTableExists();

            using (var connection = OpenSession())
            {
                    foreach (var pendingAggregateRoot in _pendingAggregateRoots)
                    {
                        CommitAggreate(pendingAggregateRoot, connection);
                    }
            }
            
        }

        private static void CommitAggreate(AggregateRoot pendingAggregateRoot, SqlConnection connection)
        {
            foreach (var @event in pendingAggregateRoot.GetUncommittedChanges())
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;

                    command.CommandText +=
                        "INSERT Events With(READCOMMITTED, ROWLOCK) (AggregateId, AggregateVersion, EventType, EventId, TimeStamp, Event) VALUES(@AggregateId, @AggregateVersion, @EventType, @EventId, @TimeStamp, @Event)";

                    command.Parameters.Add(new SqlParameter("AggregateId", @event.AggregateRootId));
                    command.Parameters.Add(new SqlParameter("AggregateVersion", @event.AggregateRootVersion));
                    command.Parameters.Add(new SqlParameter("EventType", @event.GetType().FullName));
                    command.Parameters.Add(new SqlParameter("EventId", @event.EventId));
                    command.Parameters.Add(new SqlParameter("TimeStamp", @event.TimeStamp));

                    command.Parameters.Add(new SqlParameter("Event", JsonConvert.SerializeObject(@event, Formatting.Indented)));

                    command.ExecuteNonQuery();
                }
            }


            foreach (var @event in pendingAggregateRoot.GetUncommittedChanges())
            {
                var desEvent = Converter.ChangeTo(@event, @event.GetType());
                //TODO: publish event?
            }
        }

        public void MarkCommitted()
        {
            foreach (var pendingAggregateRoot in _pendingAggregateRoots)
            {
                pendingAggregateRoot.MarkChangesAsCommitted();
            }

            _pendingAggregateRoots.Clear();
            _pendingAggregateRoots = null;
        }

        private SqlConnection OpenSession(bool suppressTransactionWarning = false)
        {
            var connection = new SqlConnection(_connectionString);
            connection.Open();
            if (!suppressTransactionWarning && Transaction.Current == null)
            {

            }
            return connection;
        }

        private IAggregateRootEvent ReadEvent(SqlDataReader eventReader)
        {
            var @event = DeserializeEvent(eventReader.GetString(0), eventReader.GetString(1));
            @event.AggregateRootId = eventReader.GetGuid(2);
            @event.AggregateRootVersion = eventReader.GetInt32(3);
            @event.EventId = eventReader.GetGuid(4);
            @event.TimeStamp = eventReader.GetDateTime(5);

            return @event;
        }

        private IAggregateRootEvent DeserializeEvent(string eventType, string eventData)
        {
            return (IAggregateRootEvent)JsonConvert.DeserializeObject(eventData, eventType.AsType());
        }

        private static readonly HashSet<string> VerifiedTables = new HashSet<string>();
        private void EnsureEventsTableExists()
        {
            lock (VerifiedTables)
            {
                if (!VerifiedTables.Contains(_connectionString))
                {
                    int exists;
                    using (var _connection = OpenSession())
                    {
                        using (var checkForTableCommand = _connection.CreateCommand())
                        {
                            checkForTableCommand.CommandText = "select count(*) from sys.tables where name = 'Events'";
                            exists = (int)checkForTableCommand.ExecuteScalar();
                        }
                        if (exists == 0)
                        {
                            using (var createTableCommand = _connection.CreateCommand())
                            {
                                createTableCommand.CommandText =
                                    @"
CREATE TABLE [dbo].[Events](
	[AggregateId] [uniqueidentifier] NOT NULL,
	[AggregateVersion] [int] NOT NULL,
	[TimeStamp] [datetime] NOT NULL,
	[SqlTimeStamp] [timestamp] NOT NULL,
	[EventType] [varchar](300) NOT NULL,
	[EventId] [uniqueidentifier] NOT NULL,
	[Event] [nvarchar](max) NOT NULL,
CONSTRAINT [IX_Uniq_EventId] UNIQUE
(
	EventId
),
CONSTRAINT [PK_Events] PRIMARY KEY CLUSTERED 
(
	[AggregateId] ASC,
	[AggregateVersion] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = OFF) ON [PRIMARY]
) ON [PRIMARY]
CREATE UNIQUE NONCLUSTERED INDEX [SqlTimeStamp] ON [dbo].[Events]
(
	[SqlTimeStamp] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
";
                                createTableCommand.ExecuteNonQuery();
                            }
                        }
                        VerifiedTables.Add(_connectionString);
                    }
                }
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
