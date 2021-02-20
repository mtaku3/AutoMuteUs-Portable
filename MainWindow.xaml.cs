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
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger("MainWindow");

        public MainWindow()
        {
            InitializeComponent();
            InitializeNLog();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            Settings.LoadSettings();
            main = new Main();
        }

        private void WriteLogText(object sender, LogEventInfo logEvent)
        {
            var textbox = LogTextBox;
            LogTextBox.Dispatcher.BeginInvoke((Action)(() =>
            {
                var LogLevelColor = "#202020";
                var LoggerNameColor = "#505050";
                var EventMessageColor = "#000000";

                Paragraph paragraph = new Paragraph();
                Run run;

                // LogLevel
                run = new Run($"[{logEvent.Level.Name}] ");
                run.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(LogLevelColor));
                paragraph.Inlines.Add(run);
                paragraph.LineHeight = 1;

                // LoggerName
                run = new Run($"[{logEvent.LoggerName}] ");
                run.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(LoggerNameColor));
                paragraph.Inlines.Add(run);
                paragraph.LineHeight = 1;

                // Event Message
                run = new Run(logEvent.Message);
                run.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(EventMessageColor));
                paragraph.Inlines.Add(run);
                paragraph.LineHeight = 1;

                textbox.Document.Blocks.Add(paragraph);
                textbox.ScrollToEnd();
            }));
        }

        private void InitializeNLog()
        {
            NLog.LogManager.Configuration = new NLog.Config.LoggingConfiguration();

            var minLevel = LogLevel.Debug;
            var maxLevel = LogLevel.Fatal;
            var target = new RichTextBoxTarget(this.LogTextBox.Name, minLevel, maxLevel);
            target.OnLog += WriteLogText;

            LogTextBox.Document.Blocks.Clear();

            logger.Debug("Initialized NLog");
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            main.TerminateProcs();
        }

    }
}
