namespace Gu.Reactive.Tests.Helpers
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class Level3 : INotifyPropertyChanged
    {
        private string? name;
        private bool isTrue;
        private int value;
        private Level4? level4;

        public event PropertyChangedEventHandler? PropertyChanged;

        public Level4? Level4
        {
            get => this.level4;

            set
            {
                if (ReferenceEquals(value, this.level4))
                {
                    return;
                }

                this.level4 = value;
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

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
