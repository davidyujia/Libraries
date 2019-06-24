using System;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using davidyujia.BackgroundService.Core;

namespace davidyujia.BackgroundService.Windows
{
    public sealed class ServiceHost : ServiceBase, IPlatformService
    {
        private HostService _service { get; set; }

        public void Init(HostService service)
        {
            _service = service;
        }

        public void Run()
        {
            ServiceBase.Run(this);
        }

        protected override void OnStart(string[] args)
        {
            _service.OnStart(args);
        }

        protected override void OnStop()
        {
            _service.OnStop();
        }

        protected override void OnContinue()
        {
            _service.OnContinue();
        }

        protected override void OnPause()
        {
            _service.OnPause();
        }

        protected override void OnShutdown()
        {
            _service.OnStop();
        }
    }
}
