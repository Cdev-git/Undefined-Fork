using BepInEx;
using CXS;
using HarmonyLib;
using UnityEngine;
using Undefined.Utilities;
using Undefined.Menu;

namespace Undefined;

[BepInPlugin(Constants.PluginGUID, Constants.PluginName, Constants.PluginVersion)]
public class Plugin : BaseUnityPlugin
{
    public static Plugin Instance { get; private set; }

    public GameObject ComponentHolder { get; private set; }

    private Harmony harmony;

    private void Awake()
    {
        Instance = this;

        GorillaTagger.OnPlayerSpawned(OnPlayerSpawned);
    }

    private void Start()
    {
        CXS.CXS.LoadCXS();

        harmony = new Harmony(Constants.PluginGUID);
        harmony.PatchAll();
    }

    private void OnPlayerSpawned()
    {
        ComponentHolder = new GameObject("menu");

        DontDestroyOnLoad(ComponentHolder);

        ComponentHolder.AddComponent<InputHandler>();
        ComponentHolder.AddComponent<Main>();
        ComponentHolder.AddComponent<NotificationLib>();
    }
}