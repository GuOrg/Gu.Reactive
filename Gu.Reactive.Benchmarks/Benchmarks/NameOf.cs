#pragma warning disable CS0618 // Type or member is obsolete
namespace Gu.Reactive.Benchmarks
{
    using BenchmarkDotNet.Attributes;

    [BenchmarkDotNet.Attributes.MemoryDiagnoser]
    public static class NameOf
    {
        [Benchmark(Baseline = true)]
        public static string UsingCsharp6Nameof()
        {
            return nameof(Fake.Name);
        }

        [Benchmark]
        public static string Property()
        {
            return Reactive.NameOf.Property<Fake, string>(x => x.Name);
        }

        [Benchmark]
        public static string PropertyNested()
        {
            return Reactive.NameOf.Property<Fake, string>(x => x.Next.Next.Name);
        }
    }
}
