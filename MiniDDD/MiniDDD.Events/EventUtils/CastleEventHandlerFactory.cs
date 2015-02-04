using System.Collections.Generic;
using Castle.Windsor;

namespace MiniDDD.Events.EventUtils
{
    public class CastleEventHandlerFactory : IEventHandlerFactory
    {
        private readonly IWindsorContainer _container;

        public CastleEventHandlerFactory(IWindsorContainer container)
        {
            _container = container;
        }

        public IEnumerable<IEventHandler<T>> GetHandlers<T>() where T : Event
        {
            return _container.ResolveAll<IEventHandler<T>>();
        }

        //private static IEnumerable<Type> GetHandlerType<T>() where T : Event
        //{
           
        //    var handlers = typeof(IEventHandler<>).Assembly.GetExportedTypes()
        //        .Where(x => x.GetInterfaces()
        //            .Any(a => a.IsGenericType && a.GetGenericTypeDefinition() == typeof(IEventHandler<>))).Where(h => h.GetInterfaces().Any(ii => ii.GetGenericArguments().Any(aa => aa == typeof(T)))).ToList();


        //    return handlers;
        //}
    }
}