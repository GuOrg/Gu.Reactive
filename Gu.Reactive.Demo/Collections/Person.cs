namespace Gu.Reactive.Demo
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using JetBrains.Annotations;

    public class Person : INotifyPropertyChanged
    {
        private string firstName;
        private string lastName;
        private IReadOnlyList<int> tagsValues;
        private string tags;

        public event PropertyChangedEventHandler PropertyChanged;

        public string FirstName
        {
            get { return this.firstName; }

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
            get { return this.lastName; }

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

        public string Tags
        {
            get { return this.tags; }

            private set
            {
                if (value == this.tags) return;
                this.tags = value;
                this.OnPropertyChanged();
            }
        }

        internal IReadOnlyList<int> TagsValues
        {
            get { return this.tagsValues; }

            set
            {
                if (Equals(value, this.tagsValues)) return;
                this.tagsValues = value;
                this.tags = String.Join(", ", this.tagsValues);
                this.OnPropertyChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}