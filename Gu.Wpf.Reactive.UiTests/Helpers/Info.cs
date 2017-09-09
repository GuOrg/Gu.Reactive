namespace Gu.Wpf.Reactive.UiTests
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Text.RegularExpressions;

    public static class Info
    {
        public static string ExeFileName { get; } = GetExeFileName();

        public static ProcessStartInfo ProcessStartInfo
        {
            get
            {
                var fileName = GetExeFileName();
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                return processStartInfo;
            }
        }

        internal static ProcessStartInfo CreateStartInfo(string args)
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = GetExeFileName(),
                Arguments = args,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            return processStartInfo;
        }

        private static string GetExeFileName()
        {
            var testDllFileName = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
            var fullFileName = Regex.Replace(testDllFileName, $"{System.IO.Path.GetFileName(testDllFileName)}$", "Gu.Reactive.Demo.exe", RegexOptions.IgnoreCase);
            return fullFileName;
        }
    }
}