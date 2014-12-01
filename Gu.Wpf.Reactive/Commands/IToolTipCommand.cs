namespace Gu.Wpf.Reactive
{
    using System.ComponentModel;
    using System.Windows.Input;

    public interface IToolTipCommand : ICommand, INotifyPropertyChanged
    {
        string ToolTipText { get; set; }
    }
}