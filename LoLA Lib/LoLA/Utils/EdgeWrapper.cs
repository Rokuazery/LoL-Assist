using LogType = LoLA.Utils.Logger.LogType;
using OpenQA.Selenium.Support.UI;
using System.Threading.Tasks;
using System.IO.Compression;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium;
using LoLA.Utils.Logger;
using System.Web;
using System.IO;
using System;

namespace LoLA.Utils
{
    public static class EdgeWrapper
    {
        private static EdgeOptions edgeOptions = new EdgeOptions();
        public static async void InitEdge()
        {
            LogService.Log(LogService.Model("Initializing EdgeDriver...", Global.name, LogType.INFO));

            edgeOptions.AddArgument("headless");
            edgeOptions.AddArgument("disable-gpu");
            edgeOptions.AddArgument("log-level=3");

            var folderName = "driver";
            var fileName = "msedgedriver.exe";
            var zipFile = "edgedriver_win64.zip";

            if (!Directory.Exists(folderName))
                Directory.CreateDirectory(folderName);

            if(!File.Exists(Path.Combine(folderName,fileName)))
            {
                var edgePath = folderName;
                var zipPath = Path.Combine(folderName, zipFile);

                LogService.Log(LogService.Model("Downloading EdgeDriver...", Global.name, LogType.INFO));
                var webModel = new WebModel() {
                    path = zipPath,
                    url = "https://msedgedriver.azureedge.net/100.0.1165.0/edgedriver_win64.zip"
                };
                
                await WebExt.RunDownloadAysnc(webModel);
                ZipFile.ExtractToDirectory(zipPath, edgePath);
                File.Delete(zipPath);
            }
        }

        public static async Task<string> getPageSource(string url)
        {
            string pageSource = null;
            await Task.Run(() => {
                using (IWebDriver driver = new EdgeDriver("./Driver", edgeOptions))
                {
                    WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                    driver.Navigate().GoToUrl(url);
                    pageSource = HttpUtility.HtmlDecode(driver.PageSource);
                }
            });
            return pageSource;
        }
    }
}
