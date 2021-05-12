using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Axel
{ 
    Front,
    Back
}

[Serializable]
public struct Wheel 
{
    public GameObject model;
    public WheelCollider collider;
    public Axel axel;
}

public class CarController : MonoBehaviour
{
    
    [SerializeField] public float maxMotorTorque = 20.0f;
    [SerializeField] public float maxVelocity = 20.0f;
    [SerializeField] public float maxBrakePower = 20.0f;
    [SerializeField] public float maxSteerAngle = 60.0f;

    [SerializeField] public List<Wheel> wheels;
    [SerializeField] private Vector3 centerOfMass;
    [SerializeField] GameObject brakeLight;
    [SerializeField] Material brakeLightMaterial;

    AIController aIController;

    private float inputX, inputY;
    public Rigidbody rb;
    private bool isBraking = false;
    private float velocity = 0f;
    public bool IsBraking { get => isBraking; set => isBraking = value; }
    public float Velocity { get => velocity; set => velocity = value; }
    public float MaxVelocity { get => maxVelocity; set => maxVelocity = value; }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = centerOfMass;
        aIController = GetComponent<AIController>();
    }


    private void Update()
    {
        ManualSteering();
        AnimateWheels();  
    }

    private void FixedUpdate()
    {
        velocity = rb.velocity.magnitude;
    }

    void ManualSteering()
    {
        if (!aIController.autoPilotOn)
        {
            // Get input
            inputX = Input.GetAxis("Horizontal");
            inputY = Input.GetAxis("Vertical");

            // Braking
            if (Input.GetKeyDown(KeyCode.S))
            {
                AIBrake(maxBrakePower);
            }
            if (Input.GetKeyUp(KeyCode.S))
            {
                AIBrake(0);
            }

            // Power
            float power = inputY;
            if (power > 0)
            {
                AIMotorTorque(power);
                AIBrake(0);
            }

            // Steering
            AISteer(inputX * 0.8f);
        }
    }

    //
    // AI movement
    //

    /// <summary>
    /// Needs input beween 0 and 1. 1 is maximum power.
    /// </summary>
    /// <param name="_input"></param>
    public void AIMotorTorque(float _input)
    {
        foreach (var wheel in wheels)
        {
            wheel.collider.motorTorque = _input * maxVelocity * maxMotorTorque;
        }
    }

    /// <summary>
    /// Needs input beween -1(steering left) and 1(steering right).
    /// </summary>
    /// <param name="_input"></param>
    public void AISteer(float _steerAmount)
    {
        if (_steerAmount < -1)
            _steerAmount = -1;

        if (_steerAmount > 1)
            _steerAmount = 1;

        foreach (var wheel in wheels)
        {
            if (wheel.axel == Axel.Front)
            {
                wheel.collider.steerAngle = _steerAmount * maxSteerAngle;
            }
        }
        
    }
    /// <summary>
    /// Needs input beween 0 and 1. 1 is maximum power.
    /// </summary>
    /// <param name="_input"></param>
    public void AIBrake(float _brakePower)
    {
        if (_brakePower > 0)
        {
            brakeLightMaterial.EnableKeyword("_EMISSION");
            brakeLight.SetActive(true);
            IsBraking = true;

            foreach (var wheel in wheels)
            {
                wheel.collider.brakeTorque = _brakePower * maxBrakePower;
            }
        }
        else
        {
            brakeLightMaterial.DisableKeyword("_EMISSION");
            brakeLight.SetActive(false);
            IsBraking = false;

            foreach (var wheel in wheels)
            {
                wheel.collider.brakeTorque = 0;
            }
        }
    }

    private void AnimateWheels()
    {
        foreach (var wheel in wheels)
        {
            Quaternion rot;
            Vector3 pos;

            wheel.collider.GetWorldPose(out pos, out rot);
            wheel.model.transform.position = pos;
            wheel.model.transform.rotation = rot;
        }
    }

    public float GetRPM()
    {
        return wheels[0].collider.rpm;
    }
}
