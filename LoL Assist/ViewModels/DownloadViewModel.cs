using LoLA.Networking.WebWrapper.DataDragon;
using LoL_Assist_WAPP.Commands;
using System.Windows.Controls;
using System.Threading.Tasks;
using LoL_Assist_WAPP.Models;
using System.ComponentModel;
using System.Windows.Input;
using System.Threading;
using System.Windows;
using System;

namespace LoL_Assist_WAPP.ViewModels
{
    public class DownloadViewModel: ViewModelBase
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
                    _closeDownloadPanelCommand = new Command(p => HideUserControl(p));
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

        private Visibility _cancelBtnVisibility;
        public Visibility CancelBtnVisibility
        {
            get => _cancelBtnVisibility;
            set
            {
                if (_cancelBtnVisibility != value)
                {
                    _cancelBtnVisibility = value;
                    OnPropertyChanged(nameof(CancelBtnVisibility));
                }
            }
        }


        CancellationTokenSource cancellationTokenSource { get; set; }

        public DownloadViewModel()
        {
            DownloadSelectContainerVisibility = Visibility.Visible;
            DownloadigContainerVisibility = Visibility.Collapsed;
            cancellationTokenSource = new CancellationTokenSource();
            DownloadCommand = new Command(key => Download(key.ToString()));
            CancelDownloadCommand = new Command(_ => { CancelChampionAssetsDownload(); });
        }

        private bool isDownloading = false;
        private void ReportProgressChanged(object sender, ProgressReportModel e)
        {
            DownloadPercentage = $"{e.Percent}%";
            DownloadProgress = e.Percent;
            DownloadStatus = e.Status;
            DownloadedTotal = e.DownloadedTotal;
        }
        Progress<ProgressReportModel> progress = new Progress<ProgressReportModel>();
        private void Download(string key)
        {
            if (!isDownloading)
            {
                isDownloading = true;
                cancellationTokenSource = new CancellationTokenSource();
                progress = new Progress<ProgressReportModel>();
                progress.ProgressChanged += ReportProgressChanged;

                DownloadigContainerVisibility = Visibility.Visible;
                DownloadSelectContainerVisibility = Visibility.Collapsed;

                DownloadChampionAssetsWorker(progress, cancellationTokenSource.Token);
            }
        }

        private async void DownloadChampionAssetsWorker(IProgress<ProgressReportModel> progress, CancellationToken cancellationToken)
        {
            int downloadedIndex = 0;
            var progressReportModel = new ProgressReportModel {
                Percent = 0,
                Status = "Starting...",
                DownloadedTotal = "0 of 0"
            };

            progress.Report(progressReportModel);

            await Task.Run(async () =>
            {
                string leaguePatch = null;

                while(leaguePatch == null)
                {
                    if (DataDragonWrapper.s_Patches.Count > 0
                    && DataDragonWrapper.s_Champions?.Data?.Values?.Count > 0)
                        leaguePatch = DataDragonWrapper.s_Patches[0];
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

                    DownloadWorkerCompleted();
                }
                catch (OperationCanceledException)
                {
                    ResetDownloadStatus();
                }
            });
        }

        private void CancelChampionAssetsDownload() 
            => cancellationTokenSource.Cancel();

        private void DownloadWorkerCompleted()
        {
            progress.ProgressChanged -= ReportProgressChanged;
            CancelBtnVisibility = Visibility.Hidden;
            DownloadStatus = "Download Completed!";
            isDownloading = false;
        }

        private void ResetDownloadStatus()
        {
            progress.ProgressChanged -= ReportProgressChanged;
            DownloadSelectContainerVisibility = Visibility.Visible;
            DownloadigContainerVisibility = Visibility.Collapsed;
            DownloadStatus = null;
            DownloadProgress = 0;
            DownloadPercentage = null;
            isDownloading = false;
        }

        private void HideUserControl(object p)
        {
            if(!isDownloading)
            {
                Utils.Animation.FadeOut(p as UserControl);
                return;
            }

            ShowMsgBox(new Action(() =>
            {
                cancellationTokenSource.Cancel();
                Utils.Animation.FadeOut(p as UserControl); }), 
            "A download is still in progress. Do you want to close the Download Panel and cancel the download?",
            width: 260, height: 155);
        }

        #region space junk
        private async void SetTopMostContent(object obj)
        {
            TopMostContent = null;
            await Task.Delay(1); // Fix
            TopMostContent = obj;
        }
        private void ShowMsgBox(Action action, string msg, double width = 330, double height = 220)
        {
            SetTopMostContent(new MessageBoxModel()
            {
                Message = msg,
                Width = width,
                Height = height,
                Action = () => { action.Invoke(); }
            });
        }
        #endregion
    }
}
