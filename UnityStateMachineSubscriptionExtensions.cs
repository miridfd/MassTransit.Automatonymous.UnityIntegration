using Automatonymous;
using Automatonymous.SubscriptionConfigurators;
using Automatonymous.SubscriptionConnectors;
using GreenPipes;
using MassTransit.Pipeline;
using MassTransit.Saga;
using MassTransit.Saga.SubscriptionConfigurators;
using Microsoft.Practices.Unity;
using System;

namespace MassTransit.Automatonymous.UnityIntegration
{
    public static class UnityStateMachineSubscriptionExtensions
    {
        /// <summary>
        /// Subscribe a state machine saga to the endpoint
        /// </summary>
        /// <typeparam name="TInstance">The state machine instance type</typeparam>
        /// <param name="configurator"></param>
        /// <param name="stateMachine">The state machine</param>
        /// <param name="container">The StructureMap Container to resolve the repository</param>
        /// <param name="configure">Optionally configure the saga</param>
        /// <returns></returns>
        public static void StateMachineSaga<TInstance>(this IReceiveEndpointConfigurator configurator, SagaStateMachine<TInstance> stateMachine,
            IUnityContainer container, Action<ISagaConfigurator<TInstance>> configure = null)
            where TInstance : class, SagaStateMachineInstance
        {
            var sagaRepository = container.Resolve<ISagaRepository<TInstance>>();

            var containerRepository = new UnityStateMachineSagaRepository<TInstance>(sagaRepository, container);

            var stateMachineConfigurator = new StateMachineSagaSpecification<TInstance>(stateMachine, containerRepository);

            configure?.Invoke(stateMachineConfigurator);

            configurator.AddEndpointSpecification(stateMachineConfigurator);
        }

        public static ConnectHandle ConnectStateMachineSaga<TInstance>(this IConsumePipeConnector bus, SagaStateMachine<TInstance> stateMachine,
            IUnityContainer container)
            where TInstance : class, SagaStateMachineInstance
        {
            var connector = new StateMachineConnector<TInstance>(stateMachine);

            var sagaRepository = container.Resolve<ISagaRepository<TInstance>>();

            var containerRepository = new UnityStateMachineSagaRepository<TInstance>(sagaRepository, container);

            return connector.ConnectSaga(bus, containerRepository);
        }
    }
}
