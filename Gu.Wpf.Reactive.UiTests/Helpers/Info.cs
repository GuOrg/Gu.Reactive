namespace Gu.Wpf.Reactive.UiTests
{
    using System;
    using System.Reflection;
    using System.Text.RegularExpressions;

    public static class Info
    {
        public static string ExeFileName { get; } = GetExeFileName();

        private static string GetExeFileName()
        {
            var testDllFileName = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
            var fullFileName = Regex.Replace(testDllFileName, $"{System.IO.Path.GetFileName(testDllFileName)}$", "Gu.Reactive.Demo.exe", RegexOptions.IgnoreCase);
            return fullFileName;
        }
    }
}