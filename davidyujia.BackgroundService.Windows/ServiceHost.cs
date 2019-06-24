using System;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using davidyujia.Service.Core;

namespace davidyujia.Service.Windows
{
    public abstract class ServiceHost : ServiceBase, IPlatformService
    {
        public ServiceHost()
        {
            GetLastRunTime();
        }

        public void Manual(string[] args)
        {
            _main(_args);
        }

        private DateTime GetNextRunningTime()
        {
            return DateTime.Now;
        }

        private void GetLastRunTime()
        {
            //get last run time from ...config?
        }

        private static void SaveLastRunTimeAndResult(DateTime lastRunTime, bool result)
        {
            //record last run time and result
        }

        public void PrusePoint()
        {
            SpinWait.SpinUntil(() => !_isPause);
        }

        private Func<string[], bool> _main { get; set; }

        public void SetProcess(Func<string[], bool> func)
        {
            _main = func;
        }

        private string[] _args { get; set; }

        private Task _task { get; set; }

        private bool _runningFlag { get; set; }

        private DateTime _lastRunTime { get; set; }

        private bool _isPause { get; set; }

        private bool IsRunning()
        {
            return _runningFlag;
        }

        private bool IsBreak()
        {
            var needBreak = false;
            if (needBreak)
            {
                OnContinue();
            }
            return needBreak;
        }

        private void SleepPoint()
        {
            //TODO: get next running time
            SpinWait.SpinUntil(() =>
            {
                return IsBreak();
            });
        }

        private void Process()
        {
            while (IsRunning())
            {
                SleepPoint();

                if (IsBreak())
                {
                    break;
                }

                _lastRunTime = DateTime.Now;

                var result = _main(_args);

                SaveLastRunTimeAndResult(_lastRunTime, result);

                if (IsBreak())
                {
                    break;
                }
            }
        }

        #region override

        protected override void OnStart(string[] args)
        {
            OnStop();

            _args = args;

            _task = new Task(Process);
            _task.Start();

            _runningFlag = true;
        }

        protected override void OnStop()
        {
            _runningFlag = false;
            OnContinue();
            SpinWait.SpinUntil(() => !IsRunning());

            _task = null;
        }

        protected override void OnContinue()
        {
            _isPause = false;
        }

        protected override void OnPause()
        {
            _isPause = true;
        }

        protected override void OnShutdown()
        {
            OnStop();
        }

        protected override void OnCustomCommand(int command)
        {
        }

        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {

        }

        #endregion
        
        public void Run()
        {
            ServiceBase.Run(this);
        }
    }
}
