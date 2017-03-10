namespace Gu.Reactive.Benchmarks
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using JetBrains.Annotations;

    public class Level : INotifyPropertyChanged
    {
        private bool isTrue;
        private Level next;
        private string name;
        private int value;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsTrue
        {
            get
            {
                return this.isTrue;
            }

            set
            {
                if (value.Equals(this.isTrue))
                {
                    return;
                }

                this.isTrue = value;
                this.OnPropertyChanged();
            }
        }

        public int Value
        {
            get
            {
                return this.value;
            }

            set
            {
                if (value == this.value)
                {
                    return;
                }

                this.value = value;
                this.OnPropertyChanged();
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                if (value == this.name)
                {
                    return;
                }

                this.name = value;
                this.OnPropertyChanged();
            }
        }

        public Level Next
        {
            get
            {
                return this.next;
            }

            set
            {
                if (Equals(value, this.next))
                {
                    return;
                }

                this.next = value;
                this.OnPropertyChanged();
            }
        }

#pragma warning disable WPF1010 // Mutable public property should notify.
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        public NotInpc NotInpc { get; private set; }
#pragma warning restore WPF1010 // Mutable public property should notify.

        public Level Method()
        {
            return this.Next;
        }

        [NotifyPropertyChangedInvocator]
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}