using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using davidyujia.BackgroundService.Core;

namespace davidyujia.BackgroundService.Linux
{
    public class ServiceHost : IHostedService, IPlatformService
    {
        private HostService _service { get; set; }

        public string[] Args { get; set; }

        public void Init(HostService service)
        {
            _service = service;
        }

        public void Run()
        {
        }

        IApplicationLifetime _appLifetime;
        ILogger<ServiceHost> _logger;
        IHostingEnvironment _environment;
        IConfiguration _configuration;

        public ServiceHost(
            IConfiguration configuration,
            IHostingEnvironment environment,
            ILogger<ServiceHost> logger,
            IApplicationLifetime appLifetime)
        {
            _configuration = configuration;
            _logger = logger;
            _appLifetime = appLifetime;
            _environment = environment;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("StartAsync method called.");

            _appLifetime.ApplicationStarted.Register(OnStarted);
            _appLifetime.ApplicationStopping.Register(OnStopping);
            _appLifetime.ApplicationStopped.Register(OnStopped);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private void OnStarted()
        {
            _service.OnStart(Args);
        }

        private void OnStopping()
        {
        }

        private void OnStopped()
        {
            _service.OnStop();
        }
    }
}