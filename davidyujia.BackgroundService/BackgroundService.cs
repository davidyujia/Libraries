using System;

namespace davidyujia.BackgroundService
{
    public sealed class BackgroundService
    {
        IPlatformService _service;
        BaseProcess _process;

        private BackgroundService(IPlatformService service, BaseProcess process)
        {
            _service = service;
            _process = process;
        }

        public void Run()
        {
            _service.SetProcess(_process.Main);
            _service.Run();
        }

        public BackgroundService Create(BaseProcess process)
        {
            IPlatformService service = null;

            return new BackgroundService(service, process);
        }
    }
}
