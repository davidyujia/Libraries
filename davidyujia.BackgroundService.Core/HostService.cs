using System;
using System.Threading;
using System.Threading.Tasks;

namespace davidyujia.BackgroundService.Core
{
    public sealed class HostService
    {
        private string[] _args;
        private bool _isPause;
        private bool _isBreak;
        private bool _runningFlag;
        private Task _task;
        private Schedule _schedule;
        private Func<string[], bool> _main { get; set; }

        public HostService(Func<string[], bool> main, Schedule schedule)
        {
            _schedule = schedule;
            _main = main;
        }

        private void Process()
        {
            while (_runningFlag)
            {
                SleepPoint();

                if (NeedBreak())
                {
                    break;
                }

                Run();
            }
        }

        private void Run()
        {
            if (_main == null)
            {
                return;
            }

            var _lastRunTime = DateTime.Now;

            var result = _main(_args);

            _schedule.SaveLastRunTimeAndResult(_lastRunTime, result);
        }

        public bool NeedBreak()
        {
            return !_runningFlag;
        }

        private void SleepPoint()
        {
            var nextRunningTime = _schedule.GetNextRunningTime();
            SpinWait.SpinUntil(() => DateTime.Now >= nextRunningTime || NeedBreak());
        }

        public void PausePoint()
        {
            SpinWait.SpinUntil(() => !_isPause);
        }

        private bool IsNotRunning()
        {
            return _task == null
            || _task.Status == TaskStatus.Canceled
            || _task.Status == TaskStatus.Faulted
            || _task.Status == TaskStatus.RanToCompletion;
        }

        public void OnStart(string[] args)
        {
            OnStop();

            _args = args;
            _isBreak = false;
            _runningFlag = true;

            _task = new Task(Process);
            _task.Start();
        }

        public void OnStop()
        {
            _runningFlag = false;
            _isBreak = true;

            OnContinue();

            SpinWait.SpinUntil(IsNotRunning);

            _task = null;
        }

        public void OnContinue()
        {
            _isPause = false;
        }

        public void OnPause()
        {
            _isPause = true;
        }

        public void OnShutdown()
        {
            OnStop();
        }
    }
}