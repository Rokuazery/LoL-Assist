using LoL_Assist_WAPP.ViewModels;
using System.Windows.Controls;
using LoL_Assist_WAPP.Models;
using LoL_Assist_WAPP.Utils;
using System.Diagnostics;
using System.Windows;
using System;
using LoLA;
using System.IO;

namespace LoL_Assist_WAPP
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(DependencyObject), new FrameworkPropertyMetadata(int.MaxValue));
            ToolTipService.InitialShowDelayProperty.OverrideMetadata(typeof(DependencyObject), new FrameworkPropertyMetadata(350));
        }

            #if DEBUG
                private const int MAX_INSTANCES = 2;
            #else
                private const int MAX_INSTANCES = 1;
            #endif

        protected override void OnStartup(StartupEventArgs e)
        {
            RuneModel.Init();

            #if DEBUG
                //Console.Title = "LoL Assist - Debug Console";
                GlobalConfig.s_Debug = true;
            #else
                GlobalConfig.s_Debug = false;
            #endif

            if (!GlobalConfig.s_Debug)
            {
                var handle = Helper.GetConsoleWindow();
                Helper.ShowWindow(handle, Helper.SW_HIDE);
            }

            var proccName = Process.GetCurrentProcess().ProcessName;
            if (Process.GetProcessesByName(proccName).Length > MAX_INSTANCES)
            {
                MessageBox.Show("LoL Assist is already running!", "LoL Assist",
                MessageBoxButton.OK, MessageBoxImage.Warning);
                Environment.Exit(0);
            }

            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();

            base.OnStartup(e);
        }
    }
}
