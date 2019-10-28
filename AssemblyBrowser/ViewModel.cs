using AssemblyBrowserLib;
using Microsoft.VisualStudio.PlatformUI;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Input;

namespace AssemblyBrowser
{
    public class ViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<ContainerInfo> _containers;

        public ViewModel()
        {
            Containers = new ObservableCollection<ContainerInfo>();
        }

        public ObservableCollection<ContainerInfo> Containers { get { return _containers; } set { _containers = value; } }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand OpenFile { get { return new OpenFileCommand(() => { OpenAssembly(); }); } }

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
