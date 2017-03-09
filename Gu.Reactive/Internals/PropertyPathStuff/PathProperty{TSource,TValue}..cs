namespace Gu.Reactive.Internals
{
    using System.Reflection;

    internal sealed class PathProperty<TSource, TValue> : IPathProperty
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PathProperty{TSource,TValue}"/> class.
        /// </summary>
        /// <param name="previous">The preivous property in the <see cref="PropertyPath"/></param>
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

        public override string ToString() => $"PathItem for: {this.Getter.Property.DeclaringType.PrettyName()}.{this.Getter.Property.Name}";

        public object GetSourceFromRoot(object rootSource)
        {
            if (this.Previous == null)
            {
                return rootSource;
            }

            return this.Previous.Getter.GetValue(this.Previous.GetSourceFromRoot(rootSource));
        }

        public PathPropertyTracker<TSource, TValue> CreateTracker(IPropertyPathTracker tracker) => new PathPropertyTracker<TSource, TValue>(tracker, this);

        IPathPropertyTracker IPathProperty.CreateTracker(IPropertyPathTracker tracker) => this.CreateTracker(tracker);
    }
}