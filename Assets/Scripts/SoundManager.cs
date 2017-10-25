using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
    public static SoundManager Instance = null;
    private AudioSource soundEffectAudio;
    public AudioClip GunFire;
    public AudioClip BottleKill;


    private void Start() {
        if (Instance == null) {
            Instance = this;
        } else if (Instance != this) {
            Destroy(gameObject);
        }
        soundEffectAudio = GetComponent<AudioSource>();
    }


    public  void PlayOneShot(AudioClip clip) {
        soundEffectAudio.PlayOneShot(clip);
    }
}
