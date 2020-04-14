using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace NBS.CRE.APIGateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IPAddress GetHost()
        {
            string host = Environment.GetEnvironmentVariable("UI_HOST");
            if (String.IsNullOrEmpty(host))
            {
                host = "0.0.0.0";
            }

            return new IPAddress(host.Split('.').Select(part => byte.Parse(part)).ToArray());
        }
        public static int GetPort()
        {
            string port = Environment.GetEnvironmentVariable("UI_PORT");
            if (String.IsNullOrEmpty(port))
            {
                port = "8080";
            }

            return int.Parse(port);
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .UseKestrel(options =>
            {
                options.Listen(GetHost(), GetPort());
            })
            .UseStartup<Startup>();
    }
}
