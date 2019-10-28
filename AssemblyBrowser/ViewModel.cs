﻿using AssemblyBrowserLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Input;

namespace AssemblyBrowser
{
    public class ViewModel : INotifyPropertyChanged
    {

        private readonly AssemblyBrowserLib.AssemblyBrowser _model = new AssemblyBrowserLib.AssemblyBrowser();
        private string _openedFile;

        public ViewModel()
        {
            Containers = new List<ContainerInfo>();
        }

        public List<ContainerInfo> Containers { get; set; }

        public string OpenedFile
        {
            get
            {
                return _openedFile;
            }
            set
            {
                _openedFile = value;
                try
                {
                    Containers = new List<ContainerInfo>(_model.GetNamespaces(value));
                }
                catch (Exception e)
                {
                    _openedFile = string.Format("Error: [{0}]", e.Message);
                }
                OnPropertyChanged(nameof(Containers));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand OpenFile { get { return new OpenFileCommand(() => { OpenAssembly(); }); } }

        public void OpenAssembly()
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Assemblies|*.dll;*.exe";
                openFileDialog.Title = "Select assembly";
                openFileDialog.Multiselect = false;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    OpenedFile = openFileDialog.FileName;
                    OnPropertyChanged(nameof(OpenedFile));
                }
            }
        }
    }
}
