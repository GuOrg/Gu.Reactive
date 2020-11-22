namespace Gu.Wpf.Reactive
{
    using System;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Threading;

    /// <summary>
    /// Extension methods for <see cref="NotifyCollectionChangedEventHandler"/>.
    /// </summary>
    public static class NotifyCollectionChangedEventHandlerExt
    {
        /// <summary>
        /// Invokes the change event on the dispatcher if needed.
        /// </summary>
        /// <param name="handler">The NotifyCollectionChangedEventHandler.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static Task InvokeOnDispatcherAsync(this NotifyCollectionChangedEventHandler handler, object sender, NotifyCollectionChangedEventArgs e)
        {
            if (handler is null)
            {
                return Task.CompletedTask;
            }

            var invocationList = handler.GetInvocationList();

            if (invocationList.Length == 0)
            {
                return Task.CompletedTask;
            }

            if (invocationList.Length == 1)
            {
                return NotifyAsync(sender, e, invocationList[0]);
            }

            return Task.WhenAll(invocationList.Select(x => NotifyAsync(sender, e, x)));
        }

        private static Task NotifyAsync(object sender, NotifyCollectionChangedEventArgs e, Delegate invocation)
        {
            var dispatcherObject = invocation.Target as DispatcherObject;
            if (dispatcherObject?.CheckAccess() == false)
            {
                return dispatcherObject.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, invocation, sender, e)
                                  .Task;
            }

            ((NotifyCollectionChangedEventHandler)invocation).Invoke(sender, e);
            return Task.CompletedTask;
        }
    }
}
