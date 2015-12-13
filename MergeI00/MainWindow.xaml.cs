using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
using Microsoft.WindowsAPICodePack.Dialogs;
using Path = System.IO.Path;

namespace MergeI00
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<MergeJob> MergeJobs { get; set; }


        private bool canStart;
        public bool CanStart
        {
            get { return canStart; }
            set
            {
                canStart = value;
                OnPropertyChanged();
            }
        }

        public MainWindow()
        {
            MergeJobs = new ObservableCollection<MergeJob>();
            DataContext = this;
            InitializeComponent();
            
            SelectFolder();
        }
        
        private void SelectFolder()
        {
            if (!CommonFileDialog.IsPlatformSupported) return;

            var folderDialog = new CommonOpenFileDialog
            {
                EnsureReadOnly = true,
                IsFolderPicker = true,
                AllowNonFileSystemItems = false,
                Multiselect = true,
                InitialDirectory = "D:\\",
                Title = "Select Folder(s) to Merge"
            };

            if (folderDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                foreach (var fileName in folderDialog.FileNames)
                {
                    var parts = new List<string>(Directory.GetFiles(fileName, "*.I*"));
                    MergeJobs.Add(new MergeJob
                    {
                        IsoName = string.Format("{0}.iso", Path.GetFileName(fileName)),
                        Parts = parts,
                        Count = 0,
                        Total = parts.Count,
                    });
                }
                CanStart = true;
            }
        }

        private async void StartBtn_OnClick(object sender, RoutedEventArgs e)
        {
            CanStart = false;

            await Task.Run(() =>
            {
                foreach (var mergeJob in MergeJobs)
                {
                    MergeParts(mergeJob, string.Format("D:\\{0}", mergeJob.IsoName));
                }
            });

            MessageBox.Show(this, "Merge Jobs Done!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void MergeParts(MergeJob mergeJob, string destFile)
        {
            using (var destStream = File.Create(destFile))
            {
                foreach (var filePath in mergeJob.Parts)
                {
                    using (var sourceStream = File.OpenRead(filePath))
                    {
                        sourceStream.CopyTo(destStream);
                    }

                    this.Dispatcher.InvokeAsync(() =>
                    {
                        mergeJob.Count++;
                    });
                }
            }
        }
    }
}
