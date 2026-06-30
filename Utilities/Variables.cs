using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Undefined.Utilities;

public class Variables
{
    public static GameObject activeMenu;
    public static GameObject bgObject;
    public static GameObject handPointer;
    public static GameObject menuCanvas;
    public static SphereCollider triggerCollider;
    public static Camera spectatorCamera;
    public static Text fpsLabel;
    public static int activePage = 0;
    public static int categoryIndex;

    public static bool fpsCounter = true;
    public static bool disconnectButton = true;
    public static bool rightHanded;
    public static bool disableNotifications;

    public static KeyCode keyboardButton = KeyCode.X;

    public static Vector3 menuSize = new Vector3(0.1f, 1.8f, 0.65f);

    public static int buttonsPerPage = 8;

    public static float gradientSpeed = 0.5f;

    public static readonly int TransparentFX = LayerMask.NameToLayer("TransparentFX");
    public static readonly int IgnoreRaycast = LayerMask.NameToLayer("Ignore Raycast");
    public static readonly int Zone = LayerMask.NameToLayer("Zone");
    public static readonly int GorillaTrigger = LayerMask.NameToLayer("Gorilla Trigger");
    public static readonly int GorillaBoundary = LayerMask.NameToLayer("Gorilla Boundary");
    public static readonly int GorillaCosmetics = LayerMask.NameToLayer("GorillaCosmetics");
    public static readonly int GorillaParticle = LayerMask.NameToLayer("GorillaParticle");

    public static int NoInvisLayerMask() =>
        ~(1 << TransparentFX | 1 << IgnoreRaycast | 1 << Zone | 1 << GorillaTrigger | 1 << GorillaBoundary | 1 << GorillaCosmetics | 1 << GorillaParticle);

    public static Vector3 RandomVector3(float range = 1f)
    {
        return UnityEngine.Random.insideUnitSphere * range;
    }

    public static Quaternion RandomQuaternion()
    {
        return UnityEngine.Random.rotationUniform;
    }

    public static Color RandomColor()
    {
        return (Color32)(new Color32((byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), byte.MaxValue));
    }

    public static bool IsMaster()
    {
        return PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient;
    }


    // gun lib stuff

    public Vector3 PointerScale { get; set; } = new Vector3(0.2f, 0.2f, 0.2f);
    public Color32 PointerColorStart { get; set; } = new Color32(0, 255, 100, 255);
    public Color32 PointerColorEnd { get; set; } = new Color32(0, 200, 255, 255);
    public Color32 TriggeredPointerColorStart { get; set; } = new Color32(255, 100, 50, 255);
    public Color32 TriggeredPointerColorEnd { get; set; } = new Color32(255, 150, 0, 255);
    public float LineWidth { get; set; } = 0.03f;
    public Color32 LineColorStart { get; set; } = new Color32(0, 255, 150, 255);
    public Color32 LineColorEnd { get; set; } = new Color32(0, 180, 255, 255);
    public Color32 TriggeredLineColorStart { get; set; } = new Color32(255, 100, 50, 255);
    public Color32 TriggeredLineColorEnd { get; set; } = new Color32(255, 150, 0, 255);
    public bool EnableAnimations { get; set; } = true;
    public float PulseSpeed { get; set; } = 2f;
    public float PulseAmplitude { get; set; } = 0.04f;
    public bool EnableParticles { get; set; } = true;
    public float ParticleStartSize { get; set; } = 0.1f;
    public float ParticleStartSpeed { get; set; } = 0.5f;
    public int ParticleMaxCount { get; set; } = 100;
    public float ParticleEmissionRate { get; set; } = 20f;
    public bool EnableBoxESP { get; set; } = true;
    public float BoxESPWidth { get; set; } = 1f;
    public float BoxESPHeight { get; set; } = 2f;
    public Color32 BoxESPColor { get; set; } = new Color32(0, 255, 100, 255);
    public Color32 BoxESPOuterColor { get; set; } = new Color32(255, 150, 0, 255);
    public int LineCurve { get; set; } = 150;
    public float WaveFrequency { get; set; } = 5f;
    public float WaveAmplitude { get; set; } = 0.05f;

    internal bool isShooting;
    internal bool isTriggered;
    internal bool isLocked;
}

public class ButtonInfo
{
    public string buttonText = "-";
    public string overlapText = null;
    public Action method = null;
    public Action enableMethod = null;
    public Action disableMethod = null;
    public bool enabled = false;
    public bool isTogglable = true;
    public string toolTip = "";
}
