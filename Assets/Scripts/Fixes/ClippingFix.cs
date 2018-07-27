using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClippingFix : MonoBehaviour {
	void Start () {
        this.GetComponent<Camera>().transparencySortMode = TransparencySortMode.Orthographic;
    }
}
