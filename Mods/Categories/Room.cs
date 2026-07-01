using BepInEx;
using GorillaNetworking;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Undefined.Utilities;
using UnityEngine;

namespace Undefined.Mods.Categories;

public class Room
{
    public static void Disconnect()
    {
        PhotonNetwork.Disconnect();
    }

    public static void JoinRandomPublic()
    {
        GorillaNetworkJoinTrigger trigger = PhotonNetworkController.Instance.currentJoinTrigger ?? GorillaComputer.instance.GetJoinTriggerForZone("forest");
        PhotonNetworkController.Instance.AttemptToJoinPublicRoom(trigger, GorillaNetworking.JoinType.Solo);
    }
    public static void PrimaryDisconnect()
    {
        if (InputHandler.Instance.RightPrimary.WasPressed | UnityInput.Current.GetKey(KeyCode.F))
        {
            PhotonNetwork.Disconnect();
        }
    }
    public static void Servers(string svr)
    {
        PhotonNetwork.ConnectToRegion(svr);
    }

    public static void JoinRoom(string RoomCode)
    {
        PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(RoomCode, GorillaNetworking.JoinType.Solo);
    }

    public static void EnableAntiAFK()
    {
        PhotonNetworkController.Instance.disableAFKKick = true;
    }

    public static void DisableAntiAFK()
    {
        PhotonNetworkController.Instance.disableAFKKick = false;
    }

    public static void DisableNetworkTriggers()
    {
        GameObject.Find("Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab").SetActive(false);
    }
    public static void EnableNetworkTriggers()
    {
        GameObject.Find("Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab").SetActive(true);
    }
}
