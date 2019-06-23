using System;

namespace davidyujia.DocPrint
{
    public sealed class Printer
    {
        private readonly string _printerName;

        public Printer(string printerName)
        {
            _printerName = printerName;
        }

        private Action<byte[]> WindowsPlatformPrint = b =>
        {
            using (var ps = PowerShell.Create())
            {
                ps.AddCommand("Out-Printer").AddParameters(new Dictionary<string, object>
                {
                    ["-Name"] = _printerName,
                    ["-InputObject"] = b,
                });

                ps.Invoke();
            }
        };

        private Action<byte[]> LinuxPlatformPrint = b =>
        {
            using (var linuxPrintProcess = new Process())
            {
                linuxPrintProcess.StartInfo.FileName = "lpr";
                linuxPrintProcess.StartInfo.Arguments = " -P " + _printerName;
                linuxPrintProcess.StandardInput.Write(bytes);
                linuxPrintProcess.Start();
            }
        };

        public void Print(byte[] bytes)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                WindowsPlatformPrint(bytes);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                LinuxPlatformPrint(bytes);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                throw NotImplementedException("OSX");
            }
            else
            {
                throw new Exception("Unknow platform.");
            }
        }
    }
}
