using Newtonsoft.Json;
using NLog;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
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

        public static ObservableCollection<CultureInfo> availableCultures = new ObservableCollection<CultureInfo>()
        {
            new CultureInfo("en"),
            new CultureInfo("ja")
        };

        public MainWindow()
        {
            if (Properties.Settings.Default.Language == "") Properties.Settings.Default.Language = CultureInfo.InstalledUICulture.TwoLetterISOLanguageName;
            if (availableCultures.IndexOf(new CultureInfo(Properties.Settings.Default.Language)) == -1) Properties.Settings.Default.Language = "en";
            Properties.Settings.Default.Save();

            var culture = new CultureInfo(Properties.Settings.Default.Language);

            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            WPFLocalizeExtension.Engine.LocalizeDictionary.Instance.Culture = Thread.CurrentThread.CurrentCulture;

            //this.DataContext = new CultureComboBoxViewModel(culture);

            InitializeComponent();

            InitializeNLog();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            Task.Run(() => {
#if PUBLISH
                CheckRVCUpdate();
                CheckUpdate();
#endif
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
            logger.Info($"{LocalizationProvider.GetLocalizedValue<string>("MainLogger_LogOutputStarted")} \"{LogFilePath}\"");

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
            if (MessageBox.Show(LocalizationProvider.GetLocalizedValue<string>("MainWindow_ExportLogBtn_Caution_Text"), LocalizationProvider.GetLocalizedValue<string>("MainWindow_ExportLogBtn_Caution_Title"), MessageBoxButton.YesNo) != MessageBoxResult.Yes)
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

            MessageBox.Show($"{LocalizationProvider.GetLocalizedValue<string>("MainWindow_ExportLogBtn_Result_Text")} \"{dist}\"", LocalizationProvider.GetLocalizedValue<string>("MainWindow_ExportLogBtn_Result_Title"), MessageBoxButton.OK);
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(LocalizationProvider.GetLocalizedValue<string>("MainWindow_SettingsBtn_Caution#1_Text"), LocalizationProvider.GetLocalizedValue<string>("MainWindow_SettingsBtn_Caution#1_Title"), MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                return;
            }

            if (main != null) main.TerminateProcs();
            Task.Run(() =>
            {
                MessageBox.Show(LocalizationProvider.GetLocalizedValue<string>("MainWindow_SettingsBtn_Caution#2_Text"), LocalizationProvider.GetLocalizedValue<string>("MainWindow_SettingsBtn_Caution#2_Title"), MessageBoxButton.OK);
                Settings.LoadSettings(true);
                LogManager.GetLogger("Main").Info(LocalizationProvider.GetLocalizedValue<string>("MainLogger_RelaunchApp"));
                MessageBox.Show(LocalizationProvider.GetLocalizedValue<string>("MainWindow_SettingsBtn_Caution#3_Text"), LocalizationProvider.GetLocalizedValue<string>("MainWindow_SettingsBtn_Caution#3_Title"), MessageBoxButton.OK);
            });
        }

        public static void UpdateLanguage(CultureInfo culture)
        {
            Properties.Settings.Default.Language = culture.Name;
            Properties.Settings.Default.Save();

            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            WPFLocalizeExtension.Engine.LocalizeDictionary.Instance.Culture = Thread.CurrentThread.CurrentCulture;
        }

        public class CultureComboBoxViewModel
        {
            public CultureComboBoxViewModel(CultureInfo culture)
            {
                if (AvailableCultures.IndexOf(culture) != -1) selectedCulture = culture;
                else selectedCulture = new CultureInfo("en");
            }

            public ObservableCollection<CultureInfo> AvailableCultures
            {
                get { return MainWindow.availableCultures; }
            }

            private int selectedCultureId;

            public int SelectedCultureId
            {
                get { return selectedCultureId; }
                set { selectedCultureId = value; }
            }

            private CultureInfo selectedCulture;

            public CultureInfo SelectedCulture
            {
                get { return selectedCulture; }
                set
                {
                    selectedCulture = value;
                    UpdateLanguage(value);
                }
            }
        }


#if PUBLISH
        private void CheckRVCUpdate()
        {
            var logger = LogManager.GetLogger("Main");

            var rvc_hash = SettingsWindow.CheckRVCHash();
            var rvc = SettingsWindow.CheckRVC();
            if (rvc_hash == null || rvc == null) return;

            if (Properties.Settings.Default.RVC_Hash != rvc_hash || Properties.Settings.Default.RVC_Hash == "")
            {
                logger.Info($"{LocalizationProvider.GetLocalizedValue<string>("MainLogger_RVCHash_UpdateDetected")} \"{Properties.Settings.Default.RVC_Hash}\" => \"{rvc_hash}\"");
                String message = LocalizationProvider.GetLocalizedValue<string>("MainWindow_RVCUpdate_Text");

                foreach (var item in rvc)
                {
                    message += $"\n{item.Key.ToString()} : {item.Value.ToString()}";
                }

                MessageBox.Show(message, LocalizationProvider.GetLocalizedValue<string>("MainWindow_RVCUpdate_Title"), MessageBoxButton.OK);

                Properties.Settings.Default.RVC_Hash = rvc_hash;
                Properties.Settings.Default.Save();
            }
        }

        private async void CheckUpdate()
        {
            var logger = LogManager.GetLogger("Main");
            
            var CurrentInformationalVersion = (string)Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

            logger.Info($"{LocalizationProvider.GetLocalizedValue<string>("MainLogger_Version")} \"{CurrentInformationalVersion}\"");

            Newtonsoft.Json.Linq.JArray AppVersionList;

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("AutoMuteUs-Portable", CurrentInformationalVersion));
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var httpResponse = await client.GetAsync("https://api.github.com/repos/mtaku3/AutoMuteUs-Portable/git/matching-refs/tags");

                if (!httpResponse.IsSuccessStatusCode)
                {
                    logger.Error(LocalizationProvider.GetLocalizedValue<string>("MainLogger_HttpClientRequestFailed"));
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

                logger.Info($"{LocalizationProvider.GetLocalizedValue<string>("MainLogger_CheckUpdate_UpdateDetected")} \"{CurrentInformationalVersion}\" => \"{LatestInformationalVersion}\"");
                if (MessageBox.Show($"{LocalizationProvider.GetLocalizedValue<string>("MainWindow_CheckUpdate_UpdateDetected_Text#1")} \"{LatestTag}\"\n{LocalizationProvider.GetLocalizedValue<string>("MainWindow_CheckUpdate_UpdateDetected_Text#2")}", LocalizationProvider.GetLocalizedValue<string>("MainWindow_CheckUpdate_UpdateDetected_Title"), MessageBoxButton.YesNo) == MessageBoxResult.Yes)
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
