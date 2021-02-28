using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
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
            Close();
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();

            Settings.SetUserVar("EnvPath", EnvPath.Text);
        }
    }
}
