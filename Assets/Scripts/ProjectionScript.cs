using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class ProjectionScript : MonoBehaviour {
    public Hand currentHand;
    public bool active = false;
    LineRenderer lr;
    public Transform targetSprite;
    private GameObject camera;
    private Transform instanciatedTarget;
    float width = 0.01f;
    float maxDistance = KeyObject.rayCastDistance;
    float offset = 0.0f;

    private GameManager gm;

    private float pointerLenghtFactor = 3.0f;

    public bool Active {
        get {
            return active;
        }

        set {
            active = value;
        }
    }

    // init
    void Start() {
        currentHand = null;
        camera = GameObject.FindGameObjectWithTag("MainCamera");
        lr = GetComponent<LineRenderer>();
        instanciatedTarget = GameObject.Instantiate(targetSprite, this.transform.position, Quaternion.identity);
        instanciatedTarget.transform.rotation = Quaternion.Euler(0.0f, -90.0f, 0.0f);
        instanciatedTarget.GetComponent<SpriteRenderer>().enabled = false;
        gm = FindObjectOfType<GameManager>();
    }

    // Determines if the parent object is close enough of the painting and / or on a potential goal. If so, 
    // makes the line renderer appear (by increasing its size) and shows the sprite. 
    // If on a potential goal, activates its particles fx
    void Update() {
        /* old
        if (Active && !gm.showCasingObject) {
            RaycastHit hit;
            if (Physics.Raycast(this.transform.position, Vector3.left, out hit)) {
                if (hit.transform.gameObject != null && hit.distance < maxDistance) {
                    if (hit.transform.gameObject.tag == "Tableau" || hit.transform.gameObject.tag == "Objective") {
                        if (hit.transform.gameObject.tag == "Objective") {
                            ParticleSystem currentParticleSystem = hit.transform.GetComponentInChildren<ParticleSystem>();
                            currentParticleSystem.Emit(10);
                            
                        }
                        Vector3 hitPosition = new Vector3(hit.transform.position.x, this.transform.position.y, this.transform.position.z);
                        Vector3[] positions = { this.transform.position, hitPosition };
                        ShowPointer(positions, hitPosition);
                    }
                } else {
                    HidePointer();
                }
            } else {
                HidePointer();
            }
        }
        else {
            HidePointer();
        }
        */
        if (Active && !gm.showCasingObject && currentHand != null) {
            RaycastHit hit;
            if (Physics.Raycast(this.transform.position, currentHand.transform.forward, out hit)) {
                if (hit.transform.gameObject != null && hit.distance < maxDistance) {
                    if (hit.transform.gameObject.tag == "Tableau" || hit.transform.gameObject.tag == "Objective") {
                        if (hit.transform.gameObject.tag == "Objective") {
                            ParticleSystem currentParticleSystem = hit.transform.GetComponentInChildren<ParticleSystem>();
                            currentParticleSystem.Emit(10);

                        }
                        Vector3 hitPosition = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                        //   Vector3[] positions = { this.transform.position, hitPosition };
                        Vector3[] positions = { this.transform.position, hitPosition };
                        ShowPointer(positions, hitPosition);
                    }
                } else {
                    HidePointer();
                }
            } else {
                HidePointer();
            }
        } else {
            HidePointer();
        }
    }
    // Shows the pointer
    public void ShowPointer(Vector3[] positions, Vector3 hitPosition) {
        lr.SetPositions(positions);
        lr.startWidth = width;
        lr.endWidth = width;
        instanciatedTarget.transform.position = new Vector3(hitPosition.x + offset, hitPosition.y, hitPosition.z);
        instanciatedTarget.GetComponent<SpriteRenderer>().enabled = true;
    }

    // Hides the pointer
    public void HidePointer() {
        lr.startWidth = 0.0f;
        lr.endWidth = 0.0f;
        instanciatedTarget.GetComponent<SpriteRenderer>().enabled = false;
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
    }
}
