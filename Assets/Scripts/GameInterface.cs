using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameInterface : MonoBehaviour
{
    public static GameInterface Instance;

    [SerializeField] private Text speedText;
    [SerializeField] private Text generationText;
    [SerializeField] private Text populationText;
    [SerializeField] private Text motor;
    [SerializeField] private Text steering;
    [SerializeField] private Text leftSensor;
    [SerializeField] private Text middleSensor;
    [SerializeField] private Text rightSensor;

    [SerializeField] SteeringBehavior steeringBehavior;
    [SerializeField] CarController carController;

    void Start()
    {
        if (Instance == null)
            Instance = this;
    }
    public void Update()
    {
        //speedText.text = Mathf.Floor(_speed * 2).ToString() + " km/h";
        speedText.text = "Car Velocity: " + Mathf.Floor(carController.Velocity);
        generationText.text = "Generation: " + (neuralController.generation + 1);
        populationText.text = "Population: " + (neuralController.currentNeuralNetwork + 1) + " / " + neuralController.staticPopulation;
        motor.text = "Motor: " + neuralController.motor;
        steering.text = "Steering: " + neuralController.steering;

        double[] sensors = steeringBehavior.GetSensorValues();

        leftSensor.text = "Left Sensor: " + sensors[0];
        middleSensor.text = "Middle Sensor: " + sensors[1];
        rightSensor.text = "Right Sensor: " + sensors[2];
    }
}
