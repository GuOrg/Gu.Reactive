// ReSharper disable All
namespace Gu.Reactive.Tests.Collections
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    public partial class MappingViewTests
    {
        public class Model
        {
            public Model(int value)
            {
                this.Value = value;
            }

            public int Value { get; }

            public override string ToString()
            {
                return $"{nameof(this.Value)}: {this.Value}";
            }
        }

        private class Vm
        {
            public Vm()
            {
            }

            public Vm(Model model,  int index)
            {
                this.Model = model;
                this.Index = index;
            }

            public Model Model { get; set; }

            public int Value { get; set; }

            public int Index { get; set; }

            public Vm WithIndex(int i)
            {
                this.Index = i;
                return this;
            }

            public override string ToString()
            {
                return $"{nameof(Value)}: {Value}, {nameof(Index)}: {Index}, {nameof(Model)}: {Model}";
            }
        }
    }
}
