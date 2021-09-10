using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Path = System.IO.Path;

namespace AutoMuteUs_Portable
{
    public class Main
    {
        private static Logger MainLogger = LogManager.GetLogger("Main");

        private readonly Dictionary<string, NLog.Logger> logger = new Dictionary<string, NLog.Logger>();
        private readonly Dictionary<string, Process> Procs = new Dictionary<string, Process>();
        private static Dictionary<string, Dictionary<string, UIElement>> IndicatorControls;
        private MainWindow mainWindow;

        private bool CancelLoop = false;

        public static readonly Dictionary<string, string[]> RequiredComponents = new Dictionary<string, string[]>()
        {
            ["v7"] = new string[]
            {
                "automuteus",
                "galactus",
                "wingman",
                "redis",
                "postgres"
            },
            ["v6"] = new string[]
            {
                "automuteus",
                "galactus",
                "redis",
                "postgres"
            },
            ["v5"] = new string[]
            {
                "automuteus",
                "galactus",
                "redis",
                "postgres"
            },
            ["v4"] = new string[]
            {
                "automuteus",
                "galactus",
                "redis"
            }
        };

        public Main(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            Initialize();
        }

        private void Initialize()
        {
            InitializeLogger();
            InitializeProcs();
            Application.Current.Dispatcher.Invoke((Action)delegate { InitializeProcIndicators(); });
            StartProcs();
            Task.Run(RunLoop);
        }

        private bool IsAutoRestartEnabled(string Key)
        {
            switch (Key)
            {
                case "automuteus":
                    return Settings.GetUserVar("AUTOMUTEUS_AUTORESTART") == "True";
                case "galactus":
                    return Settings.GetUserVar("GALACTUS_AUTORESTART") == "True";
                case "wingman":
                    return Settings.GetUserVar("WINGMAN_AUTORESTART") == "True";
                default:
                    return false;
            }
        }

        private void RunLoop()
        {
            while (true)
            {
                Task.Delay(100).Wait();
                foreach (var proc in Procs)
                {
                    var process = proc.Value;

                    try
                    {
                        if (process != null && process.HasExited && proc.Key != "postgres")
                        {
                            if (IsAutoRestartEnabled(proc.Key))
                            {
                                RestartProc(proc);
                                continue;
                            }

                            TerminateProcs();
                            return;
                        }
                    }
                    catch (InvalidOperationException)
                    {
                        process = null;
                    }

                    if (CancelLoop)
                    {
                        return;
                    }
                }
            }
        }

        public void TerminateProcs()
        {
            CancelLoop = true;

            var requiredComponent = Main.RequiredComponents[Settings.GetUserVar("ARCHITECTURE")];

            Ellipse ellipse;

            foreach (var proc in Procs)
            {
                try
                {
                    if (proc.Key == "postgres") TerminatePostgresServer();
                    else if (proc.Key == "redis") TerminateRedisServer();

                    var process = proc.Value;
                    if (process.HasExited) throw new Exception();

                    process.Kill();
                    process.WaitForExit();
                }
                catch
                {
                }

                try
                {
                    ellipse = IndicatorControls[proc.Key]["Ellipse"] as Ellipse;
                    ellipse.Dispatcher.Invoke((Action)(() => ellipse.Fill = Brushes.Red));
                }
                catch
                {
                }

                logger["Main"].Info($"{proc.Key} {LocalizationProvider.GetLocalizedValue<string>("MainLogger_ProcessClosed")}");
            }
        }

        private static void StandardOutputHandler(string process, object sender, DataReceivedEventArgs e)
        {
            LogManager.GetLogger(process).Info(e.Data);
        }

        private static void StandardErrorHandler(string process, object sender, DataReceivedEventArgs e)
        {
            LogManager.GetLogger(process).Error(e.Data);
        }

        private void RestartProc(KeyValuePair<string, Process> proc)
        {
            if (proc.Key == "postgres") AddProc("postgres", CreateProcessFromArchive("postgres.zip", "postgres\\bin\\pg_ctl.exe", "start -D data", "postgres\\")); // postgres
            if (proc.Key == "redis") AddProc("redis", CreateProcessFromArchive("redis.zip", "redis\\redis-server.exe")); // redis
            if (proc.Key == "galactus") AddProc("galactus", CreateProcessFromExecutable("galactus.exe")); // galactus
            if (proc.Key == "wingman") AddProc("wingman", CreateProcessFromExecutable("wingman.exe")); // wingman
            if (proc.Key == "automuteus") AddProc("automuteus", CreateProcessFromExecutable("automuteus.exe")); // automuteus

            var process = Procs[proc.Key];

            process.Start();
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();
            var ellipse = IndicatorControls[proc.Key]["Ellipse"] as Ellipse;
            ellipse.Dispatcher.Invoke((Action)(() => ellipse.Fill = Brushes.LimeGreen));
            logger["Main"].Info($"{proc.Key} {LocalizationProvider.GetLocalizedValue<string>("MainLogger_AutoRestarted")}");
        }

