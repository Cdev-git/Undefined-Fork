using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using UnityEngine;

namespace Undefined.Utilities
{
    public static class AudioHandler
    {
        private static readonly Dictionary<string, AudioClip> sounds = new();
        private static readonly HttpClient client = new HttpClient();

        private static readonly string[] Folders =
        {
            "Open-Close",
            "Click-Sounds",
            "Notification-Sounds"
        };

        private static string BasePath =>
            Path.Combine(GetGorillaTagPath(), Constants.PluginName, "Sounds");

        private static string GitBase =>
            Constants.UndefinedResourcesRawUrl;

        public static void LoadSounds()
        {
            Directory.CreateDirectory(BasePath);
            sounds.Clear();

            foreach (var folder in Folders)
                LoadFolder(folder);
        }

        private static void LoadFolder(string folder)
        {
            string localFolder = Path.Combine(BasePath, folder);
            Directory.CreateDirectory(localFolder);

            string manifestUrl = GitBase + folder + "/manifest.txt";

            string manifest;

            try
            {
                manifest = client.GetStringAsync(manifestUrl).Result;
            }
            catch
            {
                return;
            }

            string[] files = manifest.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var file in files)
            {
                if (!file.EndsWith(".wav")) continue;
                ProcessFile(folder, file.Trim(), localFolder);
            }
        }

        private static void ProcessFile(string folder, string file, string localFolder)
        {
            string name = Path.GetFileNameWithoutExtension(file);
            string localPath = Path.Combine(localFolder, file);
            string url = GitBase + folder + "/" + file;

            try
            {
                if (!File.Exists(localPath))
                {
                    byte[] data = client.GetByteArrayAsync(url).Result;
                    File.WriteAllBytes(localPath, data);
                }

                byte[] audio = File.ReadAllBytes(localPath);
                AudioClip clip = WavUtility.ToAudioClip(audio, name);

                if (clip != null)
                    sounds[name] = clip;
            }
            catch { }
        }

        public static void Play(string soundName, float volume = 1f)
        {
            if (!sounds.TryGetValue(soundName, out AudioClip clip))
                return;

            GameObject obj = new GameObject("Sound_" + soundName);
            AudioSource src = obj.AddComponent<AudioSource>();

            src.clip = clip;
            src.volume = volume;
            src.spatialBlend = 0f;
            src.Play();

            UnityEngine.Object.Destroy(obj, clip.length);
        }

        private static string _gorillaPath;

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
    }
}