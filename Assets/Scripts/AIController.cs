using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public enum SteeringMode { ClassicAI, NeuralNet };
    public enum ClassicAIMode { Sensors, Waypoints };

    CarController carController;
    SteeringBehavior steeringBehavior;

    [SerializeField] public bool autoPilotOn = true;
    [SerializeField] public SteeringMode steeringMode;
    [SerializeField] public ClassicAIMode classicAIMode;
    [SerializeField] public DistanceSensor middleSensor;
    [SerializeField] public DistanceSensor leftSensor;
    [SerializeField] public DistanceSensor rightSensor;
    [SerializeField] public GameObject wayPointParent;

    public int currentWayPoint = 0;

    List<DistanceSensor> sensors = new List<DistanceSensor>();

    public List<GameObject> waypoints = new List<GameObject>();

    private void Start()
    {
        carController = GetComponent<CarController>();
        steeringBehavior = new SteeringBehavior();
        sensors.Add(middleSensor);
        sensors.Add(leftSensor);
        sensors.Add(rightSensor);

        foreach (Transform child in wayPointParent.transform)
            waypoints.Add(child.gameObject);
    }

    private void FixedUpdate()
    {
        if (autoPilotOn)
        {
            UpdateSensors();
        
            switch (steeringMode)
            {
                case SteeringMode.ClassicAI:
                    SelectClassicAIMode();
                    break;
                case SteeringMode.NeuralNet:
                    NeuralNet();
                    break;
            }
        }
    }

    void SelectClassicAIMode()
    {
        switch (classicAIMode)
        {
            case ClassicAIMode.Sensors:
                steeringBehavior.SetClassicAISensorSteering(sensors[1].Distance, sensors[2].Distance, sensors[0].Distance);
                break;
            case ClassicAIMode.Waypoints:
                steeringBehavior.SetClassicAIWaypointSteering(this, carController);
                break;
        }

        steeringBehavior.ClassicAISteering(carController);
        steeringBehavior.ClassicAIMotorTorque(carController);
        steeringBehavior.ClassicAIBrake(carController, middleSensor.Distance);
    }
    private void NeuralNet()
    {
        
        float motorTorque = (float)NeuralController.motor + 0.1f;
        float steering = (float)NeuralController.steering;
        //float braking = (float)neuralController.braking;
        
        carController.AIMotorTorque(motorTorque);
        carController.AISteer(steering);

        //carController.AutopilotBrake(braking);
    }


    private void UpdateSensors()
    {
        foreach (DistanceSensor sensor in sensors)
        {
            sensor.UpdatePosition();
        }

        for (int i = 0; i < sensors.Count; i++)
        {
            NeuralController.sensors[i] = sensors[i].Distance;
        }
    }

    public void SetCurrentWayPoint(int value)
    {
        currentWayPoint = value;
    }

    public double[] GetSensorValues()
    {
        double[] sensorData = new double[4];
        sensorData[0] = leftSensor.GetNormalizedValue();
        sensorData[1] = middleSensor.GetNormalizedValue();
        sensorData[2] = rightSensor.GetNormalizedValue();
        sensorData[3] = carController.Velocity / 100;
        return sensorData;
    }
}
