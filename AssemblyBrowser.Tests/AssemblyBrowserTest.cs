using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AssemblyBrowserLib.Tests
{
    [TestClass]
    public class AssemblyBrowserTest
    {
        [TestMethod]
        public void Test()
        {
            var assemblyBrowser = new AssemblyBrowser();
            var str = assemblyBrowser.GetNamespaces(@"D:\bsuir\C#\projects\Faker\Faker\bin\Debug\net472\Faker.dll");
        }
    }
}
