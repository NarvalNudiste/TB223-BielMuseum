using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Valve.VR.InteractionSystem;

public class GameManager : MonoBehaviour {
    // QoL enums for the game state
    public enum GameState { LanguageSelect,  Playing, DisplayingScore, Finished};
    public enum Language { FR, DE};
    public enum Objects { Axe, CeramicSmall, CeramicBig, Knife }
    public Language currentLang;
    public GameState state;

    public bool showCasingObject;
   
    // Vars for HMD inactivity detection
    public Transform hmdTransform;
    private Vector3 lastPosition;

    private bool checkForGoals;

    private Goal[] goals;
    private KeyObject[] keyObjects;
    private string sceneName = "Scene_Integration_Tableau";
    ScoreManager sm;
    Text text;
    float timer = 0.0f;
    Score currentScore;

    private bool scoreAdded = false;

    ObjectHiderScript objectHiderScript;


    private float timeOutMinTreshold = 0.02f;
    private float timeOutTimer = 0.0f;
    private float timeOut = 30.0f;

    public float defaultLaserPointerThickness = 0.01f;

    private SpriteRenderer[] waitingSprites;

    public Language CurrentLang {
        get {
            return currentLang;
        }
    }
    public GameState State {
        get {
            return state;
        }
        set {
            state = value;
        }
    }

    public bool ShowCasingObject {
        get {
            return showCasingObject;
        }

        set {
            showCasingObject = value;
        }
    }

    void Awake() {
        waitingSprites = GameObject.Find("WaitingSprites").GetComponentsInChildren<SpriteRenderer>();
        this.currentLang = Language.DE;
        hmdTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
        if (hmdTransform == null) {
            Debug.Log("Error, the hmd reference is missing");
        }
        state = GameState.LanguageSelect;
        SetLaserThickness(defaultLaserPointerThickness);
        InitGame();
    }
    public void SetLanguage(Language l) {
        this.currentLang = l;
        this.state = GameState.Playing;
        SetLaserThickness(0.0f);
    }

    public void SetDE() {
        this.currentLang = Language.DE;
        this.state = GameState.Playing;
        SetLaserThickness(0.0f);
    }

    public void SetFR() {
        this.currentLang = Language.FR;
        this.state = GameState.Playing;
    }

    void InitGame() {
        timeOutTimer = 0.0f;
        goals = GameObject.FindObjectsOfType<Goal>();
        sm = this.gameObject.AddComponent<ScoreManager>();
        sm.gm = this.GetComponent<GameManager>();
        keyObjects = GameObject.FindObjectsOfType<KeyObject>();

        // Attaching a scoreManager reference to all keyObjects for callbacks
        foreach (KeyObject k in keyObjects) {
            k.sm = sm;
        }
        text = GameObject.Find("LeaderBoard_Text").GetComponent<Text>();
    }

    // Read the current List<Score> stored in the ScoreManager and updates the leaderboard
    void RefreshScores() {
        int i = 1;
        string scoreString = currentLang == Language.FR ? "Meilleurs scores \n" : "Hohe Punktzahlen \n";
        if (sm != null) {
            if (sm.getScores() != null) {
                foreach (Score s in sm.getScores()) {
                    scoreString += "#" + i.ToString() + " - " + s.Name + " - " + s.Time.ToString() + "\n";
                    i++;
                }
            }
        }
        text.text = scoreString;
    }
    void FixedUpdate() {
        CheckForInactivity();
        // If an item is shown, we don't want the timer to continue increasing. This is done in fixed update for accuracy purposes
        if (!showCasingObject) {
            timer += Time.deltaTime;

        }
    }

    //Check if the position has changed for a while (from a fixed treshold). If it didn't, load the scene again
    private void CheckForInactivity() {
        if (hmdTransform != null) {
            if (timeOutTimer == 0.0f) {
                lastPosition = hmdTransform.position;
                timeOutTimer += Time.deltaTime;
            } else {
                timeOutTimer += Time.deltaTime;
            }
            if (timeOutTimer >= timeOut) {
                if (Mathf.Abs(hmdTransform.position.x - lastPosition.x) < timeOutMinTreshold && Mathf.Abs(hmdTransform.position.y - lastPosition.y) < timeOutMinTreshold && Mathf.Abs(hmdTransform.position.z - lastPosition.z) < timeOutMinTreshold) {
                    Reset();
                }
                timeOutTimer = 0.0f;
            }
        } else {
            hmdTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
        }
    }


    private void SetLaserThickness(float t) {
        GameObject[] lasers = GameObject.FindGameObjectsWithTag("LaserPointer");
        foreach (GameObject g in lasers) {
            g.GetComponent<SteamVR_LaserPointer>().thickness = t;
        }
    }


    void Update() {
        TestState();
        // If the game is in the play state, we want to continually check all the goals. If the functions returns true, the game ends
        if (state == GameState.Playing) {
            if (CheckGoals()) {
                state = GameState.DisplayingScore;
            }
        }
        // If the state is selection of language, we just enable the laser pointers
        else if (state == GameState.LanguageSelect) {
            GameObject[] lasers = GameObject.FindGameObjectsWithTag("LaserPointer");
            foreach(GameObject g in lasers) {
                g.SetActive(true);
            }
        }

        else if (state == GameState.DisplayingScore) {
            // A boolean is necessary to avoid the score being continually added to the score list
            if (!scoreAdded) {
                scoreAdded = true;
                sm.AddCurrentScore();
            }
            // Refresh the UI text
            RefreshScores();
            timeOutTimer += 0.1f;
            if (timeOutTimer >= timeOut) {
                timeOutTimer = 0.0f;
                Reset();
            }
        }
        else {
            Debug.Log("Error - Invalid state");
            Rigidbody[] rigidbodies = FindObjectsOfType<Rigidbody>();
            foreach(Rigidbody r in rigidbodies) {
                // Something was wrong so gotta make things explode
                r.AddForce(new Vector3(Random.Range(0.0f, 5000.0f), Random.Range(0.0f, 5000.0f), Random.Range(0.0f, 5000.0f)));
            }
        }

        if (showCasingObject) {
            foreach(SpriteRenderer sr in waitingSprites){
                sr.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }
        } else {
            foreach (SpriteRenderer sr in waitingSprites) {
                sr.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            }
        }
    }
    // Returns true if all the goals are completed, false instead
    private bool CheckGoals() {
        foreach (Goal g in goals) {
            if (!g.Completed) {
                return false;
            }
        }
        if (showCasingObject) {
            return false;
        } else {
            return true;
        }
    }

    // QoL reset
    private void TestState() {
        if (Input.GetKeyDown(KeyCode.R)) {
            Reset();
        }
    }

    private void Reset() {
        SceneManager.LoadScene(sceneName);
    }

    // Unused
    public void Reinstantiate(GameObject g, Vector3 pos, Quaternion rot) {
        Destroy(g);
        Instantiate(g, pos, rot);
    }

    public void SetObjectKinematic(bool kinematic) {
        foreach(KeyObject k in keyObjects) {
            if (k != null) {
                k.GetComponent<Rigidbody>().isKinematic = kinematic;
            }
        }
    }
}
