using Automatonymous;
using GreenPipes;
using MassTransit.Saga;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassTransit.Automatonymous.UnityIntegration
{
    public class UnityStateMachineSagaRepository<TSaga> :
         ISagaRepository<TSaga>
         where TSaga : class, ISaga
    {
        readonly string _name;
        readonly ISagaRepository<TSaga> _repository;
        readonly IUnityContainer _scope;

        public UnityStateMachineSagaRepository(ISagaRepository<TSaga> repository, IUnityContainer scope, string name = "message")
        {
            _repository = repository;
            _scope = scope;
            _name = name;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("unity");
            scope.Add("name", _name);

            _repository.Probe(scope);
        }

        async Task ISagaRepository<TSaga>.Send<T>(ConsumeContext<T> context, ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next)
        {
            using (var lifetimeScope = _scope.CreateChildContainer())
            {
                ConsumeContext<T> proxy = context.CreateScope(lifetimeScope);

                proxy.GetOrAddPayload<IStateMachineActivityFactory>(() => new UnityStateMachineActivityFactory());

                await _repository.Send(proxy, policy, next).ConfigureAwait(false);
            }
        }

        async Task ISagaRepository<TSaga>.SendQuery<T>(SagaQueryConsumeContext<TSaga, T> context, ISagaPolicy<TSaga, T> policy,
            IPipe<SagaConsumeContext<TSaga, T>> next)
        {
            using (var lifetimeScope = _scope.CreateChildContainer())
            {
                SagaQueryConsumeContext<TSaga, T> proxy = context.CreateQueryScope(lifetimeScope);

                proxy.GetOrAddPayload<IStateMachineActivityFactory>(() => new UnityStateMachineActivityFactory());

                await _repository.SendQuery(proxy, policy, next).ConfigureAwait(false);
            }
        }

        static void ConfigureScope<T>(IUnityContainer containerBuilder, ConsumeContext<T> context)
            where T : class
        {
            containerBuilder.RegisterInstance(context, new ExternallyControlledLifetimeManager());
            containerBuilder.RegisterInstance<ConsumeContext>(context, new ExternallyControlledLifetimeManager());              
        }
    }
}
