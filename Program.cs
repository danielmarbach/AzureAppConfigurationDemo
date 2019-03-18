﻿using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AzureAppConfigurationDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureHostConfiguration(configHost =>
                {
                    configHost.SetBasePath(Directory.GetCurrentDirectory());
                    configHost.AddJsonFile("hostsettings.json", optional: true, reloadOnChange: true);
                })
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                    config.AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true);

                    if (hostContext.HostingEnvironment.IsProduction())
                    {
                        var settings = config.Build();
                        config.AddAzureAppConfiguration(o => o.Connect(settings.GetConnectionString("AppConfig"))
                            .Watch("MySettings:FirstValue", TimeSpan.FromSeconds(5)));
                    }
                    if (hostContext.HostingEnvironment.IsDevelopment())
                    {
                        config.AddUserSecrets<Program>();
                    }
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOptions();
                    services.AddHostedService<MyHostedService>();

                    var provider = services.BuildServiceProvider();
                    var settings = provider.GetService<IConfiguration>();

                    services.Configure<MySettings>(settings.GetSection("MySettings"));
                })
                .UseConsoleLifetime()
                .Build();

            await host.RunAsync();
        }
    }
}