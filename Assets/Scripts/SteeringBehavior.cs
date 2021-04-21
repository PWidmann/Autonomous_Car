using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehavior : MonoBehaviour
{
    public enum SteeringMode { ClassicAI, NeuralNet };
    public enum ClassicAIMode { Sensors, Waypoints };

    CarController carController;

    [SerializeField] public bool autoPilotOn = true;
    [SerializeField] public SteeringMode steeringMode;
    [SerializeField] public ClassicAIMode classicAIMode;
    [SerializeField] public DistanceSensor middleSensor;
    [SerializeField] public DistanceSensor leftSensor;
    [SerializeField] public DistanceSensor rightSensor;
    [SerializeField] public GameObject wayPointParent;

    List<DistanceSensor> sensors = new List<DistanceSensor>();

    public List<GameObject> waypoints = new List<GameObject>();
    public int currentWayPoint = 0;

    float steerAmount = 0;

    private void Start()
    {
        carController = GetComponent<CarController>();
        sensors.Add(middleSensor);
        sensors.Add(leftSensor);
        sensors.Add(rightSensor);

        foreach (Transform child in wayPointParent.transform)
            waypoints.Add(child.gameObject);
    }

    private void Update()
    {
        if (!autoPilotOn)
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                carController.AutopilotBrake(carController.maxBrakePower);
            }
            if (Input.GetKeyUp(KeyCode.S))
            {
                carController.AutopilotBrake(0);
            }
        }
    }

    private void FixedUpdate()
    {
        if (autoPilotOn)
        {
            UpdateSensors();

            switch (steeringMode)
            {
                case SteeringMode.ClassicAI:
                    ClassicAIMotorTorque();
                    ClassicAISteering();
                    ClassicAIBrake();
                    break;
                case SteeringMode.NeuralNet:
                    NeuralNet();
                    break;
            }
        }
        else
        {
            float power = Input.GetAxis("Vertical");
            if (power > 0)
            {
                carController.AutoPilotMotorTorque(power);
                carController.AutopilotBrake(0);
            }
            carController.AutoPilotSteer(Input.GetAxis("Horizontal") * 0.75f);
        }
    }

    

    private void NeuralNet()
    {
        float motorTorque = (float)NeuralController.motor + 0.1f;
        float steering = (float)NeuralController.steering;
        //float braking = (float)neuralController.braking;

        carController.AutoPilotMotorTorque(motorTorque);
        carController.AutoPilotSteer(steering);
        //carController.AutopilotBrake(braking);
    }

    private void ClassicAIBrake()
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

    private void ClassicAISteering()
    {
        switch (classicAIMode)
        {
            case ClassicAIMode.Sensors:
                ClassicAISensorSteering();
                break;
            case ClassicAIMode.Waypoints:
                ClassicAIWaypointSteering();
                break;
        }
        

        //float targetSteerAmount = Mathf.Lerp(carController.wheels[0].collider.steerAngle, steerAmount, 0.1f);
        //
        //Debug.Log(targetSteerAmount);

        carController.AutoPilotSteer(steerAmount);
    }

    private void ClassicAISensorSteering()
    {
        if (leftSensor.Distance > rightSensor.Distance)
        {
            steerAmount = -1f;
        }

        if (rightSensor.Distance > leftSensor.Distance)
        {
            steerAmount = 1f;
        }

        // Steer amount adjustment according to distance in front of the car to smooth car behavior
        // More space in front of the car = less steering angle
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
    }

    private void ClassicAIWaypointSteering()
    {
        // Get target position
        Vector3 targetPos = waypoints[currentWayPoint].transform.position;
        targetPos.y = transform.position.y;

        // Get target direction and angle to target waypoint
        Vector3 targetDir = targetPos - transform.position;
        float angle = Vector3.Angle(transform.forward, targetDir) / (carController.maxSteerAngle * 2);

        // if  1:target is on the right side
        // if -1:target on left side
        // 0 if straight
        float dotProduct = Vector3.Dot(targetDir, transform.right); 

        // Change steering angle to minus or plus if target is on the left or the right side of the car
        float targetSteerAmount = dotProduct > 0 ? angle : -angle;

        // Apply steering smoothing
        steerAmount = Mathf.Lerp(carController.wheels[0].collider.steerAngle / carController.maxSteerAngle, targetSteerAmount, 0.1f);

        // Jump to next waypoint
        if (Vector3.Distance(targetPos, transform.position) < 8f)
        {
            if (currentWayPoint != waypoints.Count - 1)
                currentWayPoint++;
            else
                currentWayPoint = 0;
        }
    }

    private void ClassicAIMotorTorque()
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
        double[] sensorData = new double[4];
        sensorData[0] = leftSensor.GetNormalizedValue();
        sensorData[1] = middleSensor.GetNormalizedValue();
        sensorData[2] = rightSensor.GetNormalizedValue();
        sensorData[3] = carController.Velocity / 100;
        return sensorData;
    }
}
