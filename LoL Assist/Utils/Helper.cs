using System.Runtime.InteropServices;
using LoL_Assist_WAPP.Models;
using LoLA.Utils.Logger;
using Newtonsoft.Json;
using System.IO;
using System;
using LoLA;
using System.Windows;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace LoL_Assist_WAPP.Utils
{
    public static class Helper
    {
        #region console
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public const int SW_HIDE = 0;
        public const int SW_SHOW = 5;
        #endregion

        #region LogService
        public static void Log(string msg, LogType logType) => LogService.Log(LogService.Model(msg, LibInfo.NAME + " - WAPP", logType));
        #endregion

        #region misc
        public static void ShowBuildEditorWindow()
        {
            var window = new BuildEditorWindow();
            window.Owner = Application.Current.MainWindow;
            window.ShowDialog();
        }

        public static ItemImageModel ItemImage(string text, string image)
        {
            var model = new ItemImageModel()
            {
                Text = text,
                Image = image
            };
            return model;
        }

        public static string FixedName(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                if (name.Contains("'"))
                    return name;
                else return name + "'s";
            }
            return "Jeff's";
        }

        public static string ImageSrc(string name, string format = "png")
        {
            return $"{ConfigModel.RESOURCE_PATH}{name.Replace(":", string.Empty)}.{format}";
        }

        public static string GetLine(string text, int lineNo)
        {
            string[] lines = text.Replace("\r", "").Split('\n');
            return lines.Length >= lineNo ? lines[lineNo - 1] : null;
        }

        #endregion

        public static async Task WhenAllWithCancellation(IEnumerable<Task> tasks, CancellationToken cancellationToken)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            using (cancellationToken.Register(() => taskCompletionSource.TrySetCanceled()))
            {
                var completedTask = await Task.WhenAny(Task.WhenAll(tasks), taskCompletionSource.Task);

                await completedTask; // Propagate any exceptions

                cancellationToken.ThrowIfCancellationRequested();
            }
        }
    }
}
