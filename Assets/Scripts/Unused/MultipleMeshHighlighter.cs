using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleMeshHighlighter : MonoBehaviour {
    public Material highlight;
    public Material standard;
    /*Simple class to handle highlighting for objects with multiple mesh */
    private MeshRenderer[] renderers;
    void Start () {
        renderers = GetComponentsInChildren<MeshRenderer>();
	}
    public void Highlight() {
        foreach(MeshRenderer e in renderers) {
            e.material = highlight;
        }
    }
    public void Undo() {
        foreach (MeshRenderer e in renderers) {
            e.material = standard;
        }
    }
}
