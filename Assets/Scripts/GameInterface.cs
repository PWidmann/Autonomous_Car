using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameInterface : MonoBehaviour
{
    public static GameInterface Instance;

    [SerializeField] private Text kmhText;
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

    [SerializeField] SteeringBehavior steeringBehavior;
    [SerializeField] CarController carController;
    [SerializeField] AIController aiController;

    float uiUpdateTimer = 0;

    void Start()
    {
        if (Instance == null)
            Instance = this;

        uiUpdateTimer = 0.2f;
    }
    public void Update()
    {
        GAPanel.SetActive(true);


        uiUpdateTimer -= Time.deltaTime;
        
        generationText.text = "Generation: " + (NeuralController.generation + 1);
        populationText.text = "Population: " + (NeuralController.currentNeuralNetwork + 1) + " / " + NeuralController.staticPopulation;
        motor.text = "Motor: " + NeuralController.motor;
        steering.text = "Steering: " + NeuralController.steering;
        braking.text = "Braking: " + NeuralController.braking;

        double[] sensors = aiController.GetSensorValues();

        leftSensor.text = "Left Sensor: " + sensors[0];
        middleSensor.text = "Middle Sensor: " + sensors[1];
        rightSensor.text = "Right Sensor: " + sensors[2];
        speedText.text = "Velocity Sensor: " + sensors[3];

        if (uiUpdateTimer <= 0)
        {
            kmhText.text = Math.Floor((carController.Velocity * 3.6f)) + " km/h";
            uiUpdateTimer = 0.2f;
        }
        
    }
}
