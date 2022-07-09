using LoLA.Networking.WebWrapper.DataDragon;
using System.Windows.Controls;
using System.Threading.Tasks;
using LoL_Assist_WAPP.Model;
using System.ComponentModel;
using System.Windows.Input;
using System.Threading;
using System.Windows;
using System;

namespace LoL_Assist_WAPP.ViewModel
{
    public class DownloadViewModel : INotifyPropertyChanged
    {
        public ICommand DownloadCommand { get; set; }
        public ICommand CancelDownloadCommand { get; set; }
        private ICommand _closeDownloadPanelCommand;
        public ICommand CloseDownloadPanelCommand
        {
            get
            {
                if (_closeDownloadPanelCommand == null)
                {
                    _closeDownloadPanelCommand = new Command(p => hideUserControl(p));
                }
                return _closeDownloadPanelCommand;
            }
        }

        private double _downloadProgress;
        public double DownloadProgress
        {
            get => _downloadProgress;
            set
            {
                if (_downloadProgress != value)
                {
                    _downloadProgress = value;
                    OnPropertyChanged(nameof(DownloadProgress));
                }
            }
        }

        private string _downloadPercentage;
        public string DownloadPercentage
        {
            get => _downloadPercentage;
            set
            {
                if (_downloadPercentage != value)
                {
                    _downloadPercentage = value;
                    OnPropertyChanged(nameof(DownloadPercentage));
                }
            }
        }

        private string _downloadStatus;
        public string DownloadStatus
        {
            get => _downloadStatus;
            set
            {
                if (_downloadStatus != value)
                {
                    _downloadStatus = value;
                    OnPropertyChanged(nameof(DownloadStatus));
                }
            }
        }

        private string _downloadedTotal;
        public string DownloadedTotal
        {
            get => _downloadedTotal;
            set
            {
                if (_downloadedTotal != value)
                {
                    _downloadedTotal = value;
                    OnPropertyChanged(nameof(DownloadedTotal));
                }
            }
        }
        private object _topMostContent = null;
        public object TopMostContent
        {
            get => _topMostContent;
            set
            {
                //Console.WriteLine("Content set: " + value);
                _topMostContent = value;
                OnPropertyChanged(nameof(TopMostContent));
            }
        }

        private Visibility _downloadigContainerVisibility;
        public Visibility DownloadigContainerVisibility
        {
            get => _downloadigContainerVisibility;
            set
            {
                if (_downloadigContainerVisibility != value)
                {
                    _downloadigContainerVisibility = value;
                    OnPropertyChanged(nameof(DownloadigContainerVisibility));
                }
            }
        }

        private Visibility _downloadSelectContainerVisibility;
        public Visibility DownloadSelectContainerVisibility
        {
            get => _downloadSelectContainerVisibility;
            set
            {
                if (_downloadSelectContainerVisibility != value)
                {
                    _downloadSelectContainerVisibility = value;
                    OnPropertyChanged(nameof(DownloadSelectContainerVisibility));
                }
            }
        }

        CancellationTokenSource cancellationTokenSource { get; set; }

        public DownloadViewModel()
        {
            DownloadSelectContainerVisibility = Visibility.Visible;
            DownloadigContainerVisibility = Visibility.Collapsed;
            cancellationTokenSource = new CancellationTokenSource();
            DownloadCommand = new Command(o => downloadChampionAssets());
            CancelDownloadCommand = new Command(o => { cancelChampionAssetsDownload(); });
        }

        private bool isDownloading = false;
        private void reportProgressChanged(object sender, ProgressReportModel e)
        {
            DownloadPercentage = $"{e.Percent}%";
            DownloadProgress = e.Percent;
            DownloadStatus = e.Status;
            DownloadedTotal = e.DownloadedTotal;
        }

        private void downloadChampionAssets()
        {
            cancellationTokenSource = new CancellationTokenSource();
            var progress = new Progress<ProgressReportModel>();
            progress.ProgressChanged += reportProgressChanged;
            if (!isDownloading)
            {
                isDownloading = true;
                DownloadigContainerVisibility = Visibility.Visible;
                DownloadSelectContainerVisibility = Visibility.Collapsed;
                downloadChampionAssetsWorker(progress, cancellationTokenSource.Token);
            }
        }

        private async void downloadChampionAssetsWorker(IProgress<ProgressReportModel> progress, CancellationToken cancellationToken)
        {
            int downloadedIndex = 0;
            var progressReportModel = new ProgressReportModel {
                Percent = 0,
                Status = "Starting...",
                DownloadedTotal = "0 of 0"
            };

            progress.Report(progressReportModel);

            await Task.Run(async() => {
                string leaguePatch = null;

                while(leaguePatch == null)
                {
                    if (DataDragonWrapper.s_Versions.Count > 0 
                    && DataDragonWrapper.s_Champions?.Data?.Values?.Count > 0)
                        leaguePatch = DataDragonWrapper.s_Versions[0];
                }

                try
                {
                    foreach (var championData in DataDragonWrapper.s_Champions.Data.Values)
                    {
                        var fixedName = Utils.Helper.FixedName(championData.name);
                        progressReportModel.Status = $"Downloading {fixedName} image - patch {leaguePatch}";

                        await DataDragonWrapper.GetChampionImage(championData.id);

                        cancellationToken.ThrowIfCancellationRequested();

                        downloadedIndex++;

                        progressReportModel.DownloadedTotal = $"{downloadedIndex} of {DataDragonWrapper.s_Champions.Data.Count}";

                        progressReportModel.Percent = (downloadedIndex * 100) / DataDragonWrapper.s_Champions.Data.Count;

                        progress.Report(progressReportModel);
                    }

                    progressReportModel.Status = "Download completed!";
                    progressReportModel.DownloadedTotal = null;
                    progress.Report(progressReportModel);

                    downloadWorkerCompleted();
                }
                catch (OperationCanceledException)
                {
                    resetDownloadStatus();
                }
            });
        }

        private void cancelChampionAssetsDownload() => cancellationTokenSource.Cancel();

        private void downloadWorkerCompleted()
        {
            DownloadStatus = "Download Completed!";
            isDownloading = false;
        }

        private void resetDownloadStatus()
        {
            DownloadSelectContainerVisibility = Visibility.Visible;
            DownloadigContainerVisibility = Visibility.Collapsed;
            DownloadStatus = null;
            DownloadProgress = 0;
            DownloadPercentage = null;
            isDownloading = false;
        }

        private void hideUserControl(object p)
        {
            if(!isDownloading)
            {
                Utils.Animation.FadeOut(p as UserControl);
                return;
            }
            showMsgBox(new Action(() => {
                cancellationTokenSource.Cancel();
                Utils.Animation.FadeOut(p as UserControl); }), 
            "A download is still in progress. Do you want to close the Download Panel and cancel the download?",
            width: 260, height: 155);
        }

        #region space junk
        private async void setTopMostContent(object obj)
        {
            TopMostContent = null;
            await Task.Delay(1); // Fix
            TopMostContent = obj;
        }
        private void showMsgBox(Action action, string msg, double width = 330, double height = 220)
        {
            setTopMostContent(new MessageBoxViewModel()
            {
                Message = msg,
                Width = width,
                Height = height,
                Action = () => { action.Invoke(); }
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}
