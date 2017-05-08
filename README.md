Gu.Reactive
===========

[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE.md)
[![Nuget](https://img.shields.io/nuget/v/Gu.Reactive.svg?label=Gu.Reactive)](https://www.nuget.org/packages/Gu.Reactive/)
[![Nuget](https://img.shields.io/nuget/v/Gu.Wpf.Reactive.svg?label=Gu.Wpf.Reactive)](https://www.nuget.org/packages/Gu.Wpf.Reactive/)
[![Build status](https://ci.appveyor.com/api/projects/status/klrt8kctqbvt2j95/branch/master?svg=true)](https://ci.appveyor.com/project/JohanLarsson/gu-reactive/branch/master)
[![Gitter](https://badges.gitter.im/JoinChat.svg)](https://gitter.im/JohanLarsson/Gu.Reactive?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

Helpers for using System.Reactive with `INotifyPropertyChanged`.

# Table of contents

- [Factory methods for creating observables.](#factory-methods-for-creating-observables)
  - [ObservePropertyChanged:](#observepropertychanged)
    - [SignalInitial](#signalinitial)
  - [ObservePropertyChangedSlim:](#observepropertychangedslim)
    - [SignalInitial](#signalinitial)
    - [ObservePropertyChangedWithValue](#observepropertychangedwithvalue)
  - [ObserveCollectionChanged:](#observecollectionchanged)
    - [SignalInitial](#signalinitial)
  - [ObservePropertyChangedSlim:](#observepropertychangedslim)
    - [SignalInitial](#signalinitial)
  - [ObserveItemPropertyChanged](#observeitempropertychanged)
- [Conditions:](#conditions)
  - [Condition](#condition)
    - [IsSatisfied](#issatisfied)
    - [Name](#name)
    - [History](#history)
    - [Negate()](#negate)
  - [OrCondition](#orcondition)
    - [IsSatisfied](#issatisfied)
  - [AndCondition](#andcondition)
    - [IsSatisfied](#issatisfied)
- [Collections](#collections)
  - [ReadOnlyFilteredView](#readonlyfilteredview)
  - [MappingView](#mappingviewtsource-tresult)
  - [ReadOnlyThrottledView](#readonlythrottledview)
  - [ReadOnlyDispatchingView](#readonlydispatchingview)
  - [ReadOnlySerialView](#readonlyserialview)
  - [Commands](#commands)
    - [AsyncCommand](#asynccommand)
    - [ConditionRelayCommand](#conditionrelaycommand)
    - [ManualRelayCommand](#manualrelaycommand)
    - [ObservingRelayCommand](#observingrelaycommand)
    - [RelayCommand](#relaycommand)
  - [MarkupExtensions](#markupextensions)
    - [EnumValuesForExtension](#enumvaluesforextension)
    - [NinjaBinding](#ninjabinding)

# Assembly redirects.
When used from .Net 4.6 assembly redirects are needed.
https://github.com/Reactive-Extensions/Rx.NET/issues/299

```xml
<dependentAssembly>
  <assemblyIdentity name="System.Reactive.Core" publicKeyToken="94bc3704cddfc263" culture="neutral" />
  <bindingRedirect oldVersion="0.0.0.0-3.0.3000.0" newVersion="3.0.3000.0" />
</dependentAssembly>
<dependentAssembly>
  <assemblyIdentity name="System.Reactive.PlatformServices" publicKeyToken="94bc3704cddfc263" culture="neutral" />
  <bindingRedirect oldVersion="0.0.0.0-3.0.3000.0" newVersion="3.0.3000.0" />
</dependentAssembly>
<dependentAssembly>
  <assemblyIdentity name="System.Reactive.Linq" publicKeyToken="94bc3704cddfc263" culture="neutral" />
  <bindingRedirect oldVersion="0.0.0.0-3.0.3000.0" newVersion="3.0.3000.0" />
</dependentAssembly>
```
Nuget can generate redirects using PM> `Get-Project –All | Add-BindingRedirect`

# Factory methods for creating observables.

## ObservePropertyChanged:

```c#
var subscription = fake.ObservePropertyChanged(x => x.Level1.Level2.Value)
					   .Subscribe(...);
```

1) Create an observable from the `PropertytChangedEvent` for fake.
2) Listens to nested changes. All steps in the property path must be INotifyPropertyChanged. Throws if not.
3) When PropertyChanged is raised with string.Empty or null the observable notifies.
4) Updates subscriptions for items in path and uses weak events.

### SignalInitial
Default true meaning that the observable will call OnNExt on Subscribe.
The sender will be tha last node in the path that has a value, in the example above it would be the value of the property `Level2` if it is not null, then `Level1` if not null if the entire path is null the root item `fake`is used as sender for the first notifixcation.
The eventags for the signal initial event is `string.Empty`

## ObservePropertyChangedSlim:

```c#
var subscription = this.ObservePropertyChangedSlim(nameof(this.Value"))
					   .Subscribe(...);
```

1) Return an `IObservable<PropertyChangedEventArgs>` so more lightweight than `ObservePropertyChanged`
2) Filters change args mathing property name or string.IsNullOrEmpty

### SignalInitial
Default true meaning that the observable will call OnNExt on Subscribe

### ObserveValue
Observe the value of a property.

```c#
var ints = new List<int>();
fake.ObserveValue(x => x.Next.Value)
	.Subscribe(ints.Add);
```

### ObservePropertyChangedWithValue
```c#
fake.ObservePropertyChangedWithValue(x => x.Collection)
	.ItemPropertyChanged(x => x.Name)
	.Subscribe(_changes.Add);
```


## ObserveCollectionChanged:

```c#
var subscription = collection.ObserveCollectionChanged()
					   .Subscribe(...);
```

1) Create an observable from the `CollectionChangedEvent` for collection.

### SignalInitial
Default true meaning that the observable will call OnNExt on Subscribe

## ObservePropertyChangedSlim:

```c#
var subscription = collection.ObserveCollectionChangedSlim()
					   .Subscribe(...);
```

1) Return an `IObservable<NotifyCollectionChangedEventArgs>` so more lightweight than `ObserveCollectionChanged`

### SignalInitial
Default true meaning that the observable will call OnNExt on Subscribe

## ObserveItemPropertyChanged
```c#
var subscription = collection.ObserveItemPropertyChanged(x => x.Name)
							 .Subscribe(...);
```
1) Listens to changes using ObservePropertyChanged
2) Removes subscriptions for elements that are removed from the collection and adds subscription to new elements.


# Conditions:
## Condition

A type that calculates `IsSatisfied` when any of the `IObservable<object>` triggers signals.
Create it in code like this:

```c#
this.isTrueCondition = new Condition(
    this.ObservePropertyChanged(x => x.IsTrue),
    () => this.IsTrue);
```

The conditions work really well when used with an IoC.
Then subclasses are created and the IoC is used to build trees of nested conditions.

```c#
public class HasFuel : Condition
{
    public HasFuel(Car car)
        : base(
            car.ObservePropertyChanged(x => x.FuelLevel),
            () => car.FuelLevel > 0)
    {
    }
}
```

### IsSatisfied

Aveluates the criteria passed in in the ctor. Recalculates when any of the observables signals and the value changes.

### Name

Default is GetType.Name but the property is mutable so other names can be specified.

### History

The last 100 times of change and values for IsSatisfied

### Negate()

Returns a condition wrapping the instance and negating the value of IsSatisfied.
Negating a negated condition returns the original condition.

## OrCondition

Calculates IsSatisfied based on if any of the prerequisites are true. Listens to changes in IsSatisfied for prerequisites and notifies when value changes.

```c#
public class IsAnyDoorOpen : OrCondition
{
    public IsAnyDoorOpen(
		IsLeftDoorOpen isLeftDoorOpen,
		IsRightDoorOpen isRightDoorOpen)
        : base(isLeftDoorOpen, isRightDoorOpen)
    {
    }
}
```
### IsSatisfied

True if IsSatisfied for any prerequisites is true.
False if IsSatisfied for all prerequisites are false.
Null if IsSatisfied for no prerequisite is true and any prerequisite is null.

## AndCondition

Calculates IsSatisfied based on if all of the prerequisites have IsSatisfied == true. Listens to changes in IsSatisfied for prerequisites and notifies when value changes.

```c#
public class IsAnyDoorOpen : AndCondition
{
    public IsAnyDoorOpen(
		IsLeftDoorOpen isLeftDoorOpen,
		IsRightDoorOpen isRightDoorOpen)
        : base(isLeftDoorOpen.Negate(), isRightDoorOpen.Negate())
    {
    }
}
```
### IsSatisfied

True if IsSatisfied for all prerequisites are true.
False if IsSatisfied for any prerequisite is false.
Null if IsSatisfied for no prerequisite is false and any prerequisite is null.

Se demo for more code samples.

# Collections

## ReadOnlyFilteredView<T>

A typed filtered view of a collection. Sample usage:

```c#
public sealed class ViewModel : INotifyPropertyChanged, IDisposable
{
    private string filterText;
    private bool disposed;

    public ViewModel(ObservableCollection<Person> people, IWpfSchedulers schedulers)
    {
        this.FilteredPeople = people.AsReadOnlyFilteredView(
            this.IsMatch,
            TimeSpan.FromMilliseconds(10),
            schedulers.Dispatcher,
            this.ObservePropertyChangedSlim(nameof(this.FilterText)));
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public IReadOnlyObservableCollection<Person> FilteredPeople { get; }

    public string FilterText
    {
        get
        {
            return this.filterText;
        }

        set
        {
            if (value == this.filterText)
            {
                return;
            }

            this.filterText = value;
            this.OnPropertyChanged();
        }
    }

    public void Dispose()
    {
        if (this.disposed)
        {
            return;
        }

        this.disposed = true;
        this.FilteredPeople.Dispose();
    }

    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool IsMatch(Person person)
    {
        if (string.IsNullOrEmpty(this.filterText))
        {
            return true;
        }

        var indexOf = CultureInfo.InvariantCulture.CompareInfo.IndexOf(person.FirstName, this.filterText, CompareOptions.OrdinalIgnoreCase);
        if (this.filterText.Length == 1)
        {
            return indexOf == 0;
        }

        return indexOf >= 0;
    }
}
```

## MappingView<TSource, TResult>

A view that maps from one type to another. If thge source type is a reference type the same instance produces the same mapped instance if the item appears more than once in the collection.

### Simple

```c#
public sealed class ViewModel : IDisposable
{
    private bool disposed;

    public ViewModel(ObservableCollection<Person> people)
    {
        this.PersonViewModels = people.AsMappingView(x => new PersonViewModel(x));
    }

    public MappingView<Person, PersonViewModel> PersonViewModels { get; }

    public void Dispose()
    {
        if (this.disposed)
        {
            return;
        }

        this.disposed = true;
        this.PersonViewModels.Dispose();
    }
}
```

### When creating `IDisposable`

Pass in an action that is invoked when the last instance is removed from the mapped collection.

```c#
public sealed class ViewModel : IDisposable
{
    private bool disposed;

    public ViewModel(ObservableCollection<Person> people)
    {
        this.PersonViewModels = people.AsMappingView(
            x => new PersonViewModel(x),
            x => x.Dispose());
    }

    public IReadOnlyObservableCollection<PersonViewModel> PersonViewModels { get; }

    public void Dispose()
    {
        if (this.disposed)
        {
            return;
        }

        this.disposed = true;
        (this.PersonViewModels as IDisposable)?.Dispose();
    }
}
```

## ReadOnlyThrottledView<T>

A view that buffers changes. If there are many changes within the buffer time one reset event is raised instead of on event per change.

```c#
public sealed class ViewModel : IDisposable
{
    private bool disposed;

    public ViewModel(ObservableCollection<Person> people)
    {
        this.PersonViewModels = people.AsReadOnlyThrottledView(TimeSpan.FromMilliseconds(100));
    }

    public IReadOnlyObservableCollection<Person> PersonViewModels { get; }

    public void Dispose()
    {
        if (this.disposed)
        {
            return;
        }

        this.disposed = true;
        this.PersonViewModels.Dispose();
    }
}
```

## ReadOnlyDispatchingView<T>

A view that notifies on the dispatcher. Useful if the collection is updated on another thread.

```c#
public sealed class ViewModel : IDisposable
{
    private bool disposed;

    public ViewModel(ObservableCollection<Person> people)
    {
        this.PersonViewModels = people.AsReadOnlyDispatchingView();
    }

    public IReadOnlyObservableCollection<Person> PersonViewModels { get; }

    public void Dispose()
    {
        if (this.disposed)
        {
            return;
        }

        this.disposed = true;
        this.PersonViewModels.Dispose();
    }
}
```

## ReadOnlySerialView<T>

A view that can have source set to different collections. 
Useful when composing with for example MappedView

```c#
public sealed class ViewModel : IDisposable
{
    private bool disposed;

    public ViewModel(ObservableCollection<Person> people)
    {
        this.Update(people);
    }

    public ReadOnlySerialView<Person> People { get; } = new ReadOnlySerialView<Person>();

    public void Update(ObservableCollection<Person> people)
    {
        this.People.SetSource(people);
    }

    public void Dispose()
    {
        if (this.disposed)
        {
            return;
        }

        this.disposed = true;
        this.People.Dispose();
    }
}
```

## Sample

The following sample filters a list and then maps it to another type. This happens on the task pool.
Then changhes are notified to the view on the dispatcher.

```c#
public sealed class ViewModel : INotifyPropertyChanged, IDisposable
{
    private string filterText;
    private bool disposed;

    public ViewModel(ObservableCollection<Person> people, IWpfSchedulers schedulers)
    {
        this.FilteredPeople = people.AsReadOnlyFilteredView(
                                        this.IsMatch,
                                        TimeSpan.FromMilliseconds(10),
                                        schedulers.TaskPool,
                                        this.ObservePropertyChangedSlim(nameof(this.FilterText)))
                                    .AsMappingView(
                                        x => new PersonViewModel(x),
                                        x => x.Dispose())
                                    .AsReadOnlyDispatchingView();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public IReadOnlyObservableCollection<PersonViewModel> FilteredPeople { get; }

    public string FilterText
    {
        get
        {
            return this.filterText;
        }

        set
        {
            if (value == this.filterText)
            {
                return;
            }

            this.filterText = value;
            this.OnPropertyChanged();
        }
    }

    public void Dispose()
    {
        if (this.disposed)
        {
            return;
        }

        this.disposed = true;
        (this.FilteredPeople as IDisposable)?.Dispose();
    }

    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool IsMatch(Person person)
    {
        if (string.IsNullOrEmpty(this.filterText))
        {
            return true;
        }

        var indexOf = CultureInfo.InvariantCulture.CompareInfo.IndexOf(person.FirstName, this.filterText, CompareOptions.OrdinalIgnoreCase);
        if (this.filterText.Length == 1)
        {
            return indexOf == 0;
        }

        return indexOf >= 0;
    }
}
```
Gu.Wpf.Reactive
===========

Helpers for using System.Reactive with `INotifyPropertyChanged` in  WPF applications.

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

## MarkupExtensions

### EnumValuesForExtension

Markupextension for getting enum values for a type.

Sample code:

```xaml
xmlns:reactive="http://Gu.com/Reactive"
...
<ComboBox ItemsSource="{reactive:EnumValuesFor {x:Type Visibility}}" />
```

### NinjaBinding

Markupextension for binding when not in the visual tree.

Sample code:

```xaml
xmlns:reactive="http://Gu.com/Reactive"
...
<CheckBox x:Name="CheckBox" IsChecked="{Binding Visible}" />
...
<DataGrid AutoGenerateColumns="False">
    <DataGrid.Columns>
	    <!--Here the viewmodel has a Visibility property-->
        <DataGridTextColumn Header="Binding" 
		                    Visibility="{reactive:NinjaBinding {Binding Visibility}}" />

        <DataGridTextColumn Header="ElementName" 
		                    Visibility="{reactive:NinjaBinding Binding={Binding IsChecked, 
														                        ElementName=CheckBox, 
																				Converter={StaticResource BooleanToVisibilityConverter}}}" />
    </DataGrid.Columns>
</DataGrid>
```


