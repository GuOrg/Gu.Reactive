namespace Gu.Reactive.Internals
{
    using System.Reflection;

    internal interface IPathProperty
    {
        /// <summary>
        /// The getter for the <see cref="PropertyInfo"/>
        /// </summary>
        IGetter Getter { get; }
    }
}