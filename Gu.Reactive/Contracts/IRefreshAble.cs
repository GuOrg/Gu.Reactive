namespace Gu.Reactive
{
    /// <summary>
    /// For manually refreshing.
    /// </summary>
    public interface IRefreshAble
    {
        /// <summary>
        /// Force a refresh.
        /// </summary>
        void Refresh();
    }
}