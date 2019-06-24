using System;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace davidyujia.Service.Windows
{
    public abstract class WindowsService : ServiceBase
    {
        /// <summary>
        /// Main
        /// </summary>
        /// <returns>Result</returns>
        protected abstract bool Main(string[] args);

        public WindowsService()
        {
            GetLastRunTime();
        }

        public void Manual(string[] args)
        {
            Main(args);
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

        protected void PausePoint()
        {
            SpinWait.SpinUntil(() =>
            {
                return !_isPause;
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

                var result = Main(_args);

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
    }
}
