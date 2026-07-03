using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Undefined.Mods;
using Undefined.MENUSETTINGS;
using static Undefined.Mods.ModButtons;
using static Undefined.MENUSETTINGS.Settings;

namespace Undefined.Utilities
{
    public class SaveHandler : MonoBehaviour
    {
        [Serializable]
        public class IncrementalEntry
        {
            public string key;
            public int value;
        }

        [Serializable]
        public class ModEntry
        {
            public string key;
            public bool value;
        }

        [Serializable]
        public class SaveData
        {
            public int fontIndex;
            public string buttonSound;
            public string menuOpenSound;
            public string menuCloseSound;
            public string notificationSound;

            public List<IncrementalEntry> incrementals = new List<IncrementalEntry>();
            public List<ModEntry> mods = new List<ModEntry>();
        }

        public static SaveHandler Instance;
        public SaveData data = new SaveData();

        private static string _gorillaPath;

        string BasePath =>
            Path.Combine(GetGorillaTagPath(), Constants.PluginName);

        string Folder => Path.Combine(BasePath, Constants.PluginName);
        string FilePath => Path.Combine(Folder, "save.json");

        void Awake()
        {
            Instance = this;
            Load();
        }

        void OnApplicationQuit()
        {
            Save();
        }

        private static string GetGorillaTagPath()
        {
            if (!string.IsNullOrEmpty(_gorillaPath))
                return _gorillaPath;

            string appPath = Application.dataPath;
            if (!string.IsNullOrEmpty(appPath))
            {
                string dir = Path.GetDirectoryName(appPath);
                if (!string.IsNullOrEmpty(dir))
                {
                    string exe = Path.Combine(dir, "Gorilla Tag.exe");
                    if (File.Exists(exe))
                    {
                        _gorillaPath = dir;
                        return _gorillaPath;
                    }
                }
            }

            string[] roots = {
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFilesX86),
                Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles)
            };

            foreach (string root in roots)
            {
                if (string.IsNullOrEmpty(root)) continue;

                string steam = Path.Combine(root, "Steam");
                if (Directory.Exists(steam))
                {
                    string path = FindGorillaTag(steam);
                    if (!string.IsNullOrEmpty(path))
                    {
                        _gorillaPath = path;
                        return _gorillaPath;
                    }
                }
            }

            try
            {
                DriveInfo[] drives = DriveInfo.GetDrives();
                foreach (DriveInfo drive in drives)
                {
                    if (!drive.IsReady) continue;

                    string[] folders = Directory.GetDirectories(drive.RootDirectory.FullName, "Steam", SearchOption.AllDirectories);
                    foreach (string folder in folders)
                    {
                        string path = FindGorillaTag(folder);
                        if (!string.IsNullOrEmpty(path))
                        {
                            _gorillaPath = path;
                            return _gorillaPath;
                        }
                    }
                }
            }
            catch { }

            string[] fallbacks = {
                @"C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag",
                @"C:\Program Files\Steam\steamapps\common\Gorilla Tag",
                @"D:\Steam\steamapps\common\Gorilla Tag",
                @"E:\Steam\steamapps\common\Gorilla Tag",
                @"C:\Steam\steamapps\common\Gorilla Tag",
                @"D:\Games\Steam\steamapps\common\Gorilla Tag"
            };

            foreach (string path in fallbacks)
            {
                if (Directory.Exists(path))
                {
                    _gorillaPath = path;
                    return _gorillaPath;
                }
            }

