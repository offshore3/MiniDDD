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

namespace MiniDDD.Storage.Installers
{
    public class MiniDDDInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var storeage =
                new SqlServerEventStorage(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);

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
