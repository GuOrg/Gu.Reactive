namespace Gu.Reactive.Internals
{
    using System;
    using System.Reflection;

    internal interface IPathProperty
    {
        /// <summary>
        /// The previous property in the path.
        /// </summary>
        IPathProperty Previous { get; }

        /// <summary>
        /// The getter for the <see cref="PropertyInfo"/>
        /// </summary>
        IGetter Getter { get; }

        /// <summary>
        /// Gets value all the way from the root recursively.
        /// Checks for null along the way.
        /// </summary>
        /// <param name="rootSource">The source object</param>
        /// <returns>The source of the value for this instance.</returns>
        [Obsolete("Remove")]
        object GetSourceFromRoot(object rootSource);
    }
}