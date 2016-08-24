namespace Gu.Reactive.Benchmarks
{
    using BenchmarkDotNet.Attributes;

    public class NameOf
    {
        [Benchmark(Baseline = true)]
        public string UsingCsharp6Nameof()
        {
            return nameof(Fake.Name);
        }

        [Benchmark]
        public string Property()
        {
            return Reactive.NameOf.Property<Fake, string>(x => x.Name);
        }

        [Benchmark]
        public string PropertyNested()
        {
            return Reactive.NameOf.Property<Fake, string>(x => x.Next.Next.Name);
        }
    }
}