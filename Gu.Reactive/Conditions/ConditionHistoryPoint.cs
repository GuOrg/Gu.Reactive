namespace Gu.Reactive
{
    using System;

    /// <summary>
    /// Conditions keeps a log of the last changed states.
    /// </summary>
    public struct ConditionHistoryPoint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionHistoryPoint"/> struct.
        /// </summary>
        public ConditionHistoryPoint(bool? state)
            : this(DateTime.UtcNow, state)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionHistoryPoint"/> struct.
        /// </summary>
        public ConditionHistoryPoint(DateTime timeStamp, bool? state)
            : this()
        {
            this.TimeStamp = timeStamp;
            this.State = state;
        }

        /// <summary>
        /// The time when the change occurred.
        /// </summary>
        public DateTime TimeStamp { get; }

        /// <summary>
        /// The state at <see cref="TimeStamp"/>.
        /// </summary>
        public bool? State { get; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"TimeStamp: {this.TimeStamp}, State: {this.State}";
        }
    }
}