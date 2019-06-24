using System;

namespace davidyujia.BackgroundService.Core
{
    public interface IPlatformService
    {
        void SetProcess(Func<string[], bool> func);
        void PrusePoint();

        void Run();
    }

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

    public abstract class BaseProcess
    {
        private IPlatformService _service { get; set; }

        public void SetProcess(IPlatformService service)
        {
            _service = service;
        }

        protected void PrusePoint()
        {
            _service.PrusePoint();
        }

        public abstract bool Main(string[] args);
    }
}
