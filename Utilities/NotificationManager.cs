using Undefined.MENUSETTINGS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using static Undefined.Mods.ModButtons;
using static Undefined.MENUSETTINGS.Settings;
using Undefined.Mods;
using static Undefined.Utilities.Variables;

namespace Undefined.Utilities;

public class NotificationLib : MonoBehaviour
{
    public enum NotificationType
    {
        Enabled,
        Disabled,
        Saved,
        Loaded,
        Deleted,
        Room,
        Error,
        Alert,
        Info
    }

    private static readonly Dictionary<string, float> _notificationTimestamps = new Dictionary<string, float>();

    private GameObject _hudObj;
    private GameObject _hudObj2;
    private GameObject _mainCamera;
    private Text _notificationText;
    private Text _arrayListText;
    private Material _notificationMaterial;
    private Material _arrayListMaterial;

    private readonly List<GameObject> _trackedObjects = new List<GameObject>();
    private bool _hasInitialized;
    private float _fadeAlpha = 1f;
    private bool _isFading;

    private const float NOTIFICATION_DELAY = 3f;
    private const float FADE_DURATION = 0.5f;

    private int _arrayListCacheVersion = -1;
    private string _arrayListCache = "";
    private readonly StringBuilder _arrayListSB = new StringBuilder(512);
    private readonly List<string> _enabledModsBuffer = new List<string>(32);

    public static bool inRoom;
    public static bool RoomNotifications = true;
    public static bool ArrayListEnabled;

    private static readonly Dictionary<NotificationType, string> _typeColors = new Dictionary<NotificationType, string>
    {
        { NotificationType.Enabled, "#00FF00" },
        { NotificationType.Disabled, "#FF4040" },
        { NotificationType.Saved, "#00AAFF" },
        { NotificationType.Loaded, "#00FFFF" },
        { NotificationType.Deleted, "#FF8C00" },
        { NotificationType.Room, "#C040FF" },
        { NotificationType.Error, "#FF0000" },
        { NotificationType.Alert, "#FFD700" },
        { NotificationType.Info, "#B0B0B0" }
    };

