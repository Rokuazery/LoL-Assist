using LoL_Assist_WAPP.ViewModels;
using LoLA.Networking.LCU.Enums;
using System.Windows.Controls;
using LoL_Assist_WAPP.Models;
using LoL_Assist_WAPP.Views;
using System.Windows.Input;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Windows;
using LoLA.Data;
using System.IO;
using System;
using LoLA;

namespace LoL_Assist_WAPP
{
    /// <summary>
    /// Interaction logic for BuildEditorWindow.xaml
    /// </summary>
    public partial class BuildEditorWindow : Window
    {
        BuildEditorViewModel viewModel = new BuildEditorViewModel();
        public BuildEditorWindow()
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if(!string.IsNullOrEmpty(viewModel.SelectedBuildName))
            {
                MsgBox exitMsg = new MsgBox($"Are you sure you want to delete '{viewModel.SelectedBuildName}'?", 260, 160);
                exitMsg.Margin = new Thickness(0, Height, 0, 0);
                MainGrid.Children.Add(exitMsg);
                Grid.SetRowSpan(exitMsg, 3);
                exitMsg.Decided += delegate (bool result)
                {
                    if (result)
                        viewModel.DeleteConfigExecute();

                    Utils.Animation.FadeOut(BackDrop, 0.13);
                    Utils.Animation.Margin(exitMsg, ConfigModel.r_MarginOpen, new Thickness(0, Height, 0, 0), 0.13);
                };
                Animate(exitMsg, new Thickness(0, Height, 0, 0), ConfigModel.r_MarginOpen, 0.13);
            }
        }

        private string ImportFromPath;
        private void SearchFileGrid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Title = "LoL Assist | Search a Runes & Spells Configuration File";
            fileDialog.Filter = "Json files (*.json)|*.json";
            fileDialog.Multiselect = false;
            fileDialog.CheckFileExists = true;
            DialogResult result = fileDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                ImportFromPath = fileDialog.FileName;

                using(var reader = new StreamReader(fileDialog.FileName))
                {
                    try
                    {
                        var championBuildConfig = JsonConvert.DeserializeObject<ChampionBuild>(ImportFromPath);
                        if (championBuildConfig != null)
                            AnimateOpen(ImportPanel);
                    }
                    catch { saveStatus.Text = "Invalid Build Config"; }
                    
                }
            }
        }

        private void CancelImportBtn_Click(object sender, RoutedEventArgs e) => AnimateClose(ImportPanel);
        private void ImportBtn_Click(object sender, RoutedEventArgs e)
        {
            var fileName = Path.GetFileName(ImportFromPath);
            string championId = ImportChampionList.SelectedValue.ToString();

            GameMode gameMode = ImportGameModeList.SelectedValue == null ? GameMode.NONE :
            (GameMode)Enum.Parse(typeof(GameMode), ImportGameModeList.SelectedValue.ToString());

            if (!string.IsNullOrEmpty(championId) && gameMode != GameMode.NONE)
            {
                string filePath = LocalBuild.DataPath(championId, Path.GetFileNameWithoutExtension(fileName), gameMode);
                File.Copy(ImportFromPath, filePath, true);
                viewModel.SelectedChampion = championId;
                viewModel.SelectedGameMode = gameMode;
                viewModel.SelectedBuildName = fileName;
                viewModel.FetchBuild();
                AnimateClose(ImportPanel);
            }
            ImportChampionList.SelectedValue = null;
            ImportGameModeList.SelectedValue = null;
        }

        #region Animations

        private void Animate(FrameworkElement element, Thickness from, Thickness to, double time = 0.2)
        {
            Utils.Animation.FadeIn(BackDrop, time);
            Utils.Animation.Margin(element, from, to, time);
        }

        private void AnimateOpen(FrameworkElement element, double time = 0.13)
        {
            Utils.Animation.FadeIn(BackDrop, time);
            Utils.Animation.FadeIn(element, time);
        }
        private void AnimateClose(FrameworkElement element, double time = 0.13)
        {
            Utils.Animation.FadeOut(BackDrop, time);
            Utils.Animation.FadeOut(element, time);
        }
        #endregion

        private void CloseBtn_Click(object sender, RoutedEventArgs e) => Close();
    }
}
