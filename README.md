Gu.Reactive
===========

A collection of useful classes that uses System.Reactive

Nice feature:

var observable = fake.ToObservable(x => x.Next.Value);
1) Listens to nested changes. All steps in the property path must be INotifyPropertyChanged. Thorws if not.
2) Unsubscribes and uses weak events. Tested for memory leaks.
3) Refactor friendly cos lambdas.
