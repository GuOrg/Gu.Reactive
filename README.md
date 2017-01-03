Gu.Reactive
===========

[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE.md)
[![NuGet](https://img.shields.io/nuget/v/Gu.Reactive.svg)](https://www.nuget.org/packages/Gu.Reactive/)
[![NuGet](https://img.shields.io/nuget/v/Gu.Wpf.Reactive.svg)](https://www.nuget.org/packages/Gu.Wpf.Reactive/)
[![Build status](https://ci.appveyor.com/api/projects/status/klrt8kctqbvt2j95?svg=true)](https://ci.appveyor.com/project/JohanLarsson/gu-reactive)
[![Gitter](https://badges.gitter.im/Join Chat.svg)](https://gitter.im/JohanLarsson/Gu.Reactive?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

Helpers for using System.Reactive with `INotifyPropertyChanged`.

### ObservePropertyChanged:

```
var subscription = fake.ObservePropertyChanged(x => x.Next.Value)
					   .Subscribe(...);
```

1) Listens to nested changes. All steps in the property path must be INotifyPropertyChanged. Throws if not.

2) Updates subscriptions for items in path and uses weak events. Tested for memory leaks.

3) Refactor friendly cos lambdas.

### ObserveItemPropertyChanged
```
var subscription = collection.ObserveItemPropertyChanged(x => x.Name)
							 .Subscribe(...);
```
1) Listens to changes using ObservePropertyChanged
2) Removes subscriptions for elements that are removed from the collection and adds subscription to new elements.

### Composes
```
fake.ObservePropertyChangedWithValue(x => x.Collection)
	.ItemPropertyChanged(x => x.Name)
	.Subscribe(_changes.Add);
```

### Conditions:
Se demo code



### FilteredView<T>
* No more CollectionViewSource in code.
* Typed so you get intellisense in xaml.
* Takes Filter<T, bool> and params IObservable<object> for max composability.


# Gu.Wpf.Reactive
Helpers for using System.Reactive with `INotifyPropertyChanged`  WPF applications.

## Commands
A set of relay commands. The generic versions take a command parameter of the generic type.
The nongeneric version does not use the command parameter.

### AsyncCommand
For executing tasks. If the overload that takes a `CancellationToken` is used the `CancelCommand` cancels the execution.
By default the command is disabled while running.
If no condition is passed in IsEnabled is true when not running.

```C#
public ViewModel()
{
	var canExecute = new Condition(
	    this.ObservePropertyChanged(x => x.CanExecute), 
		() => this.CanExecute);

	this.SimpleTaskCommand = new AsyncCommand(this.SimpleTask, canExecute);
	this.CancelableTaskCommand = new AsyncCommand(this.CancelableTask, canExecute);
	this.ParameterTaskCommand = new AsyncCommand<string>(this.ParameterTask, canExecute);
	this.CancelableParameterTaskCommand = new AsyncCommand<string>(this.CancelableParameterTask, canExecute);
}

public AsyncCommand SimpleTaskCommand { get; }

public AsyncCommand CancelableTaskCommand { get; }

public AsyncCommand ParameterTaskCommand { get; }

public AsyncCommand CancelableParameterTaskCommand { get; }

private async Task SimpleTask()
{
    await Task.Delay(500).ConfigureAwait(false);
}

private async Task CancelableTask(CancellationToken token)
{
    this.Count = 0;
    for (int i = 0; i < 5; i++)
    {
        token.ThrowIfCancellationRequested();
        this.Count++;
        await Task.Delay(this.Delay, token).ConfigureAwait(false);
    }
}

private Task ParameterTask(string arg)
{
    return this.SimpleTask();
}

private Task CancelableParameterTask(string arg, CancellationToken token)
{
    return this.CancelableTask(token);
}
```

### ConditionRelayCommand

A relay command where canexecute is controlled by a `ICondition`

```C#
public ViewModel()
{
	var canExecute = new Condition(
	    this.ObservePropertyChanged(x => x.CanExecute), 
		() => this.CanExecute);

    this.ConditionRelayCommand = new ConditionRelayCommand(() => ..., canExecute);
    this.ConditionRelayCommandWithParameter = new ConditionRelayCommand<string>(parameter => ..., canExecute);
}

public ConditionRelayCommand ConditionRelayCommand { get; }

public ConditionRelayCommand ConditionRelayCommandWithParameter { get; }
```

### ManualRelayCommand

A command where you need to manually call RaiseCanExecuteChanged`.

```C#
public ViewModel()
{
    this.ManualRelayCommand = new ManualRelayCommand(() => ..., () => this.CanExecute);
    this.ManualRelayCommandWithParameter = new ManualRelayCommand<string>(parameter => ..., () => this.CanExecute);
}

public ManualRelayCommand ManualRelayCommand { get; }

public ManualRelayCommand ManualRelayCommandWithParameter { get; }
```

### ObservingRelayCommand

A command where an observable is passed in for raising `CanExecuteChanged`.

```C#
public ViewModel()
{
    var onCanExecute = this.ObservePropertyChanged(x => x.CanExecute)
    this.ObservingRelayCommand = new ObservingRelayCommand(() => ..., onCanExecute, () => this.CanExecute);
    this.ObservingRelayCommandWithParameter = new ObservingRelayCommand<string>(parameter => ..., onCanExecute, () => this.CanExecute);
}

public ObservingRelayCommand ObservingRelayCommand { get; }

public ObservingRelayCommand ObservingRelayCommandWithParameter { get; }
```


### RelayCommand

A command that uses the `CommandManager.RequerySuggested` event.
It exposes a RaiseCanExecuteChanged method for forcing notification.

```C#
public ViewModel()
{
    this.RelayCommand = new RelayCommand(() => ..., () => this.CanExecute);
    this.RelayCommandWithParameter = new RelayCommand<string>(parameter => ..., () => this.CanExecute);
}

public RelayCommand ManualRelayCommand { get; }

public RelayCommand ManualRelayCommandWithParameter { get; }
```

