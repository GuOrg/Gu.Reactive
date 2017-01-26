namespace Gu.Wpf.Reactive
{
    using System.Collections.Specialized;
    using System.Windows.Threading;

    /// <summary>
    /// Extension methods for <see cref="NotifyCollectionChangedEventHandler"/>
    /// </summary>
    public static class NotifyCollectionChangedEventHandlerExt
    {
        /// <summary>
        /// Invokes the change event on the dispatcher if needed.
        /// </summary>
        /// <param name="handler">The NotifyCollectionChangedEventHandler</param>
        /// <param name="sender">The sender</param>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/></param>
        public static void InvokeOnDispatcher(this NotifyCollectionChangedEventHandler handler, object sender, NotifyCollectionChangedEventArgs e)
        {
            if (handler == null)
            {
                return;
            }

            foreach (var invocation in handler.GetInvocationList())
            {
                var dispatcherObject = invocation.Target as DispatcherObject;

                if (dispatcherObject?.CheckAccess() == true)
                {
#pragma warning disable GU0011 // Don't ignore the returnvalue.
                    dispatcherObject.Dispatcher.Invoke(DispatcherPriority.DataBind, invocation, sender, e);
#pragma warning restore GU0011 // Don't ignore the returnvalue.
                }
                else
                {
                    // note : this does not execute invocation in target thread's context
                    ((NotifyCollectionChangedEventHandler)invocation).Invoke(sender, e);
                }
            }
        }
    }
}
