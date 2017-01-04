namespace Gu.Reactive
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    /// http://www.codeproject.com/Articles/28405/Make-the-debugger-show-the-contents-of-your-custom
    /// </summary>
    /// <typeparam name="T">The type of the items in the collection.</typeparam>
    public class CollectionDebugView<T>
    {
        private static readonly T[] Empty = new T[0];
        private readonly IEnumerable<T> collection;

        public CollectionDebugView(IEnumerable<T> collection)
        {
            this.collection = collection;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get
            {
                var array = this.collection as T[];
                if (array != null)
                {
                    return array;
                }

                return this.collection?.ToArray() ?? Empty;
            }
        }
    }
}