namespace Gu.Reactive.Tests
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;

    using Moq;

    public static class NotifyPropertyChangedMockExt
    {
        public static void SetAndRaisePropertyChanged<TCLass, TProp>(this Mock<TCLass> mock, Expression<Func<TCLass, TProp>> prop, TProp returnValue)
            where TCLass : class, INotifyPropertyChanged
        {
            mock.Setup(prop)
                .Returns(returnValue);
            var name = ((MemberExpression)prop.Body).Member.Name;
            mock.Raise(x => x.PropertyChanged += null, new PropertyChangedEventArgs(name));
        }
    }
}
