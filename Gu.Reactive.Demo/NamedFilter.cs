namespace Gu.Reactive.Demo
{
    using System;

    public class NamedFilter
    {
        public NamedFilter(string name, Predicate<object> filter)
        {
            Name = name;
            Filter = filter;
        }

        public string Name { get; private set; }

        public Predicate<object> Filter { get; private set; }
    }
}