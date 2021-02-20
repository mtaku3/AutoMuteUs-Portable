using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoMuteUs_Portable
{
    class Main
    {
        private static readonly Dictionary<string, NLog.Logger> logger = new Dictionary<string, NLog.Logger>();
        private static readonly Dictionary<string, Process> Procs = new Dictionary<string, Process>();

        public Main()
        {
            InitializeLogger();
            InitializeProcs();
            StartProcs();
            Task.Factory.StartNew(() => RunLoop());
        }

        private void RunLoop()
        {
            logger["Main"].Debug("########## BEGIN : Running Process Loop ##########");
            while (true)
            {
                foreach (var proc in Procs)
                {
                    var process = proc.Value;

                    if (process.HasExited && proc.Key != "postgres")
                    {
                        logger[proc.Key].Error("Process has been exited");
                        logger["Main"].Debug("########## END : Ending Process Loop ##########");
                        TerminateProcs();
                        return;
                    }
                }
            }
        }

        public void TerminateProcs()
        {
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
                    logger["Main"].Info($"Process {proc.Key} closed.");
                }
                catch
                {
                }
            }

            TerminatePostgresServer();
        }

        private static void StandardOutputHandler(string process, object sender, DataReceivedEventArgs e)
        {
            if (e.Data != "")
            {
                if (logger.ContainsKey(process))
                {
                    logger[process].Info(e.Data);
                }
                else
                {
                    NLog.LogManager.GetLogger(process).Info(e.Data);
                }
            }
        }

        private static void StandardErrorHandler(string process, object sender, DataReceivedEventArgs e)
        {
            if (e.Data != "")
            {
                if (logger.ContainsKey(process))
                {
                    logger[process].Error(e.Data);
                }
                else
                {
                    NLog.LogManager.GetLogger(process).Error(e.Data);
                }
            }
        }

        private void StartProcs()
        {
            logger["Main"].Info("Starting Processes");
            foreach (var proc in Procs)
            {
                var process = proc.Value;
                process.Start();
                process.BeginErrorReadLine();
                process.BeginOutputReadLine();
                logger["Main"].Info($"Process {proc.Key} located in {process.StartInfo.FileName} started.");
            }
        }

        public static Process CreateProcess(string FileName, string Arguments, string WorkingDir)
        {
            return new Process()
            {
                StartInfo =
                {
                    FileName = FileName,
                    Arguments = Arguments,
                    WorkingDirectory = WorkingDir,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    StandardErrorEncoding = Encoding.UTF8,
                    StandardOutputEncoding = Encoding.UTF8
                }
            };
        }

        public static void TerminatePostgresServer()
        {
            var logger = NLog.LogManager.GetLogger("postgres");
            try
            {
                var server_process = Main.CreateProcessFromArchive("postgres.zip", "postgres\\bin\\pg_ctl.exe", "-D data stop", "postgres\\");
                Main.RedirectProcessStandardIO("postgres", server_process);
                server_process.Start();
                server_process.BeginErrorReadLine();
                server_process.BeginOutputReadLine();
                server_process.WaitForExit();
                logger.Info($"Process postgres closed.");
            }
            catch
            {
                logger.Error($"Failed to close process postgres.");
                logger.Error($"Try to close manually.");
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
            logger["Main"].Debug("########## BEGIN : Initializing Processes ##########");

            AddProc("postgres", CreateProcessFromArchive("postgres.zip", "postgres\\bin\\pg_ctl.exe", "-D data start", "postgres\\")); // postgres
            AddProc("redis", CreateProcessFromArchive("redis.zip", "redis\\redis-server.exe")); // redis
            if (Settings.GetUserVar("ARCHITECTURE") == "v7") AddProc("wingman", CreateProcessFromExecutable("wingman.exe")); // wingman
            AddProc("galactus", CreateProcessFromExecutable("galactus.exe")); // galactus
            AddProc("automuteus", CreateProcessFromExecutable("automuteus.exe")); // automuteus

            logger["Main"].Debug("########## END : Initialized Processes ##########");
        }

        private void InitializeLogger()
        {
            var list = new[] { "Main", "automuteus", "galactus", "wingman", "postgres", "redis" };
            foreach (var name in list)
            {
                logger.Add(name, NLog.LogManager.GetLogger(name));
            }
            logger["Main"].Debug("Initialized Logger");
        }
    }
}
