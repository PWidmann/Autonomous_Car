using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehavior :MonoBehaviour
{
    private float steerAmount;

    public void ClassicAIBrake(CarController carController, float middleSensorDistance)
    {
        if (middleSensorDistance < 16f && carController.Velocity > carController.MaxVelocity * 0.7f && middleSensorDistance != 0)
        {
            carController.AIBrake(1f);
        }
        else
        {
            carController.AIBrake(0f);
        }
    }

    public void ClassicAISteering(CarController carController)
    {
        float targetSteerAmount = Mathf.Lerp(carController.wheels[0].collider.steerAngle / carController.maxSteerAngle, steerAmount, 0.1f);

        carController.AISteer(targetSteerAmount);
    }

    public void SetClassicAISensorSteering(float leftSensorDistance, float rightSensorDistance, float middleSensorDistance)
    {
        if (leftSensorDistance > rightSensorDistance)
        {
            steerAmount = -1f;
        }
        
        if (rightSensorDistance > leftSensorDistance)
        {
            steerAmount = 1f;
        }
        
        // Steer amount adjustment according to distance in front of the car to smooth car behavior
        // More space in front of the car = less steering angle
        if (middleSensorDistance > 20f)
        {
            steerAmount /= 20;
        }
        else if (middleSensorDistance <= 20f && middleSensorDistance > 10f)
        {
            steerAmount /= 6;
        }
        else
        {
            steerAmount /= 3;
        }
    }

    public void SetClassicAIWaypointSteering(AIController aiController, CarController carController)
    {
        // Get target position
        Vector3 targetPos = aiController.waypoints[aiController.currentWayPoint].transform.position;
        targetPos.y = aiController.transform.position.y;
        
        // Get target direction and angle to target waypoint
        Vector3 targetDir = targetPos - aiController.transform.position;
        float angle = Vector3.Angle(aiController.transform.forward, targetDir) / (carController.maxSteerAngle * 2);
        
        // if  1:target is on the right side
        // if -1:target on left side
        // 0 if straight
        float dotProduct = Vector3.Dot(targetDir, aiController.transform.right); 
        
        // Change steering angle to minus or plus if target is on the left or the right side of the car
        float targetSteerAmount = dotProduct > 0 ? angle : -angle;
        
        // Apply steering smoothing
        steerAmount = Mathf.Lerp(carController.wheels[0].collider.steerAngle / carController.maxSteerAngle, targetSteerAmount, 0.7f);

        // Jump to next waypoint
        if (Vector3.Distance(targetPos, aiController.transform.position) < 8f)
        {
            if (aiController.currentWayPoint != aiController.waypoints.Count - 1)
                aiController.SetCurrentWayPoint(aiController.currentWayPoint + 1);
            else
                aiController.SetCurrentWayPoint(0);
        }
    }

    public void ClassicAIMotorTorque(CarController carController)
    {
        if (!carController.IsBraking && carController.Velocity < carController.MaxVelocity)
        {
            float velocity = Mathf.Lerp(carController.Velocity, carController.MaxVelocity, 0.1f);
            float torqueValue = velocity / carController.MaxVelocity;
        
            // Needs value between 0 and 1
            carController.AIMotorTorque(torqueValue);
        }
        else
        {
            carController.AIMotorTorque(0f);
        }
        
        if (carController.IsBraking)
        {
            carController.AIMotorTorque(0f);
        }
    }

    
}
