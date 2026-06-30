using System.Collections.Generic;
using UnityEngine;

namespace Undefined.Utilities;

public static class FontManager
{
    private static Dictionary<string, Font> _fonts = new();

    private static List<Font> _dynamicFonts = new();

    public static void CleanupFonts()
    {
        _fonts.Clear();

        foreach (Font font in _dynamicFonts)
        {
            if (font != null)
                Object.Destroy(font);
        }

        _dynamicFonts.Clear();
    }


    public static void LoadFonts()
    {
        CleanupFonts();

        Font arial = Resources.GetBuiltinResource<Font>("Arial.ttf");

        if (arial != null)
            RegisterFont("Arial", arial);

        Font comic = Font.CreateDynamicFontFromOSFont(
            "Comic Sans MS",
            22
        );

        if (comic != null)
        {
            _dynamicFonts.Add(comic);
            RegisterFont("Comic Sans", comic);
        }

        AssetBundle bundle = AssetBundle.LoadFromStream(
            typeof(FontManager).Assembly.GetManifestResourceStream(
                "Undefined.Resources.Embedded.minecraftfont"
            )
        );


        if (bundle != null)
        {
            Font minecraft = bundle.LoadAsset<Font>(
                "Minecraftia-Regular"
            );

            if (minecraft != null)
                RegisterFont("Minecraft", minecraft);

            bundle.Unload(false);
        }
    }



    public static void RegisterFont(string name, Font font)
    {
        if (font == null)
            return;

        _fonts[name] = font;
    }



    public static Font GetFont(string name)
    {
        if (_fonts.TryGetValue(name, out Font font))
            return font;

        return _fonts["Arial"];
    }
}