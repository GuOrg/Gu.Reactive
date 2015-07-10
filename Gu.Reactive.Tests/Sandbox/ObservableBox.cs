using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gu.Reactive.Tests.Sandbox
{
    using System.Data;
    using System.Data.SqlClient;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using NUnit.Framework;

    public class ObservableBox
    {
        [Test]
        public void TestName()
        {
            var observable = Observable.Create<int>(x =>
            {
                x.OnNext(1);
                return Disposable.Empty;
            });
            observable.Subscribe(Console.WriteLine);
            observable.Subscribe(Console.WriteLine);
        }
    }
}
