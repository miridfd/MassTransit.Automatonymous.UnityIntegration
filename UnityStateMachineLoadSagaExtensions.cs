using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Unity;
using Automatonymous;
using MassTransit.Internals.Extensions;

namespace MassTransit.Automatonymous.UnityIntegration
{
    public static class UnityStateMachineLoadSagaExtensions
    {
        /// <summary>
        /// Scans the lifetime scope and registers any state machines sagas which are found in the scope using the Unity saga repository
        /// and the appropriate state machine saga repository under the hood.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="container"></param>
        /// <param name="name"></param>
        public static void LoadStateMachineSagas(this IReceiveEndpointConfigurator configurator, IUnityContainer container, string name = "message")
        {
            //var scope = context.Container.Resolve<IUnityContainer>();

            IList<Type> sagaTypes = FindStateMachineSagaTypes(container);

            foreach (var sagaType in sagaTypes)
            {
                StateMachineSagaConfiguratorCache.Configure(sagaType, configurator, container, name);
            }
        }

        public static IList<Type> FindStateMachineSagaTypes(IUnityContainer container)
        {
            Type type = typeof(SagaStateMachine<>);
            bool hasInterface = type.HasInterface(typeof(StateMachine));

            return container.Registrations                
                .Where(r => r.MappedToType.HasInterface(typeof(SagaStateMachine<>)))
                .Select(rs => rs.MappedToType.GetClosingArguments(typeof(SagaStateMachine<>)).Single())
                .Distinct()
                .ToList();
        }
    }
}
