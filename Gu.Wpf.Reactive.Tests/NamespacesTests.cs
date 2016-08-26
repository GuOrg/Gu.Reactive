namespace Gu.Wpf.Reactive.Tests
{
    using System;
    using System.Linq;
    using System.Windows.Markup;

    using Gu.Reactive;

    using NUnit.Framework;

    public class NamespacesTests
    {
        private const string Uri = @"http://Gu.com/Reactive";

        [Test]
        public void XmlnsDefinitions()
        {
            string[] skip = { "Annotations", "Properties", "XamlGeneratedNamespace", "Internals", "TypeConverters", "PropertyPathStuff" };
            var assemblies = new[] { typeof(ConditionControl).Assembly, typeof(ICondition).Assembly };

            var strings = assemblies.SelectMany(x => x.GetTypes())
                                  .Select(x => x.Namespace)
                                  .Where(x => x != null)
                                  .Distinct()
                                  .Where(x => !skip.Any(x.EndsWith))
                                  .OrderBy(x => x)
                                  .ToArray();
            var attributes = typeof(ConditionControl).Assembly.CustomAttributes.Where(x => x.AttributeType == typeof(XmlnsDefinitionAttribute))
                                     .ToArray();
            var actuals = attributes.Select(a => a.ConstructorArguments[1].Value)
                                                             .OrderBy(x => x);
            foreach (var s in strings)
            {
                Console.WriteLine(@"[assembly: XmlnsDefinition(""{0}"", ""{1}"")]", Uri, s);
            }
            CollectionAssert.AreEqual(strings, actuals);
            foreach (var attribute in attributes)
            {
                Assert.AreEqual(Uri, attribute.ConstructorArguments[0].Value);
            }
        }

        [Test]
        public void XmlnsPrefix()
        {
            var assembly = typeof(ConditionControl).Assembly;
            var prefixAttribute = assembly.CustomAttributes.Single(x => x.AttributeType == typeof(XmlnsPrefixAttribute));
            Assert.AreEqual(Uri, prefixAttribute.ConstructorArguments[0].Value);
        }
    }
}
