using BepInEx;
using CXS;
using HarmonyLib;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using Undefined.Menu;
using Undefined.Utilities;
using UnityEngine;
using UnityEngine.Networking;
using Valve.Newtonsoft.Json.Linq;
using JObject = Newtonsoft.Json.Linq.JObject;

namespace Undefined;

[BepInPlugin(Constants.PluginGUID, Constants.PluginName, Constants.PluginVersion)]
public class Plugin : BaseUnityPlugin
{
    public static Plugin Instance { get; private set; }

    public GameObject ComponentHolder { get; private set; }

    private Harmony harmony;

    private bool versionOkay;
    private bool initialized;

    private Version latestVersion;
    private Version minimumVersion;


    private void Awake()
    {
        Instance = this;

        GorillaTagger.OnPlayerSpawned(OnPlayerSpawned);
    }


    private void Start()
    {
        CXS.CXS.LoadCXS();

        AudioHandler.LoadSounds();

        harmony = new Harmony(Constants.PluginGUID);
        harmony.PatchAll();


        ComponentHolder = new GameObject("Undefined");

        DontDestroyOnLoad(ComponentHolder);

        ComponentHolder.AddComponent<InputHandler>();
        ComponentHolder.AddComponent<Main>();
        ComponentHolder.AddComponent<NotificationLib>();

        StartCoroutine(StartVersionCheck());
    }


    private void OnPlayerSpawned()
    {
        if (!versionOkay || initialized)
            return;


        initialized = true;


        ComponentHolder.AddComponent<InputHandler>();
        ComponentHolder.AddComponent<Main>();


        StartCoroutine(VersionLoop());
    }



    private IEnumerator StartVersionCheck()
    {
        yield return CheckVersion(true);
    }



    private IEnumerator VersionLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(300f);

            yield return CheckVersion(false);
        }
    }



    private IEnumerator CheckVersion(bool startup)
    {
        using UnityWebRequest request = UnityWebRequest.Get(Constants.UndefinedDataUrl);

        yield return request.SendWebRequest();


        if (request.result != UnityWebRequest.Result.Success)
        {
            if (startup)
            {
                NotificationLib.SendNotification(
                    NotificationLib.NotificationType.Error,
                    "Unable to connect to Undefined servers."
                );
            }

            yield break;
        }


        JObject data = JObject.Parse(request.downloadHandler.text);


        latestVersion = new Version(
            data["menu-version"]?.ToString() ?? "0.0.0"
        );


        minimumVersion = new Version(
            data["min-version"]?.ToString() ?? "0.0.0"
        );


        Version current = new Version(Constants.PluginVersion);



        if (current < minimumVersion)
        {
            versionOkay = false;

            NotificationLib.SendNotification(
                NotificationLib.NotificationType.Error,
                "Your Undefined version is outdated. Please update the menu."
            );

            yield break;
        }



        if (current > latestVersion)
        {
            versionOkay = false;

            NotificationLib.SendNotification(
                NotificationLib.NotificationType.Error,
                "Modified or invalid Undefined version detected. Please download the official version."
            );

            yield break;
        }



        if (current < latestVersion)
        {
            if (startup)
            {
                NotificationLib.SendNotification(
                    NotificationLib.NotificationType.Alert,
                    $"Undefined update available.\nLatest: {latestVersion}\nCurrent: {current}"
                );
            }

            versionOkay = true;

            yield break;
        }



        if (startup)
        {
            NotificationLib.SendNotification(
                NotificationLib.NotificationType.Info,
                "Undefined is up to date!"
            );
        }


        versionOkay = true;
    }
}