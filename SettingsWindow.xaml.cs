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
using Path = System.IO.Path;
using System.Security.Cryptography;
using System.Net;

namespace AutoMuteUs_Portable
{
    /// <summary>
    /// SettingsWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private Dictionary<string, string> OldEnvVars;
        private Dictionary<string, string> OldUserVars;

        public static bool useRecommendedVersionCombination = false;

        public bool hasClosed { get; private set; }

        private Dictionary<string, Dictionary<string, UIElement>> AllControls;

        private static RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();

        public SettingsWindow()
        {
            InitializeComponent();
            InitializeControls();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            hasClosed = true;
        }

        private bool SkipComponent(string Key)
        {
            var requiredComponents = Main.RequiredComponents[OldUserVars["ARCHITECTURE"]];

            string ComponentName;

            switch (Key)
            {
                case "AUTOMUTEUS_TAG":
                    ComponentName = "automuteus";
                    break;
                case "GALACTUS_TAG":
                    ComponentName = "galactus";
                    break;
                case "WINGMAN_TAG":
                    ComponentName = "wingman";
                    break;
                case "AUTOMUTEUS_AUTORESTART":
                    ComponentName = "automuteus";
                    break;
                case "GALACTUS_AUTORESTART":
                    ComponentName = "galactus";
                    break;
                case "WINGMAN_AUTORESTART":
                    ComponentName = "wingman";
                    break;
                default:
                    return false;
            }

            if (requiredComponents.Contains(ComponentName)) return false;

            return true;
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
                if (SkipComponent(variable.Key)) continue;

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

                    comboBox.SelectedValue = OldUserVars["AUTOMUTEUS_TAG"];

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

                    comboBox.SelectedValue = OldUserVars["GALACTUS_TAG"];

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

                    comboBox.SelectedValue = OldUserVars["WINGMAN_TAG"];

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
                    comboBox.Items.Add("v5");
                    comboBox.Items.Add("v4");

                    comboBox.SelectedValue = OldUserVars["ARCHITECTURE"];

                    comboBox.DropDownClosed += ComboBox_DropDownClosed;

                    grid.Children.Add(comboBox);
                    Grid.SetColumn(comboBox, 1);
                    controls.Add("ComboBox", comboBox);
                }
                else if (variable.Key == "EnvPath")
                {
                    var changeEnvPathButton = new Button()
                    {
                        Width = 300,
                        Content = "Change",
                        Name = variable.Key
                    };
                    changeEnvPathButton.Click += ChangeEnvPathButton_Click;
                    grid.Children.Add(changeEnvPathButton);
                    Grid.SetColumn(changeEnvPathButton, 1);
                    controls.Add("Button", changeEnvPathButton);
                }
                else if (variable.Key == "AUTOMUTEUS_AUTORESTART" || variable.Key == "GALACTUS_AUTORESTART" || variable.Key == "WINGMAN_AUTORESTART")
                {
                    var checkBox = new CheckBox()
                    {
                        Name = variable.Key,
                        IsChecked = variable.Value == "True"
                    };
                    grid.Children.Add(checkBox);
                    Grid.SetColumn(checkBox, 1);
                    controls.Add("CheckBox", checkBox);
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

            button = new Button()
            {
                Content = "Use Recommemded Version Combination",
                Name = "UseRVCBtn",
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 5, 0, 0)
            };

            button.AddHandler(Button.ClickEvent, new RoutedEventHandler(useRVCBtn_Click));

            stackPanel.Children.Add(button);

