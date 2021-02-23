using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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
        private Dictionary<string, Dictionary<string, UIElement>> IndicatorControls;
        private MainWindow mainWindow;

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
            InitializeLogger();
            InitializeProcs();
            STATask.Run(() => InitializeProcIndicators()).Wait();
            StartProcs();
            Task.Factory.StartNew(() => RunLoop());
        }

        private void RunLoop()
        {
            while (true)
            {
                foreach (var proc in Procs)
                {
                    var process = proc.Value;

                    if (process.HasExited && proc.Key != "postgres")
                    {
                        TerminateProcs();
                        return;
                    }
                }
            }
        }

        public void TerminateProcs()
        {
            Ellipse ellipse;

            foreach (var proc in Procs)
            {
                try
                {
                    var process = proc.Value;
                    if (!process.HasExited)
                    {
                        process.Kill();
                        process.WaitForExit();
                    }
                    ellipse = IndicatorControls[proc.Key]["Ellipse"] as Ellipse;
                    ellipse.Dispatcher.Invoke((Action)(() => ellipse.Fill = Brushes.Red));

                    logger["Main"].Info($"{proc.Key} closed.");
                }
                catch
                {
                }
            }

            TerminatePostgresServer();
        }

        private static void StandardOutputHandler(string process, object sender, DataReceivedEventArgs e)
        {
            LogManager.GetLogger(process).Info(e.Data);
        }

        private static void StandardErrorHandler(string process, object sender, DataReceivedEventArgs e)
        {
            LogManager.GetLogger(process).Error(e.Data);
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
                logger["Main"].Info($"{proc.Key} started.");
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
                var server_process = Main.CreateProcessFromArchive("postgres.zip", "postgres\\bin\\pg_ctl.exe", "-D data stop", "postgres\\");
                Main.RedirectProcessStandardIO("postgres", server_process);
                server_process.Start();
                server_process.BeginErrorReadLine();
                server_process.BeginOutputReadLine();
                server_process.WaitForExit();
                MainLogger.Info("postgres closed.");
            }
            catch
            {
                MainLogger.Error("Failed to close postgres.");
                MainLogger.Error("Try to close manually.");
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
            Procs.Add(key, process);
        }

        private void InitializeProcs()
        {
            var requiredComponents = RequiredComponents[Settings.GetUserVar("ARCHITECTURE")];

            if (requiredComponents.Contains("postgres")) AddProc("postgres", CreateProcessFromArchive("postgres.zip", "postgres\\bin\\pg_ctl.exe", "-D data start", "postgres\\")); // postgres
            if (requiredComponents.Contains("redis"))  AddProc("redis", CreateProcessFromArchive("redis.zip", "redis\\redis-server.exe")); // redis
            if (requiredComponents.Contains("wingman")) AddProc("wingman", CreateProcessFromExecutable("wingman.exe")); // wingman
            if (requiredComponents.Contains("galactus"))  AddProc("galactus", CreateProcessFromExecutable("galactus.exe")); // galactus
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
