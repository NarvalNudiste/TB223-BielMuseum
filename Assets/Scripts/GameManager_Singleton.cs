using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager_Singleton : MonoBehaviour {
    GameManager gm;
    void Awake() {
        if (this.GetComponent<GameManager>() != null) {
            gm = this.GetComponent<GameManager>();
        } else {
            gm = this.gameObject.AddComponent<GameManager>();
        }
    }
    public void SetFR() {
        gm.SetLanguage(GameManager.Language.FR);
    }
    public void SetDE() {
        gm.SetLanguage(GameManager.Language.DE);
    }
}
