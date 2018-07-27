using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Score {
    private int id;
    private float time, score;
    private string name;
    private int bestStreak, mistakesCount;
    private bool succeededLastTime;

    public Score(   float t, string n, float s) {
        Time = t;
        Name = n;
        bestStreak = 0;
        mistakesCount = 0;
        score = s;
    }

    public float Time {
        get {
            return time;
        }

        set {
            time = value;
        }
    }
    public string Name {
        get {
            return name;
        }

        set {
            name = value;
        }
    }
}
