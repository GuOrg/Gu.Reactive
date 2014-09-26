namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reactive;
    using System.Reactive.Subjects;
    using System.Reflection;

    public class NestedObservable<TClass, TProp> : ObservableBase<EventPattern<PropertyChangedEventArgs>>, IDisposable
        where TClass : INotifyPropertyChanged
    {
        private readonly WeakReference _value;
        private readonly List<PathItem> _path = new List<PathItem>();
        private readonly Subject<EventPattern<PropertyChangedEventArgs>> _subject = new Subject<EventPattern<PropertyChangedEventArgs>>();
        public NestedObservable(TClass source, Expression<Func<TClass, TProp>> propertyExpression)
        {
            var path = PathExpressionVisitor.GetPath(propertyExpression);
            foreach (var pathItem in path)
            {
                var item = new PathItem(pathItem.Expression.Type.GetProperty(pathItem.Member.Name));
                this._path.Add(item);
            }

            this._path.First().Source = source;
            this._path.Last().IsLast = true;
            AssertPathNotifies(this._path);
            this.AddSubscriptions(0);
            this._value = new WeakReference(this._path.Last().Value, false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected override IDisposable SubscribeCore(IObserver<EventPattern<PropertyChangedEventArgs>> observer)
        {
            return this._subject.Subscribe(observer);
        }
       
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this._path.Any())
                {
                    foreach (var pathItem in this._path)
                    {
                        pathItem.Dispose();
                    }
                    this._path.Clear();
                }
            }
        }

        private static void AssertPathNotifies(IEnumerable<PathItem> path)
        {
            var notNotifyings = path.Where(x => !x.PropertyInfo.DeclaringType.GetInterfaces()
                                               .Contains(typeof(INotifyPropertyChanged)))
                                    .ToArray();
            if (notNotifyings.Any())
            {
                var props = string.Join(", ", notNotifyings.Select(x => "." + x.PropertyInfo.Name));
                throw new ArgumentException(string.Format("All levels in the path must notify (parent must be : INotifyPropertyChanged)  {{{0}}} does not.", props));
            }
        }

        private void AddSubscriptions(int toIndex)
        {
            if (toIndex > 0 && toIndex < this._path.Count)
            {
                this._path[toIndex].Source = this._path[toIndex - 1].Value;
            }
            for (int j = toIndex; j < this._path.Count; j++)
            {
                var pathItem = this._path[j];
                var o = pathItem.Source;

                if (o == null)
                {
                    break;
                }
                pathItem.Subscription = o.ToObservable(pathItem.PropertyInfo.Name, false)
                                         .Subscribe(this.OnPathItemChanged);
                if (!pathItem.IsLast)
                {
                    this._path[j + 1].Source = pathItem.Value;
                }
            }
        }
        private void RemoveSubscriptions(int fromIndex)
        {
            for (int j = fromIndex; j < this._path.Count; j++)
            {
                var pathItem = this._path[j];
                if (pathItem.Subscription == null)
                {
                    break;
                }
                pathItem.Subscription.Dispose();
                pathItem.Subscription = null;
                pathItem.Source = null;
            }
        }
        private void OnPathItemChanged(EventPattern<PropertyChangedEventArgs> eventPattern)
        {
            var i = this.IndexOf((INotifyPropertyChanged)eventPattern.Sender);
            this.RemoveSubscriptions(i + 1);
            this.AddSubscriptions(i + 1);

            var value = this._path.Last().Value;
            if (!(value == null && this._value.Target == null))
            {
                this._value.Target = value;
                var pattern = new EventPattern<PropertyChangedEventArgs>(this._path.Last().Source, new PropertyChangedEventArgs(this._path.Last().PropertyInfo.Name));
                this._subject.OnNext(pattern);
            }
        }
        private int IndexOf(INotifyPropertyChanged sender)
        {
            for (int i = 0; i < this._path.Count; i++)
            {
                var pathItem = this._path[i];
                if (ReferenceEquals(pathItem.Source, sender))
                {
                    return i;
                }
            }
            throw new ArgumentOutOfRangeException();
        }
        internal class PathItem : IDisposable
        {
            public PathItem(PropertyInfo propertyInfo)
            {
                this.PropertyInfo = propertyInfo;
            }
            public INotifyPropertyChanged Source { get; set; }
            public PropertyInfo PropertyInfo { get; private set; }
            public IDisposable Subscription { get; set; }
            public bool IsLast { get; set; }
            public dynamic Value
            {
                get
                {
                    if (this.Source == null)
                    {
                        return null;
                    }
                    return this.PropertyInfo.GetMethod.Invoke(this.Source, null);
                }
            }
            public override string ToString()
            {
                return string.Format("{0}.{1}", this.Source != null ? this.Source.GetType().Name : "null", this.PropertyInfo.Name);
            }
            public void Dispose()
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }
            private void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if (this.Subscription != null)
                    {
                        this.Subscription.Dispose();
                        this.Subscription = null;
                    }
                }
            }
        }
    }
}