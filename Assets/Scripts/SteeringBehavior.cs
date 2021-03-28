using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehavior : MonoBehaviour
{
    CarController carController;

    [SerializeField] bool autoPilotOn = true;
    [SerializeField] DistanceSensor middleSensor;
    [SerializeField] DistanceSensor leftSensor;
    [SerializeField] DistanceSensor rightSensor;

    List<DistanceSensor> sensors = new List<DistanceSensor>();

    float steerChangeCooldown = 0.1f;
    float steerAmount = 0;

    private void Start()
    {
        carController = GetComponent<CarController>();
        sensors.Add(middleSensor);
        sensors.Add(leftSensor);
        sensors.Add(rightSensor);
    }

    private void Update()
    {
        if (autoPilotOn)
        {
            UpdateSensors();

            UpdateMotorPower();

            UpdateSteering();

            UpdateBrake();
        }
    }

    private void UpdateBrake()
    {
        if (middleSensor.Distance < 15f && middleSensor.Distance > 13f)
        {
            carController.AutopilotBrake(1000f);
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
            steerAmount = -0.7f / middleSensor.Distance;
        }

        if (rightSensor.Distance > leftSensor.Distance)
        {
            steerAmount = 0.7f / middleSensor.Distance;
        }
        
        if (middleSensor.Distance > 60f)
            steerAmount /= 4;

        if (middleSensor.Distance < 60f && middleSensor.Distance > 30f)
            steerAmount /= 2;
        
        carController.AutoPilotSteer(steerAmount * 250f);
    }

    private void UpdateMotorPower()
    {
        if (!carController.IsBraking && carController.GetMotorRPM() < 400f)
        {
            float power = Mathf.Lerp(carController.GetMotorTorque(), 400f, 20f);
            carController.AutoPilotMotorTorque(power);
        }
        else
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
}