        private void StartProcs()
        {
            foreach (var proc in Procs)
            {
                var process = proc.Value;
                process.Start();
                process.BeginErrorReadLine();
                process.BeginOutputReadLine();
                var ellipse = IndicatorControls[proc.Key]["Ellipse"] as Ellipse;
                ellipse.Dispatcher.Invoke((Action)(() => ellipse.Fill = Brushes.LimeGreen));
                logger["Main"].Info($"{proc.Key} {LocalizationProvider.GetLocalizedValue<string>("MainLogger_ProcessStarted")}");
            }
        }

        public static Process CreateProcess(string FileName, string Arguments, string WorkingDir)
        {
            var process = new Process()
            {
                StartInfo =
                {
                    FileName = FileName,
                    Arguments = Arguments,
                    WorkingDirectory = WorkingDir,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true
                }
            };

            MainLogger.Debug("########## New Process Created ##########");
            MainLogger.Debug(JsonConvert.SerializeObject(process.StartInfo));
            MainLogger.Debug("#########################################");
            return process;
        }

        public static void TerminatePostgresServer()
        {
            try
            {
                if (!File.Exists(Path.Combine(Settings.GetUserVar("EnvPath"), "postgres\\bin\\postgres.exe"))) {
                    return;
                }
                var server_process = Main.CreateProcessFromArchive("postgres.zip", "postgres\\bin\\pg_ctl.exe", "stop -w -D data", "postgres\\");
                server_process.Start();
                _ = Main.ConsumeOutputReader("postgres", server_process.StandardOutput);
                _ = Main.ConsumeErrorReader("postgres", server_process.StandardError);
                server_process.WaitForExit();
                MainLogger.Info(LocalizationProvider.GetLocalizedValue<string>("MainLogger_PostgresClosed"));

                Ellipse ellipse = null;
                try
                {
                    if (IndicatorControls != null && IndicatorControls.ContainsKey("postgres")) ellipse = IndicatorControls["postgres"]["Ellipse"] as Ellipse;
                }
                catch
                { 
                }
                if (ellipse != null) ellipse.Dispatcher.Invoke((Action)(() => ellipse.Fill = Brushes.Red));
            }
            catch
            {
                MainLogger.Error(LocalizationProvider.GetLocalizedValue<string>("MainLogger_PostgresFailedToClose"));
                MainLogger.Error(LocalizationProvider.GetLocalizedValue<string>("MainLogger_TryCloseManually"));
            }
        }

        public static void TerminateRedisServer()
        {
            try
            {
                if (!File.Exists(Path.Combine(Settings.GetUserVar("EnvPath"), "redis\\redis-server.exe")))
                {
                    return;
                }
                var server_process = Main.CreateProcessFromArchive("redis.zip", "redis\\redis-cli.exe", "shutdown");
                Main.RedirectProcessStandardIO("redis", server_process);
                server_process.Start();
                server_process.BeginErrorReadLine();
                server_process.BeginOutputReadLine();
                server_process.WaitForExit();
                MainLogger.Info("redis closed.");

                Ellipse ellipse = null;
                try
                {
                    if (IndicatorControls != null && IndicatorControls.ContainsKey("redis")) ellipse = IndicatorControls["redis"]["Ellipse"] as Ellipse;
                }
                catch
                {
                }
                if (ellipse != null) ellipse.Dispatcher.Invoke((Action)(() => ellipse.Fill = Brushes.Red));
            }
            catch
            {
                MainLogger.Error(LocalizationProvider.GetLocalizedValue<string>("MainLogger_RedisFailToClose"));
                MainLogger.Error(LocalizationProvider.GetLocalizedValue<string>("MainLogger_TryCloseManually"));
            }
        }

        public static Process CreateProcessFromExecutable(string FileName, string Arguments = "", string WorkingDir = "")
        {
            string envPath = Settings.GetUserVar("EnvPath");
            string path = Path.Combine(envPath, FileName);
            if (!File.Exists(path))
            {
                Settings.LoadSettings();
            }

            WorkingDir = Path.Combine(envPath, WorkingDir);

            return CreateProcess(path, Arguments, WorkingDir);
        }

        public static Process CreateProcessFromArchive(string FileName, string ExecutablePath, string Arguments = "", string WorkingDir = "")
        {
            string envPath = Settings.GetUserVar("EnvPath");

            if (!File.Exists(Path.Combine(envPath, FileName)))
            {
                Settings.LoadSettings();
            }

            WorkingDir = Path.Combine(envPath, WorkingDir);

            return CreateProcess(Path.Combine(envPath, ExecutablePath), Arguments, WorkingDir);
        }

        public static void RedirectProcessStandardIO(string key, Process process)
        {
            process.OutputDataReceived += new DataReceivedEventHandler((object sender, DataReceivedEventArgs e) => StandardOutputHandler(key, sender, e));
            process.ErrorDataReceived += new DataReceivedEventHandler((object sender, DataReceivedEventArgs e) => StandardErrorHandler(key, sender, e));
        }

        private void AddProc(string key, Process process)
        {
            RedirectProcessStandardIO(key, process);
            if (!Procs.ContainsKey(key)) Procs.Add(key, process);
            else Procs[key] = process;
        }

