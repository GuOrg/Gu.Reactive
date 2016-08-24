namespace Gu.Reactive.Benchmarks
{
    using System;
    using BenchmarkDotNet.Attributes;

    public class Get
    {
        private static readonly IValuePath<Get, string> Path = Reactive.Get.ValuePath<Get, string>(x => x.Fake.Next.Name);
        private static readonly Func<Get, string> Getter = x => x?.Fake?.Next?.Name;
        public Fake Fake { get; } = new Fake { Next = new Level { Name = "Johan" } };

        public Get()
        {
            Fake = null;
        }

        [Benchmark(Baseline = true)]
        public string Func()
        {
            return Getter(this);
        }

        [Benchmark]
        public string ValueOrDefault()
        {
            return Reactive.Get.ValueOrDefault(this, x => Fake.Next.Name);
        }

        [Benchmark]
        public IMaybe<string> GetValueCachedPath()
        {
            return Path.GetValue(this);
        }
    }
}