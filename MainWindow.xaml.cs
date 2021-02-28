using Newtonsoft.Json;
using NLog;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
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
using System.Xml;
using Path = System.IO.Path;

namespace AutoMuteUs_Portable
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public static Main main;

        private static string LogFilePath;

        public MainWindow()
        {
            InitializeComponent();
            InitializeNLog();
#if PUBLISH
            CheckUpdate();
#endif
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() => {
                Settings.LoadSettings();
                main = new Main(this);
            });
        }

        private string GetLogLevelColor(string logLevel)
        {
            switch (logLevel)
            {
                case "Trace":
                    return "#808080";
                case "Debug":
                    return "#C0C0C0";
                case "Info":
                    return "#FFFFFF";
                case "Warn":
                    return "#FF00FF";
                case "Error":
                    return "#FFFF00";
                case "Fatal":
                    return "FF0000";
                default:
                    return "";
            }
        }

        private Paragraph CreateColoredLogParagraph(LogEventInfo logEvent)
        {
            var LogLevelColor = GetLogLevelColor(logEvent.Level.Name);
            var LoggerNameColor = "#2bc454";
            var EventMessageColor = LogLevelColor;

            Paragraph paragraph = new Paragraph();
            Run run;

            // LoggerName
            run = new Run($"[{logEvent.LoggerName}] ");
            run.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(LoggerNameColor));
            paragraph.Inlines.Add(run);
            paragraph.LineHeight = 1;

            // LogLevel
            run = new Run($"[{logEvent.Level.Name}] ");
            run.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(LogLevelColor));
            paragraph.Inlines.Add(run);
            paragraph.LineHeight = 1;

            // Event Message
            run = new Run(logEvent.Message);
            run.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(EventMessageColor));
            paragraph.Inlines.Add(run);
            paragraph.LineHeight = 1;

            return paragraph;
        }

        private void mainWriteLogText(object sender, LogEventInfo logEvent)
        {
            if (logEvent.LoggerName != "Main") return;

            var textBox = mainLogTextBox;
            mainLogTextBox.Dispatcher.BeginInvoke((Action)(() =>
            {
                textBox.Document.Blocks.Add(CreateColoredLogParagraph(logEvent));
                textBox.ScrollToEnd();
            }));
        }

        private void detailedWriteLogText(object sender, LogEventInfo logEvent)
        {
            if (logEvent.LoggerName == "Main") return;

            var textBox = detailedLogTextBox;
            detailedLogTextBox.Dispatcher.BeginInvoke((Action)(() =>
            {
                textBox.Document.Blocks.Add(CreateColoredLogParagraph(logEvent));
                textBox.ScrollToEnd();
            }));
        }

        private void InitializeRichTextBoxTarget(RichTextBox richTextBox, EventHandler<LogEventInfo> writeLogText)
        {
            richTextBox.Document.Blocks.Clear();

            var target = new RichTextBoxTarget(richTextBox.Name, LogLevel.Info, LogLevel.Fatal);
            target.OnLog += writeLogText;
        }

        public static void InitializeFileTarget()
        {
            var config = NLog.LogManager.Configuration;

            LogFilePath = $"{Path.Combine(Path.GetTempPath(), $"AutoMuteUs-Portable")}.log";

            var logger = NLog.LogManager.GetLogger("Main");
            logger.Info($"Log output started: \"{LogFilePath}\"");

            var fileTarget = new FileTarget()
            {
                FileName = LogFilePath,
                DeleteOldFileOnStartup = true
            };

            config.AddTarget("LogFile", fileTarget);
            config.AddRule(LogLevel.Trace, LogLevel.Fatal, "LogFile", "*");

            NLog.LogManager.Configuration = config;
        }

        private void InitializeNLog()
        {
            NLog.LogManager.Configuration = new NLog.Config.LoggingConfiguration();

            InitializeRichTextBoxTarget(mainLogTextBox, mainWriteLogText);
            InitializeRichTextBoxTarget(detailedLogTextBox, detailedWriteLogText);

            InitializeFileTarget();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (main != null) main.TerminateProcs();
            Environment.Exit(0);
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            var resetWindow = new ResetWindow();
            resetWindow.Show();
        }

        private void ExportLogButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("To export current log, application will close the server.\nAre you sure to proceed?", "Caution", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                return;
            }

            if (main != null) main.TerminateProcs();
            var source = LogFilePath;
            var dist = Path.Combine(AppContext.BaseDirectory, "AutoMuteUs-Portable.log");
            if (File.Exists(dist))
            {
                File.Delete(dist);
            }
            File.Move(source, dist);

            MessageBox.Show($"Successfully put log into current executable directory: \"{dist}\"", "Output Log", MessageBoxButton.OK);
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("To change settings, application will close the server.\nAre you sure to proceed?", "Caution", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                return;
            }

            if (main != null) main.TerminateProcs();
            Task.Factory.StartNew(() =>
            {
                MessageBox.Show("Do not close the app until new message box be shown.", "Caution", MessageBoxButton.OK);
                Settings.LoadSettings(true);
                LogManager.GetLogger("Main").Info("To host AutoMuteUs again, please relaunch AutoMuteUs Portable.");
                MessageBox.Show("To host AutoMuteUs again, please relaunch AutoMuteUs Portable.", "Caution", MessageBoxButton.OK);
            });
        }

#if PUBLISH
        private async void CheckUpdate()
        {
            var logger = LogManager.GetLogger("Main");
            
            var CurrentInformationalVersion = (string)Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

            logger.Info($"Version: \"{CurrentInformationalVersion}\"");

            Newtonsoft.Json.Linq.JArray AppVersionList;

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("AutoMuteUs-Portable", CurrentInformationalVersion));
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var httpResponse = await client.GetAsync("https://api.github.com/repos/mtaku3/AutoMuteUs-Portable/git/matching-refs/tags");

                if (!httpResponse.IsSuccessStatusCode)
                {
                    logger.Error("Failed to get refs/tags.");
                    return;
                }

                var responseContent = await httpResponse.Content.ReadAsStringAsync();
                AppVersionList = (Newtonsoft.Json.Linq.JArray)JsonConvert.DeserializeObject(responseContent);
            }

            var LatestVersion = AppVersionList.Last;
            var LatestTag = ((string)LatestVersion["ref"]).Replace("refs/tags/", "");
            var LatestInformationalVersion = (string)LatestVersion["object"]["sha"];

            if (LatestInformationalVersion != CurrentInformationalVersion)
            {

                logger.Info($"New update detected: \"{CurrentInformationalVersion}\" => \"{LatestInformationalVersion}\"");
                if (MessageBox.Show($"There's newer version \"{LatestTag}\" xD\nDo you wanna check it out on Github?", "Update Checker", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    string url = "https://github.com/mtaku3/AutoMuteUs-Portable/releases/latest";

                    try
                    {
                        Process.Start(url);
                    }
                    catch
                    {
                        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        {
                            url = url.Replace("&", "^&");
                            Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                        }
                    }
                }
            }
        }
#endif

    }
}
