using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace MiniDDD.Storage.DocumentDB
{
    public interface IDocumentDbSession
    {
        void Save<T>(T document) where T:BaseDocument;
        T GetById<T>(Guid id);
        IEnumerable<T> GetAll<T>() where T:BaseDocument;
    }

    public class BaseDocument
    {
       public  Guid Id { get; set; }
       public string DocumentType { get; set; }
       public object Value { get; set; }

    }

    public class SQLServerDocumentDbSession:IDocumentDbSession
    {
        private readonly string _connectionString;

        public SQLServerDocumentDbSession(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Save<T>(T document) where T : BaseDocument
        {
            throw new NotImplementedException();
        }

        public T GetById<T>(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetAll<T>() where T : BaseDocument
        {
            throw new NotImplementedException();
        }



        private static readonly HashSet<string> VerifiedTables = new HashSet<string>();

        private SqlConnection OpenSession(bool suppressTransactionWarning = false)
        {
            var connection = new SqlConnection(_connectionString);
            connection.Open();
            if (!suppressTransactionWarning && Transaction.Current == null)
            {

            }
            return connection;
        }

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
                            checkForTableCommand.CommandText = "select count(*) from sys.tables where name = 'QueryModels'";
                            exists = (int)checkForTableCommand.ExecuteScalar();
                        }
                        if (exists == 0)
                        {
                            using (var createTableCommand = _connection.CreateCommand())
                            {
                                createTableCommand.CommandText =
                                    @"
CREATE TABLE [dbo].[QueryModels](
	[QueryModelId] [uniqueidentifier] NOT NULL,
    [Key] [uniqueidentifier] NOT NULL,
    [Value] [string] NOT NULL,
	[QueryModelVersion] [int] NOT NULL,
	[TimeStamp] [datetime] NOT NULL,
	[SqlTimeStamp] [timestamp] NOT NULL,
	[QueryModelType] [varchar](300) NOT NULL	
	
CONSTRAINT [IX_Uniq_EventId] UNIQUE
(
	QueryModelId
),
";
                                createTableCommand.ExecuteNonQuery();
                            }
                        }
                        VerifiedTables.Add(_connectionString);
                    }
                }
            }
        }

    }
}
