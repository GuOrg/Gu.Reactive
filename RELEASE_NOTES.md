#### 3.4.1
* BUGFIX: Expose prerquisites for NullIsFale<TCondition>.

#### 3.4.0
* BUGFIXES: Many bugfixes in tha analyzers.
* FEATURE: observable.AsMappingView(...)
* FEATURE: observable.AsReadonlyFilteredView(...)
* FEATURE: observable.AsReadonlyView(...)
* FEATURE: NullIsFalse<TCondition>)
* FEATURE: ObservableBatchCollection.ResetTo(newItems)

#### 3.3.1
* BUGFIX: Make prerequisites immutable.

#### 3.3.0
* BUGFIX: MappingView handles nulls in source.
* FEATURE: Negated<TCondition>
* FEATURE: Better exception message when prerequisites are not distinct


#### 3.2.0
* FEATURE: AsReadOnlyView, turn IObservable<T> into a bindable IReadOnlyView<T>

#### 3.1.0
* BUGFIX: ObserveItemPropertyChanged should only signal when the tracked property changes.
* FEATURE: Handle dynamic list of conditions in AndCondition & OrCondition.
* MINOR BREAKING: Rename ReadonlySerialViewBase{TSource,TMapped}, can't imagine anyone ever used this.
* FEATURE: ObserveFullPropertyPathSlim, observe all steps in a property path.

#### 3.0.1
* BUGFIX: Handle dispose twice for conditions.

#### 3.0.0
* FEATURE: MinTracker, MaxTracker & MinMaxTracker.
* FEATURE: MappingView now with action invoked when item is removed, useful for disposing.
* FEATURE: ObservableExt.Chunks.
* PERF: Tweak all the things.
* BREAKING CHANGE: Removed markup converters, not a good fit for this library.
* BREAKING CHANGE: Renamed extension methods creating read only views, prefixed with AsReadOnly now.
* BREAKING CHANGE: Moved mutable views { FilteredView, ThrottledView, DispatchingView } to Gu.Wpf.Reactive, allowing them on the dispatcher only.
* BREAKING CHANGE: Disposing source collection when disposing the view. Overloads with `leaveOpen` flag.
* BREAKING CHANGE: Removed WeakCompositeDisposable, it was a weird idea.
* BREAKING CHANGE: Removed some NameOf helpers.
* BREAKING CHANGE: Removed Get and IValuePath.

#### 2.1.0
* FEATURE: ObserveValue.
* FEATURE: ObservePropertyChangedSlim with lamda.
* BUGFIX: PropertyTracker thread safe.

#### 2.0.1
* BUGFIX: Don't use PropertyInfoComparer.

#### 2.0.1
* BUGFIX: PropertyInfoComparer must handle generic properties.

#### 2.0.0
* BREAKING: Use rx 3.0 packages.