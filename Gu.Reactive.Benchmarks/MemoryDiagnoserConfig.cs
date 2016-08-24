namespace Gu.Reactive.Benchmarks
{
    using BenchmarkDotNet.Configs;

    public class MemoryDiagnoserConfig : ManualConfig
    {
        public MemoryDiagnoserConfig()
        {
            this.Add(new BenchmarkDotNet.Diagnostics.Windows.MemoryDiagnoser());
        }
    }
}