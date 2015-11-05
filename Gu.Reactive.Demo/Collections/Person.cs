namespace Gu.Reactive.Demo
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using JetBrains.Annotations;

    public class Person : INotifyPropertyChanged
    {
        private string _firstName;
        private string _lastName;
        private IReadOnlyList<int> _tagsValues;
        private string _tags;

        public event PropertyChangedEventHandler PropertyChanged;

        public string FirstName
        {
            get { return _firstName; }
            set
            {
                if (value == _firstName)
                {
                    return;
                }
                _firstName = value;
                OnPropertyChanged();
            }
        }

        public string LastName
        {
            get { return _lastName; }
            set
            {
                if (value == _lastName)
                {
                    return;
                }
                _lastName = value;
                OnPropertyChanged();
            }
        }

        public string Tags
        {
            get { return _tags; }
            private set
            {
                if (value == _tags) return;
                _tags = value;
                OnPropertyChanged();
            }
        }

        internal IReadOnlyList<int> TagsValues
        {
            get { return _tagsValues; }
            set
            {
                if (Equals(value, _tagsValues)) return;
                _tagsValues = value;
                _tags = String.Join(", ", _tagsValues);
                OnPropertyChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}