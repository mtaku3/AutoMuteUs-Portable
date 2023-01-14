using Ionic.Zip;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace AutoMuteUs_Portable
{
    static class Settings
    {
        private static NLog.Logger logger = NLog.LogManager.GetLogger("Main");

        public static Dictionary<string, string> EnvVars;
        public static Dictionary<string, string> UserVars;

        public static Dictionary<string, int> EnvLines;

        public static Dictionary<string, Dictionary<string, string>> VersionList;

        public static void LoadSettings(bool ForceToSet = false)
        {
            var ForceToLoad = ForceToSet;

            while (true)
            {
                EnvVars = new Dictionary<string, string>();
                UserVars = new Dictionary<string, string>();
                EnvLines = new Dictionary<string, int>();

                GetListFromGithub();

                var properties = new[] { "EnvPath", "ARCHITECTURE", "AUTOMUTEUS_TAG", "GALACTUS_TAG", "WINGMAN_TAG", "AUTOMUTEUS_AUTORESTART", "GALACTUS_AUTORESTART", "WINGMAN_AUTORESTART" };

                foreach (string property in properties)
                {
                    if (property != "RVC_Hash") UserVars.Add(property, (string)Properties.Settings.Default[property]);
                }

                logger.Debug("UserVars loaded.");
                logger.Debug("########## UserVars ##########");
                logger.Debug(JsonConvert.SerializeObject(UserVars));
                logger.Debug("##############################");

                if (!Directory.Exists(GetUserVar("EnvPath")))
                {
                    Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                        var chooseEnvPathWindow = new ChooseEnvPathWindow();
                        chooseEnvPathWindow.ShowDialog();
                    });

                    SettingsWindow.useRecommendedVersionCombination = true;
                }

                logger.Info($"EnvPath: {GetUserVar("EnvPath")}");

                var requiredComponent = Main.RequiredComponents[GetUserVar("ARCHITECTURE")];

                if (!File.Exists(Path.Combine(GetUserVar("EnvPath"), ".env")))
                {
                    DownloadEnv(GetUserVar("EnvPath"), GetUserVar("ARCHITECTURE"));
                }

                if (!File.Exists(Path.Combine(GetUserVar("EnvPath"), "diffenvs.json")))
                {
                    LoadDiffEnvs(GetUserVar("EnvPath"), GetUserVar("ARCHITECTURE"));
                }

                EnvVars = LoadEnv(Path.Combine(GetUserVar("EnvPath"), ".env"));

                logger.Debug("EnvVars loaded.");
                logger.Debug("########## EnvVars ##########");
                logger.Debug(JsonConvert.SerializeObject(EnvVars));
                logger.Debug("#############################");

                if (CheckAllRequiredVariable(UserVars, EnvVars) || ForceToSet)
                {
                    Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                        var settingsWindow = new SettingsWindow();
                        if (!settingsWindow.hasClosed) settingsWindow.ShowDialog();
                    });

                    ForceToSet = false;
                }
                else
                {
                    try
                    {
                        LoadBinaries(ForceToLoad);
                        if (requiredComponent.Contains("postgres")) SetupPostgres();
                        return;
                    }
                    catch (Exception e)
                    {
                        logger.Error($"{LocalizationProvider.GetLocalizedValue<string>("MainLogger_FailToLoadBinary")} {e.Message}");
                        continue;
                    }
                }
            }
        }

        public static bool CheckAllRequiredVariable(Dictionary<string, string> UserVars, Dictionary<string, string> EnvVars)
        {
            foreach (var variable in UserVars) {
                if (CheckRequiredVariable(variable.Key, variable.Value) != null) return true; 
            }

            foreach (var variable in EnvVars)
            {
                if (CheckRequiredVariable(variable.Key, variable.Value) != null) return true;
            }

            return false;
        }

        #nullable enable
        public static string? CheckRequiredVariable(string Key, string Value)
        {
            var EmptyAllowed = new[]
            {
                "EMOJI_GUILD_ID",
                "WORKER_BOT_TOKENS",
                "CAPTURE_TIMEOUT",
                "AUTOMUTEUS_LISTENING",
                "DISCORD_BOT_TOKEN_2",
                "BASE_MAP_URL",
                "AUTOMUTEUS_GLOBAL_PREFIX",
                "SLASH_COMMAND_GUILD_IDS",
                "STOP_GRACE_PERIOD"
            };

            if (EmptyAllowed.Contains(Key))
            {
                return null;
            }
            else
            {
                if (Value == "")
                {
                    return Key;
                }
                return null;
            }
        }
        #nullable restore

        public static void DownloadEnv(string envPath, string ARCHITECTURE)
        {
            var url = $"https://raw.githubusercontent.com/mtaku3/AutoMuteUs-Portable/main/{ARCHITECTURE}.env";
            logger.Info($"{ARCHITECTURE}.env {LocalizationProvider.GetLocalizedValue<string>("MainLogger_EnvFile_StartDownload")}");
            using (WebClient client = new WebClient())
            {
                var path = Path.Combine(envPath, ".env");
                string result = client.DownloadString(url);
                File.WriteAllText(path, result);
                logger.Info(LocalizationProvider.GetLocalizedValue<string>("MainLogger_EnvFile_Loaded"));
                LoadDiffEnvs(envPath, ARCHITECTURE);
                SaveUserVar("ARCHITECTURE", ARCHITECTURE);
            }
        }

        public static void SetupPostgres()
        {
            SetupPostgres(GetEnvVar("POSTGRES_USER"), GetEnvVar("POSTGRES_PASS"));
        }

        private static void UpdatePostgresUser(string user)
        {
            WriteEnvVar("POSTGRES_USER", user);
        }

        private static void UpdatePostgresPass(string pass)
        {
            WriteEnvVar("POSTGRES_PASS", pass);
        }

        public static void SetupPostgres(string newUser, string newPass)
        {
            string oldUser = GetEnvVar("POSTGRES_USER");
            string oldPass = GetEnvVar("POSTGRES_PASS");

            if (!Directory.Exists(Path.Combine(GetUserVar("EnvPath"), "postgres\\")))
            {
                LoadPostgresBinary();
            }

            try
            {
                Process process, server_process = null;
                bool IsServerStartUp = false;
                if (!Directory.Exists(Path.Combine(GetUserVar("EnvPath"), "postgres\\data")))
                {
                    logger.Info(LocalizationProvider.GetLocalizedValue<string>("MainLogger_Postgres_StartInitialize#1"));
                    process = Main.CreateProcessFromArchive("postgres.zip", "postgres\\bin\\initdb.exe", $"-E UTF8 -U root -A trust -D data", "postgres\\");
                    Main.RedirectProcessStandardIO("postgres", process);
                    process.Start();
                    process.BeginErrorReadLine();
                    process.BeginOutputReadLine();
                    process.WaitForExit();
                    oldPass = "";

                    server_process = Main.CreateProcessFromArchive("postgres.zip", "postgres\\bin\\pg_ctl.exe", "start -w -D data", "postgres\\");
                    server_process.Start();
                    _ = Main.ConsumeOutputReader("postgres", server_process.StandardOutput);
                    _ = Main.ConsumeErrorReader("postgres", server_process.StandardError);
                    if (!server_process.HasExited) server_process.WaitForExit();
                    IsServerStartUp = true;

                    process = Main.CreateProcessFromArchive("postgres.zip", "postgres\\bin\\psql.exe", $"-U root -d postgres -c \"CREATE DATABASE root owner root;\"");
                    Main.RedirectProcessStandardIO("postgres", process);
                    process.Start();
                    process.BeginErrorReadLine();
                    process.BeginOutputReadLine();
                    process.WaitForExit();
                    logger.Info($"{LocalizationProvider.GetLocalizedValue<string>("MainLogger_Postgres_DatabaseCreated")} \"root\".");

                    process = Main.CreateProcessFromArchive("postgres.zip", "postgres\\bin\\createuser.exe", $"-U root --superuser {newUser}");
                    Main.RedirectProcessStandardIO("postgres", process);
                    process.Start();
                    process.BeginErrorReadLine();
                    process.BeginOutputReadLine();
                    process.WaitForExit();
                    logger.Info($"{LocalizationProvider.GetLocalizedValue<string>("MainLogger_Postgres_UserCreated")} \"{newUser}\".");

                    if (newUser != "postgres")
                    {
                        process = Main.CreateProcessFromArchive("postgres.zip", "postgres\\bin\\psql.exe", $"-U root -c \"ALTER DATABASE postgres RENAME TO {newUser};\"");
                        Main.RedirectProcessStandardIO("postgres", process);
                        process.Start();
                        process.BeginErrorReadLine();
                        process.BeginOutputReadLine();
                        process.WaitForExit();
                        logger.Info($"{LocalizationProvider.GetLocalizedValue<string>("MainLogger_Postgres_DatabaseRenamed")} \"{newUser}\".");
                    }
                    UpdatePostgresUser(newUser);
                    oldUser = newUser;

                    logger.Info(LocalizationProvider.GetLocalizedValue<string>("MainLogger_Postgres_StartInitialize#2"));
                    process = Main.CreateProcessFromArchive("postgres.zip", "postgres\\bin\\psql.exe", $"-U {newUser} -f postgres.sql", "postgres\\");
                    Main.RedirectProcessStandardIO("postgres", process);
                    process.Start();
                    process.BeginErrorReadLine();
                    process.BeginOutputReadLine();
                    process.WaitForExit();
                    if (process.ExitCode != 0)
                    {
                        logger.Error(LocalizationProvider.GetLocalizedValue<string>("MainLogger_Postgres_FailToInitialize"));
                        logger.Error(LocalizationProvider.GetLocalizedValue<string>("MainLogger_Postgres_InitializeManually"));
                    }
                    else
                    {
                        logger.Info(LocalizationProvider.GetLocalizedValue<string>("MainLogger_Postgres_InitializedSuccessfully"));
                    }
                }

                if (oldUser != newUser || oldPass != newPass)
                {
                    logger.Info(LocalizationProvider.GetLocalizedValue<string>("MainLogger_Postgres_StartSetup"));
                    if (!IsServerStartUp)
                    {
                        server_process = Main.CreateProcessFromArchive("postgres.zip", "postgres\\bin\\pg_ctl.exe", "start -w -D data", "postgres\\");
                        server_process.Start();
                        _ = Main.ConsumeOutputReader("postgres", server_process.StandardOutput);
                        _ = Main.ConsumeErrorReader("postgres", server_process.StandardError);
                        if (!server_process.HasExited) server_process.WaitForExit();
                        IsServerStartUp = true;
                    }

                    if (oldUser != newUser)
                    {
                        logger.Info($"{LocalizationProvider.GetLocalizedValue<string>("MainLogger_Postgres_RenameUser")} '{oldUser}' => '{newUser}'.");
                        process = Main.CreateProcessFromArchive("postgres.zip", "postgres\\bin\\psql.exe", $"-U root -c \"ALTER USER {oldUser} RENAME TO {newUser};\"", "postgres\\");
                        Main.RedirectProcessStandardIO("postgres", process);
                        process.Start();
                        process.BeginErrorReadLine();
                        process.BeginOutputReadLine();
                        process.WaitForExit();
                        oldPass = "";

                        process = Main.CreateProcessFromArchive("postgres.zip", "postgres\\bin\\psql.exe", $"-U root -c \"ALTER DATABASE {oldUser} RENAME TO {newUser};\"", "postgres\\");
                        Main.RedirectProcessStandardIO("postgres", process);
                        process.Start();
                        process.BeginErrorReadLine();
                        process.BeginOutputReadLine();
                        process.WaitForExit();

                        if (process.ExitCode != 0)
                        {
                            logger.Error(LocalizationProvider.GetLocalizedValue<string>("MainLogger_Postgres_FailToRenameUser"));
                            logger.Error($"{LocalizationProvider.GetLocalizedValue<string>("MainLogger_Postgres_TryRenameUserManually")} '{oldUser}'.");
                        }
                        else
                        {
                            logger.Info($"{LocalizationProvider.GetLocalizedValue<string>("MainLogger_Postgres_RenamedUserSuccessfully")} '{newUser}'.");
                        }
                        UpdatePostgresUser(newUser);
                        oldUser = newUser;
                    }
                    if (oldPass != newPass)
                    {
                        logger.Info($"{LocalizationProvider.GetLocalizedValue<string>("MainLogger_Postgres_UpdatePass")} '{oldPass}' => '{newPass}'.");
                        process = Main.CreateProcessFromArchive("postgres.zip", "postgres\\bin\\psql.exe", $"-U root -c \"ALTER USER {newUser} WITH PASSWORD '{newPass}';\"", "postgres\\");
                        Main.RedirectProcessStandardIO("postgres", process);
                        process.Start();
                        process.BeginErrorReadLine();
                        process.BeginOutputReadLine();
                        process.WaitForExit();
                        if (process.ExitCode != 0)
                        {
                            logger.Error(LocalizationProvider.GetLocalizedValue<string>("MainLogger_Postgres_FailToRenamePass"));
                            logger.Error($"{LocalizationProvider.GetLocalizedValue<string>("MainLogger_Postgres_TryUpdatePasswordManually")} '{oldPass}'.");
                        }
                        else
                        {
                            logger.Info($"{LocalizationProvider.GetLocalizedValue<string>("MainLogger_Postgres_UpdatedPasswordSuccessfully")} '{newPass}'.");
                        }
                        UpdatePostgresPass(newPass);
                        oldPass = newPass;
                    }

                    try
                    {
                        Main.TerminatePostgresServer();
                    }
                    catch
                    {

                    }
                    try
                    {
                        if (server_process != null) server_process.Kill();
                    }
                    catch
                    {

                    }
                }
            }
            catch (Exception e)
            {
                logger.Error($"{LocalizationProvider.GetLocalizedValue<string>("MainLogger_Postgres_FailToSetup")} {e.Message}");
                return;
            }
        }

        public static void LoadWingmanExecutable()
        {
            var envPath = GetUserVar("EnvPath");

            logger.Info(LocalizationProvider.GetLocalizedValue<string>("MainLogger_Wingman_StartDownload"));
            using (WebClient client = new WebClient())
            {
                var path = Path.Combine(envPath, "wingman.exe");
                client.DownloadFile(VersionList["wingman"][GetUserVar("WINGMAN_TAG")], path);
                logger.Info(LocalizationProvider.GetLocalizedValue<string>("MainLogger_Wingman_Loaded"));
            }
        }

        public static void LoadGalactusExecutable()
        {
            var envPath = GetUserVar("EnvPath");

            logger.Info(LocalizationProvider.GetLocalizedValue<string>("MainLogger_Galactus_StartDownload"));
            using (WebClient client = new WebClient())
            {
                var path = Path.Combine(envPath, "galactus.exe");
                client.DownloadFile(VersionList["galactus"][GetUserVar("GALACTUS_TAG")], path);
                logger.Info(LocalizationProvider.GetLocalizedValue<string>("MainLogger_Galactus_Loaded"));
            }
        }

        public static void LoadAutomuteusExecutable()
        {
            var envPath = GetUserVar("EnvPath");

            logger.Info(LocalizationProvider.GetLocalizedValue<string>("MainLogger_Automuteus_StartDownload"));
            using (WebClient client = new WebClient())
            {
                var path = Path.Combine(envPath, Path.GetFileName(VersionList["automuteus"][GetUserVar("AUTOMUTEUS_TAG")]));
                client.DownloadFile(VersionList["automuteus"][GetUserVar("AUTOMUTEUS_TAG")], path);
                if (Path.GetExtension(path) == ".zip")
                {
                    using (ZipFile zipFile = ZipFile.Read(path))
                    {
                        path = envPath;
                        zipFile.ExtractAll(path, ExtractExistingFileAction.OverwriteSilently);
                    };
                }
                logger.Info(LocalizationProvider.GetLocalizedValue<string>("MainLogger_Automuteus_Loaded"));
            }
        }

        public static void LoadRedisBinary()
        {
            var envPath = GetUserVar("EnvPath");

            logger.Info(LocalizationProvider.GetLocalizedValue<string>("MainLogger_Redis_StartDownload"));
            using (WebClient client = new WebClient())
            {
                var path = Path.Combine(envPath, "redis.zip");
                client.DownloadFile("https://github.com/AutoMuteUs-Portable/old.redis/releases/download/5.0.10/redis.zip", path);
                using (ZipFile zipFile = ZipFile.Read(path))
                {
                    path = envPath;
                    zipFile.ExtractAll(path, ExtractExistingFileAction.OverwriteSilently);
                    logger.Info(LocalizationProvider.GetLocalizedValue<string>("MainLogger_Redis_Loaded"));
                };
            }
        }

        public static void LoadPostgresBinary()
        {
            var envPath = GetUserVar("EnvPath");

            logger.Info(LocalizationProvider.GetLocalizedValue<string>("MainLogger_Postgres_StartDownload"));
            using (WebClient client = new WebClient())
            {
                var path = Path.Combine(envPath, "postgres.zip");
                client.DownloadFile("https://github.com/AutoMuteUs-Portable/old.postgres/releases/download/12.6/postgres.zip", path);
                using (ZipFile zipFile = ZipFile.Read(path))
                {
                    path = envPath;
                    zipFile.ExtractAll(path, ExtractExistingFileAction.OverwriteSilently);
                    logger.Info(LocalizationProvider.GetLocalizedValue<string>("MainLogger_Postgres_Loaded"));
                };
            }

            if (!File.Exists(Path.Combine(envPath, "postgres\\postgres.sql"))) 
            {
                LoadPostgresSql();
            }
        }

        public static void LoadDiffEnvs(string envPath, string ARCHITECTURE)
        {
            var url = $"https://raw.githubusercontent.com/mtaku3/AutoMuteUs-Portable/main/{ARCHITECTURE}.diffenvs.json";
            logger.Info($"{ARCHITECTURE}.diffenvs.json {LocalizationProvider.GetLocalizedValue<string>("MainLogger_LoadDiffEnvs_StartDownload")}");
            using (WebClient client = new WebClient())
            {
                var path = Path.Combine(envPath, "diffenvs.json");
                string result = client.DownloadString(url);
                File.WriteAllText(path, result);
                logger.Info(LocalizationProvider.GetLocalizedValue<string>("MainLogger_LoadDiffEnvs_Loaded"));
            }
        }

        public static void LoadPostgresSql()
        {
            var envPath = GetUserVar("EnvPath");

            var url = $"https://raw.githubusercontent.com/AutoMuteUs-Portable/automuteus/{GetUserVar("AUTOMUTEUS_TAG")}/storage/postgres.sql";

            logger.Info(LocalizationProvider.GetLocalizedValue<string>("MainLogger_Postgres_SQLFile_StartDownload"));
            using (WebClient client = new WebClient())
            {
                var path = Path.Combine(envPath, "postgres\\postgres.sql");
                string result = client.DownloadString(url);
                File.WriteAllText(path, result);
                logger.Info(LocalizationProvider.GetLocalizedValue<string>("MainLogger_Postgres_SQLFile_Loaded"));
            }
        }

        private static bool RequireComponent(string Key)
        {
            var requiredComponents = Main.RequiredComponents[Settings.GetUserVar("ARCHITECTURE")];

            if (Key == "EnvPath" || Key == "ARCHITECTURE") return true;

            if (requiredComponents.Contains(Key)) return true;

            return false;
        }

        public static void LoadBinaries(bool ForceToLoad = false)
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
                    logger.Error($"{LocalizationProvider.GetLocalizedValue<string>("MainLogger_LoadBinaries_FailToCreateDirectory")} \"{envPath}\": {e.Message}");
                    return;
                }
            }

            if ((!Directory.Exists(Path.Combine(envPath, "postgres\\")) && RequireComponent("postgres")))
            {
                LoadPostgresBinary();
            }

            if (!Directory.Exists(Path.Combine(envPath, "redis\\")) && RequireComponent("redis"))
            {
                LoadRedisBinary();
            }

            if (!File.Exists(Path.Combine(envPath, ".env")) || ForceToLoad)
            {
                string[] allLines = new string[EnvVars.Count];

                int index = 0;
                foreach (var variable in EnvVars)
                {
                    allLines[index] = $"{variable.Key}={variable.Value}";
                    index++;
                }

                File.WriteAllLines(Path.Combine(envPath, ".env"), allLines);

                logger.Info($"{LocalizationProvider.GetLocalizedValue<string>("MainLogger_LoadBinaries_EnvFileSaved")} {envPath}");
            }

            if (!File.Exists(Path.Combine(envPath, "diffenvs.json")) || ForceToLoad)
            {
                LoadDiffEnvs(envPath, GetUserVar("ARCHITECTURE"));
            }

            if (!File.Exists(Path.Combine(envPath, "automuteus.exe")) || ForceToLoad && RequireComponent("automuteus"))
            {
                LoadAutomuteusExecutable();
            }

            if (!File.Exists(Path.Combine(envPath, "galactus.exe")) || ForceToLoad && RequireComponent("galactus"))
            {
                LoadGalactusExecutable();
            }

            if (!File.Exists(Path.Combine(envPath, "wingman.exe")) || ForceToLoad && RequireComponent("wingman"))
            {
                LoadWingmanExecutable();
            }
        }

        public static void GetListFromGithub()
        {
            var list = new Dictionary<string, string>()
            {
                { "automuteus", "https://raw.githubusercontent.com/mtaku3/AutoMuteUs-Portable/old/automuteus.list" },
                { "galactus", "https://raw.githubusercontent.com/mtaku3/AutoMuteUs-Portable/old/galactus.list" },
                { "wingman", "https://raw.githubusercontent.com/mtaku3/AutoMuteUs-Portable/old/wingman.list" }
            };

            VersionList = new Dictionary<string, Dictionary<string, string>>();

            logger.Debug("Version list has been downloading.");
            foreach (var item in list)
            {
                var url = item.Value;

                string ss = $"########## {item.Key} ##########";
                string es = "";
                for (var i = 0; i < ss.Length; i++) es += "#";

                logger.Debug(ss);
                logger.Debug(item.Value);
                using (WebClient client = new WebClient())
                {
                    string downloadedString = client.DownloadString(url);
                    var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(downloadedString);
                    logger.Debug(downloadedString);
                    VersionList.Add(item.Key, result);
                }
                logger.Debug(es);
            }
        }

        private static Dictionary<string, string> LoadEnv(string envPath)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            using (StreamReader reader = new StreamReader(envPath))
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

        private static void WriteEnvVar(string Key, string Value)
        {
            logger.Debug($"{Key}: \"{GetEnvVar(Key)}\" => \"{Value}\"");

            var envPath = GetUserVar("EnvPath");

            EnvVars[Key] = Value;

            var path = Path.Combine(envPath, ".env");

            string[] allLines = File.ReadAllLines(path);
            allLines[EnvLines[Key]] = $"{Key}={Value}";
            File.WriteAllLines(path, allLines);
        }

        public static void SetEnvVar(string Key, string Value)
        {
            if (EnvVars.ContainsKey(Key) && GetEnvVar(Key) != Value)
            {
                if (Key == "POSTGRES_PASS")
                {
                    SetupPostgres(GetEnvVar("POSTGRES_USER"), Value);
                }
                else if (Key == "POSTGRES_USER")
                {
                    SetupPostgres(Value, GetEnvVar("POSTGRES_PASS"));
                }
                else
                {
                    WriteEnvVar(Key, Value);
                }
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

        private static void SaveUserVar(string Key, string Value)
        {
            logger.Debug($"{Key}: \"{GetUserVar(Key)}\" => \"{Value}\"");

            UserVars[Key] = Value;

            Properties.Settings.Default[Key] = Value;
            Properties.Settings.Default.Save();
        }

        private static void DirectoryMove(string OldDirectory, string NewDirectory)
        {
            if (!Directory.Exists(OldDirectory))
            {
                return;
            }

            var stack = new Stack<Dictionary<string, string>>();
            stack.Push(new Dictionary<string, string>()
            {
                { "src", OldDirectory },
                { "dest", NewDirectory }
            });

            string src, dest;
            while (stack.Count != 0)
            {
                var cur = stack.Pop();
                src = cur["src"];
                dest = cur["dest"];
                if (!Directory.Exists(dest))
                {
                    Directory.Move(src, dest);
                }
                else
                {
                    string[] filePaths = Directory.GetFiles(src);
                    foreach (string filePath in filePaths)
                    {
                        File.Move(filePath, Path.Combine(dest, Path.GetFileName(filePath)), true);
                    }

                    string[] directoryPaths = Directory.GetDirectories(src);
                    foreach (string directoryPath in directoryPaths)
                    {
                        stack.Push(new Dictionary<string, string>()
                        {
                            { "src", directoryPath },
                            { "dest", Path.Combine(dest, Path.GetFileName(directoryPath)) }
                        });
                    }
                }
            }

            if (Directory.Exists(OldDirectory))
            {
                Directory.Delete(OldDirectory);
            }
        }

        private static void EnvPathChange(string OldDirectory, string NewDirectory)
        {
            if (!Directory.Exists(OldDirectory))
            {
                if (!Directory.Exists(NewDirectory))
                {
                    Directory.CreateDirectory(NewDirectory);
                }

                return;
            }

            try
            {
                DirectoryMove(OldDirectory, NewDirectory);

            }
            catch (Exception e)
            {
                MessageBox.Show($"{LocalizationProvider.GetLocalizedValue<string>("Settings_EnvPathChange_FailToMoveEnvPath_Text")} {e.Message}");
                logger.Error($"{LocalizationProvider.GetLocalizedValue<string>("MainLogger_EnvPathChange_FailToMoveEnvPath")} {e.Message}");
            }
        }

        public static void SetUserVar(string Key, string Value)
        {
            if (UserVars.ContainsKey(Key) && GetUserVar(Key) != Value)
            {
                if (Key == "EnvPath")
                {
                    EnvPathChange(GetUserVar("EnvPath"), Value);

                    SaveUserVar(Key, Value);
                }
                else if (Key == "ARCHITECTURE")
                {
                    DownloadEnv(GetUserVar("EnvPath"), Value);
                }
                else
                {
                    SaveUserVar(Key, Value);

                    switch (Key)
                    {
                        case "AUTOMUTEUS_TAG":
                            LoadAutomuteusExecutable();
                            break;
                        case "GALACTUS_TAG":
                            LoadGalactusExecutable();
                            break;
                        case "WINGMAN_TAG":
                            LoadWingmanExecutable();
                            break;
                    }
                }
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
