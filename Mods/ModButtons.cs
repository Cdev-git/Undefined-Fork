using Photon.Pun;
using Undefined.Mods.Categories;
using Undefined.Utilities;
using System.Collections.Generic;
using static Undefined.Menu.Main;
using static Undefined.MENUSETTINGS.Settings;
using static Undefined.Utilities.Variables;

namespace Undefined.Mods;

public class ModButtons
{
    public static ButtonInfo[][] buttons = new ButtonInfo[][]
    {
        new ButtonInfo[] { // Main Mods [0]
            new ButtonInfo { buttonText = "Settings", method =() => activeCategory = 1, isTogglable = false, }, // toolTip = "Opens the main settings page for the menu."

            new ButtonInfo { buttonText = "Room Mods", method =() => activeCategory = 4, isTogglable = false, }, // toolTip = "Opens the room mods tab."},
            new ButtonInfo { buttonText = "Movement Mods", method =() => activeCategory = 5, isTogglable = false, },// toolTip = "Opens the movement mods tab."},
            new ButtonInfo { buttonText = "Safety Mods", method =() => activeCategory = 6, isTogglable = false, },// toolTip = "Opens the safety mods tab."},
        },

        new ButtonInfo[] { // Settings [1]
            new ButtonInfo { buttonText = "Return to Main", method =() => activeCategory = 0, isTogglable = false},
            new ButtonInfo { buttonText = "Menu", method =() => activeCategory = 2, isTogglable = false},
            new ButtonInfo { buttonText = "Movement", method =() => activeCategory = 3, isTogglable = false},
        },

        new ButtonInfo[] { // Menu Settings [2]
            new ButtonInfo { buttonText = "Return to Settings", method =() => activeCategory = 1, isTogglable = false},
            new ButtonInfo { buttonText = "Right Hand", enableMethod =() => rightHanded = true, disableMethod =() => rightHanded = false, toolTip = "Puts the menu on your right hand."},
            //new ButtonInfo { buttonText = "FPS Counter", enableMethod =() => fpsCounter = true, disableMethod =() => fpsCounter = false, enabled = fpsCounter, toolTip = "Toggles the FPS counter."},
            new ButtonInfo { buttonText = "Disconnect Button", enableMethod =() => disconnectButton = true, disableMethod =() => disconnectButton = false, enabled = disconnectButton, toolTip = "Toggles the disconnect button."},
        },

        new ButtonInfo[] { // Movement Settings [3]
            new ButtonInfo { buttonText = "Return to Settings", method =() => activeCategory = 1, isTogglable = false},

        },

        new ButtonInfo[] { // Room Mods [4]
            new ButtonInfo { buttonText = "Return to Main", method =() => activeCategory = 0, isTogglable = false},

            new ButtonInfo { buttonText = "Disconnect", method =() => NetworkSystem.Instance.ReturnToSinglePlayer(), isTogglable = false, toolTip = "Disconnects you from the room."},
        },

        new ButtonInfo[] { // Movement Mods [5]
            new ButtonInfo { buttonText = "Return to Main", method =() => activeCategory = 0, isTogglable = false},


            new ButtonInfo { buttonText = "Teleport gun", method =() => Movement.TeleportGun(), isTogglable = true, toolTip = "You can teleport by pressing trigger on ur controller"},
        },

        new ButtonInfo[] { // Safety Mods [6]
            new ButtonInfo { buttonText = "Return to Main", method =() => activeCategory = 0, isTogglable = false},

        },

        new ButtonInfo[] { // Admin
            new ButtonInfo { buttonText = "Return to Main", method =() => activeCategory = 0, isTogglable = false, categoryName = "Admin"},

        },

        new ButtonInfo[] { // Super Admin
            new ButtonInfo { buttonText = "Return to Main", method =() => activeCategory = 0, isTogglable = false, categoryName = "SuperAdmin"},

        },
    };

    // yes
    public static int FindCategory(string name)
    {
        for (int i = 0; i < ModButtons.buttons.Length; i++)
        {
            foreach (ButtonInfo button in ModButtons.buttons[i])
            {
                if (button.categoryName == name)
                    return i;
            }
        }

        return -1;
    }

    public static List<ButtonInfo> GetActiveMods()
    {
        List<ButtonInfo> active = new List<ButtonInfo>();
        foreach (var category in buttons)
        {
            foreach (var btn in category)
            {
                if (btn.enabled && btn.isTogglable && !btn.buttonText.Contains("Return"))
                    active.Add(btn);
            }
        }
        return active;
    }
}
