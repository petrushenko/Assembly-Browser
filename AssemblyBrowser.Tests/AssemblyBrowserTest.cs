using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AssemblyBrowserLib.Tests
{
    [TestClass]
    public class AssemblyBrowserTest
    {
        //TODO: add more tests
        [TestMethod]
        public void Test()
        {
            var assemblyBrowser = new AssemblyBrowser();
            var str = assemblyBrowser.GetNamespaces(@"D:\bsuir\C#\projects\ConsoleApp1\ConsoleApp2\bin\Debug\ConsoleApp2.exe");
        }
    }
}
