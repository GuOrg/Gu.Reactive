namespace Gu.Reactive
{
    using System;
    using System.ComponentModel;
    using System.Reflection;

    internal class PathItem : IDisposable
    {
        private readonly WeakReference _sourceRef = new WeakReference(null);

        /// <summary>
        /// Initializes a new instance of the <see cref="PathItem"/> class.
        /// </summary>
        /// <param name="propertyInfo">
        /// The property info.
        /// </param>
        public PathItem(PathItem previous, PropertyInfo propertyInfo)
        {
            this.Previous = previous;
            if (previous != null)
            {
                previous.Next = this;
            }
            this.PropertyInfo = propertyInfo;
        }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        public INotifyPropertyChanged Source
        {
            get
            {
                return (INotifyPropertyChanged)this._sourceRef.Target;
            }
            set
            {
                this._sourceRef.Target = value;
            }
        }

        public PathItem Next { get; private set; }

        public PathItem Previous { get; private set; }

        /// <summary>
        /// Gets the property info.
        /// </summary>
        public PropertyInfo PropertyInfo { get; private set; }

        /// <summary>
        /// Gets or sets the subscription.
        /// </summary>
        public IDisposable Subscription { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is last.
        /// </summary>
        public bool IsLast
        {
            get { return Next == null; }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public object Value
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

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}.{1}", this.Source != null ? this.Source.GetType().Name : "null", this.PropertyInfo.Name);
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
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