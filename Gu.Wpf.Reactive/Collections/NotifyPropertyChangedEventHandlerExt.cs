namespace Gu.Wpf.Reactive
{
    using System.Collections.Specialized;
    using System.Linq;
    using System.Windows.Threading;

    public static class NotifyCollectionChangedEventHandlerExt
    {
        public static void InvokeOnDispatcher(this NotifyCollectionChangedEventHandler handler, object sender, NotifyCollectionChangedEventArgs e)
        {
            if (handler == null)
            {
                return;
            }
            var invocationList = handler.GetInvocationList().OfType<NotifyCollectionChangedEventHandler>();
            foreach (var invocation in invocationList)
            {
                var dispatcherObject = invocation.Target as DispatcherObject;

                if (dispatcherObject != null && !dispatcherObject.CheckAccess())
                {
                    dispatcherObject.Dispatcher.Invoke(DispatcherPriority.DataBind, invocation, sender, e);
                }
                else
                {
                    invocation(sender, e); // note : this does not execute invocation in target thread's context
                }
            }
        }
    }
}
