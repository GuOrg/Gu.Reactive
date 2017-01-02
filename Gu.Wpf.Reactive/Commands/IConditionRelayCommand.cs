namespace Gu.Wpf.Reactive
{
    using System;
    using System.Windows.Input;

    using Gu.Reactive;

    /// <summary>
    /// A command with a <see cref="ICondition"/> controlling <see cref="ICommand.CanExecute(object)"/>
    /// </summary>
    public interface IConditionRelayCommand : ICommand, IDisposable
    {
        /// <summary>
        /// The condition controlling if the command can execute.
        /// </summary>
        ICondition Condition { get; }
    }
}