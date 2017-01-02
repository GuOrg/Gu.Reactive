namespace Gu.Reactive.Benchmarks
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using JetBrains.Annotations;

    public class Fake : INotifyPropertyChanged, IFake
    {
        private bool isTrue;
        private Level next;
        private string name;

        private StructLevel structLevel;

        private int value;

        public event PropertyChangedEventHandler PropertyChanged;

        public int WriteOnly { set { return; } }

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
                this.structLevel = value;
                this.OnPropertyChanged();
            }
        }

        public NotInpc NotInpc { get; private set; }

        public int Value
        {
            get
            {
                return this.value;
            }

            set
            {
                this.value = value;
                this.OnPropertyChanged();
            }
        }

        public Level Method()
        {
            return this.Next;
        }

        [NotifyPropertyChangedInvocator]
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}