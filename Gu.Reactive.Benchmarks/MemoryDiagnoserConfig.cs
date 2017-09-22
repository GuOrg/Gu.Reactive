[assembly: BenchmarkDotNet.Attributes.Config(typeof(Gu.Reactive.Benchmarks.MemoryDiagnoserConfig))]
namespace Gu.Reactive.Benchmarks
{
    using BenchmarkDotNet.Configs;
    using BenchmarkDotNet.Diagnosers;

    public class MemoryDiagnoserConfig : ManualConfig
    {
        public MemoryDiagnoserConfig()
        {
            this.Add(new MemoryDiagnoser());
        }
    }
}