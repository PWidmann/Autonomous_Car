using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehavior : MonoBehaviour
{
    enum SteeringMode { ClassicAI, NeuralNet };

    CarController carController;

    [SerializeField] bool autoPilotOn = true;
    [SerializeField] SteeringMode steeringMode;
    [SerializeField] public DistanceSensor middleSensor;
    [SerializeField] public DistanceSensor leftSensor;
    [SerializeField] public DistanceSensor rightSensor;

    List<DistanceSensor> sensors = new List<DistanceSensor>();

    float steerAmount = 0;

    private void Start()
    {
        carController = GetComponent<CarController>();
        sensors.Add(middleSensor);
        sensors.Add(leftSensor);
        sensors.Add(rightSensor);
    }

    private void FixedUpdate()
    {
        if (autoPilotOn)
        {
            UpdateSensors();

            switch (steeringMode)
            {
                case SteeringMode.ClassicAI:
                    UpdateMotorPower();
                    UpdateSteering();
                    UpdateBrake();
                    break;
                case SteeringMode.NeuralNet:
                    NeuralNet();
                    break;
            }  
        }
    }

    

    private void NeuralNet()
    {
        float motorTorque = (float)neuralController.motor + 0.1f;
        float steering = (float)neuralController.steering;
        float brakeTorque = 0;

        carController.AutoPilotMotorTorque(motorTorque);
        carController.AutoPilotSteer(steering);
    }

    private void UpdateBrake()
    {
        if (middleSensor.Distance < 15f && carController.Velocity > carController.MaxVelocity * 0.7f && middleSensor.Distance != 0)
        {
            carController.AutopilotBrake(1f);
        }
        else
        {
            carController.AutopilotBrake(0f);
        }
    }

    private void UpdateSteering()
    {
        if (leftSensor.Distance > rightSensor.Distance)
        {
            steerAmount = -1f;
        }

        if (rightSensor.Distance > leftSensor.Distance)
        {
            steerAmount = 1f;
        }

        // Steer amount adjustment 
        //steerAmount /= 4;

        if (middleSensor.Distance > 20f)
        {
            steerAmount /= 15;
        }
        else if (middleSensor.Distance <= 20f && middleSensor.Distance > 10f)
        {
            steerAmount /= 6;
        }
        else
        {
            steerAmount /= 3;
        }

        float targetSteerAmount = Mathf.Lerp(carController.wheels[0].collider.steerAngle, steerAmount, 0.1f);

        carController.AutoPilotSteer(targetSteerAmount);
    }

    private void UpdateMotorPower()
    {
        if (!carController.IsBraking && carController.Velocity < carController.MaxVelocity)
        {
            float velocity = Mathf.Lerp(carController.Velocity, carController.MaxVelocity, 0.1f);


            float torqueValue = velocity / carController.MaxVelocity;

            // Needs value between 0 and 1
            carController.AutoPilotMotorTorque(torqueValue);
        }
        else
        {
            carController.AutoPilotMotorTorque(0f);
        }

        if (carController.IsBraking)
        {
            carController.AutoPilotMotorTorque(0f);
        }
    }

    private void UpdateSensors()
    {
        foreach (DistanceSensor sensor in sensors)
        {
            sensor.UpdatePosition();
        }
    }

    public double[] GetSensorValues()
    {
        double[] sensorData = new double[3];
        sensorData[0] = leftSensor.GetNormalizedValue();
        sensorData[1] = middleSensor.GetNormalizedValue();
        sensorData[2] = rightSensor.GetNormalizedValue();
        return sensorData;
    }
}
