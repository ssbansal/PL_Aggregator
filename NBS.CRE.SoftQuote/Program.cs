using Microsoft.Extensions.DependencyInjection;
using NBS.CRE.Common.Worker;
using System;

namespace NBS.CRE.SoftQuote
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Soft Quote Service");
            var workerHost = new SoftQuoteWorkerHost();
            workerHost.Run();
        }
    }

    public class SoftQuoteWorkerHost : AbstractWorkerHost
    {
        protected override void ConfigureServices(IServiceCollection serviceCollection)
        {
            // Nothing to configure
        }
        protected override AbstractWorker GetWorker(IServiceProvider serviceProvider)
        {
            return new SoftQuoteWorker(serviceProvider);
        }
    }
}
