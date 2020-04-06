using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using System;
using System.Threading;

namespace NBS.CRE.Common.Worker
{

    /// <summary>
    /// Abstract base class for creating Remote Worker Host
    /// </summary>
    public abstract class AbstractWorkerHost
    {
        public AbstractWorkerHost()
        {
        }

        /// <summary>
        /// Configure the dependency injection container.
        /// </summary>
        /// <param name="serviceCollection">Configured services collection.</param>
        protected abstract void ConfigureServices(IServiceCollection serviceCollection);

        /// <summary>
        /// Get the Remote Worker Instance
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns>Remote Worker Instance. See <see cref="AbstractWorker"/>.</returns>
        protected abstract AbstractWorker GetWorker(IServiceProvider serviceProvider);

        /// <summary>
        /// Runs the Remote Worker Host. Connects to RabbitMQ broker, starts the remote worker and waits for [Ctrl+C] key press.
        /// </summary>
        public void Run()
        {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);

            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IConnection>(provider => {
                string hostName = Environment.GetEnvironmentVariable("RMQ_HOST");
                if (String.IsNullOrEmpty(hostName))
                {
                    hostName = "localhost";
                }

                var factory = new ConnectionFactory() { HostName = hostName, UserName = "guest", Password = "guest" };
                return factory.CreateConnection();
            });

            ConfigureServices(serviceCollection);
            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
            var worker = GetWorker(serviceProvider);

            Console.WriteLine("Server: STARTED");
            Console.WriteLine("Press [Ctrl + C] to quit ...");

            Console.CancelKeyPress += (object sender, ConsoleCancelEventArgs e) =>
            {
                Console.WriteLine("Stopping server ...");
                autoResetEvent.Set();
            };

            worker.Run(autoResetEvent);

            var connection = serviceProvider.GetService<IConnection>();
            if (connection != null)
            {
                connection.Close();
                connection.Dispose();
            }

            Console.WriteLine("Server: STOPPED");
        }
    }
}