        public async static Task ConsumeOutputReader(string name, TextReader reader)
        {
            string text;

            while ((text = await reader.ReadLineAsync()) != null)
            {
                LogManager.GetLogger(name).Info(text);
            }
        }

        public async static Task ConsumeErrorReader(string name, TextReader reader)
        {
            string text;

            while ((text = await reader.ReadLineAsync()) != null)
            {
                LogManager.GetLogger(name).Error(text);
            }
        }

        private void DownloadRedisCli()
        {
            var requiredComponents = RequiredComponents[Settings.GetUserVar("ARCHITECTURE")];

            if (requiredComponents.Contains("redis") && Directory.Exists(Path.Combine(Properties.Settings.Default.EnvPath, "redis\\")) && !File.Exists(Path.Combine(Properties.Settings.Default.EnvPath, "redis\\redis-cli.exe")))
            {
                var envPath = Properties.Settings.Default.EnvPath;

                MainLogger.Info(LocalizationProvider.GetLocalizedValue<string>("MainLogger_RedisCli_StartDownload"));
                using (WebClient client = new WebClient())
                {
                    var path = Path.Combine(envPath, "redis\\redis-cli.exe");
                    client.DownloadFile("https://github.com/mtaku3/AutoMuteUs-Portable/releases/download/v2.4.1/redis-cli.exe", path);
                    MainLogger.Info(LocalizationProvider.GetLocalizedValue<string>("MainLogger_RedisCli_Loaded"));
                }
            }
        }

        private void InitializeProcs()
        {
            var requiredComponents = RequiredComponents[Settings.GetUserVar("ARCHITECTURE")];

            DownloadRedisCli();

            if (requiredComponents.Contains("postgres")) AddProc("postgres", CreateProcessFromArchive("postgres.zip", "postgres\\bin\\pg_ctl.exe", "start -D data", "postgres\\")); // postgres
            if (requiredComponents.Contains("redis"))  AddProc("redis", CreateProcessFromArchive("redis.zip", "redis\\redis-server.exe", "redis.windows.conf", "redis\\")); // redis
            if (requiredComponents.Contains("galactus"))  AddProc("galactus", CreateProcessFromExecutable("galactus.exe")); // galactus
            if (requiredComponents.Contains("wingman")) AddProc("wingman", CreateProcessFromExecutable("wingman.exe")); // wingman
            if (requiredComponents.Contains("automuteus"))  AddProc("automuteus", CreateProcessFromExecutable("automuteus.exe")); // automuteus
        }

        private void InitializeProcIndicators()
        {
            IndicatorControls = new Dictionary<string, Dictionary<string, UIElement>>();

            var indicatorsGrid = mainWindow.indicatorsGrid;

            for (var ind = 0; ind < Procs.Count(); ind++)
            {
                var proc = Procs.ElementAt(ind);

                var diameter = 10;

                indicatorsGrid.Dispatcher.Invoke((Action)(() =>
                {
                    var controls = new Dictionary<string, UIElement>();

                    var grid = new Grid();
                    controls.Add("Grid", grid);

                    for (var i = 0; i < 2; i++)
                    {
                        grid.ColumnDefinitions.Add(new ColumnDefinition()
                        {
                            Width = GridLength.Auto
                        });
                    }

                    var ellipse = new Ellipse()
                    {
                        VerticalAlignment = VerticalAlignment.Center,
                        Height = diameter,
                        Width = diameter,
                        Fill = Brushes.DarkGray,
                        Margin = new Thickness(2, 0, 4, 2)
                    };
                    controls.Add("Ellipse", ellipse);
                    grid.Children.Add(ellipse);
                    Grid.SetColumn(ellipse, 0);

                    var textBlock = new TextBlock()
                    {
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Height = 19,
                        Text = proc.Key,
                        Margin = new Thickness(0, 0, 0, 2)
                    };
                    controls.Add("TextBlock", textBlock);
                    grid.Children.Add(textBlock);
                    Grid.SetColumn(textBlock, 1);

                    for (var i = 0; i < 2; i++)
                    {
                        indicatorsGrid.RowDefinitions.Add(new RowDefinition()
                        {
                            Height = GridLength.Auto
                        });
                    }
                    indicatorsGrid.Children.Add(grid);
                    Grid.SetRow(grid, ind * 2);

                    if (ind != Procs.Count() - 1)
                    {
                        var separator = new Separator()
                        {
                            Margin = new Thickness(0, 2, 0, 2)
                        };
                        controls.Add("Separator", separator);
                        indicatorsGrid.Children.Add(separator);
                        Grid.SetRow(separator, ind * 2 + 1);
                    }

                    IndicatorControls.Add(proc.Key, controls);
                }));
            }
        }

        private void InitializeLogger()
        {
            var list = new[] { "Main", "automuteus", "galactus", "wingman", "postgres", "redis" };
            foreach (var name in list)
            {
                logger.Add(name, NLog.LogManager.GetLogger(name));
            }
        }
    }
}
