# MassTransit.Automatonymous.UnityIntegration
Container for Registering Automatonymous.SagaStateMachine&lt;TSaga> using Unity

The functionality is currently minimal, you are welcome to add your code.

# Usage e.g.
using MassTransit;

using MassTransit.Automatonymous.UnityIntegration;

using Microsoft.Practices.Unity;

using Microsoft.Practices.Unity.Configuration;

using System;

...

var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>

            {
                var host = cfg.Host(busUri, h =>
                {
                    h.Username(username);
                    h.Password(password);
                });

                cfg.ReceiveEndpoint(host, queueName, ec =>
                {                                                            
                    ec.PrefetchCount = prefetchCount;
                    ec.AutoDelete = autoDeleteQueue;
                    ec.PurgeOnStartup = purgeOnStartup;
                    ec.UseConcurrencyLimit(concurrencyLimit);
                    ec.UseInMemoryOutbox();
                    // loading saga state machines
                    ec.LoadStateMachineSagas(container);
                });
            });

# Nuget.org
https://www.nuget.org/packages/MassTransit.Automatonymous.UnityIntegration
