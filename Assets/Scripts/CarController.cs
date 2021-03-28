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
    [SerializeField] private float maxAcceleration = 20.0f;
    [SerializeField] private float turnSensitivity = 0.3f;
    [SerializeField] private float maxSteerAngle = 45.0f;
    [SerializeField] private List<Wheel> wheels;
    [SerializeField] private Vector3 centerOfMass;
    [SerializeField] GameObject brakeLight;
    [SerializeField] Material brakeLightMaterial;

    private float inputX, inputY;
    private Rigidbody rb;
    private bool isBraking = false;

    public bool IsBraking { get => isBraking; set => isBraking = value; }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = centerOfMass;
    }


    private void Update()
    {
        AnimateWheels();
        GetInputs();

        GameInterface.Instance.SetSpeedText(GetMotorRPM());
    }

    private void FixedUpdate()
    {
        //Move();
        //Turn();
        //HandBrake();
    }

    private void GetInputs()
    {
        inputX = Input.GetAxis("Horizontal");
        inputY = Input.GetAxis("Vertical");
    }

    public float GetMotorRPM()
    {
        return wheels[0].collider.rpm;
    }

    public float GetMotorTorque()
    {
        return wheels[0].collider.motorTorque;
    }

    private void Move()
    {
        foreach (var wheel in wheels)
        {
            //if(wheel.axel == Axel.Back)
                wheel.collider.motorTorque = inputY * maxAcceleration * 2500 * Time.fixedDeltaTime;
        }
    }

    private void Turn()
    {
        foreach (var wheel in wheels)
        {
            if (wheel.axel == Axel.Front)
            {
                var steerAngle = inputX * turnSensitivity * maxSteerAngle;

                wheel.collider.steerAngle = Mathf.Lerp(wheel.collider.steerAngle, steerAngle, 0.5f);
            }
        }
    }

    

    private void HandBrake()
    {
        float breakPower = 0;
        IsBraking = false;

        if (Input.GetKey(KeyCode.Space))
        {
            brakeLightMaterial.EnableKeyword("_EMISSION");

            breakPower = 1000f;
            IsBraking = true;
        }
        else
        {
            brakeLightMaterial.DisableKeyword("_EMISSION");
        }

        foreach (var wheel in wheels)
        {
            //if (wheel.axel == Axel.Back)
                wheel.collider.brakeTorque = breakPower;
        }

        brakeLight.SetActive(IsBraking);
    }

    public void AutoPilotMotorTorque(float _input)
    {
        foreach (var wheel in wheels)
        {
            wheel.collider.motorTorque = _input * 15f;
        }
    }

    public void AutoPilotSteer(float _angle)
    {
        foreach (var wheel in wheels)
        {
            if (wheel.axel == Axel.Front)
            {
                float cappedAngle = _angle;

                if (cappedAngle > maxSteerAngle)
                    cappedAngle = maxSteerAngle;

                wheel.collider.steerAngle = Mathf.Lerp(wheel.collider.steerAngle, cappedAngle, 0.05f);
            }
        }
    }

    public void AutopilotBrake(float _brakePower)
    {
        if (_brakePower > 0)
        {
            brakeLightMaterial.EnableKeyword("_EMISSION");
            brakeLight.SetActive(true);
            IsBraking = true;
        }
        else
        {
            brakeLightMaterial.DisableKeyword("_EMISSION");
            brakeLight.SetActive(false);
            IsBraking = false;
        }

        foreach (var wheel in wheels)
        {
            wheel.collider.brakeTorque = _brakePower;
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
}
