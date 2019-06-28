using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management.Automation;
using System.Runtime.InteropServices;

namespace davidyujia.DocPrint
{
    public sealed class Printer
    {
        private readonly string _printerName;

        public Printer(string printerName)
        {
            _printerName = printerName;
        }

        private void WindowsPlatformPrint(byte[] bytes)
        {
            using (var ps = PowerShell.Create())
            {
                ps.AddCommand("Out-Printer").AddParameters(new Dictionary<string, object>
                {
                    ["-Name"] = _printerName,
                    ["-InputObject"] = bytes,
                });

                ps.Invoke();
            }
        }

        private void LinuxPlatformPrint(byte[] bytes)
        {
            using (var linuxPrintProcess = new Process())
            {
                linuxPrintProcess.StartInfo.FileName = "lpr";
                linuxPrintProcess.StartInfo.Arguments = " -P " + _printerName;
                linuxPrintProcess.StandardInput.Write(bytes);
                linuxPrintProcess.Start();
            }
        }

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
                throw new NotImplementedException("OSX");
            }
            else
            {
                throw new Exception("Unknow platform.");
            }
        }
    }
}
