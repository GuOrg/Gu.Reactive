namespace Gu.Reactive
{
    using System;

    /// <summary>
    /// Conditions keeps a log of the last changed states
    /// </summary>
    public struct ConditionHistoryPoint
    {
        public ConditionHistoryPoint(bool? state)
            : this(DateTime.UtcNow, state)
        {
        }

        public ConditionHistoryPoint(DateTime timeStamp, bool? state)
            :this()
        {
            this.TimeStamp = timeStamp;
            this.State = state;
        }

        public DateTime TimeStamp { get; }

        public bool? State { get; }

        public override string ToString()
        {
            return $"TimeStamp: {this.TimeStamp}, State: {this.State}";
        }
    }
}