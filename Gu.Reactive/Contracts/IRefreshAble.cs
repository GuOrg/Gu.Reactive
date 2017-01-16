namespace Gu.Reactive
{
    /// <summary>
    /// For manually refreshing.
    /// </summary>
    public interface IRefreshAble
    {
        /// <summary>
        /// Force a refresh.
        /// May be deferred if there is a buffer time.
        /// </summary>
        void Refresh();
    }
}