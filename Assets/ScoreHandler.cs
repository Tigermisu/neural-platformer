using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreHandler : MonoBehaviour {
    Text inputText,
        scoreText,
        generationText,
        weightText;

    public float Score {
        get { return score; }
    }

    float score = 0,
        maxScore = 0;

    int generations = 0;

	// Use this for initialization
	void Awake () {
        inputText = GameObject.Find("inputs").GetComponent<Text>();
        scoreText = GameObject.Find("score").GetComponent<Text>();
        generationText = GameObject.Find("generation").GetComponent<Text>();
        weightText = GameObject.Find("weights").GetComponent<Text>();
        generationText.text = string.Format("Generation: {0}", ++generations);
        displayScore();
    }

    public void addScore(float s) {
        score += s;
        displayScore();
    }

    public void saveMaxScore() {
        if (score > maxScore) { 
            maxScore = score;
        }
        score = 0;
        generationText.text = string.Format("Generation: {0}", ++generations);
        displayScore();
    }

    private void displayScore() {
        scoreText.text = string.Format("Score: {0}\nMax Score: {1}", score, maxScore);
    }
	

    public void displayInputs(float[] inputs) {
        string txt = "";

        for (int i = 0; i < inputs.Length; i++) {
            txt += string.Format("X{0}: {1}\n", i+1, inputs[i]);
        }

        inputText.text = txt;
    }

    public void displayWeights(float[] weights) {
        string txt = "";

        for (int i = 0; i < weights.Length; i++) {
            txt += string.Format("W{0}: {1}\n", i, weights[i]);
        }

        weightText.text = txt;
    }
}
