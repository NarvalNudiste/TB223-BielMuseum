using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {
  //  public AudioClip AClipAxeFR, AClipAxeDE, AClipCeramicSmallFR, AClipCeramicSmallDE, AClipCeramicBigFR, AClipCeramicBigDE, AClipKnifeFR, AClipKnifeDE;
  //  public AudioClip AClipOkSound, AClipMissedSound;
    private GameManager gm;
    private AudioSource objectSource;
    private AudioSource fxSource;
    public AudioClip victorySound, defeatSound;
    public float defaultObjectDescriptionVolume = 0.3f;
    private bool mute = false;

    void Start() {
        gm = FindObjectOfType<GameManager>();
        objectSource = this.gameObject.AddComponent<AudioSource>();
        fxSource = this.gameObject.AddComponent<AudioSource>();
        objectSource.volume = defaultObjectDescriptionVolume;
    }

    public void PlayObjectDescription(AudioClip ac) {
        if (!mute) {
            if (!objectSource.isPlaying) {
                objectSource.clip = ac;
                objectSource.Play();
            } else {
                //todo fadeOut
            }
        }
    }

    void Update() {
        if (Input.GetKey(KeyCode.Y)) {
            PlayCorrectSound();
        }
    }

    public void PlayCorrectSound() {
        fxSource.clip = victorySound;
        fxSource.Play();
    }
    public void PlayDefeatSound() {
        fxSource.clip = defeatSound;
        fxSource.Play();
    }
}