            _gorillaPath = Application.dataPath;
            return _gorillaPath;
        }

        private static string FindGorillaTag(string steamPath)
        {
            string[] libs = {
                Path.Combine(steamPath, "steamapps"),
                Path.Combine(Path.GetDirectoryName(steamPath), "steamapps")
            };

            foreach (string lib in libs)
            {
                if (!Directory.Exists(lib)) continue;

                string gt = Path.Combine(lib, "common", "Gorilla Tag");
                if (Directory.Exists(gt))
                    return gt;
            }

            string vdf = Path.Combine(steamPath, "steamapps", "libraryfolders.vdf");
            if (!File.Exists(vdf)) return null;

            try
            {
                string[] lines = File.ReadAllLines(vdf);
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];
                    if (!line.Contains("path")) continue;

                    int first = line.IndexOf('"');
                    int second = line.IndexOf('"', first + 1);
                    int third = line.IndexOf('"', second + 1);
                    int fourth = line.IndexOf('"', third + 1);

                    if (third == -1 || fourth == -1) continue;

                    string path = line.Substring(third + 1, fourth - third - 1);
                    path = path.Replace("/", "\\").Trim();

                    if (string.IsNullOrEmpty(path)) continue;
                    if (path.IndexOfAny(Path.GetInvalidPathChars()) >= 0) continue;

                    string steamapps = Path.Combine(path, "steamapps");
                    if (!Directory.Exists(steamapps)) continue;

                    string gt = Path.Combine(steamapps, "common", "Gorilla Tag");
                    if (Directory.Exists(gt))
                        return gt;
                }
            }
            catch { }

            return null;
        }

        public void Save()
        {
            data.fontIndex = Settings.fontIndex;

            data.buttonSound = Undefined.Mods.Categories.Settings.currentButtonSound;
            data.menuOpenSound = Undefined.Mods.Categories.Settings.currentMenuOpenSound;
            data.menuCloseSound = Undefined.Mods.Categories.Settings.currentMenuCloseSound;
            data.notificationSound = Undefined.Mods.Categories.Settings.currentNotificationSound;

            data.incrementals.Clear();
            data.mods.Clear();

            List<ButtonInfo> activeMods = ModButtons.GetActiveMods();

            HashSet<string> activeModNames = new HashSet<string>();
            foreach (ButtonInfo mod in activeMods)
            {
                if (!string.IsNullOrEmpty(mod.buttonText))
                    activeModNames.Add(mod.buttonText);
            }

            for (int i = 0; i < buttons.Length; i++)
            {
                for (int j = 0; j < buttons[i].Length; j++)
                {
                    var b = buttons[i][j];
                    if (b == null) continue;

                    if (b.isIncremental)
                    {
                        data.incrementals.Add(new IncrementalEntry
                        {
                            key = b.buttonText,
                            value = b.currentIncrementalIndex
                        });
                    }

                    if (b.isTogglable && !string.IsNullOrEmpty(b.buttonText) && !b.buttonText.StartsWith("Return"))
                    {
                        data.mods.Add(new ModEntry
                        {
                            key = b.buttonText,
                            value = activeModNames.Contains(b.buttonText)
                        });
                    }
                }
            }

            Directory.CreateDirectory(Folder);
            File.WriteAllText(FilePath, JsonUtility.ToJson(data, true));
        }

        public void Load()
        {
            if (!File.Exists(FilePath))
            {
                Save();
                return;
            }

            data = JsonUtility.FromJson<SaveData>(File.ReadAllText(FilePath));
            Apply();
        }

        void Apply()
        {
            Settings.fontIndex = data.fontIndex;

            Undefined.Mods.Categories.Settings.currentButtonSound = data.buttonSound;
            Undefined.Mods.Categories.Settings.currentMenuOpenSound = data.menuOpenSound;
            Undefined.Mods.Categories.Settings.currentMenuCloseSound = data.menuCloseSound;
            Undefined.Mods.Categories.Settings.currentNotificationSound = data.notificationSound;

            for (int i = 0; i < buttons.Length; i++)
            {
                for (int j = 0; j < buttons[i].Length; j++)
                {
                    var b = buttons[i][j];
                    if (b == null) continue;

                    if (b.isIncremental)
                    {
                        for (int k = 0; k < data.incrementals.Count; k++)
                        {
                            if (data.incrementals[k].key == b.buttonText)
                            {
                                b.currentIncrementalIndex = data.incrementals[k].value;
                                b.incrementalMethod?.Invoke(b.GetCurrentIncrementalValue());
                                break;
                            }
                        }
                    }

                    if (b.isTogglable && !string.IsNullOrEmpty(b.buttonText))
                    {
                        for (int k = 0; k < data.mods.Count; k++)
                        {
                            if (data.mods[k].key == b.buttonText)
                            {
                                bool state = data.mods[k].value;

                                if (b.enabled != state)
                                {
                                    b.enabled = state;

                                    if (state) b.enableMethod?.Invoke();
                                    else b.disableMethod?.Invoke();
                                }

                                break;
                            }
                        }
                    }
                }
            }
        }

        public void SaveNow() => Save();
    }
}