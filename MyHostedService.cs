using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace AzureAppConfigurationDemo
{
    class MyHostedService : IHostedService
    {
        private readonly IOptionsMonitor<MySettings> optionsMonitor;
        private IDisposable subscription;

        public MyHostedService(IOptionsMonitor<MySettings> optionsMonitor)
        {
            this.optionsMonitor = optionsMonitor;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            var settings = optionsMonitor.CurrentValue;
            Console.WriteLine(settings.FirstValue);
            Console.WriteLine(settings.SecondValue);

            subscription = optionsMonitor.OnChange(MySettingsChanged);
            return Task.CompletedTask;
        }

        private void MySettingsChanged(MySettings arg1, string arg2)
        {
            Console.WriteLine(arg2);
            Console.WriteLine(arg1.FirstValue);
            Console.WriteLine(arg1.SecondValue);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            subscription.Dispose();
            return Task.CompletedTask;
        }
    }
}
