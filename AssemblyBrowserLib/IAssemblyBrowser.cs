using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyBrowserLib
{
    interface IAssemblyBrowser
    {
        ContainerInfo[] GetNamespaces(string assemblyPath);
    }
}
