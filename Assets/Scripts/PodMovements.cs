using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PodMovements : MonoBehaviour
{
    public float maxSpeed = 200f;
    public float deceleration = 0.99f;
    public float brake = 0.95f;

    public float maxTurnSpeed = 80f;

    private PlayerInput input;

    [HideInInspector]
    public float forwardSpeed;
    [HideInInspector]
    public float rotationSpeed;

    void Start()
    {
        input = GetComponent<PlayerInput>();
    }

    void Update()
    {
        if (input.accel > 0)
        {
            forwardSpeed += input.accel;

            forwardSpeed = Mathf.Clamp(forwardSpeed, 0f, maxSpeed);
        }
        else if (input.accel < 0f)
        {
            forwardSpeed *= 0.95f;
        }
        else
        {
            forwardSpeed *= 0.99f;
        }

        transform.position += transform.forward * (forwardSpeed * Time.deltaTime);


        if (input.yaw != 0f)
        {
            rotationSpeed += input.yaw;
        }
        else
        {
            Debug.Log("stopping");
            rotationSpeed *= 0.95f;
        }

        

        rotationSpeed = Mathf.Clamp(rotationSpeed, -maxTurnSpeed, +maxTurnSpeed);

        Debug.Log(input.yaw);
        Debug.Log(rotationSpeed);

        transform.localRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + (rotationSpeed * Time.deltaTime), transform.eulerAngles.z);
    }
}
