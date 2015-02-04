using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using MiniDDD.Events.EventUtils;
using MiniDDD.Storage.UnitOfWork;

namespace MiniDDD.Storage.Installers
{
    public class MiniDDDInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Classes.FromThisAssembly().BasedOn(typeof(IEventHandler<>))
                .LifestylePerWebRequest().WithServiceAllInterfaces());

            container.Register(Component.For<IEventHandlerFactory>()
                .ImplementedBy<CastleEventHandlerFactory>().LifestyleSingleton(),
                Component.For<IEventBus>()
                .ImplementedBy<EventBus>().LifestyleSingleton());

            var storeage =
                new SqlServerEventStorage(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString,container.Resolve<IEventBus>());

            container.Register(
                Component.For<IEventStorage>().Instance(storeage).LifestylePerThread(),
                Component.For<IUnitOfWorkFactory>().ImplementedBy<UnitOfWorkFactory>(),
                Component.For<IEventStorageProvider>().ImplementedBy<EventStorageProvider>(),
                Component.For(typeof(IRepository<>)).ImplementedBy(typeof(Repository<>)),
                Component.For<IUnitOfWork>().ImplementedBy<SqlServerUnitOfWork>().LifestylePerThread()
                );
        }
    }
}
