using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AssemblyBrowserLib.Tests
{
    [TestClass]
    public class AssemblyBrowserTest
    {
        public IAssemblyBrowser assemblyBrowser = new AssemblyBrowser();

        [TestMethod]
        public void NamespaceTest()
        {
            var location = Assembly.GetExecutingAssembly().Location;
            var namespaces = assemblyBrowser.GetNamespaces(location);
            var _namespace = namespaces[0];
            var currentNamespace = Assembly.GetExecutingAssembly().GetTypes()[0].Namespace;
            Assert.AreEqual(_namespace.Name, currentNamespace);
        }

        [TestMethod]
        public void TypesTest()
        {
            var location = Assembly.GetExecutingAssembly().Location;
            var namespaces = assemblyBrowser.GetNamespaces(location);
            var _namespace = namespaces[0];
            var types = _namespace.Members;
            Assert.AreEqual(Assembly.GetExecutingAssembly().GetTypes().Length, types.Count);
        }
    }
}