            ApplyRecommendedVersionCombination();
        }

        public static String CheckRVCHash()
        {
            var logger = LogManager.GetLogger("Main");

            Newtonsoft.Json.Linq.JObject rvc_hash;

            logger.Info("RVC Hash has been downloading.");
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Headers.Add("accept", "application/json");
                    client.Headers.Add("User-Agent", "request");

                    string downloadedString = client.DownloadString("https://api.github.com/repos/mtaku3/AutoMuteUs-Portable/contents/rvc.json");
                    rvc_hash = (Newtonsoft.Json.Linq.JObject)JsonConvert.DeserializeObject(downloadedString);
                    logger.Debug(downloadedString);
                }
            }
            catch
            {
                logger.Error("Failed to download RVC Hash.");
                return null;
            }

            if (!rvc_hash.ContainsKey("sha"))
            {
                logger.Error("Failed to download RVC Hash.");
                return null;
            }

            return rvc_hash.GetValue("sha").ToString();
        }

        public static Newtonsoft.Json.Linq.JObject CheckRVC()
        {
            var logger = LogManager.GetLogger("Main");

            Newtonsoft.Json.Linq.JObject rvc;

            logger.Info("RVC has been downloading.");
            try
            {
                using (WebClient client = new WebClient())
                {
                    string downloadedString = client.DownloadString("https://raw.githubusercontent.com/mtaku3/AutoMuteUs-Portable/main/rvc.json");
                    if (downloadedString.Contains("404"))
                    {
                        logger.Error("Failed to download RVC.");
                        return null;
                    }
                    rvc = (Newtonsoft.Json.Linq.JObject)JsonConvert.DeserializeObject(downloadedString);
                    logger.Debug(downloadedString);
                }
            }
            catch
            {
                logger.Error("Failed to download RVC.");
                return null;
            }

            return rvc;
        }

        private void ApplyRecommendedVersionCombination()
        {
            if (!useRecommendedVersionCombination) return;

            var logger = LogManager.GetLogger("Main");

            var rvc_hash = CheckRVCHash();
            var rvc = CheckRVC();

            if (rvc_hash == null || rvc == null) return;

            var comboBox = AllControls["ARCHITECTURE"]["ComboBox"] as ComboBox;
            comboBox.SelectedValue = rvc.GetValue("ARCHITECTURE").ToString();
            if (OldUserVars["ARCHITECTURE"] != (string)comboBox.SelectedValue)
            {
                UpdateUserVars();
                Close();
                return;
            }

            foreach (var item in rvc)
            {
                if (!AllControls.ContainsKey(item.Key.ToString())) continue;
                comboBox = AllControls[item.Key.ToString()]["ComboBox"] as ComboBox;
                comboBox.SelectedValue = rvc.GetValue(item.Key).ToString();
            }
            useRecommendedVersionCombination = false;
        }

        private void useRVCBtn_Click(object sender, RoutedEventArgs e)
        {
            useRecommendedVersionCombination = true;

            ApplyRecommendedVersionCombination();
        }

        private void ChangeEnvPathButton_Click(object sender, RoutedEventArgs e)
        {
            STATask.Run(() =>
            {
                var chooseEnvPathWindow = new ChooseEnvPathWindow();
                chooseEnvPathWindow.ShowDialog();
            }).Wait();
        }

        private void FolderBrowserOpenButton_Click(object sender, RoutedEventArgs e)
        {
            var browser = new System.Windows.Forms.FolderBrowserDialog();

            if (browser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var textBox = AllControls["EnvPath"]["TextBox"] as TextBox;
                textBox.Text = Path.Combine(browser.SelectedPath, "AutoMuteUs-Portable\\");
            }
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
                Close();
            }
        }

        private static char GenerateChar()
        {
            string CapitalLetters = "QWERTYUIOPASDFGHJKLZXCVBNM";
            string SmallLetters = "qwertyuiopasdfghjklzxcvbnm";
            string Digits = "0123456789";
            string availableChars = CapitalLetters + SmallLetters + Digits;

            var byteArray = new byte[1];
            char c;

            do
            {
                provider.GetBytes(byteArray);
                c = (char)byteArray[0];
            } while (!availableChars.Any(x => x == c));

            return c;
        }

        private static string GeneratePassword()
        {
            int PasswordLength = 20;

            StringBuilder sb = new StringBuilder();
            for (int n = 0; n < PasswordLength; n++)
            {
                sb = sb.Append(GenerateChar());
            }

            return sb.ToString();
        }

        private void UpdateEnvVars()
        {
            var logger = LogManager.GetLogger("Main");

            logger.Debug("########## EnvVars Update ##########");

            foreach (var variable in OldEnvVars)
            {
                TextBox textBox = AllControls[variable.Key]["TextBox"] as TextBox;

                if (Settings.CheckRequiredVariable(variable.Key, textBox.Text) != null)
                {
                    if (variable.Key == "POSTGRES_PASS")
                    {
                        if (MessageBox.Show($"POSTGRESS_PASS is required to fill. Do you want to use auto generated password?", "PASSWORD GENERATOR", MessageBoxButton.YesNo) == MessageBoxResult.No)
                            return;

                        textBox.Text = GeneratePassword();

                        MessageBox.Show($"POSTGRES_PASS is set as \"{textBox.Text}\".", "PASSWORD GENERATOR", MessageBoxButton.OK);
                    }
                    else
                    {
                        MessageBox.Show($"{variable.Key} is required to fill.\nFill it and try again.");
                        return;
                    }
                }

                if (OldEnvVars[variable.Key] != textBox.Text)
                {
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
                    if (SkipComponent(variable.Key)) continue;
                    ComboBox comboBox = AllControls[variable.Key]["ComboBox"] as ComboBox;

                    if (Settings.CheckRequiredVariable(variable.Key, (string)comboBox.SelectedValue) != null)
                    {
                        MessageBox.Show($"{variable.Key} is required to fill.\nFill it and try again.");
                        return;
                    }

                    if (OldUserVars[variable.Key] != (string)comboBox.SelectedValue)
                    {
                        Settings.SetUserVar(variable.Key, (string)comboBox.SelectedValue);
                    }
                }
                else if (variable.Key == "EnvPath")
                {
                    continue;
                }
                else if (variable.Key == "AUTOMUTEUS_AUTORESTART" || variable.Key == "GALACTUS_AUTORESTART" || variable.Key == "WINGMAN_AUTORESTART")
                {
                    if (SkipComponent(variable.Key)) continue;

                    CheckBox checkBox = AllControls[variable.Key]["CheckBox"] as CheckBox;

                    var Value = (bool)checkBox.IsChecked ? "True" : "False";

                    if (OldUserVars[variable.Key] != Value)
                    {
                        Settings.SetUserVar(variable.Key, Value);
                    }
                }
                else
                {
                    TextBox textBox = AllControls[variable.Key]["TextBox"] as TextBox;

                    if (Settings.CheckRequiredVariable(variable.Key, textBox.Text) != null)
                    {
                        MessageBox.Show($"{variable.Key} is required to fill.\nFill it and try again.");
                        return;
                    }

                    if (OldUserVars[variable.Key] != textBox.Text)
                    {
                        Settings.SetUserVar(variable.Key, textBox.Text);
                    }
                }
            }

            logger.Debug("#####################################");
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();

            UpdateUserVars();

            UpdateEnvVars();
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
