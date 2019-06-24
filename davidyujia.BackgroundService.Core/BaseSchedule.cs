using System;

namespace davidyujia.BackgroundService.Core
{
    public abstract class Schedule
    {
        public abstract DateTime GetNextRunningTime();

        public virtual void SaveLastRunTimeAndResult(DateTime lastRunTime, bool result)
        {
        }
    }
}