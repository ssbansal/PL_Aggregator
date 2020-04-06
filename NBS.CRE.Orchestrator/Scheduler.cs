using Microsoft.Extensions.DependencyInjection;
using NBS.CRE.Common.Models;
using NBS.CRE.Common.RPC;
using NBS.CRE.Orchestrator.Mappers;
using RabbitMQ.Client;
using System;
using System.Threading.Tasks;

namespace NBS.CRE.Orchestrator
{
    /// <summary>
    /// Scheduler Interface
    /// </summary>
    public interface IScheduler
    {
        Task<RESPONSE> Execute<REQUEST, RESPONSE>(REQUEST request);
        void Shutdown();
    }

    /// <summary>
    /// Scheduler
    /// </summary>
    public class Scheduler : IScheduler
    {
        private IServiceProvider _serviceProvider;

        public Scheduler()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        /// <summary>
        /// Configure the dependency injection container.
        /// </summary>
        /// <param name="serviceCollection">Service collection of the injection container.</param>
        private void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IConnection>(provider => {
                string hostName = Environment.GetEnvironmentVariable("RMQ_HOST");
                if (String.IsNullOrEmpty(hostName))
                {
                    hostName = "localhost";
                }

                var factory = new ConnectionFactory() { HostName = hostName, UserName = "guest", Password = "guest" };
                return factory.CreateConnection();
            });

            serviceCollection.AddSingleton<RpcClient>(provider => {
                return new RpcClient(provider.GetService<IConnection>());
            });

            serviceCollection.AddTransient<AbstractMapper<PLAggregatorDecisionRequest, PLAggregatorDecisionResponse>>(provider => {
                return new PLAggregatorDecisionMapper();
            });

        }

        /// <summary>
        /// Performs resource cleanup. Closes RabbitMQ connection.
        /// </summary>
        public void Shutdown()
        {
            var connection = _serviceProvider.GetService<IConnection>();
            if (connection != null)
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /// <summary>
        /// Starting point for the orchestration.
        /// </summary>
        /// <typeparam name="REQUEST">Request model.</typeparam>
        /// <typeparam name="RESPONSE">Response model.</typeparam>
        /// <param name="request">Request object instance.</param>
        /// <returns><c>Task</c> that completes when the orchestration finishes.</returns>
        public async Task<RESPONSE> Execute<REQUEST, RESPONSE>(REQUEST request)
        {
            Type[] typeArgs = { typeof(REQUEST), typeof(RESPONSE) };
            var mapperType = typeof(AbstractMapper<,>).MakeGenericType(typeArgs);
            var mapper = _serviceProvider.GetService(mapperType) as AbstractMapper<REQUEST, RESPONSE>;

            if (mapper != null)
            {
                ISchedulerContext context = new SchedulerContext(_serviceProvider);
                var result = await mapper.Execute(request, context);

                return result;
            }
            else
            {
                throw new InvalidOperationException("Mapper not found!");
            }
        }
    }
}
