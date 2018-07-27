using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The Goal class symbolises an area in the painting where an object is missing.
public class Goal : MonoBehaviour {
    //game vars
    private bool completed;
    public bool Completed {
        get {
            return completed;
        }
        set {
            completed = value;
        }
    }
    public int id = -1;

    // Game objects
    public AudioClip descriptionFR;
    public AudioClip descriptionDE;
    private SpriteRenderer sr;

    private GameManager gm;

    // highlight vars
    public bool willHighlightAfterObjectReached = false;
    public bool highlighted = false;
    private float audioTimer = 0.0f;
    private float audioDuration = 0.0f;

    // Appear fx vars
    private float timer = 0.0f;
    private float timerColor = 0.0f;
    private float timeIncrement = 0.01f;
    public float animationTime = 3.0f;
    public bool animating;
    

    public bool Animating {
        get {
            return animating;
        }
        set {
            animating = value;
        }
    }

    // Init
    void Start() {
        Completed = false;
        Animating = false;
        sr = GetComponent<SpriteRenderer>();
        SetAlpha(0.0f);
        gm = FindObjectOfType<GameManager>();
    }

    void SetAlpha(float a) {
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, a);
    }
    
    void FixedUpdate() {
        // We use the Sin function to obtain an oscillating color for the highlight effect
        float colorValue = Mathf.Sin(timerColor);
        if (highlighted) {
            // Sets the game manager showCasingObject boolean to true, to alter the object behaviour
            gm.ShowCasingObject = true;
            float audioLenght = gm.CurrentLang == GameManager.Language.FR ? descriptionFR.length : descriptionDE.length;
            if (audioTimer <= audioLenght) {
                //highlighting
                sr.color = new Color(Mathf.Abs(colorValue), 1.0f, 1.0f);
                timerColor += 0.1f;
                if (timerColor >= 360.0f) {
                    timerColor = 0.0f;
                }
            } else {
                highlighted = false;
                gm.ShowCasingObject = false;
                gm.SetObjectKinematic(false);
                float currentAlpha = sr.color.a;
                sr.color = new Color(1.0f, 1.0f, 1.0f, currentAlpha);
            }
            audioTimer += Time.deltaTime;
        } 
        // We want the object to be kinematic if animated, to avoid Unity physics interfering with the item
        if (animating) {
            gm.SetObjectKinematic(true);
            gm.ShowCasingObject = true;
            timer += timeIncrement;
            float delta = timer / animationTime;
            SetAlpha(delta);
            if (timer >= animationTime) {
                animating = false;
                highlighted = true;
            }
        }
    }
}
