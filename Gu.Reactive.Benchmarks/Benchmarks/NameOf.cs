#pragma warning disable CS0618 // Type or member is obsolete
#pragma warning disable CS8602, CS8603
namespace Gu.Reactive.Benchmarks
{
    using BenchmarkDotNet.Attributes;

    [BenchmarkDotNet.Attributes.MemoryDiagnoser]
    public class NameOf
    {
        [Benchmark(Baseline = true)]
        public string UsingCsharp6Nameof() => nameof(Fake.Name);

        [Benchmark]
        public string Property() => Reactive.NameOf.Property<Fake, string>(x => x.Name);

        [Benchmark]
        public string PropertyNested() => Reactive.NameOf.Property<Fake, string>(x => x.Next.Next.Name);
    }
}
