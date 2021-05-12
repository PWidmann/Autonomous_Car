using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameInterface : MonoBehaviour
{
    public static GameInterface Instance;

    [SerializeField] private Text kmhText;
    [SerializeField] private Text lapTimerText;
    [SerializeField] private Text speedText;
    [SerializeField] private Text generationText;
    [SerializeField] private Text populationText;
    [SerializeField] private Text motor;
    [SerializeField] private Text steering;
    [SerializeField] private Text braking;
    [SerializeField] private Text leftSensor;
    [SerializeField] private Text middleSensor;
    [SerializeField] private Text rightSensor;
    [SerializeField] private GameObject GAPanel;
    [SerializeField] public GameObject NNVisualisation;
    [SerializeField] public GameObject controlPanel;
    [SerializeField] public GameObject speedoMeterPanel;
    [SerializeField] public GameObject lapTimePanel;

    [SerializeField] CarController carController;
    [SerializeField] AIController aiController;

    float uiUpdateTimer = 0;

    void Start()
    {
        if (Instance == null)
            Instance = this;

        uiUpdateTimer = 0.2f;
        controlPanel.SetActive(true);
        NNVisualisation.SetActive(true);
        speedoMeterPanel.SetActive(true);
        lapTimePanel.SetActive(true);
    }
    public void Update()
    {
        uiUpdateTimer -= Time.deltaTime;

        lapTimerText.text = GetLapTime(aiController.lapTimer);
        
        generationText.text = "Generation: " + (NeuralController.generation + 1);
        populationText.text = "Population: " + (NeuralController.currentNeuralNetwork + 1) + " / " + "16";
        motor.text = "Motor: " + NeuralController.motor;
        steering.text = "Steering: " + NeuralController.steering;
        braking.text = "Braking: " + NeuralController.braking;

        double[] sensors = NeuralController.sensors;

        leftSensor.text = "Left Sensor: " + sensors[1];
        middleSensor.text = "Middle Sensor: " + sensors[0];
        rightSensor.text = "Right Sensor: " + sensors[2];
        speedText.text = "Velocity Sensor: " + sensors[3];

        if (uiUpdateTimer <= 0)
        {
            kmhText.text = Math.Floor((carController.Velocity * 3.6f)) + " km/h";
            uiUpdateTimer = 0.2f;
        }
        
    }

    String GetLapTime(float currentTime)
    {
        string lapTime = "";

        float minutes = Mathf.Floor(currentTime / 60);
        float seconds = Mathf.Floor(currentTime % 60);
        float hundredthSecond = Mathf.Floor((currentTime - (minutes*60) - seconds) * 100);

        // Add a 0 when single digit
        string minutesS = (minutes < 10 ? "0" + minutes : minutes.ToString());
        string secondsS = (seconds < 10 ? "0" + seconds : seconds.ToString());

        string hundredthSecondS;
        if (currentTime == 0)
            hundredthSecondS = "00";
        else
            hundredthSecondS = (hundredthSecond < 10 ? "0" + hundredthSecond : hundredthSecond.ToString());

        lapTime = minutesS + ":" + secondsS + ":" + hundredthSecondS;

        return lapTime;
    }
}
