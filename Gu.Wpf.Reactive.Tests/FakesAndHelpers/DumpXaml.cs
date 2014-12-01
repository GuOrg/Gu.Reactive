namespace Gu.Wpf.Reactive.Tests.FakesAndHelpers
{
    using System.Xml.Linq;

    using NUnit.Framework;

    public class DumpXaml
    {
        [Test]
        public void DumpTaskCompletion()
        {
            var propertyInfos = typeof(NotifyTaskCompletion).GetProperties();
            var grid = new XElement( "Grid");
            var rowDefs = new XElement("RowDefinitions");
            grid.Add(rowDefs);
        }
    }
}
