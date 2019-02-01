namespace Gu.Reactive
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    /// http://www.codeproject.com/Articles/28405/Make-the-debugger-show-the-contents-of-your-custom.
    /// </summary>
    /// <typeparam name="T">The type of the items in the collection.</typeparam>
    public class CollectionDebugView<T>
    {
        private static readonly T[] Empty = new T[0];
        private readonly IEnumerable<T> collection;

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionDebugView{T}"/> class.
        /// </summary>
        public CollectionDebugView(IEnumerable<T> collection)
        {
            this.collection = collection;
        }

        /// <summary>
        /// The items in the collection.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
#pragma warning disable CA1819 // Properties should not return arrays
        public T[] Items
#pragma warning restore CA1819 // Properties should not return arrays
        {
            get
            {
                if (this.collection is T[] array)
                {
                    return array;
                }

                return this.collection?.ToArray() ?? Empty;
            }
        }
    }
}
