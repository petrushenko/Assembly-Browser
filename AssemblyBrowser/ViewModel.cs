using AssemblyBrowserLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyBrowser
{
    public class ViewModel
    {
        public static ObservableCollection<ContainerInfo> Containers = new ObservableCollection<ContainerInfo>();
    }
}
