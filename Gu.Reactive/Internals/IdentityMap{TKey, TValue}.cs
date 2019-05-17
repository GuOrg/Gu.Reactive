﻿namespace Gu.Reactive.Internals
{
    using System.Collections.Generic;

    internal class IdentityMap<TKey, TValue> : Dictionary<TKey, TValue>
        where TKey : class
    {
        public IdentityMap()
            : base(ObjectIdentityComparer<TKey>.Default)
        {
        }
    }
}