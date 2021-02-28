using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AutoMuteUs_Portable
{
    /// <summary>
    /// ChooseEnvPathWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ChooseEnvPathWindow : Window
    {
        public ChooseEnvPathWindow()
        {
            InitializeComponent();
            EnvPath.Text = Settings.GetUserVar("EnvPath");
            if (EnvPath.Text == "")
            {
                EnvPath.Text = Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), "AutoMuteUs-Portable\\");
            }
        }

        private void FolderBrowserOpenButton_Click(object sender, RoutedEventArgs e)
        {
            var browser = new System.Windows.Forms.FolderBrowserDialog();

            if (browser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                EnvPath.Text = browser.SelectedPath;
            }
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            Settings.SetUserVar("EnvPath", EnvPath.Text);

            if (!Directory.Exists(Settings.GetUserVar("EnvPath")))
            {
                MessageBox.Show("EnvPath is required to set properly.");
                return;
            }

            Close();
        }

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        const int GWL_STYLE = -16;
        const int WS_SYSMENU = 0x80000;

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            IntPtr handle = new WindowInteropHelper(this).Handle;
            int style = GetWindowLong(handle, GWL_STYLE);
            style = style & (~WS_SYSMENU);
            SetWindowLong(handle, GWL_STYLE, style);
        }
    }
}
