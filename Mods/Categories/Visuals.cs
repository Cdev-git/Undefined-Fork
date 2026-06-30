using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Undefined.Mods.Categories;

public class Visuals
{
    private class ESPData
    {
        public GameObject[] objs;
        public Renderer[] rends;
    }

    private static Dictionary<VRRig, ESPData> esp = new();
    private static Material mat;

    public static void BoxESP2DEnable()
    {
        mat = new Material(Shader.Find("GUI/Text Shader"));
    }

    public static void BoxESP2D()
    {
        if (mat == null) return;

        foreach (VRRig rig in VRRigCache.ActiveRigs)
        {
            if (rig == null || rig == GorillaTagger.Instance.offlineVRRig)
                continue;

            if (!esp.TryGetValue(rig, out var data))
            {
                data = new ESPData
                {
                    objs = new GameObject[4],
                    rends = new Renderer[4]
                };

                for (int i = 0; i < 4; i++)
                {
                    var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    Object.Destroy(cube.GetComponent<BoxCollider>());

                    var r = cube.GetComponent<Renderer>();
                    r.material = mat;

                    data.objs[i] = cube;
                    data.rends[i] = r;
                }

                esp[rig] = data;
            }

            Transform t = rig.transform;

            Quaternion rot = Quaternion.LookRotation(
                t.position - GorillaTagger.Instance.headCollider.transform.position
            );

            Vector3 up = rot * Vector3.up;
            Vector3 right = rot * Vector3.right;
            Vector3 pos = t.position;

            Vector3[] p =
            {
                pos + up * 0.35f,
                pos + up * -0.55f,
                pos + right * -0.33f + up * -0.10f,
                pos + right * 0.33f + up * -0.10f
            };

            Vector3[] s =
            {
                new Vector3(0.66f, 0.02f, 0.01f),
                new Vector3(0.66f, 0.02f, 0.01f),
                new Vector3(0.02f, 0.9f, 0.01f),
                new Vector3(0.02f, 0.9f, 0.01f)
            };

            Color c = rig.playerColor;

            var d = esp[rig];

            for (int i = 0; i < 4; i++)
            {
                var obj = d.objs[i];
                if (!obj) continue;

                obj.transform.position = p[i];
                obj.transform.rotation = rot;
                obj.transform.localScale = s[i];

                d.rends[i].material.color = c;
            }
        }
        List<VRRig> remove = new();

        foreach (var kvp in esp)
        {
            if (!VRRigCache.ActiveRigs.Contains(kvp.Key))
            {
                foreach (var o in kvp.Value.objs)
                    if (o) Object.Destroy(o);

                remove.Add(kvp.Key);
            }
        }

        foreach (var r in remove)
            esp.Remove(r);
    }

    public static void BoxESP2DDisable()
    {
        foreach (var kvp in esp)
        {
            foreach (var o in kvp.Value.objs)
                if (o) Object.Destroy(o);
        }

        esp.Clear();

        if (mat)
            Object.Destroy(mat);

        mat = null;
    }
}