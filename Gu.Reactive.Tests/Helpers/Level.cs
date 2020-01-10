namespace Gu.Reactive.Tests.Helpers
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class Level : INotifyPropertyChanged
    {
        private bool isTrue;
        private Level? next;
        private Level<int>? nextInt;
        private NotNotifying? notNotifying;
        private string? name;

        private bool? isTrueOrNull;

        private int value;
        private int value1;
        private int value2;

        public event PropertyChangedEventHandler? PropertyChanged;

        public bool IsTrue
        {
            get => this.isTrue;

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

        public bool? IsTrueOrNull
        {
            get => this.isTrueOrNull;

            set
            {
                if (Equals(value, this.isTrueOrNull))
                {
                    return;
                }

                this.isTrueOrNull = value;
                this.OnPropertyChanged();
            }
        }

        public int Value
        {
            get => this.value;

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

        public int Value1
        {
            get => this.value1;

            set
            {
                if (value == this.value1)
                {
                    return;
                }

                this.value1 = value;
                this.OnPropertyChanged();
            }
        }

        public int Value2
        {
            get => this.value2;

            set
            {
                if (value == this.value2)
                {
                    return;
                }

                this.value2 = value;
                this.OnPropertyChanged();
            }
        }

        public string? Name
        {
            get => this.name;

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

        public Level? Next
        {
            get => this.next;

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

        public Level<int>? NextInt
        {
            get => this.nextInt;

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

        public NotNotifying? NotNotifying
        {
            get => this.notNotifying;

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

        public Level Method()
        {
            return this.Next;
        }

        public virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
