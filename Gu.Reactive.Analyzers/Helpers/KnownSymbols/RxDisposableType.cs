namespace Gu.Reactive.Analyzers
{
    internal class RxDisposableType : QualifiedType
    {
        internal readonly QualifiedMethod Create;

        internal RxDisposableType()
            : base("System.Reactive.Disposables.Disposable")
        {
            this.Create = new QualifiedMethod(this, nameof(this.Create));
        }
    }
}