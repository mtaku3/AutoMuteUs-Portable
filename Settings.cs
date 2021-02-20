using Ionic.Zip;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AutoMuteUs_Portable
{
    static class Settings
    {
        private static NLog.Logger logger = NLog.LogManager.GetLogger("Settings");

        public static Dictionary<string, string> EnvVars;
        public static Dictionary<string, string> UserVars;

        public static Dictionary<string, int> EnvLines;

        public static Dictionary<string, Dictionary<string, string>> VersionList;

        public static string POSTGRES_USER;
        public static string POSTGRES_PASS;

        public static void LoadSettings()
        {
            while (true)
            {
                EnvVars = new Dictionary<string, string>();
                UserVars = new Dictionary<string, string>();
                EnvLines = new Dictionary<string, int>();
                POSTGRES_USER = Properties.Settings.Default.POSTGRES_USER;
                POSTGRES_PASS = Properties.Settings.Default.POSTGRES_PASS;

                GetListFromGithub();

                var properties = new[] { "EnvPath", "ARCHITECTURE", "AUTOMUTEUS_TAG", "GALACTUS_TAG", "WINGMAN_TAG" };

                foreach (string property in properties)
                {
                        UserVars.Add(property, (string)Properties.Settings.Default[property]);
                }

                if (!File.Exists(Path.Combine(GetUserVar("EnvPath"), ".env")))
                {
                    UserVars["EnvPath"] = Path.Combine(Path.GetTempPath(), "AutoMuteUs-Portable\\");
                    if (!Directory.Exists(GetUserVar("EnvPath"))) Directory.CreateDirectory(GetUserVar("EnvPath"));
                    File.WriteAllBytes(Path.Combine(GetUserVar("EnvPath"), ".env"), Properties.Resources.env);
                    Properties.Settings.Default.EnvPath = GetUserVar("EnvPath");
                    Properties.Settings.Default.Save();
                }

                EnvVars = LoadEnv(Path.Combine(GetUserVar("EnvPath"), ".env"));

                if (!EnvVars.ContainsKey("DISCORD_BOT_TOKEN") || EnvVars["DISCORD_BOT_TOKEN"] == "")
                {
                    var settingsWindow = new SettingsWindow();
                    settingsWindow.ShowDialog();
                }
                else
                {
                    try
                    {
                        LoadBinaries();
                        SetupPostgres(GetEnvVar("POSTGRES_USER"), GetEnvVar("POSTGRES_PASS"));
                        break;
                    }
                    catch (Exception e)
                    {
                        logger.Error($"Failed to load binaries: {e.Message}");
                        continue;
                    }
                }
            }
        }

        public static void SetupPostgres()
        {
            SetupPostgres(POSTGRES_USER, POSTGRES_PASS);
        }

        private static void SetPostgressUser(string user)
        {
            Properties.Settings.Default.POSTGRES_USER = user;
            POSTGRES_USER = user;
        }

        private static void SetPostgresPass(string pass)
        {
            Properties.Settings.Default.POSTGRES_PASS = pass;
            POSTGRES_PASS = pass;
        }

        public static void SetupPostgres(string newUser, string newPass)
        {
            string oldUser = POSTGRES_USER;
            string oldPass = POSTGRES_PASS;

            var logger = NLog.LogManager.GetLogger("postgres");
            logger.Info("########## BEGIN: Running Setup Postgres ##########");
            try
            {
                Process process;
                if (!Directory.Exists(Path.Combine(GetUserVar("EnvPath"), "postgres\\data")))
                {
                    process = Main.CreateProcessFromArchive("postgres.zip", "postgres\\bin\\initdb.exe", $"-U {newUser} -A trust -E UTF8 -D data", "postgres\\");
                    Main.RedirectProcessStandardIO("postgres", process);
                    process.Start();
                    process.BeginErrorReadLine();
                    process.BeginOutputReadLine();
                    logger.Info($"Process postgres located in {process.StartInfo.FileName} started. Now we wait for exit.");
                    logger.Debug($"Argument: {process.StartInfo.Arguments}");
                    process.WaitForExit();
                    logger.Info($"Process postgres closed.");
                    SetPostgressUser(newUser);
                    oldUser = newUser;
                }

                if (oldUser != newUser || oldPass != newPass)
                {
                    var server_process = Main.CreateProcessFromArchive("postgres.zip", "postgres\\bin\\pg_ctl.exe", "-D data start", "postgres\\");
                    Main.RedirectProcessStandardIO("postgres", server_process);
                    server_process.Start();
                    server_process.BeginErrorReadLine();
                    server_process.BeginOutputReadLine();
                    logger.Info($"Process postgres located in {server_process.StartInfo.FileName} started.");

                    if (oldUser != newUser)
                    {
                        process = Main.CreateProcessFromArchive("postgres.zip", "postgres\\bin\\psql.exe", $"-U {oldUser} -c \"ALTER USER {oldUser} RENAME TO '{newUser}';\"", "postgres\\");
                        Main.RedirectProcessStandardIO("postgres", process);
                        process.Start();
                        process.BeginErrorReadLine();
                        process.BeginOutputReadLine();
                        logger.Info($"Process postgres located in {process.StartInfo.FileName} started. Now we wait for exit.");
                        process.WaitForExit();
                        logger.Info($"Process postgres closed.");
                        if (process.ExitCode != 0)
                        {
                            logger.Info($"Postgres user name didn't set properly.");
                            logger.Info($"Set it manually. Current user name: {oldUser}.");
                        }
                        else
                        {
                            logger.Info($"Postgres user name successfully set to {newUser}.");
                            SetPostgressUser(newUser);
                            oldUser = newUser;
                        }
                    }
                    if (oldPass != newPass)
                    {
                        process = Main.CreateProcessFromArchive("postgres.zip", "postgres\\bin\\psql.exe", $"-U {newUser} -c \"ALTER USER {newUser} WITH PASSWORD '{newPass}';\"", "postgres\\");
                        Main.RedirectProcessStandardIO("postgres", process);
                        process.Start();
                        process.BeginErrorReadLine();
                        process.BeginOutputReadLine();
                        logger.Info($"Process postgres located in {process.StartInfo.FileName} started. Now we wait for exit.");
                        process.WaitForExit();
                        logger.Info($"Process postgres closed.");
                        if (process.ExitCode != 0)
                        {
                            logger.Info($"Postgres password didn't set properly.");
                            logger.Info($"Set it manually. Current password: {oldPass}.");
                        }
                        else
                        {
                            logger.Info($"Postgres password successfully set to {newPass}.");
                            SetPostgresPass(newPass);
                            oldPass = newPass;
                        }
                    }

                    try
                    {
                        server_process.Kill();
                    }
                    catch
                    {

                    }
                    try
                    {
                        Main.TerminatePostgresServer();
                    }
                    catch
                    {

                    }
                }
            }
            catch (Exception e)
            {
                logger.Error($"Failed to run setup postgres: {e.Message}");
                return;
            }
            logger.Info("########## END: Run Setup Postgres ##########");
        }

        public static void LoadBinaries()
        {
            var envPath = GetUserVar("EnvPath");

            if (!Directory.Exists(envPath))
            {
                try
                {
                    Directory.CreateDirectory(envPath);
                }
                catch (Exception e)
                {
                    logger.Error($"Failed to create directory {envPath}: {e.Message}");
                    return;
                }
            }

            if (!Directory.Exists(Path.Combine(envPath, "postgres\\")))
            {
                using (WebClient client = new WebClient())
                {
                    var path = Path.Combine(envPath, "postgres.zip");
                    client.DownloadFile("https://github.com/AutoMuteUs-Portable/postgres/releases/download/12.6/postgres.zip", path);
                    using (ZipFile zipFile = ZipFile.Read(path))
                    {
                        path = envPath;
                        zipFile.ExtractAll(path, ExtractExistingFileAction.OverwriteSilently);
                        logger.Info($"Binary postgres.zip extracted in {path}.");
                    };
                }
            }

            if (!Directory.Exists(Path.Combine(envPath, "redis\\")))
            {
                using (WebClient client = new WebClient())
                {
                    var path = Path.Combine(envPath, "redis.zip");
                    client.DownloadFile("https://github.com/AutoMuteUs-Portable/redis/releases/download/5.0.10/redis.zip", path);
                    using (ZipFile zipFile = ZipFile.Read(path))
                    {
                        path = envPath;
                        zipFile.ExtractAll(path, ExtractExistingFileAction.OverwriteSilently);
                        logger.Info($"Binary redis.zip extracted in {path}.");
                    };
                }
            }

            if (!File.Exists(Path.Combine(envPath, ".env")))
            {
                string[] allLines = new string[EnvVars.Count];

                int index = 0;
                foreach (var variable in EnvVars)
                {
                    allLines[index] = $"{variable.Key}={variable.Value}";
                    index++;
                }

                File.WriteAllLines(Path.Combine(envPath, ".env"), allLines);

                logger.Info($"File .env saved in {envPath}.");
            }

            if (!File.Exists(Path.Combine(envPath, "automuteus.exe")))
            {
                using (WebClient client = new WebClient())
                {
                    var path = Path.Combine(envPath, "automuteus.exe");
                    client.DownloadFile(VersionList["automuteus"][GetUserVar("AUTOMUTEUS_TAG")], path);
                    logger.Info($"Binary automuteus.exe saved in {path}.");
                }
            }

            if (!File.Exists(Path.Combine(envPath, "galactus.exe")))
            {
                using (WebClient client = new WebClient())
                {
                    var path = Path.Combine(envPath, "galactus.exe");
                    client.DownloadFile(VersionList["galactus"][GetUserVar("GALACTUS_TAG")], path);
                    logger.Info($"Binary galactus.exe saved in {path}.");
                }
            }

            if (GetUserVar("ARCHITECTURE") == "v7")
            {
                if (!File.Exists(Path.Combine(envPath, "wingman.exe")))
                {
                    using (WebClient client = new WebClient())
                    {
                        var path = Path.Combine(envPath, "wingman.exe");
                        client.DownloadFile(VersionList["wingman"][GetUserVar("WINGMAN_TAG")], path);
                        logger.Info($"Binary wingman.exe saved in {path}.");
                    }
                }
            }
        }

        public static void GetListFromGithub()
        {
            var list = new Dictionary<string, string>()
            {
                { "automuteus", "https://raw.githubusercontent.com/mtaku3/AutoMuteUs-Portable/main/automuteus.list" },
                { "galactus", "https://raw.githubusercontent.com/mtaku3/AutoMuteUs-Portable/main/galactus.list" },
                { "wingman", "https://raw.githubusercontent.com/mtaku3/AutoMuteUs-Portable/main/wingman.list" }
            };

            VersionList = new Dictionary<string, Dictionary<string, string>>();

            foreach (var item in list)
            {
                var url = item.Value;

                using (WebClient client = new WebClient())
                {
                    var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(client.DownloadString(url));
                    VersionList.Add(item.Key, result);
                }
            }
        }

        private static Dictionary<string, string> LoadEnv(string envPath)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            using (var reader = new StreamReader(envPath))
            {
                string output;
                int index = 0;
                while ((output = reader.ReadLine()) != null)
                {
                    if (output == "" || output[0] == '#')
                    {
                        index++;
                        continue;
                    }

                    string[] split = output.Split('=');

                    if (split.Length == 1)
                    {
                        result.Add(split[0], "");
                        EnvLines.Add(split[0], index);
                    }
                    else if (split.Length == 2)
                    {
                        result.Add(split[0], split[1]);
                        EnvLines.Add(split[0], index);
                    }

                    index++;
                }
            }

            return result;
        }

        public static void SetEnvVar(string Key, string Value)
        {
            if (EnvVars.ContainsKey(Key) && GetEnvVar(Key) != Value)
            {
                var envPath = GetUserVar("EnvPath");
                string path;

                if (Key == "POSTGRES_PASS")
                {
                    SetupPostgres(Properties.Settings.Default.POSTGRES_USER, Value);
                }
                else if (Key == "POSTGRES_USER")
                {
                    SetupPostgres(Value, Properties.Settings.Default.POSTGRES_PASS);
                }

                EnvVars[Key] = Value;

                path = Path.Combine(envPath, ".env");

                string[] allLines = File.ReadAllLines(path);
                allLines[EnvLines[Key]] = $"{Key}={Value}";
                File.WriteAllLines(path, allLines);
            }
        }

        public static string GetEnvVar(string Key)
        {
            if (EnvVars.ContainsKey(Key))
            {
                return EnvVars[Key];
            }
            else
            {
                return "";
            }
        }

        public static void SetUserVar(string Key, string Value)
        {
            if (UserVars.ContainsKey(Key) && GetUserVar(Key) != Value)
            {
                if (Key == "EnvPath")
                {
                    try
                    {
                        Directory.Move(GetUserVar("EnvPath"), Value);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                        logger.Error(e.Message);
                        return;
                    }
                }

                UserVars[Key] = Value;

                Properties.Settings.Default[Key] = Value;
                Properties.Settings.Default.Save();
            }
        }

        public static string GetUserVar(string Key)
        {
            if (UserVars.ContainsKey(Key))
            {
                return UserVars[Key];
            }
            else
            {
                return "";
            }
        }

    }
}
