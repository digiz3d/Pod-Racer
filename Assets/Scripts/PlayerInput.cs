using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{

    [HideInInspector]
    public float accel = 0f;

    [HideInInspector]
    public float yaw = 0f;

    // Update is called once per frame
    void Update()
    {
        accel = Input.GetAxis("Vertical");
        yaw = Input.GetAxis("Horizontal");
    }
}
