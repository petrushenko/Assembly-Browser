using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AssemblyBrowserLib;

namespace AssemblyBrowser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            AssemblyBrowserLib.AssemblyBrowser ab = new AssemblyBrowserLib.AssemblyBrowser();
            var nm = ab.GetNamespaces(@"D:\bsuir\C#\projects\ConsoleApp1\ConsoleApp2\bin\Debug\ConsoleApp2.exe");
            ObservableCollection<ContainerInfo> oc = new ObservableCollection<ContainerInfo>(nm);
            ViewModel.Containers = oc;
            membersTree.ItemsSource = ViewModel.Containers;
        }
    }
}
