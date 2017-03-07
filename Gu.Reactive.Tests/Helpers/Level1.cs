namespace Gu.Reactive.Tests.Helpers
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class Level1 : INotifyPropertyChanged
    {
        private string name;
        private bool isTrue;
        private int value;
        private Level2 level2;

        public event PropertyChangedEventHandler PropertyChanged;

        public Level2 Level2
        {
            get
            {
                return this.level2;
            }

            set
            {
                if (ReferenceEquals(value, this.level2))
                {
                    return;
                }

                this.level2 = value;
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

        public bool IsTrue
        {
            get
            {
                return this.isTrue;
            }

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

        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}