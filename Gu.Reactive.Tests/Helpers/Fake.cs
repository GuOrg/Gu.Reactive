namespace Gu.Reactive.Tests.Helpers
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using JetBrains.Annotations;

    public class Fake : IFake
    {
        private bool? isTrueOrNull;
        private bool isTrue;
        private Level next;
        private Level<int> nextInt;
        private string name;
        private Level1 level1;
        private StructLevel structLevel;
        private NotNotifying notNotifying;
        private int value;

        // ReSharper disable once NotAccessedField.Local
        private int writeOnly;

        public event PropertyChangedEventHandler PropertyChanged;

        public int WriteOnly
        {
            set
            {
                this.writeOnly = value;
            }
        }

        public bool? IsTrueOrNull
        {
            get
            {
                return this.isTrueOrNull;
            }

            set
            {
                if (value == this.isTrueOrNull)
                {
                    return;
                }

                this.isTrueOrNull = value;
                this.OnPropertyChanged();
            }
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

        public Level1 Level1
        {
            get
            {
                return this.level1;
            }

            set
            {
                if (ReferenceEquals(value, this.level1))
                {
                    return;
                }

                this.level1 = value;
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

        public Level<int> NextInt
        {
            get
            {
                return this.nextInt;
            }

            set
            {
                if (ReferenceEquals(value, this.nextInt))
                {
                    return;
                }

                this.nextInt = value;
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

        public NotNotifying NotNotifying
        {
            get
            {
                return this.notNotifying;
            }

            set
            {
                if (ReferenceEquals(value, this.notNotifying))
                {
                    return;
                }

                this.notNotifying = value;
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