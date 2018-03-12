using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Net;

namespace Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                 .UseKestrel(options =>
                 {
                     options.Limits.MaxConcurrentConnections = 100;
                     options.Limits.MaxRequestBodySize = 10 * 1024;
                     options.Limits.MinRequestBodyDataRate =
                         new MinDataRate(bytesPerSecond: 100, gracePeriod: TimeSpan.FromSeconds(10));
                     options.Limits.MinResponseDataRate =
                         new MinDataRate(bytesPerSecond: 100, gracePeriod: TimeSpan.FromSeconds(10));
                     options.Listen(new IPAddress(new byte[] { 192,168,0,101}), 5000);
                 })
                .Build();
    }
}
