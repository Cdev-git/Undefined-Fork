using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Undefined.Utilities;

public static class AudioHandler
{
    private static readonly Dictionary<string, AudioClip> sounds = new();

    private static readonly string[] SoundList =
    {
        "NotificationSound",
        "Open",
        "Close",
        "click1",
        "click2",
        "click3"
    };

    public static void LoadSounds()
    {
        sounds.Clear();

        foreach (string sound in SoundList)
        {
            Load(sound);
        }
    }


    private static void Load(string soundName)
    {
        string path = $"Undefined.Resources.Embedded.{soundName}.wav";

        Stream stream = typeof(AudioHandler)
            .Assembly
            .GetManifestResourceStream(path);


        if (stream == null)
        {
            Debug.LogError($"[AudioHandler] Missing sound: {soundName}.wav");
            return;
        }


        byte[] bytes = new byte[stream.Length];
        stream.Read(bytes, 0, bytes.Length);


        AudioClip clip = WavUtility.ToAudioClip(bytes, soundName);

        if (clip == null)
        {
            Debug.LogError($"[AudioHandler] Failed loading {soundName}");
            return;
        }

        sounds[soundName] = clip;

        Debug.Log($"[AudioHandler] Loaded {soundName}.wav");
    }


    public static void Play(string soundName, float volume = 1f)
    {
        if (!sounds.TryGetValue(soundName, out AudioClip clip))
        {
            Debug.LogError($"[AudioHandler] Sound not loaded: {soundName}");
            return;
        }


        GameObject obj = new GameObject("Sound_" + soundName);

        AudioSource source = obj.AddComponent<AudioSource>();

        source.clip = clip;
        source.volume = volume;
        source.spatialBlend = 0f;

        source.Play();


        Object.Destroy(obj, clip.length);
    }
}