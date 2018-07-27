using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class KeyObject : MonoBehaviour {

    public enum ObjectState {OnBadGoal, OnGoodGoal, NotOnGoal};

    public bool drawGizmos = false;
    private Collider col;
    public int id = -1;
    public string description = "";
    float radius = 0.1f;
    public bool willSucceed = false;
    private bool isHeld;
    Goal currentGoal;
    MeshRenderer mr;

    // External references
    GameManager gm;
    AudioManager am;
    public ScoreManager sm;

    public Rigidbody rig;

    Vector3 startingPos;
    Quaternion startingRot;

    // Registering callbacks if needed
    public UnityEvent onSucceeded;

    private string floorTag = "Floor";

    // Raycast variables (Used by ProjectionScript)
    public static float rayCastDistance = 2.5f;
    ProjectionScript projectionScript;

    // Animations variables
    public float travelTime = 10.0f;
    private float travelTimeCounter = 0.0f;
    private bool travelling = false;
    private float dt;
    private Vector3 animationStartVector;
    private Vector3 animationStopVector;

    public void SetHeld() {
        isHeld = true;
        projectionScript.Active = true;
    }
    public void UnsetHeld() {
        isHeld = false;
        projectionScript.Active = false;
    }

    void Awake() {
        projectionScript = GetComponentInChildren<ProjectionScript>();
        am = GameObject.FindObjectOfType<AudioManager>();
        col = this.GetComponentInChildren<MeshCollider>();
        rig = this.transform.GetComponent<Rigidbody>();
        projectionScript.enabled = true;
        mr = this.GetComponent<MeshRenderer>();
    }

    void Start () {
        if (id == -1) {
            throw (new UnityException("keyobject id not set"));
        }
        startingPos = this.transform.position;
        startingRot = this.transform.rotation;
        dt = 1.0f / travelTime;
        gm = FindObjectOfType<GameManager>();

    }

    void FixedUpdate() {
        // If an object is travelling, animate it
        if (travelling) {
            this.transform.position = Vector3.Lerp(animationStartVector, animationStopVector, travelTimeCounter);
            this.mr.material.color = new Color(1.0f, 1.0f, 1.0f, 1.0f - travelTimeCounter);
            travelTimeCounter += 0.01f;
            if (travelTimeCounter >= 1.0f) {
                travelling = false;
                travelTimeCounter = 0.0f;
                Destroy(this.gameObject);
            }
        }
    }

    //Called by VR Rig
    public ObjectState TryGoal() {
        RaycastHit hit;

        // Bit mask for layer filtering 
        LayerMask lm = 1 << 12;

        //Collision detection with colliders
        Goal g = GetGoal();
        if (g != null) {
            if (g.id == this.id) {
                g.willHighlightAfterObjectReached = true;
                currentGoal = g;
                if (currentGoal.descriptionFR != null) {
                    am.PlayObjectDescription(currentGoal.descriptionFR);
                }
                onSucceeded.Invoke();
                am.PlayCorrectSound();
                sm.EvaluateStreak();
                if (currentGoal.GetComponent<SpriteRenderer>() != null) {
                    currentGoal.Completed = true;
                    currentGoal.Animating = true; 
                }
            } else {
                am.PlayDefeatSound();
                sm.AddMistake();
            } 
        }
        //Detection by raycast
        else if (Physics.Raycast(this.transform.position, Vector3.left, out hit, lm)) {
            if (hit.distance < rayCastDistance) {
                Goal rayg = hit.transform.GetComponent<Goal>();
                if (rayg != null) {
                    if (rayg.id == this.id && gm.showCasingObject == false) {
                        currentGoal = rayg;
                        Succeed();
                       // Destroy(this.gameObject); //demo purpose
                        return ObjectState.OnGoodGoal;
                    } else {
                        Fail();
                        return ObjectState.OnBadGoal;
                    }
                }
            }
        }
        return ObjectState.NotOnGoal;
    }

    private void Fail() {
        am.PlayDefeatSound();
        sm.AddMistake();
        rig.velocity = Vector3.zero;
        rig.angularVelocity = Vector3.zero;
        rig.position = startingPos;
        transform.position = startingPos;
        rig.isKinematic = false;
    }

    // If an item is dropped on the good goal, play its decription and highlight the ad-hoc goal
    private void Succeed() {
            if (gm.CurrentLang == GameManager.Language.FR) {
                am.PlayObjectDescription(currentGoal.descriptionFR);
            } else {
                am.PlayObjectDescription(currentGoal.descriptionDE);
            }

        rig.isKinematic = true;
        currentGoal.willHighlightAfterObjectReached = true;
        animationStopVector = currentGoal.GetComponent<Collider>().ClosestPoint(this.transform.position);
        animationStartVector = this.transform.position;
        travelling = true;


        onSucceeded.Invoke();
        am.PlayCorrectSound();
        sm.EvaluateStreak();
        if (currentGoal.GetComponent<SpriteRenderer>() != null) {
            currentGoal.Completed = true;
            currentGoal.Animating = true;
            projectionScript.HidePointer();
        }
    }

    private void ResetPosition() {
        rig.angularVelocity = Vector3.zero;
        rig.velocity = Vector3.zero;
        rig.isKinematic = true;
        rig.rotation = startingRot;
        this.transform.position = new Vector3(startingPos.x, startingPos.y + 0.10f, startingPos.z);
        rig.angularVelocity = Vector3.zero;
        rig.velocity = Vector3.zero;
        rig.isKinematic = true;
        rig.rotation = startingRot;
        rig.isKinematic = false;
    }

    private Goal GetGoal() {
        Collider[] cols = Physics.OverlapSphere(this.transform.position, radius);
        if (cols != null) {
            foreach(Collider c in cols) {
                Goal g = c.GetComponent<Goal>();
                if (g != null) {
                    return g;
                }
            }
            return null;
        }
        return null;
    }

    private bool CollisionDetection() {
        Collider[] cols = Physics.OverlapSphere(this.transform.position, radius);
        if (cols != null) {
            foreach (Collider c in cols){
                Goal g = c.GetComponent<Goal>();
                if (g != null) {
                    //Collides with a goal
                    if (g.id == this.id) {

                        //Collided with the good goal
                        sm.EvaluateStreak();
                        currentGoal = g;
                        return true;
                    } else {
                        //False goal
                        sm.AddMistake();
                        return false;
                    }
                } else {
                    return false;
                }
            }
        } else {
            return false;
        }
        return false;
    }
    void OnDrawGizmos() {
        if (drawGizmos) {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position, radius);
        }
    }

    void OnCollisionEnter(Collision c) {
        if (c.gameObject.tag == floorTag) {
            ResetPosition();
        }
    }
}