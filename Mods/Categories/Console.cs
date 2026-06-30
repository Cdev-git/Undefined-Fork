using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Text;

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
}
