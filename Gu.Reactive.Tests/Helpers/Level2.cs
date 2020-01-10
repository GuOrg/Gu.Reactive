namespace Gu.Reactive.Tests.Helpers
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class Level2 : INotifyPropertyChanged
    {
        private string name;
        private bool isTrue;
        private int value;
        private Level3 level3;

        public event PropertyChangedEventHandler? PropertyChanged;

        public Level3 Level3
        {
            get => this.level3;

            set
            {
                if (ReferenceEquals(value, this.level3))
                {
                    return;
                }

                this.level3 = value;
                this.OnPropertyChanged();
            }
        }

        public string Name
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

        public bool IsTrue
        {
            get => this.isTrue;

            set
            {
                if (value == this.isTrue)
                {
                    return;
                }

                this.isTrue = value;
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

        public virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    class C
    {
        private string p;

        public string P
        {
            get => this.p;
            set => this.p = value;
        }
    }
}
