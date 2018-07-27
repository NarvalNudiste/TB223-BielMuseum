using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Color_Changer : MonoBehaviour {
    public Color albedo;
    void Start() {
        MeshRenderer[] meshes = GetComponentsInChildren<MeshRenderer>();
        foreach(MeshRenderer r in meshes) {
            r.material.color = albedo;
        }
    }
}
