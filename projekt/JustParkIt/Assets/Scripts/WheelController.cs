using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelController : MonoBehaviour
{
    [SerializeField] WheelCollider frontRight;
    [SerializeField] WheelCollider frontLeft;
    [SerializeField] WheelCollider rearRight;
    [SerializeField] WheelCollider rearLeft;
    [SerializeField] Transform frontRightVisual;
    [SerializeField] Transform frontLeftVisual;
    [SerializeField] Transform rearRightVisual;
    [SerializeField] Transform rearLeftVisual;

    public float acceleration = 1400;
    public float brakingForce = 300f;
    public float maxTurnAngle = 150f;

    private float currentAcceleration = 0f;
    private float currentBreakingForce = 0f;
    public float currentTurnAngle = 0f;

    private void FixedUpdate() {

        currentAcceleration = acceleration * Input.GetAxis("Vertical");

        if( Input.GetKey(KeyCode.Space)) {
            Debug.Log("space");
            currentBreakingForce = brakingForce;
        }
        else {
            currentBreakingForce = 0f;
        }

        rearRight.motorTorque = currentAcceleration;
        rearLeft.motorTorque = currentAcceleration;

        rearRight.brakeTorque = currentBreakingForce;
        rearLeft.brakeTorque = currentBreakingForce;

        currentTurnAngle = maxTurnAngle * Input.GetAxis("Horizontal");
        frontLeft.steerAngle = currentTurnAngle;
        frontRight.steerAngle = currentTurnAngle;

        UpdateWheel(rearLeft, rearLeftVisual);
        UpdateWheel(rearRight, rearRightVisual);
        UpdateWheel(frontLeft, frontLeftVisual);
        UpdateWheel(frontRight, frontRightVisual);

    }

    public void UpdateWheel(WheelCollider collision, Transform transform) {
        Vector3 position;
        Quaternion rotation;
        
        collision.GetWorldPose(out position, out rotation);
        transform.position = position;
        transform.rotation = rotation;
    }
}
