namespace Gu.Reactive.Demo
{
    using System;

    public class NamedFilter
    {
        public NamedFilter(string name, Predicate<object> filter)
        {
            this.Name = name;
            this.Filter = filter;
        }

        public string Name { get; }

        public Predicate<object> Filter { get; }
    }
}