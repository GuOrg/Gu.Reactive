namespace Gu.Reactive.Demo
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class Person : INotifyPropertyChanged
    {
        private string firstName;
        private string lastName;
        private IReadOnlyList<int> tagsValues;
        private string tags;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Tags => this.tags;

        public string FirstName
        {
            get => this.firstName;

            set
            {
                if (value == this.firstName)
                {
                    return;
                }

                this.firstName = value;
                this.OnPropertyChanged();
            }
        }

        public string LastName
        {
            get => this.lastName;

            set
            {
                if (value == this.lastName)
                {
                    return;
                }

                this.lastName = value;
                this.OnPropertyChanged();
            }
        }

        internal IReadOnlyList<int> TagsValues
        {
            get => this.tagsValues;

            set
            {
                if (Equals(value, this.tagsValues))
                {
                    return;
                }

                this.tagsValues = value;
                this.tags = string.Join(", ", this.tagsValues);
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(this.Tags));
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}