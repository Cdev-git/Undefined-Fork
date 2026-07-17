using Photon.Pun;

namespace Undefined.Utilities;

public class Guardian
{
    public static bool Islocalplayerguardian()
    {
        return GorillaGuardianZoneManager.zoneManagers[0].IsPlayerGuardian(PhotonNetwork.LocalPlayer);
    }
}