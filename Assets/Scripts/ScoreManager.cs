using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] GameObject barPrefab;
    [SerializeField] GameObject scoreBarPanel;
    [SerializeField] GameObject currentScoreBar;

    private float currentMaxScore = 500f; // Good start value for visual progress
    private float currentMaxHeight = 0; // How tall can the score bar panel be max
    private float currentNetScore = 0;
    private float currentGenMaxScore = 0;

    RectTransform currentBarRect;

    private List<int[]> entries = new List<int[]>();
    private List<GameObject> bars = new List<GameObject>();

    private void Start()
    {
        currentBarRect = currentScoreBar.GetComponent<RectTransform>();
        currentMaxHeight = scoreBarPanel.GetComponent<RectTransform>().rect.height -4f; // -4 border pixels
    }

    private void Update()
    {
        // Get current active score from neural controller
        currentNetScore = (float)NeuralController.points[NeuralController.currentNeuralNetwork];

        // Adjust max score for visual calculations
        if (currentNetScore > currentMaxScore)
            currentMaxScore = currentNetScore;

        if (currentNetScore > currentGenMaxScore)
            currentGenMaxScore = currentNetScore;

        float calculatedHeight = (currentNetScore / currentMaxScore) * currentMaxHeight;
        currentBarRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, calculatedHeight);
    }


    // When start button is pressed
    public void ResetScore()
    {
        currentMaxScore = 500f;
        entries.Clear();

        //delete generation bars
        foreach (GameObject go in bars)
        {
            Destroy(go);
        }
        bars.Clear();
    }

    public void AddScoreEntry(int generation, int points)
    {
        int[] score = new int[2];
        score[0] = points;
        score[1] = generation;

        entries.Add(score);

        currentGenMaxScore = 0;

        GameObject temp = Instantiate(barPrefab);
        temp.transform.parent = scoreBarPanel.transform;
        bars.Add(temp);

        for (int i = 0; i < bars.Count; i++)
        {
            int barScore = entries[i][0];

            if (barScore > currentMaxScore)
                currentMaxScore = barScore;
        }

        for (int i = 0; i < bars.Count; i++)
        {
            int barScore = entries[i][0];

            float calculatedHeight = (barScore / currentMaxScore) * currentMaxHeight;
            
            // make bigger if to small to maintain bar visibility
            if (calculatedHeight < 6f)
                calculatedHeight = 6f;

            bars[i].GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, calculatedHeight);
        }       
    }
}
