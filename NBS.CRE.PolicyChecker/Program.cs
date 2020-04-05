using Microsoft.Extensions.DependencyInjection;
using NBS.CRE.Common.Worker;
using NBS.CRE.PolicyChecker.Services;
using System;

namespace NBS.CRE.PolicyChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Policy Checker Service");
            var workerHost = new PolicyCheckerWorkerHost();
            workerHost.Run();
        }
    }

    public class PolicyCheckerWorkerHost : AbstractWorkerHost
    {
        protected override void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ILookupService>(new LookupService());
        }
        protected override AbstractWorker GetWorker(IServiceProvider serviceProvider)
        {
            return new PolicyCheckerWorker(serviceProvider);
        }
    }
}
