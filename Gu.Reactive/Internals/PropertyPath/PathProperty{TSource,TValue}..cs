namespace Gu.Reactive.Internals
{
    internal class PathProperty<TSource, TValue> : IPathProperty<TValue>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PathProperty{TSource,TValue}"/> class.
        /// </summary>
        /// <param name="previous">The preivous property in the <see cref="IPropertyPath"/></param>
        /// <param name="getter">
        /// The getter for the property.
        /// </param>
        public PathProperty(IPathProperty previous, Getter<TSource, TValue> getter)
        {
            Ensure.NotNull(getter, nameof(getter));
            this.Previous = previous;
            this.Getter = getter;
        }

        public IPathProperty Previous { get; }

        public Getter<TSource, TValue> Getter { get; }

        IGetter IPathProperty.Getter => this.Getter;

        public Maybe<TValue> GetMaybe(object source) => this.Getter.GetMaybe((TSource)source);

        public override string ToString() => $"PathItem for: {this.Getter.Property.DeclaringType.PrettyName()}.{this.Getter.Property.Name}";
    }
}