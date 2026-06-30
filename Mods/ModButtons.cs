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
            new ButtonInfo { buttonText = "Settings", method =() => activeCategory = 1, isTogglable = false, toolTip = "Opens the main settings page for the menu."},

            new ButtonInfo { buttonText = "Room Mods", method =() => activeCategory = 4, isTogglable = false, toolTip = "Opens the room mods tab."},
            new ButtonInfo { buttonText = "Movement Mods", method =() => activeCategory = 5, isTogglable = false, toolTip = "Opens the movement mods tab."},
            new ButtonInfo { buttonText = "Safety Mods", method =() => activeCategory = 6, isTogglable = false, toolTip = "Opens the safety mods tab."},
        },

        new ButtonInfo[] { // Settings [1]
            new ButtonInfo { buttonText = "Return to Main", method =() => activeCategory = 0, isTogglable = false, toolTip = "Returns to the main page of the menu."},
            new ButtonInfo { buttonText = "Menu", method =() => activeCategory = 2, isTogglable = false, toolTip = "Opens the settings for the menu."},
            new ButtonInfo { buttonText = "Movement", method =() => activeCategory = 3, isTogglable = false, toolTip = "Opens the movement settings for the menu."},
        },

        new ButtonInfo[] { // Menu Settings [2]
            new ButtonInfo { buttonText = "Return to Settings", method =() => activeCategory = 1, isTogglable = false, toolTip = "Returns to the main settings page for the menu."},
            new ButtonInfo { buttonText = "Right Hand", enableMethod =() => rightHanded = true, disableMethod =() => rightHanded = false, toolTip = "Puts the menu on your right hand."},
            new ButtonInfo { buttonText = "Notifications", enableMethod =() => disableNotifications = false, disableMethod =() => disableNotifications = true, enabled = !disableNotifications, toolTip = "Toggles the notifications."},
            new ButtonInfo { buttonText = "FPS Counter", enableMethod =() => fpsCounter = true, disableMethod =() => fpsCounter = false, enabled = fpsCounter, toolTip = "Toggles the FPS counter."},
            new ButtonInfo { buttonText = "Disconnect Button", enableMethod =() => disconnectButton = true, disableMethod =() => disconnectButton = false, enabled = disconnectButton, toolTip = "Toggles the disconnect button."},
        },

        new ButtonInfo[] { // Movement Settings [3]
            new ButtonInfo { buttonText = "Return to Settings", method =() => activeCategory = 1, isTogglable = false, toolTip = "Returns to the main settings page for the menu."},

        },

        new ButtonInfo[] { // Room Mods [4]
            new ButtonInfo { buttonText = "Return to Main", method =() => activeCategory = 0, isTogglable = false, toolTip = "Returns to the main page of the menu."},

            new ButtonInfo { buttonText = "Disconnect", method =() => NetworkSystem.Instance.ReturnToSinglePlayer(), isTogglable = false, toolTip = "Disconnects you from the room."},
        },

        new ButtonInfo[] { // Movement Mods [5]
            new ButtonInfo { buttonText = "Return to Main", method =() => activeCategory = 0, isTogglable = false, toolTip = "Returns to the main page of the menu."},


            new ButtonInfo { buttonText = "Teleport gun", method =() => Movement.TeleportGun(), isTogglable = true, toolTip = "You can teleport by pressing trigger on ur controller"},
        },

        new ButtonInfo[] { // Safety Mods [6]
            new ButtonInfo { buttonText = "Return to Main", method =() => activeCategory = 0, isTogglable = false, toolTip = "Returns to the main page of the menu."},

        },

    };

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
