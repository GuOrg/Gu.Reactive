namespace Gu.Wpf.Reactive
{
    using System;
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    ///
    /// </summary>
    public class CanExecuteChangedEventManager : WeakEventManager
    {
        private CanExecuteChangedEventManager()
        {
        }

        public static CanExecuteChangedEventManager CurrentManager
        {
            get
            {
                var managerType = typeof(CanExecuteChangedEventManager);
                var currentManager = (CanExecuteChangedEventManager)GetCurrentManager(managerType);
                if (currentManager == null)
                {
                    currentManager = new CanExecuteChangedEventManager();
                    SetCurrentManager(managerType, (WeakEventManager)currentManager);
                }
                return currentManager;
            }
        }

        /// <summary>
        /// Adds the specified listener to the list of listeners on the specified source.
        /// </summary>
        /// <param name="source">The object with the event.</param>
        /// <param name="listener">The object to add as a listener.</param>
        public static void AddListener(ICommand source, IWeakEventListener listener)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (listener == null)
            {
                throw new ArgumentNullException(nameof(listener));
            }
            CurrentManager.ProtectedAddListener(source, listener);
        }

        /// <summary>
        /// Removes the specified listener from the list of listeners on the provided source.
        /// </summary>
        /// <param name="source">The object to remove the listener from.</param>
        /// <param name="listener">The listener to remove.</param>
        public static void RemoveListener(ICommand source, IWeakEventListener listener)
        {
            if (listener == null)
            {
                throw new ArgumentNullException(nameof(listener));
            }
            CurrentManager.ProtectedRemoveListener(source, listener);
        }

        /// <summary>
        /// Adds the specified event handler
        /// </summary>
        /// <param name="source">The source object that the raises the event.</param>
        /// <param name="handler">The delegate that handles the event.</param>
        public static void AddHandler(ICommand source, EventHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }
            CurrentManager.ProtectedAddHandler(source, handler);
        }

        /// <summary>
        /// Removes the specified event handler from the specified source.
        /// </summary>
        /// <param name="source">The source object that the raises the event.</param>
        /// <param name="handler">The delegate that handles the event.</param>
        public static void RemoveHandler(ICommand source, EventHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));
            CurrentManager.ProtectedRemoveHandler(source, handler);
        }

        /// <summary>
        /// Begins listening for the event on the provided source.
        /// </summary>
        /// <param name="source">The object on which to start listening to.</param>
        protected override void StartListening(object source)
        {
            ((ICommand)source).CanExecuteChanged += this.DeliverEvent;
        }

        /// <summary>
        /// Stops listening for the event on the provided source.
        /// </summary>
        /// <param name="source">The source object on which to stop listening to.</param>
        protected override void StopListening(object source)
        {
            ((ICommand)source).CanExecuteChanged -= this.DeliverEvent;
        }
    }
}