    public static string PreviousNotification { get; private set; }
    public static bool IsEnabled { get; set; } = true;
    public static NotificationLib Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        UpdateNotifications();
        UpdateArrayList();
    }

    public void Init()
    {
        if (_hasInitialized)
            return;

        _mainCamera = GameObject.Find("Main Camera");
        if (_mainCamera == null)
            return;

        _hudObj2 = CreateAndTrackHUDObject("HUD_Notification_Parent");
        _hudObj2.transform.position = _mainCamera.transform.position + new Vector3(-1.5f, 0f, -4.5f);

        _hudObj = CreateAndTrackHUDObject("HUD_Notification", _hudObj2.transform);

        Canvas canvas = _hudObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = _mainCamera.GetComponent<Camera>();

        CanvasScaler scaler = _hudObj.AddComponent<CanvasScaler>();
        scaler.dynamicPixelsPerUnit = 10f;

        _hudObj.AddComponent<GraphicRaycaster>();

        RectTransform rectTransform = _hudObj.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(5f, 5f);
        rectTransform.localScale = Vector3.one;
        rectTransform.localPosition = new Vector3(0f, 0f, 1.6f);
        rectTransform.rotation = Quaternion.Euler(0f, -250f, 0f);

        _notificationText = CreateTextElement("NotificationText", _hudObj,
            new Vector3(-1.2f, -0.75f, 0f), new Vector2(300f, 70f), 7);
        _notificationText.font = currentFont;
        _notificationText.fontStyle = FontStyle.Bold;
        _notificationText.alignment = TextAnchor.MiddleCenter;
        _notificationMaterial = new Material(Shader.Find("GUI/Text Shader"));
        _notificationText.material = _notificationMaterial;

        _arrayListText = CreateTextElement("ArrayListText", _hudObj,
            new Vector3(-1.2f, -0.5f, 0f), new Vector2(300f, 400f), 7);
        _arrayListText.font = currentFont;
        _arrayListText.alignment = TextAnchor.UpperLeft;
        _arrayListMaterial = new Material(Shader.Find("GUI/Text Shader"));
        _arrayListText.material = _arrayListMaterial;

        _hasInitialized = true;
    }

    private Text CreateTextElement(string name, GameObject parent, Vector3 position, Vector2 size, int fontSize)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.parent = parent.transform;

        Text text = textObj.AddComponent<Text>();
        text.fontSize = fontSize;
        text.alignment = TextAnchor.MiddleCenter;
        text.rectTransform.sizeDelta = size;
        text.rectTransform.localScale = new Vector3(0.01f, 0.01f, 1f);
        text.rectTransform.localPosition = position;

        _trackedObjects.Add(textObj);
        return text;
    }

    private GameObject CreateAndTrackHUDObject(string name, Transform parent = null)
    {
        GameObject obj = new GameObject(name);
        if (parent != null)
        {
            obj.transform.parent = parent;
        }
        _trackedObjects.Add(obj);
        return obj;
    }

    public void UpdateNotifications()
    {
        if (!_hasInitialized)
            Init();

        if (_hudObj2 != null && _mainCamera != null)
        {
            _hudObj2.transform.SetPositionAndRotation(
                _mainCamera.transform.position,
                _mainCamera.transform.rotation);
        }

        ProcessExpiredNotifications();
    }

    public void ProcessExpiredNotifications()
    {
        if (_notificationTimestamps.Count == 0)
            return;

        float currentTime = Time.time;
        float oldestTimestamp = float.MaxValue;

        foreach (float timestamp in _notificationTimestamps.Values)
        {
            if (timestamp < oldestTimestamp)
                oldestTimestamp = timestamp;
        }

        float elapsed = currentTime - oldestTimestamp;

        if (elapsed >= NOTIFICATION_DELAY)
        {
            if (!_isFading)
            {
                _isFading = true;
                StartCoroutine(FadeOutNotification());
            }
        }
        else
        {
            _fadeAlpha = 1f;
            _isFading = false;
            UpdateTextAlpha();
        }
    }

    public void UpdateNotificationText()
    {
        if (_notificationText != null)
        {
            _notificationText.text = string.Join(Environment.NewLine, _notificationTimestamps.Keys);
        }
    }

    public void UpdateTextAlpha()
    {
        if (_notificationText != null)
        {
            Color color = _notificationText.color;
            color.a = _fadeAlpha;
            _notificationText.color = color;
        }
    }

    public void UpdateArrayList()
    {
        if (!_hasInitialized)
            Init();

        if (_arrayListText == null)
            return;

        if (!ArrayListEnabled)
        {
            _arrayListText.text = "";
            return;
        }

        // Use your new GetActiveMods() method
        List<ButtonInfo> activeMods = ModButtons.GetActiveMods();

        // Simple version check - if the count changed, rebuild the list
        int currentVersion = activeMods.Count;
        if (_arrayListCacheVersion == currentVersion)
        {
            _arrayListText.text = _arrayListCache;
            return;
        }

        _enabledModsBuffer.Clear();
        foreach (ButtonInfo mod in activeMods)
        {
            _enabledModsBuffer.Add(mod.buttonText);
        }

        _enabledModsBuffer.Sort((a, b) => b.Length.CompareTo(a.Length));

        _arrayListSB.Clear();
        for (int i = 0; i < _enabledModsBuffer.Count; i++)
        {
            if (i > 0)
                _arrayListSB.Append('\n');

            _arrayListSB.Append("<color=blue>></color> ");
            _arrayListSB.Append(_enabledModsBuffer[i]);
        }

        _arrayListCache = _arrayListSB.ToString();
        _arrayListCacheVersion = currentVersion;
        _arrayListText.text = _arrayListCache;
    }

    private IEnumerator FadeInNotification()
    {
        if (_isFading)
            yield break;

        float elapsed = 0f;
        while (elapsed < FADE_DURATION)
        {
            elapsed += Time.deltaTime;
            _fadeAlpha = Mathf.Lerp(0f, 1f, elapsed / FADE_DURATION);
            UpdateTextAlpha();
            yield return null;
        }

        _fadeAlpha = 1f;
        UpdateTextAlpha();
    }

    private IEnumerator FadeOutNotification()
    {
        float elapsed = 0f;
        while (elapsed < FADE_DURATION)
        {
            elapsed += Time.deltaTime;
            _fadeAlpha = Mathf.Lerp(1f, 0f, elapsed / FADE_DURATION);
            UpdateTextAlpha();
            yield return null;
        }

        _fadeAlpha = 0f;
        UpdateTextAlpha();

        string oldestKey = null;
        float oldestVal = float.MaxValue;

        foreach (KeyValuePair<string, float> kvp in _notificationTimestamps)
        {
            if (kvp.Value < oldestVal)
            {
                oldestVal = kvp.Value;
                oldestKey = kvp.Key;
            }
        }

        if (oldestKey != null)
        {
            _notificationTimestamps.Remove(oldestKey);
        }

        UpdateNotificationText();
        _isFading = false;
        _fadeAlpha = 1f;
    }

    public static void SendNotification(NotificationType type, string content)
    {
        if (!IsEnabled ||
            string.IsNullOrEmpty(content) ||
            Instance == null || Instance._notificationText == null)
            return;

        string colorHex;
        if (!_typeColors.TryGetValue(type, out colorHex))
            colorHex = "#FFFFFF";

        string text = $"<color={colorHex}>{type}</color> : {content}";

        if (text == PreviousNotification)
            return;

        _notificationTimestamps[text] = Time.time;
        PreviousNotification = text;

        AudioHandler.Play("NotificationSound", 0.5f);

        Instance.UpdateNotificationText();
        Instance.StartCoroutine(Instance.FadeInNotification());
    }

    public static void ClearAllNotifications()
    {
        _notificationTimestamps.Clear();
        if (Instance != null && Instance._notificationText != null)
        {
            Instance.UpdateNotificationText();
        }
    }

    public static void RefreshFonts()
    {
        if (Instance == null)
            return;

        Font font = UseMinecraftFont
            ? FontManager.GetFont("Minecraft")
            : FontManager.GetFont("Arial");

        if (font == null)
            return;

        if (Instance._notificationText != null)
            Instance._notificationText.font = font;

        if (Instance._arrayListText != null)
        {
            Instance._arrayListText.font = font;
            Instance._arrayListCacheVersion = -1;
        }
    }
}