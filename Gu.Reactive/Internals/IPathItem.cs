namespace Gu.Reactive.Internals
{
    internal interface IPathItem
    {
        /// <summary>
        /// Gets or sets a value indicating whether is last.
        /// </summary>
        bool IsLast { get; }
        
        /// <summary>
        /// Gets the value.
        /// </summary>
        object Value { get; }
    }
}