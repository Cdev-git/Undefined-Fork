using BepInEx;
using GorillaLocomotion;
using System;
using System.Collections.Generic;
using System.Text;
using Undefined.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;
using ExitGames.Client.Photon;
using GorillaExtensions;
using GorillaLocomotion.Climbing;
using GorillaLocomotion.Swimming;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using UnityEngine.XR;
using Valve.Newtonsoft.Json.Linq;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Undefined.Mods.Categories;

public class Movement
{
    public static float startX = -1f;
    public static float startY = -1f;

    public static float subThingy;
    public static float subThingyZ;

    public static float FlySpeed = 10f; // this is very bad, but it works for now. I will fix this later

    public static void Fly()
    {
        if (InputHandler.Instance.RightPrimary.IsPressed)
        {
            GTPlayer.Instance.transform.position += GorillaTagger.Instance.headCollider.transform.forward * (Time.deltaTime * FlySpeed);
            GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;
        }
    }

    public static void TPSTUMP()
    {
        Noclipistuff(true);
        GTPlayer.Instance.TeleportTo(new Vector3(-68.647f, 12.406f, -83.699f), GTPlayer.Instance.transform.rotation, false, true);
        GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;
        Noclipistuff(false);
    }

    public static void NoClip()
    {
        bool DisableColliders = InputHandler.Instance.RightPrimary.IsPressed;
        MeshCollider[] colliders = Resources.FindObjectsOfTypeAll<MeshCollider>();

        foreach (MeshCollider collider in colliders)
        {
            collider.enabled = !DisableColliders;
        }

    }

    public static void Bouncy()
    {
        GorillaTagger.Instance.bodyCollider.material.bounciness = 1f;
        GorillaTagger.Instance.bodyCollider.material.bounceCombine = (PhysicsMaterialCombine)3;
        GorillaTagger.Instance.bodyCollider.material.dynamicFriction = 0f;
    }

    public static void ResetBouncy()
    {
        GorillaTagger.Instance.bodyCollider.material.bounciness = 0f;
        GorillaTagger.Instance.bodyCollider.material.bounceCombine = 0;
        GorillaTagger.Instance.bodyCollider.material.dynamicFriction = 0f;
    }

    public static void WASDFly()
    {
        var kb = Keyboard.current;

        bool W = kb.wKey.isPressed;
        bool A = kb.aKey.isPressed;
        bool S = kb.sKey.isPressed;
        bool D = kb.dKey.isPressed;
        bool Space = kb.spaceKey.isPressed;
        bool Ctrl = kb.leftCtrlKey.isPressed;
        bool Shift = kb.leftShiftKey.isPressed;
        bool Alt = kb.leftAltKey.isPressed;

        bool LeftArrow = kb.leftArrowKey.isPressed;
        bool RightArrow = kb.rightArrowKey.isPressed;
        bool UpArrow = kb.upArrowKey.isPressed;
        bool DownArrow = kb.downArrowKey.isPressed;

        Transform parentTransform = GTPlayer.Instance.GetControllerTransform(false).parent;

        float turnSpeed = 250f;

        if (LeftArrow)
            parentTransform.eulerAngles += new Vector3(0, -turnSpeed, 0) * Time.deltaTime;
        if (RightArrow)
            parentTransform.eulerAngles += new Vector3(0, turnSpeed, 0) * Time.deltaTime;
        if (UpArrow)
            parentTransform.eulerAngles += new Vector3(-turnSpeed, 0, 0) * Time.deltaTime;
        if (DownArrow)
            parentTransform.eulerAngles += new Vector3(turnSpeed, 0, 0) * Time.deltaTime;

        if (Mouse.current.rightButton.isPressed)
        {
            Quaternion currentRotation = parentTransform.rotation;
            Vector3 euler = currentRotation.eulerAngles;

            if (startX < 0)
            {
                startX = euler.y;
                subThingy = Mouse.current.position.value.x / Screen.width;
            }
            if (startY < 0)
            {
                startY = euler.x;
                subThingyZ = Mouse.current.position.value.y / Screen.height;
            }

            float newX = startY - (Mouse.current.position.value.y / Screen.height - subThingyZ) * 360 * 1.33f;
            float newY = startX + (Mouse.current.position.value.x / Screen.width - subThingy) * 360 * 1.33f;

            newX = newX > 180f ? newX - 360f : newX;
            newX = Mathf.Clamp(newX, -90f, 90f);

            parentTransform.rotation = Quaternion.Euler(newX, newY, euler.z);
        }
        else
        {
            startX = -1;
            startY = -1;
        }

        float speed = FlySpeed;
        if (Shift) speed *= 2f;
        else if (Alt) speed /= 2f;

        Transform cam = parentTransform;

        if (W)
            GorillaTagger.Instance.rigidbody.transform.position += cam.forward * (Time.deltaTime * speed);

        if (S)
            GorillaTagger.Instance.rigidbody.transform.position += cam.forward * (-Time.deltaTime * speed);

        if (A)
            GorillaTagger.Instance.rigidbody.transform.position += cam.right * (-Time.deltaTime * speed);

        if (D)
            GorillaTagger.Instance.rigidbody.transform.position += cam.right * (Time.deltaTime * speed);

        if (Space)
            GorillaTagger.Instance.rigidbody.transform.position += Vector3.up * (Time.deltaTime * speed);

        if (Ctrl)
            GorillaTagger.Instance.rigidbody.transform.position += Vector3.down * (Time.deltaTime * speed);

        VRRig.LocalRig.head.rigTarget.transform.rotation =
            GorillaTagger.Instance.headCollider.transform.rotation;
    }

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