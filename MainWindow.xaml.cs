using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

namespace AutoMuteUs_Portable
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private static Main main;

        public MainWindow()
        {
            InitializeComponent();
            InitializeNLog();
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

            var target = new RichTextBoxTarget(richTextBox.Name, LogLevel.Trace, LogLevel.Fatal);
            target.OnLog += writeLogText;
        }

        private void InitializeNLog()
        {
            NLog.LogManager.Configuration = new NLog.Config.LoggingConfiguration();

            InitializeRichTextBoxTarget(mainLogTextBox, mainWriteLogText);
            InitializeRichTextBoxTarget(detailedLogTextBox, detailedWriteLogText);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (main != null) main.TerminateProcs();
        }

    }
}
