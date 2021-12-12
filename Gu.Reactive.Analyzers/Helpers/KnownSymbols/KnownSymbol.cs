// ReSharper disable InconsistentNaming
namespace Gu.Reactive.Analyzers
{
    using Gu.Roslyn.AnalyzerExtensions;

    internal static class KnownSymbol
    {
        internal static readonly QualifiedType Object = QualifiedType.System.Object;
        internal static readonly NullableOfTType Nullable = new();
        internal static readonly StringType String = new();
        internal static readonly QualifiedType DateTimeOffset = Create("System.DateTimeOffset");
        internal static readonly QualifiedType DateTime = Create("System.DateTime");
        internal static readonly QualifiedType TimeSpan = Create("System.TimeSpan");
        internal static readonly QualifiedType FuncOfT = Create("System.Func`1");
        internal static readonly QualifiedType Type = Create("System.Type");
        internal static readonly QualifiedType INotifyPropertyChanged = Create("System.ComponentModel.INotifyPropertyChanged");

        internal static readonly QualifiedType Condition = new("Gu.Reactive.Condition");
        internal static readonly QualifiedType OrCondition = new("Gu.Reactive.OrCondition");
        internal static readonly QualifiedType AndCondition = new("Gu.Reactive.AndCondition");
        internal static readonly IConditionType ICondition = new();
        internal static readonly QualifiedType IObservableOfT = new("System.IObservable`1");
        internal static readonly ObservableType Observable = new();
        internal static readonly ObservableExtensionsType ObservableExtensions = new();
        internal static readonly NotifyPropertyChangedExtType NotifyPropertyChangedExt = new();

        private static QualifiedType Create(string qualifiedName)
        {
            return new QualifiedType(qualifiedName);
        }
    }
}
