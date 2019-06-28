using System;
using System.Runtime.InteropServices;
using davidyujia.BackgroundService.Core;

namespace davidyujia.BackgroundService
{
    public sealed class BackgroundService
    {
        IPlatformService _platformService;
        BackgroundProcess _backgroundProcess;
        Schedule _schedule;

        private BackgroundService(IPlatformService platformService, BackgroundProcess backgroundProcess, Schedule schedule)
        {
            _platformService = platformService;
            _backgroundProcess = backgroundProcess;
        }

        public void Run()
        {
            var service = new HostService(_backgroundProcess.Main, _schedule);
            _platformService.Init(service);
            _platformService.Run();
        }

        public BackgroundService Create(BackgroundProcess process, Schedule schedule)
        {
            IPlatformService service = null;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                service = new Windows.ServiceHost();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                throw new NotImplementedException("Not support on Linux.");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                throw new NotImplementedException("Not support on OSX.");
            }
            else
            {
                throw new NotImplementedException("Unknow platform.");
            }

            return new BackgroundService(service, process, schedule);
        }
    }
}
