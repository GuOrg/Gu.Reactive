#pragma warning disable CS0618 // Type or member is obsolete
namespace Gu.Reactive.Benchmarks
{
    using System;
    using BenchmarkDotNet.Attributes;

    public class Get
    {
        private static readonly IValuePath<Get, string> Path = Reactive.Get.ValuePath<Get, string>(x => x.Fake.Next.Name);
        private static readonly Func<Get, string> Getter = x => x?.Fake?.Next?.Name;

        public Get()
        {
            this.Fake = null;
        }

        public Fake Fake { get; }

        [Benchmark(Baseline = true)]
        public string Func()
        {
            return Getter(this);
        }

        [Benchmark]
        public string ValueOrDefault()
        {
            return Reactive.Get.ValueOrDefault(this, x => this.Fake.Next.Name);
        }

        [Benchmark]
        public IMaybe<string> GetValueCachedPath()
        {
            return Path.GetValue(this);
        }
    }
}