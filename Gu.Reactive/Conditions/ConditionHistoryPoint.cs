// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConditionHistoryPoint.cs" company="">
//   
// </copyright>
// <summary>
//   Conditions keeps a log of the last changed states
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gu.Reactive
{
    using System;

    /// <summary>
    /// Conditions keeps a log of the last changed states
    /// </summary>
    public class ConditionHistoryPoint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionHistoryPoint"/> class. 
        /// : this(DateTime.UtcNow, state)
        /// </summary>
        /// <param name="state">
        /// </param>
        public ConditionHistoryPoint(bool? state)
            : this(DateTime.UtcNow, state)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionHistoryPoint"/> class.
        /// </summary>
        /// <param name="timeStamp">
        /// The time stamp.
        /// </param>
        /// <param name="state">
        /// The state.
        /// </param>
        public ConditionHistoryPoint(DateTime timeStamp, bool? state)
        {
            TimeStamp = timeStamp;
            State = state;
        }

        /// <summary>
        /// Gets the time stamp.
        /// </summary>
        public DateTime TimeStamp { get; private set; }

        /// <summary>
        /// Gets the state.
        /// </summary>
        public bool? State { get; private set; }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("TimeStamp: {0}, State: {1}", TimeStamp, State);
        }
    }
}