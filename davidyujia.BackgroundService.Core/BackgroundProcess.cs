using System;

namespace davidyujia.BackgroundService.Core
{
    public abstract class BackgroundProcess
    {
        private HostService _service { get; set; }

        public void SetProcess(HostService service)
        {
            _service = service;
        }

        protected bool NeedBreak()
        {
            return _service.NeedBreak();
        }

        protected void PausePoint()
        {
            _service.PausePoint();
        }

        public abstract bool Main(string[] args);
    }
}
