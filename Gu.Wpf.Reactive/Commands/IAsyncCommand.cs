namespace Gu.Wpf.Reactive
{
    using System.Threading.Tasks;
    using System.Windows.Input;

    public interface IAsyncCommand<in T> : ICommand
    {
        Task ExecuteAsync(T parameter);
    }
}
