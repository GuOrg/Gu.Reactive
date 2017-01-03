namespace Gu.Wpf.Reactive
{
    using System.Threading.Tasks;
    using System.Windows.Input;

    /// <summary>
    /// A command for executing tasks
    /// </summary>
    /// <typeparam name="T">The type of the command parameter.</typeparam>
    public interface IAsyncCommand<in T> : ICommand
    {
        /// <summary>
        /// Execute a task
        /// </summary>
        /// <param name="parameter">The command parameter.</param>
        /// <returns>The task.</returns>
        Task ExecuteAsync(T parameter);
    }
}
