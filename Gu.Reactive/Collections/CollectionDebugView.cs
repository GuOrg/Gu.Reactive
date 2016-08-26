namespace Gu.Reactive
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    /// http://www.codeproject.com/Articles/28405/Make-the-debugger-show-the-contents-of-your-custom
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CollectionDebugView<T>
    {
        private readonly IEnumerable<T> _collection;

        private static readonly T[] Empty = new T[0];

        public CollectionDebugView(IEnumerable<T> collection)
        {
            _collection = collection;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get
            {
                var array = _collection as T[];
                if (array != null)
                {
                    return array;
                }

                return _collection?.ToArray()?? Empty;
            }
        }
    }
}