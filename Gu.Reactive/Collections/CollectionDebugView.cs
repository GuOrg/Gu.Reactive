namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// http://www.codeproject.com/Articles/28405/Make-the-debugger-show-the-contents-of-your-custom
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CollectionDebugView<T>
    {
        private readonly IReadOnlyList<T> _collection;

        public CollectionDebugView(IReadOnlyList<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            _collection = collection;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get
            {
                T[] array = new T[_collection.Count];
                for (int i = 0; i < _collection.Count; i++)
                {
                    array[i] = _collection[i];
                }
                return array;
            }
        }
    }
}