#### 3.0.0
* FEATURE: MinTracker, MaxTracker & MinMaxTracker.
* FEATURE: MappingView now with action invoked when item is removed, useful for disposing.
* FEATURE: ObservableExt.Chunks.
* PERF: Tweak all the things.
* BREAKING CHANGE: Removed markup converters, not a good fit for this library.
* BREAKING CHANGE: Renamed extension methods creating read only views, prefixed with AsReadOnly now.
* BREAKING CHANGE: Marked mutable views as [Obsolete] { FilteredView, ThrottledView, DispatchingView }. I think they are unfixable.
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