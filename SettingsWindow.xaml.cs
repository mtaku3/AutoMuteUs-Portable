using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace AutoMuteUs_Portable
{
    /// <summary>
    /// SettingsWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private Dictionary<string, string> OldEnvVars;
        private Dictionary<string, string> OldUserVars;


        private Dictionary<string, Dictionary<string, UIElement>> AllControls;

        public SettingsWindow()
        {
            InitializeComponent();
            InitializeControls();
        }

        private void InitializeControls()
        {
            OldEnvVars = new Dictionary<string, string>(Settings.EnvVars);
            OldUserVars = new Dictionary<string, string>(Settings.UserVars);
            var VersionList = Settings.VersionList;
            AllControls = new Dictionary<string, Dictionary<string, UIElement>>();

            var logger = LogManager.GetLogger("Main");

            logger.Debug("########## OldEnvVars Output ##########");
            logger.Debug(JsonConvert.SerializeObject(OldEnvVars));
            logger.Debug("#######################################");

            logger.Debug("########## OldUserVars Output ##########");
            logger.Debug(JsonConvert.SerializeObject(OldUserVars));
            logger.Debug("########################################");

            Grid grid;
            foreach (var variable in OldUserVars)
            {
                var controls = new Dictionary<string, UIElement>();

                grid = new Grid();
                controls.Add("Grid", grid);

                for (var i = 0; i < 2; i++)
                {
                    grid.ColumnDefinitions.Add(new ColumnDefinition());
                }

                var textBlock = new TextBlock()
                {
                    Text = variable.Key,
                    Margin = new Thickness(0, 0, 10, 0)
                };
                grid.Children.Add(textBlock);
                Grid.SetColumn(textBlock, 0);
                controls.Add("TextBlock", textBlock);

                if (variable.Key == "AUTOMUTEUS_TAG")
                {
                    var comboBox = new ComboBox()
                    {
                        Width = 300,
                        Name = variable.Key
                    };

                    foreach (var item in Settings.VersionList["automuteus"])
                    {
                        comboBox.Items.Add(item.Key);
                    }

                    comboBox.SelectedValue = Settings.GetUserVar("AUTOMUTEUS_TAG");

                    grid.Children.Add(comboBox);
                    Grid.SetColumn(comboBox, 1);
                    controls.Add("ComboBox", comboBox);
                }
                else if (variable.Key == "GALACTUS_TAG")
                {
                    var comboBox = new ComboBox()
                    {
                        Width = 300,
                        Name = variable.Key
                    };

                    foreach (var item in Settings.VersionList["galactus"])
                    {
                        comboBox.Items.Add(item.Key);
                    }

                    comboBox.SelectedValue = Settings.GetUserVar("GALACTUS_TAG");

                    grid.Children.Add(comboBox);
                    Grid.SetColumn(comboBox, 1);
                    controls.Add("ComboBox", comboBox);
                }
                else if (variable.Key == "WINGMAN_TAG")
                {
                    var comboBox = new ComboBox()
                    {
                        Width = 300,
                        Name = variable.Key
                    };

                    foreach (var item in Settings.VersionList["wingman"])
                    {
                        comboBox.Items.Add(item.Key);
                    }

                    comboBox.SelectedValue = Settings.GetUserVar("WINGMAN_TAG");

                    if (Settings.GetUserVar("ARCHITECTURE") == "v7")
                    {
                        comboBox.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        comboBox.Visibility = Visibility.Hidden;
                        var tb = grid.Children.OfType<TextBlock>().FirstOrDefault();
                        tb.Visibility = Visibility.Hidden;
                    }

                    grid.Children.Add(comboBox);
                    Grid.SetColumn(comboBox, 1);
                    controls.Add("ComboBox", comboBox);
                }
                else if (variable.Key == "ARCHITECTURE")
                {
                    var comboBox = new ComboBox()
                    {
                        Width = 300,
                        Name = variable.Key
                    };

                    comboBox.Items.Add("v7");
                    comboBox.Items.Add("v6");

                    comboBox.SelectedValue = Settings.GetUserVar("ARCHITECTURE");

                    comboBox.DropDownClosed += ComboBox_DropDownClosed;

                    if (Settings.GetUserVar("ARCHITECTURE") != "v7")
                    {
                        AllControls["WINGMAN_TAG"]["Grid"].Visibility = Visibility.Collapsed;
                        AllControls["WINGMAN_TAG"]["Separator"].Visibility = Visibility.Collapsed;
                    }

                    grid.Children.Add(comboBox);
                    Grid.SetColumn(comboBox, 1);
                    controls.Add("ComboBox", comboBox);
                }
                else
                {
                    var textBox = new TextBox()
                    {
                        Width = 300,
                        Text = variable.Value,
                        Name = variable.Key
                    };
                    grid.Children.Add(textBox);
                    Grid.SetColumn(textBox, 1);
                    controls.Add("TextBox", textBox);
                }


                stackPanel.Children.Add(grid);

                var separetor = new Separator();
                stackPanel.Children.Add(separetor);
                controls.Add("Separator", separetor);

                AllControls.Add(variable.Key, controls);
            }

            for (var ind = 0; ind < OldEnvVars.Count(); ind++)
            {
                var variable = OldEnvVars.ElementAt(ind);

                var controls = new Dictionary<string, UIElement>();

                grid = new Grid();
                controls.Add("Grid", grid);

                for (var i = 0; i < 2; i++)
                {
                    grid.ColumnDefinitions.Add(new ColumnDefinition());
                }

                var textBlock = new TextBlock()
                {
                    Text = variable.Key,
                    Margin = new Thickness(0, 0, 10, 0)
                };
                grid.Children.Add(textBlock);
                Grid.SetColumn(textBlock, 0);
                controls.Add("TextBlock", textBlock);

                var textBox = new TextBox()
                {
                    Width = 300,
                    Text = variable.Value,
                    Name = variable.Key
                };
                grid.Children.Add(textBox);
                Grid.SetColumn(textBox, 1);
                controls.Add("TextBox", textBox);


                stackPanel.Children.Add(grid);

                if (ind != OldEnvVars.Count() - 1)
                {
                    var separetor = new Separator();
                    stackPanel.Children.Add(separetor);
                    controls.Add("Separator", separetor);
                }

                AllControls.Add(variable.Key, controls);
            }

            var button = new Button()
            {
                Content = "Save",
                Name = "SaveBtn",
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center
            };

            button.AddHandler(Button.ClickEvent, new RoutedEventHandler(saveBtn_Click));

            stackPanel.Children.Add(button);

            button = new Button()
            {
                Content = "Cancel",
                Name = "CancelBtn",
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 5, 0, 0)
            };

            button.AddHandler(Button.ClickEvent, new RoutedEventHandler(cancelBtn_Click));

            stackPanel.Children.Add(button);
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void ComboBox_DropDownClosed(object sender, EventArgs e)
        {
            var comboBox = AllControls["ARCHITECTURE"]["ComboBox"] as ComboBox;

            if (OldUserVars["ARCHITECTURE"] != (string)comboBox.SelectedValue)
            {
                UpdateUserVars();
            }

            if ((string)comboBox.SelectedValue == "v7")
            {
                AllControls["WINGMAN_TAG"]["Grid"].Visibility = Visibility.Visible;
                AllControls["WINGMAN_TAG"]["Separator"].Visibility = Visibility.Visible;
            }
            else
            {
                AllControls["WINGMAN_TAG"]["Grid"].Visibility = Visibility.Collapsed;
                AllControls["WINGMAN_TAG"]["Separator"].Visibility = Visibility.Collapsed;
            }
        }

        private void UpdateEnvVars()
        {
            var logger = LogManager.GetLogger("Main");

            logger.Debug("########## EnvVars Update ##########");

            foreach (var variable in OldEnvVars)
            {
                TextBox textBox = AllControls[variable.Key]["TextBox"] as TextBox;

                if (check(variable.Key, textBox.Text) == false)
                {
                    MessageBox.Show("Some required variable are set to empty.\nFill it out and retry.");
                    return;
                }

                if (OldEnvVars[variable.Key] != textBox.Text)
                {
                    logger.Debug($"{variable.Key}: {OldEnvVars[variable.Key]} => {textBox.Text}");
                    Settings.SetEnvVar(variable.Key, textBox.Text);
                }
            }

            logger.Debug("####################################");
        }

        private void UpdateUserVars()
        {
            var logger = LogManager.GetLogger("Main");

            logger.Debug("########## UserVars Update ##########");

            foreach (var variable in OldUserVars)
            {
                if (variable.Key == "AUTOMUTEUS_TAG" || variable.Key == "GALACTUS_TAG" || variable.Key == "WINGMAN_TAG" || variable.Key == "ARCHITECTURE")
                {
                    ComboBox comboBox = AllControls[variable.Key]["ComboBox"] as ComboBox;

                    if (check(variable.Key, (string)comboBox.SelectedValue) == false)
                    {
                        MessageBox.Show("Some required variable are set to empty.\nFill it out and retry.");
                        return;
                    }

                    if (OldUserVars[variable.Key] != (string)comboBox.SelectedValue)
                    {
                        logger.Debug($"{variable.Key}: {OldUserVars[variable.Key]} => {(string)comboBox.SelectedValue}");
                        Settings.SetUserVar(variable.Key, (string)comboBox.SelectedValue);
                    }
                }
                else
                {
                    TextBox textBox = AllControls[variable.Key]["TextBox"] as TextBox;

                    if (check(variable.Key, textBox.Text) == false)
                    {
                        MessageBox.Show("Some required variable are set to empty.\nFill it out and retry.");
                        return;
                    }

                    if (OldUserVars[variable.Key] != textBox.Text)
                    {
                        logger.Debug($"{variable.Key}: {OldUserVars[variable.Key]} => {textBox.Text}");
                        Settings.SetUserVar(variable.Key, textBox.Text);
                    }
                }
            }

            logger.Debug("#####################################");
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            UpdateEnvVars();

            UpdateUserVars();

            Close();
        }

        private bool check(string Key, string Value)
        {
            if (Value == "")
            {
                if (Key == "DISCORD_BOT_TOKEN" || Key == "EnvPath")
                {
                    return false;
                }
            }
            return true;
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
