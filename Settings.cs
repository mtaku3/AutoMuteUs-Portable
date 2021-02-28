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
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

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
                    UserVars.Add(property, (string)Properties.Settings.Default[property]);
                }

                logger.Debug("UserVars loaded.");
                logger.Debug("########## UserVars ##########");
                logger.Debug(JsonConvert.SerializeObject(UserVars));
                logger.Debug("##############################");

                if (!Directory.Exists(GetUserVar("EnvPath")))
                {
                    STATask.Run(() =>
                    {
                        var chooseEnvPathWindow = new ChooseEnvPathWindow();
                        chooseEnvPathWindow.ShowDialog();
                    }).Wait();
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
                    STATask.Run(() =>
                    {
                        var settingsWindow = new SettingsWindow();
                        settingsWindow.ShowDialog();
                    }).Wait();
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
                        logger.Error($"Failed to load binaries: {e.Message}");
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
                "DISCORD_BOT_TOKEN_2"
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
            logger.Info($"{ARCHITECTURE}.env has been downloading.");
            using (WebClient client = new WebClient())
            {
                var path = Path.Combine(envPath, ".env");
                string result = client.DownloadString(url);
                File.WriteAllText(path, result);
                logger.Info(".env successfully loaded.");
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
                    logger.Info("Initializing Postgres server.");
                    process = Main.CreateProcessFromArchive("postgres.zip", "postgres\\bin\\initdb.exe", $"-E UTF8 -U root -A trust --lc-messages=en_US -D data", "postgres\\");
                    Main.RedirectProcessStandardIO("postgres", process);
                    process.Start();
                    process.BeginErrorReadLine();
                    process.BeginOutputReadLine();
                    process.WaitForExit();
                    logger.Info($"Initialized Postgres server.");
                    oldPass = "";

                    server_process = Main.CreateProcessFromArchive("postgres.zip", "postgres\\bin\\pg_ctl.exe", "-w -D data start", "postgres\\");
                    Main.RedirectProcessStandardIO("postgres", server_process);
                    server_process.Start();
                    server_process.BeginErrorReadLine();
                    server_process.BeginOutputReadLine();
                    IsServerStartUp = true;

                    logger.Info("Creating database named \"root\".");
                    process = Main.CreateProcessFromArchive("postgres.zip", "postgres\\bin\\psql.exe", $"-U root -d postgres -c \"CREATE DATABASE root owner root;\"");
                    Main.RedirectProcessStandardIO("postgres", process);
                    process.Start();
                    process.BeginErrorReadLine();
                    process.BeginOutputReadLine();
                    process.WaitForExit();
                    logger.Info("Created database named \"root\".");

                    logger.Info($"Creating user named \"{newUser}\".");
                    process = Main.CreateProcessFromArchive("postgres.zip", "postgres\\bin\\createuser.exe", $"-U root --superuser {newUser}");
                    Main.RedirectProcessStandardIO("postgres", process);
                    process.Start();
                    process.BeginErrorReadLine();
                    process.BeginOutputReadLine();
                    process.WaitForExit();
                    logger.Info($"Created user named \"{newUser}\".");

                    logger.Info($"Creating database named \"{newUser}\".");
                    process = Main.CreateProcessFromArchive("postgres.zip", "postgres\\bin\\psql.exe", $"-U root -c \"CREATE DATABASE {newUser} owner {newUser};\"");
                    Main.RedirectProcessStandardIO("postgres", process);
                    process.Start();
                    process.BeginErrorReadLine();
                    process.BeginOutputReadLine();
                    process.WaitForExit();
                    logger.Info($"Creating database named \"{newUser}\".");
                    UpdatePostgresUser(newUser);
                    oldUser = newUser;

                    logger.Info("Initializing Postgres database using postgres.sql");
                    process = Main.CreateProcessFromArchive("postgres.zip", "postgres\\bin\\psql.exe", $"-U {newUser} -f postgres.sql", "postgres\\");
                    Main.RedirectProcessStandardIO("postgres", process);
                    process.Start();
                    process.BeginErrorReadLine();
                    process.BeginOutputReadLine();
                    process.WaitForExit();
                    if (process.ExitCode != 0)
                    {
                        logger.Error("Failed to initialize Postgres database properly.");
                        logger.Error($"Initialize manually.");
                    }
                    else
                    {
                        logger.Info($"Successfully initialized Postgres database");
                    }
                }

                if (oldUser != newUser || oldPass != newPass)
                {
                    logger.Info("Postgres started to setup.");
                    if (!IsServerStartUp)
                    {
                        server_process = Main.CreateProcessFromArchive("postgres.zip", "postgres\\bin\\pg_ctl.exe", "-w -D data start", "postgres\\");
                        Main.RedirectProcessStandardIO("postgres", server_process);
                        server_process.Start();
                        server_process.BeginErrorReadLine();
                        server_process.BeginOutputReadLine();
                        IsServerStartUp = true;
                    }

                    if (oldUser != newUser)
                    {
                        logger.Info($"Updating Postgres user name '{oldUser}' => '{newUser}'.");
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
                            logger.Error("Postgres user name didn't set properly.");
                            logger.Error($"Set it manually. Current user name: '{oldUser}'.");
                        }
                        else
                        {
                            logger.Info($"Postgres user name successfully set to '{newUser}'.");
                        }
                        UpdatePostgresUser(newUser);
                        oldUser = newUser;
                    }
                    if (oldPass != newPass)
                    {
                        logger.Info($"Updating Postgres pass '{oldPass}' => '{newPass}'.");
                        process = Main.CreateProcessFromArchive("postgres.zip", "postgres\\bin\\psql.exe", $"-U root -c \"ALTER USER {newUser} WITH PASSWORD '{newPass}';\"", "postgres\\");
                        Main.RedirectProcessStandardIO("postgres", process);
                        process.Start();
                        process.BeginErrorReadLine();
                        process.BeginOutputReadLine();
                        process.WaitForExit();
                        if (process.ExitCode != 0)
                        {
                            logger.Error($"Postgres password didn't set properly.");
                            logger.Error($"Set it manually. Current password: '{oldPass}'.");
                        }
                        else
                        {
                            logger.Info($"Postgres password successfully set to '{newPass}'.");
                        }
                        UpdatePostgresPass(newPass);
                        oldPass = newPass;
                    }

                    try
                    {
                        if (server_process != null) server_process.Kill();
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
                logger.Error($"Failed to setup Postgres: {e.Message}");
                return;
            }
        }

        public static void LoadWingmanExecutable()
        {
            var envPath = GetUserVar("EnvPath");

            logger.Info("wingman.exe has been downloading.");
            using (WebClient client = new WebClient())
            {
                var path = Path.Combine(envPath, "wingman.exe");
                client.DownloadFile(VersionList["wingman"][GetUserVar("WINGMAN_TAG")], path);
                logger.Info("wingman.exe successfully loaded.");
            }
        }

        public static void LoadGalactusExecutable()
        {
            var envPath = GetUserVar("EnvPath");

            logger.Info("galactus.exe has been downloading.");
            using (WebClient client = new WebClient())
            {
                var path = Path.Combine(envPath, "galactus.exe");
                client.DownloadFile(VersionList["galactus"][GetUserVar("GALACTUS_TAG")], path);
                logger.Info("galactus.exe successfully loaded.");
            }
        }

        public static void LoadAutomuteusExecutable()
        {
            var envPath = GetUserVar("EnvPath");

            logger.Info("automuteus.exe has been downloading.");
            using (WebClient client = new WebClient())
            {
                var path = Path.Combine(envPath, "automuteus.exe");
                client.DownloadFile(VersionList["automuteus"][GetUserVar("AUTOMUTEUS_TAG")], path);
                logger.Info("automuteus.exe successfully loaded.");
            }
        }

        public static void LoadRedisBinary()
        {
            var envPath = GetUserVar("EnvPath");

            logger.Info("redis.zip has been downloading.");
            using (WebClient client = new WebClient())
            {
                var path = Path.Combine(envPath, "redis.zip");
                client.DownloadFile("https://github.com/AutoMuteUs-Portable/redis/releases/download/5.0.10/redis.zip", path);
                using (ZipFile zipFile = ZipFile.Read(path))
                {
                    path = envPath;
                    zipFile.ExtractAll(path, ExtractExistingFileAction.OverwriteSilently);
                    logger.Info("Redis' binary successfully loaded.");
                };
            }
        }

        public static void LoadPostgresBinary()
        {
            var envPath = GetUserVar("EnvPath");

            logger.Info("postgres.zip has been downloading.");
            using (WebClient client = new WebClient())
            {
                var path = Path.Combine(envPath, "postgres.zip");
                client.DownloadFile("https://github.com/AutoMuteUs-Portable/postgres/releases/download/12.6/postgres.zip", path);
                using (ZipFile zipFile = ZipFile.Read(path))
                {
                    path = envPath;
                    zipFile.ExtractAll(path, ExtractExistingFileAction.OverwriteSilently);
                    logger.Info("Postgres' binary successfully loaded.");
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
            logger.Info($"{ARCHITECTURE}.diffenvs.json has been downloading.");
            using (WebClient client = new WebClient())
            {
                var path = Path.Combine(envPath, "diffenvs.json");
                string result = client.DownloadString(url);
                File.WriteAllText(path, result);
                logger.Info("diffenvs.json successfully loaded.");
            }
        }

        public static void LoadPostgresSql()
        {
            var envPath = GetUserVar("EnvPath");

            var url = $"https://raw.githubusercontent.com/AutoMuteUs-Portable/automuteus/{GetUserVar("AUTOMUTEUS_TAG")}/storage/postgres.sql";

            logger.Info("postgres.sql has been downloading.");
            using (WebClient client = new WebClient())
            {
                var path = Path.Combine(envPath, "postgres\\postgres.sql");
                string result = client.DownloadString(url);
                File.WriteAllText(path, result);
                logger.Info("postgres.sql successfully loaded.");
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
                    logger.Error($"Failed to create directory \"{envPath}\": {e.Message}");
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

                logger.Info($"File .env saved in {envPath}.");
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
                { "automuteus", "https://raw.githubusercontent.com/mtaku3/AutoMuteUs-Portable/main/automuteus.list" },
                { "galactus", "https://raw.githubusercontent.com/mtaku3/AutoMuteUs-Portable/main/galactus.list" },
                { "wingman", "https://raw.githubusercontent.com/mtaku3/AutoMuteUs-Portable/main/wingman.list" }
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
                MessageBox.Show($"Failed to move EnvPath: {e.Message}");
                logger.Error($"Failed to move EnvPath: {e.Message}");
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
