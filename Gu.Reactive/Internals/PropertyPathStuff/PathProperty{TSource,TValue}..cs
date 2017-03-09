namespace Gu.Reactive.Internals
{
    using System;
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

        /// <summary>
        /// Gets the property info.
        /// </summary>
        [Obsolete("Remove, use property in getter.")]
        public PropertyInfo PropertyInfo => this.Getter.Property;

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

        /// <summary>
        /// Gets value all the way from the root recursively.
        /// Checks for null along the way.
        /// </summary>
        /// <param name="rootSource">The source object</param>
        /// <returns>The value of the property.</returns>
        internal Maybe<TValue> GetValueFromRoot(object rootSource)
        {
            if (rootSource == null)
            {
                return Maybe<TValue>.None;
            }

            if (this.Previous == null)
            {
                return this.Getter.GetMaybe((TSource)rootSource);
            }

            return this.Getter.GetMaybe((TSource)this.GetSourceFromRoot(rootSource));
        }

        internal Maybe<TValue> GetPropertyValue(TSource source) => source == null
                                                                       ? Maybe<TValue>.None
                                                                       : Maybe.Some(this.Getter.GetValue(source));
    }
}