using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace AutoMuteUs_Portable
{
    /// <summary>
    /// ResetWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ResetWindow : Window
    {
        private bool RestartApplication = false;

        public ResetWindow()
        {
            InitializeComponent();
        }

        private void ResetAllProperties(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("This operation requires restarting application.\nAre you sure to do?", "RESET ALL PROPERTIES", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                return;
            }

            if (MainWindow.main != null) MainWindow.main.TerminateProcs();

            Properties.Settings.Default.Reset();
            File.Delete(Path.Combine(Settings.GetUserVar("EnvPath"), ".env"));

            MessageBox.Show("After you close this window, application will close.\nPlease start the application manually.", "RESTART CAUTION", MessageBoxButton.OK);
            RestartApplication = true;
        }

        private void DeleteAllBinaries(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("This operation requires restarting application.\nAre you sure to do?", "DELETE ALL BINARIES", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                return;
            }

            if (MainWindow.main != null) MainWindow.main.TerminateProcs();

            RMRF(Settings.GetUserVar("EnvPath"));

            MessageBox.Show("After you close this window, application will close.\nPlease start the application manually.", "RESTART CAUTION", MessageBoxButton.OK);
            RestartApplication = true;
        }

        private void RMRF(string targetDirectoryPath)
        {
            if (!Directory.Exists(targetDirectoryPath))
            {
                return;
            }

            string[] filePaths = Directory.GetFiles(targetDirectoryPath);
            foreach (string filePath in filePaths)
            {
                File.SetAttributes(filePath, FileAttributes.Normal);
                File.Delete(filePath);
            }

            string[] directoryPaths = Directory.GetDirectories(targetDirectoryPath);
            foreach (string directoryPath in directoryPaths)
            {
                RMRF(directoryPath);
            }

            Directory.Delete(targetDirectoryPath, false);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (RestartApplication)
            {
                Environment.Exit(0);
            }
        }
    }
}
