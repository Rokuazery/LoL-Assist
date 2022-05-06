﻿using LoL_Assist_WAPP.ViewModel;
using System.Windows.Controls;
using LoL_Assist_WAPP.Model;
using System.Windows.Input;
using LoL_Assist_WAPP.View;
using System.Windows.Forms;
using System.Windows;
using System.IO;
using LoLA.LCU;
using System;
using LoLA;
using Newtonsoft.Json;
using LoLA.Objects;

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

        private void CloseBtn_Clicked(object sender, MouseButtonEventArgs e) => Close();
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
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
                    Utils.Animation.Margin(exitMsg, ConfigModel.marginOpen, new Thickness(0, Height, 0, 0), 0.13);
                };
                Animate(exitMsg, new Thickness(0, Height, 0, 0), ConfigModel.marginOpen, 0.13);
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
                        var championBuildConfig = JsonConvert.DeserializeObject<ChampionBD>(ImportFromPath);
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
            string gameMode = ImportGameModeList.SelectedValue.ToString();
            if (!string.IsNullOrEmpty(championId) && !string.IsNullOrEmpty(gameMode))
            {
                string filePath = Main.localBuild.DataPath(championId, Path.GetFileNameWithoutExtension(fileName), (GameMode)Enum.Parse(typeof(GameMode), gameMode));
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
    }
}
