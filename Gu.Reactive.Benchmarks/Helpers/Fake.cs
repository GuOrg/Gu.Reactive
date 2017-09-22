namespace Gu.Reactive.Benchmarks
{
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;

    public class Fake : INotifyPropertyChanged, IFake
    {
        private bool isTrue;
        private Level next;
        private string name;
        private StructLevel structLevel;
        private int value;
        //// ReSharper disable once NotAccessedField.Local
        private int writeOnly;

        public event PropertyChangedEventHandler PropertyChanged;

        public int WriteOnly
        {
            set { this.writeOnly = value; }
        }

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

        public StructLevel StructLevel
        {
            get
            {
                return this.structLevel;
            }

            set
            {
                if (System.Collections.Generic.EqualityComparer<StructLevel>.Default.Equals(value, this.structLevel))
                {
                    return;
                }

                this.structLevel = value;
                this.OnPropertyChanged();
            }
        }

#pragma warning disable WPF1010 // Mutable public property should notify.
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
        public NotInpc NotInpc { get; private set; }
#pragma warning restore WPF1010 // Mutable public property should notify.

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

        public Level Method()
        {
            return this.Next;
        }

        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}