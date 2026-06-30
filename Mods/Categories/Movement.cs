using Undefined.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Undefined.Mods.Categories;

public class Movement
{
    public static void TeleportGun()
    {
        GunLib.start2guns(delegate ()
        {
            Vector3 targetPos = GunLib.GetPointerPos();

            Noclipistuff(true);

            GorillaLocomotion.GTPlayer.Instance.transform.position = targetPos;
            GorillaTagger.Instance.transform.position = targetPos;

            Noclipistuff(false);
        }, false);

        Noclipistuff(false);
    }

    public static void Noclipistuff(bool b)
    {
        foreach (MeshCollider collider in Resources.FindObjectsOfTypeAll<MeshCollider>())
        {
            if (b)
            {
                collider.enabled = false;
            }
            else
            {
                collider.enabled = true;
            }
        }
    }
}