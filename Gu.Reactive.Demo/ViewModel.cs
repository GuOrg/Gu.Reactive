namespace Gu.Reactive.Demo
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Gu.Reactive.Demo.Annotations;

    public class ViewModel : INotifyPropertyChanged
    {
        private bool isOk1;

        private bool isOk2;

        public ViewModel()
        {
            this.Condition1 = new Condition(this.ToObservable(x => x.IsOk1), () => this.IsOk1) { Name = "Condition1" };
            this.Condition2 = new Condition(this.ToObservable(x => x.IsOk2), () => this.IsOk1) { Name = "Condition2" };
            DependingCondition = new AndCondition(Condition1, Condition2){Name = "Depending"};
            NegatedCondition1 = Condition1.Negate();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsOk1
        {
            get
            {
                return this.isOk1;
            }
            set
            {
                if (value.Equals(this.isOk1))
                {
                    return;
                }
                this.isOk1 = value;
                this.OnPropertyChanged();
            }
        }

        public bool IsOk2
        {
            get
            {
                return this.isOk2;
            }
            set
            {
                if (value.Equals(this.isOk2))
                {
                    return;
                }
                this.isOk2 = value;
                this.OnPropertyChanged();
            }
        }

        public Condition Condition1 { get; private set; }
        public Condition Condition2 { get; private set; }
        public Condition DependingCondition { get; private set; }
        public ICondition NegatedCondition1 { get; private set; }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
