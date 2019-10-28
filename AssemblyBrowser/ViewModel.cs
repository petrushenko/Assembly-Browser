using AssemblyBrowserLib;
using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AssemblyBrowser
{
    public class ViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<ContainerInfo> _containers;

        public ViewModel()
        {

        }

        public ObservableCollection<ContainerInfo> Containers { get { return _containers; } set { _containers = value; OnPropertyChanged(nameof(Containers)); } }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public DelegateCommand OpenFile { get; }

        public void OpenAssembly()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Assemblies|*.dll;*.exe",
                Title = "Select assembly",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                AssemblyBrowserLib.AssemblyBrowser assemblyBrowser = new AssemblyBrowserLib.AssemblyBrowser();
                Containers = new ObservableCollection<ContainerInfo>(assemblyBrowser.GetNamespaces(openFileDialog.FileName));

                OnPropertyChanged(nameof(Containers));
            }
        }
    }
}
