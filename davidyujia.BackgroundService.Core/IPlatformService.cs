using System;

namespace davidyujia.BackgroundService.Core
{
    public interface IPlatformService
    {
        void Init(HostService service);
        void Run();
    }
}