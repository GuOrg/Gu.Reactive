Gu.Reactive
===========
[![Gitter](https://badges.gitter.im/Join Chat.svg)](https://gitter.im/JohanLarsson/Gu.Reactive?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

A collection of useful classes that uses System.Reactive

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

### Commands
* Not using CommandManager for raising CanExecuteChanged. Weak events & tests for memory leaks.
* Typed parameters or no parameter.
* ManualRelayCommand & ManualRelayCommand<T>
* RelayCommand & RelayCommand<T>
* ObservingRelayCommand<T>
* ConditionRelayCommand & ConditionRelayCommand<T>

### FilteredView<T>
* No more CollectionViewSource in code.
* Typed so you get intellisense in xaml.
* Takes Filter<T, bool> and params IObservable<object> for max composability.

License
=======

Gu.Reactive is licensed under the MIT license. See License.md for the
full license text, or at [opensource.org][1] if the license file was not
provided.

[1]: http://opensource.org/licenses/MIT
