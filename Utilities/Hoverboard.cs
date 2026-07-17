namespace Undefined.Utilities;
using UnityEngine;

public struct Hoverboard
{
    public static bool Hand = VRRig.LocalRig.hoverboardVisual.IsLeftHanded;
    public static Vector3 HandPosition = VRRig.LocalRig.hoverboardVisual.NominalLocalPosition;
    public static Quaternion HandRotation = VRRig.LocalRig.hoverboardVisual.NominalLocalRotation;
    public static bool IsHeld = VRRig.LocalRig.hoverboardVisual.IsHeld;
}