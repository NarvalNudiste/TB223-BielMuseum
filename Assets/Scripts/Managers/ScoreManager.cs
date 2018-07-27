using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour{
    public GameManager gm; 
    public Text text;
    private List<Score> scores;
    public int scoreCount;

    private string[] animalNamesFr = { "buffle", "lion", "cheval" };
    private string[] animalNamesDE = { "büffel", "löwe", "pferd"};

    private string[] colorsFR = { "bleu", "rouge", "rose" };
    private string[] colorsDE = { "blau", "rot", "pink" };



    private List<string> colors_fr;

    private string filename = "Scores.bin";

    //score vars
    private int currentStreak;
    private float score;
    private int mistakeCount;
    private float timer;
    private bool previousSuccess;

    private float basePoints = 100.0f;
    private float baseMalus = 25.0f;

    private float baseTime = 60.0f;


    public float Timer {
        get {
            return timer;
        }
    }

    void InitValues() {
        currentStreak = 0;
        mistakeCount = 0;
        score = 0.0f;
        timer = 0.0f;
        previousSuccess = false;
    }
    void FixedUpdate() {
        if (gm.State == GameManager.GameState.Playing) {
            if (!gm.ShowCasingObject) {
                timer += Time.deltaTime;
            }
            string scoreString = "";
            if (gm.currentLang == GameManager.Language.FR) {
                scoreString = "Temps : " + timer.ToString() + "\n Réussites consécutives : " + currentStreak.ToString() + "\n Erreurs : " + mistakeCount.ToString() + "\n Score : " + score.ToString();
            } else {
                scoreString = "Zeit : " + timer.ToString() + "\n Konsekutiver Erfolg : " + currentStreak.ToString() + "\n Fehler : " + mistakeCount.ToString() + "\n Ergebnis : " + score.ToString();

            }

            text.text = scoreString;
        }
        else if (gm.State == GameManager.GameState.DisplayingScore) {

        }
        else if (gm.State == GameManager.GameState.LanguageSelect) {

        }
    }

    public void AddCurrentScore() {
        this.addScore(new Score(Timer, GetRandomName(), score));
    }

    void Start() {
        //Init
        InitValues();
        text = GameObject.Find("Score_Debug_Text").GetComponent<Text>();
        if (File.Exists("Scores.bin"))
            Read();
        if (scores == null) {
            scores = new List<Score>();
            Write();
        }
        scoreCount = scores.Count;
    }
    public void addScore(Score s) {
        scores.Add(s);
        sortScores();
        Write();
        scoreCount++;
    }

    public List<Score> getScores() {
        return scores;
    }

    public int getScoreCount() {
        return scores.Count;
    }

    private void sortScores() {
        ScoreComparer sc = new ScoreComparer();
        scores.Sort(sc);
    }

    private void Write() {
        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None);
        formatter.Serialize(stream, scores);
        stream.Close();
    }
    private void Read() {
        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
        this.scores = (List<Score>)formatter.Deserialize(stream);
        stream.Close();
    }

    public void EvaluateStreak() {
        float scoreMultiplier = 1.0f + (float)currentStreak;
        score += basePoints * scoreMultiplier;

        if (previousSuccess) {
            currentStreak++;
        } else {
            previousSuccess = true;
        }
    }

    public void AddMistake() {
        score -= baseMalus;
        previousSuccess = false;
        currentStreak = 0;
        mistakeCount += 1;
    }

    private void finalScore() {
        float timeRatio = baseTime / timer;
        score *= timeRatio;
    }
    private string GetRandomName() {
        string animal = "";
        string color = "";
        string concatened = "";
        if (gm.CurrentLang == GameManager.Language.FR) {
            animal = animalNamesFr[Random.Range(0, animalNamesFr.Length)];
            color = colorsFR[Random.Range(0, colorsFR.Length)];
            concatened = animal + " " + color;
        } else {
            animal = animalNamesDE[Random.Range(0, animalNamesFr.Length)];
            color = colorsDE[Random.Range(0, colorsFR.Length)];
            concatened = color + " " + animal;
        }
         return concatened.First().ToString().ToUpper() + concatened.Substring(1);
    }

}


public class ScoreComparer : IComparer<Score> {
    public int Compare(Score x, Score y) {
        return x.Time > y.Time ? 1 : -1;
    }
}



