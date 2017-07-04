namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using Gu.Reactive.Internals;

    /// <summary>
    /// Used internally in <see cref="OrCondition"/>
    /// </summary>
    internal class OrConditionCollection : ConditionCollection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrConditionCollection"/> class.
        /// </summary>
        /// <param name="prerequisites">The children.</param>
        /// <param name="leaveOpen">True to not dispose <paramref name="prerequisites"/> when this instance is disposed.</param>
        internal OrConditionCollection(IReadOnlyList<ICondition> prerequisites, bool leaveOpen)
            : base(GetIsSatisfied, prerequisites, leaveOpen)
        {
        }

        private static bool? GetIsSatisfied(IReadOnlyList<ICondition> prerequisites)
        {
            if (prerequisites.Count == 0)
            {
                return null;
            }

            // We do the retries here as we don't own prerequisites
            // Not pretty and maybe allowing mutable notifying prerequisites was a huge mistake.
            var retry = 0;
            while (true)
            {
                try
                {
                    var isNull = false;
                    foreach (var prerequisite in prerequisites)
                    {
                        var prerequisiteIsSatisfied = prerequisite.IsSatisfied;
                        if (prerequisiteIsSatisfied == true)
                        {
                            return true;
                        }

                        isNull |= prerequisiteIsSatisfied == null;
                    }

                    return isNull ? (bool?)null : false; // Mix of false & nulls means not enough info
                }
                catch (InvalidOperationException e) when (e.Message == Exceptions.CollectionWasModified.Message &&
                                                          retry < 5)
                {
                    retry++;
                }
            }
        }
    }
}