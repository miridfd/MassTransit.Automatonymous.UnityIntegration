using Automatonymous;
using MassTransit.Saga;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassTransit.Automatonymous.UnityIntegration
{
    public static class StateMachineSagaConfiguratorCache
    {
        static CachedConfigurator GetOrAdd(Type type)
        {
            return Cached.Instance.GetOrAdd(type, _ => (CachedConfigurator)Activator.CreateInstance(typeof(CachedConfigurator<>).MakeGenericType(type)));
        }

        public static void Configure(Type sagaType, IReceiveEndpointConfigurator configurator, IUnityContainer scope, string name)
        {
            GetOrAdd(sagaType).Configure(configurator, scope, name);
        }


        static class Cached
        {
            internal static readonly ConcurrentDictionary<Type, CachedConfigurator> Instance = new ConcurrentDictionary<Type, CachedConfigurator>();
        }


        interface CachedConfigurator
        {
            void Configure(IReceiveEndpointConfigurator configurator, IUnityContainer scope, string name);
        }


        class CachedConfigurator<T> :
            CachedConfigurator
            where T : class, SagaStateMachineInstance
        {
            public void Configure(IReceiveEndpointConfigurator configurator, IUnityContainer scope, string name)
            {
                var sagaRepository = scope.Resolve<ISagaRepository<T>>();
                var stateMachine = scope.Resolve<SagaStateMachine<T>>();

                var unitySagaRepository = new UnityStateMachineSagaRepository<T>(sagaRepository, scope, name);

                configurator.StateMachineSaga(stateMachine, unitySagaRepository);
            }
        }
    }
}
