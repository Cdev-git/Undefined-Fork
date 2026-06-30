using Undefined.Utilities;
using UnityEngine;

namespace Undefined.MENUSETTINGS;

public class Settings
{
    public static ExtGradient backgroundColor = new ExtGradient
    {
        colors = ExtGradient.GetSimpleGradient(
            new Color32(60, 20, 120, 255),
            new Color32(140, 40, 255, 255)
        )
    };

    public static ExtGradient[] buttonColors =
    {
        new ExtGradient
        {
            colors = ExtGradient.GetSolidGradient(
                new Color32(35, 15, 60, 255)
            )
        },

        new ExtGradient
        {
            colors = ExtGradient.GetSimpleGradient(
                new Color32(155, 60, 255, 255),
                new Color32(210, 120, 255, 255)
            )
        }
    };

    public static Color[] textColors =
    {
        Color.white,
        Color.white
    };

    public static Font currentFont =
        Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
}