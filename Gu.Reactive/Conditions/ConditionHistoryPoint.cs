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
            TimeStamp = timeStamp;
            State = state;
        }

        public DateTime TimeStamp { get; private set; }

        public bool? State { get; private set; }

        public override string ToString()
        {
            return string.Format("TimeStamp: {0}, State: {1}", TimeStamp, State);
        }
    }
}