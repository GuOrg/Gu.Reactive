namespace Gu.Reactive.PropertyPathStuff
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reactive;
    using System.Reactive.Disposables;

    /// <summary>
    /// The nested observable.
    /// </summary>
    /// <typeparam name="TClass">
    /// </typeparam>
    /// <typeparam name="TProp">
    /// </typeparam>
    internal sealed class PropertyPathObservable<TClass, TProp> :
        ObservableBase<EventPattern<PropertyChangedEventArgs>>
        where TClass : INotifyPropertyChanged
    {
        internal readonly PropertyChangedEventArgs PropertyChangedEventArgs;
        private readonly WeakReference sourceReference = new WeakReference(null);
        private readonly PropertyPath<TClass, TProp> propertyPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyPathObservable{TClass,TProp}"/> class.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="propertyExpression">
        /// The property expression.
        /// </param>
        public PropertyPathObservable(TClass source, Expression<Func<TClass, TProp>> propertyExpression)
            : this(source, PropertyPath.Create(propertyExpression))
        {
        }

        public PropertyPathObservable(TClass source, PropertyPath<TClass, TProp> propertyPath)
        {
            this.sourceReference.Target = source;
            this.propertyPath = propertyPath;
            VerifyPath(this.propertyPath);
            this.PropertyChangedEventArgs = new PropertyChangedEventArgs(this.propertyPath.Last.PropertyInfo.Name);
        }

        public object Sender => this.propertyPath.GetSender((TClass)this.sourceReference.Target);

        protected override IDisposable SubscribeCore(IObserver<EventPattern<PropertyChangedEventArgs>> observer)
        {
            var rootItem = new RootItem((INotifyPropertyChanged)this.sourceReference.Target);
            var path = new NotifyingPath(rootItem, this.propertyPath);

            var subscription = path.Last()
                                   .ObservePropertyChanged()
                                   .Subscribe(observer.OnNext, observer.OnError);
            return new CompositeDisposable(2) { path, subscription };
        }

        /// <summary>
        /// All steps in the path must implement INotifyPropertyChanged, this throws if this condition is not met.
        /// </summary>
        /// <param name="path">
        /// </param>
        private static void VerifyPath(IPropertyPath path)
        {
            for (int i = 0; i < path.Count - 1; i++)
            {
                var propertyInfo = path[i].PropertyInfo;
                if (propertyInfo.PropertyType.IsValueType)
                {
                    var message = string.Format(
                            "Property path cannot have structs in it. Copy by value will make subscribing error prone. Also mutable struct much?" + Environment.NewLine +
                            "The type {0} is a value type not so {1}.{2} will not notify when it changes." + Environment.NewLine +
                            "The path is: {3}",
                            propertyInfo.PropertyType.PrettyName(),
                            i == 0 ? "x" : path[i - 1].PropertyInfo.Name,
                            propertyInfo.Name,
                            path);
                    throw new ArgumentException(message, nameof(path));
                }

                if (!typeof(INotifyPropertyChanged).IsAssignableFrom(propertyInfo.PropertyType))
                {
                    var message = string.Format(
                        "All levels in the path must implement INotifyPropertyChanged." + Environment.NewLine +
                        "The type {0} does not so {1}.{2} will not notify when it changes." + Environment.NewLine +
                        "The path is: {3}",
                        propertyInfo.PropertyType.PrettyName(),
                        i == 0 ? "x" : path[i - 1].PropertyInfo.Name,
                        propertyInfo.Name,
                        path);
                    throw new ArgumentException(message, nameof(path));
                }
            }
        }
    }
}