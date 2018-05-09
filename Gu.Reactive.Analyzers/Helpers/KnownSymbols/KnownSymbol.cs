// ReSharper disable InconsistentNaming
namespace Gu.Reactive.Analyzers
{
    using Gu.Roslyn.AnalyzerExtensions;

    internal static class KnownSymbol
    {
        internal static readonly QualifiedType Object = QualifiedType.System.Object;
        internal static readonly NullableOfTType Nullable = new NullableOfTType();
        internal static readonly StringType String = new StringType();
        internal static readonly QualifiedType DateTimeOffset = Create("System.DateTimeOffset");
        internal static readonly QualifiedType DateTime = Create("System.DateTime");
        internal static readonly QualifiedType TimeSpan = Create("System.TimeSpan");
        internal static readonly QualifiedType FuncOfT = Create("System.Func`1");
        internal static readonly QualifiedType Type = Create("System.Type");
        internal static readonly QualifiedType INotifyPropertyChanged = Create("System.ComponentModel.INotifyPropertyChanged");

        internal static readonly QualifiedType Condition = new QualifiedType("Gu.Reactive.Condition");
        internal static readonly QualifiedType OrCondition = new QualifiedType("Gu.Reactive.OrCondition");
        internal static readonly QualifiedType AndCondition = new QualifiedType("Gu.Reactive.AndCondition");
        internal static readonly IConditionType ICondition = new IConditionType();
        internal static readonly QualifiedType IObservableOfT = new QualifiedType("System.IObservable`1");
        internal static readonly ObservableType Observable = new ObservableType();
        internal static readonly ObservableExtensionsType ObservableExtensions = new ObservableExtensionsType();
        internal static readonly NotifyPropertyChangedExtType NotifyPropertyChangedExt = new NotifyPropertyChangedExtType();

        private static QualifiedType Create(string qualifiedName)
        {
            return new QualifiedType(qualifiedName);
        }
    }
}
