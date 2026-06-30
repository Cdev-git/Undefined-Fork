using System;
using UnityEngine;

namespace Undefined.Utilities;

public static class WavUtility
{
    public static AudioClip ToAudioClip(byte[] data, string name)
    {
        try
        {
            int channels = BitConverter.ToInt16(data, 22);
            int sampleRate = BitConverter.ToInt32(data, 24);

            int dataPosition = FindDataChunk(data);

            if (dataPosition == -1)
            {
                Debug.LogError("[WavUtility] Could not find audio data");
                return null;
            }


            int dataSize = BitConverter.ToInt32(data, dataPosition + 4);

            float[] samples = new float[dataSize / 2];


            for (int i = 0; i < samples.Length; i++)
            {
                short value = BitConverter.ToInt16(
                    data,
                    dataPosition + 8 + i * 2
                );

                samples[i] = value / 32768f;
            }


            AudioClip clip = AudioClip.Create(
                name,
                samples.Length / channels,
                channels,
                sampleRate,
                false
            );


            clip.SetData(samples, 0);

            return clip;
        }
        catch (Exception e)
        {
            Debug.LogError("[WavUtility] Failed: " + e);
            return null;
        }
    }


    private static int FindDataChunk(byte[] data)
    {
        for (int i = 0; i < data.Length - 4; i++)
        {
            if (data[i] == 'd' &&
                data[i + 1] == 'a' &&
                data[i + 2] == 't' &&
                data[i + 3] == 'a')
            {
                return i;
            }
        }

        return -1;
    }
}