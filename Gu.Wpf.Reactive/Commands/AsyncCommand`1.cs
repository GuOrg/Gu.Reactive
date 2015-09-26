namespace Gu.Wpf.Reactive
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// http://msdn.microsoft.com/en-us/magazine/dn630647.aspx
    /// An async command that uses command parameter
    /// Returns a Task
    /// </summary>
    public class AsyncCommand<T> : AsyncParameterCommandBase<T, Task>
    {
        public AsyncCommand(
            Func<T, Task> action,
            Func<T,bool> condition,
            bool disableMultipleRequests = true)
            : base(
                action,
                condition,
                x => new NotifyTaskCompletion(x),
                disableMultipleRequests)
        {
        }

        public AsyncCommand(
            Func<T, Task> action,
            bool disableMultipleRequests = true)
            : this(action, _ => true, disableMultipleRequests)
        {
        }
    }
}