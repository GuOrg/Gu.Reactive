#pragma warning disable GU0073 // Member of non-public type should be internal.
namespace Gu.Reactive
{
    using System;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    /// http://www.codeproject.com/Articles/28405/Make-the-debugger-show-the-contents-of-your-custom.
    /// </summary>
    internal sealed class CollectionConditionDebugView
    {
        private readonly CollectionCondition condition;

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionConditionDebugView"/> class.
        /// </summary>
        public CollectionConditionDebugView(CollectionCondition condition)
        {
            this.condition = condition;
        }

        /// <summary>
        /// Gets the items in the collection.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public ICondition[] Items => this.condition?.Prerequisites?.ToArray() ?? Array.Empty<ICondition>();
    }
}
