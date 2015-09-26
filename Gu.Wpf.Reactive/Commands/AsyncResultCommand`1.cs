namespace Gu.Wpf.Reactive
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// http://msdn.microsoft.com/en-us/magazine/dn630647.aspx
    /// An async command that uses commandparameter <typeparam name="TParameter"></typeparam>
    /// Returns a Task<typeparam name="TResult"></typeparam>
    /// </summary>
    public class AsyncResultCommand<TParameter, TResult> : AsyncParameterCommandBase<TParameter, Task<TResult>>
    {
        public AsyncResultCommand(
            Func<TParameter, Task<TResult>> action,
            Func<TParameter, bool> condition,
            bool disableMultipleRequests = true,
            bool raiseCanExecuteOnDispatcher = true)
            : base(
                action,
                condition,
                x => new NotifyTaskCompletion<TResult>(x),
                disableMultipleRequests)
        {
        }

        public AsyncResultCommand(
            Func<TParameter, Task<TResult>> action,
            bool disableMultipleRequests = true)
            : this(action, _ => true, disableMultipleRequests)
        {
        }
    }

    /// <summary>
    /// http://msdn.microsoft.com/en-us/magazine/dn630647.aspx
    /// An async command that does not use command parameter
    /// Returns a Task<typeparam name="TResult"></typeparam>
    /// </summary>
    public class AsyncResultCommand<TResult> : AsyncCommandBase<Task<TResult>>
    {
        public AsyncResultCommand(
            Func<Task<TResult>> action,
            Func<bool> condition,
            bool disableMultipleRequests = true)
            : base(
                action,
                condition,
                x => new NotifyTaskCompletion<TResult>(x),
                disableMultipleRequests)
        {
        }

        public AsyncResultCommand(
            Func<Task<TResult>> action,
            bool disableMultipleRequests = true)
            : this(action, () => true, disableMultipleRequests)
        {
        }
    }
}