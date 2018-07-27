using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable_Handle : MonoBehaviour {
    public Transform target_door;

    [Range(0.0f, 90.0f)]
    public float minimumAngle;

    [Range(-90.0f, 90.0f)]
    public float minAngleDoor;

    [Range(-90.0f, 90.0f)]
    public float maxAngleDoor;

    private bool isBeingInteractedWith = false;
    private HingeJoint hinge;
	void Start () {
        hinge = this.GetComponent<HingeJoint>();
	}
	void Update () {
        hinge.useSpring = !isBeingInteractedWith;
        if (hinge.angle > Mathf.Abs(minimumAngle)) {
            Debug.Log("can open door now");
        }
	}
}
