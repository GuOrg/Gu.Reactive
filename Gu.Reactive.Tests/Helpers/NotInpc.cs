// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Gu.Reactive.Tests.Helpers
{
    public class NotInpc
    {
        public NotInpc(bool isTrue, bool? isTrueOrNull, string name, int value)
        {
            this.IsTrue = isTrue;
            this.IsTrueOrNull = isTrueOrNull;
            this.Name = name;
            this.Value = value;
        }

        public bool IsTrue { get; }

        public bool? IsTrueOrNull { get; }

        public string Name { get; }

        public int Value { get; }
    }
}