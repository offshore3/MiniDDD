using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Windsor;
using Castle.Windsor.Installer;
using MiniDDD.Storage.Installers;
using Component = Castle.MicroKernel.Registration.Component;

namespace MiniDDD.Tests
{
    public class BootStartup
    {
        public static IWindsorContainer Start()
        {
            var container = new WindsorContainer();
            container.Register(Component.For<IWindsorContainer>().Instance(container).LifestylePerThread());

            container.Install(
                FromAssembly.This(),
                FromAssembly.Containing<MiniDDDInstaller>()
              
                );
            container.Register(Component.For<UserService>());
           
            return container;
        }
    }
}
