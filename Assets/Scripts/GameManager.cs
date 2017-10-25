using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;

public class GameManager : MonoBehaviour {
    public static GameManager Instance = null;
    private static int score = 0;
    private static int bottleInGame = 0;
    private TextMesh text;
    private bool gameIsReady = false;
    private bool setScoreActive = false;
    public GameObject ScoreObject;

    public static int Score {
        get {
            return score;
        }

        set {
            score = value;
        }
    }

    public static int BottleInGame {
        get {
            return bottleInGame;
        }

        set {
            bottleInGame = value;
        }
    }

    public bool GameIsReady {
        get {
            return gameIsReady;
        }

        set {
            gameIsReady = value;
        }
    }

    private void Start() {
        if (Instance == null) {
            Instance = this; }
        else if (Instance != this){
            Destroy(gameObject); }
        text = gameObject.GetComponentInChildren<TextMesh>();
    }

    private void Update() {
        if (gameIsReady) {
            Scoretext();
            if (!setScoreActive) {
                ScoreObject.SetActive(true);
                setScoreActive = true;
            }
        }

        
    }

    public void Scoretext() {
        if (bottleInGame > 0) {
            text.text = "Bottle in Game " + BottleInGame + " Score is " + Score;
        } else {
            text.text = "you win Cowboy";
        }
    }
    private void SetChildActive() {
        ScoreObject.SetActive(true);

}

}
