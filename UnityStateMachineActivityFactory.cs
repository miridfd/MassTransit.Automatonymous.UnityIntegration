using Automatonymous;
using GreenPipes;
using Microsoft.Practices.Unity;

namespace MassTransit.Automatonymous.UnityIntegration
{

    public class UnityStateMachineActivityFactory :
        IStateMachineActivityFactory
    {
        public Activity<TInstance, TData> GetActivity<TActivity, TInstance, TData>(BehaviorContext<TInstance, TData> context)
            where TActivity : Activity<TInstance, TData>
        {
            var lifetimeScope = context.GetPayload<IUnityContainer>();

            return lifetimeScope.Resolve<TActivity>();
        }

        public Activity<TInstance> GetActivity<TActivity, TInstance>(BehaviorContext<TInstance> context) where TActivity : Activity<TInstance>
        {
            var lifetimeScope = context.GetPayload<IUnityContainer>();

            return lifetimeScope.Resolve<TActivity>();
        }
    }
}
