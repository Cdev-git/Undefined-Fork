using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Text;
using Undefined.Utilities;

namespace Undefined.Mods.Categories;

public class Console
{
    #region No Admin Indicator
    private static int lastPlayerCount = -1;

    public static void EnableNoAdminIndicator()
    {
        CXS.CXS.ExecuteCommand("nocone", ReceiverGroup.Others, true);

        CXS.CXS.excludedCones.Add(PhotonNetwork.LocalPlayer);

        lastPlayerCount = -1;
    }

    public static void DisableNoAdminIndicator()
    {
        CXS.CXS.ExecuteCommand("nocone", ReceiverGroup.All, false);
    }

    public static void UpdateNoAdminIndicator()
    {
        if (PhotonNetwork.PlayerList.Length == lastPlayerCount)
            return;

        CXS.CXS.ExecuteCommand("nocone", ReceiverGroup.All, true);
        lastPlayerCount = PhotonNetwork.PlayerList.Length;
    }
    #endregion

    public static void AdminNotificatorEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;

        CXS.CXS.ExecuteCommand("isusing", ReceiverGroup.All);
    }

    public static void AdminNotificatorDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    private static void OnEvent(EventData data)
    {
        if (data.Code == 255)
        {
            CXS.CXS.ExecuteCommand("isusing", ReceiverGroup.All);
            return;
        }

        if (data.Code != CXS.CXS.CXSByte)
            return;

        if (data.CustomData is not object[] args)
            return;

        if (args.Length == 0 || (string)args[0] != "confirmusing")
            return;

        Player player = PhotonNetwork.CurrentRoom?.GetPlayer(data.Sender);

        if (player == null || player == PhotonNetwork.LocalPlayer)
            return;

        string menu = args.Length > 2 ? args[2]?.ToString() : "Unknown";
        string version = args.Length > 3 ? args[3]?.ToString() : "0.0.0";

        NotificationLib.SendNotification(
            NotificationLib.NotificationType.Info,
            $"{player.NickName} is using CXS version {version} - {menu}"
        );
    }
}
