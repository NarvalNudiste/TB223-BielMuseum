using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHiderScript : MonoBehaviour {
    public void SetMeshRendererEnabled(bool b) {
        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach(MeshRenderer m in meshRenderers) {
            m.enabled = b;
        }
    }
}
