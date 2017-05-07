using Automatonymous;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MassTransit.Automatonymous.UnityIntegration
{
    public static class UnityStateMachineRegisterSagaExtensions
    {
        /// <summary>
        /// Register the state machine sagas found in the specified assemblies using the ContainerBuilder provided. The
        /// machines are registered using their SagaStateMachine&lt;&gt; type, as well as the concrete type for use by
        /// the application.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="assemblies"></param>
        public static void RegisterStateMachineSagas(this IUnityContainer container,
            params Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes()
                    .Where(p => typeof(SagaStateMachine<>).IsAssignableFrom(p));
                foreach (var consumerType in types)
                    container.RegisterType(consumerType, new ContainerControlledLifetimeManager());
            }            
        }
    }
}
