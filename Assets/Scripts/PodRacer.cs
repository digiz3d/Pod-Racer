using System.Collections.Generic;
using UnityEngine;

public class PodRacer : MonoBehaviour
{
    private bool controllable = true;

    #region Acceleration settings

    public AnimationCurve accelCurve;
    private float timeToFullAcceleration = 1f;
    private float accelFactor = 0f;

    #endregion

    #region Speed settings

    public float maxSpeed = 200f;                                   // meters/second
    public float timeToFullspeed = 7f;                              // second
    public AnimationCurve speedCurve;

    private float speedFactor = 0f;
    public float currentSpeed = 0f;                                 // unit/second
    private Dictionary<int, float> speedCurveApproximation;         // key = speed in m/s , value = factor from 0f to 1f;
    private float speedCurveApproximationPrecision = 0.0001f;       // lower =  more accurate speeds but slower loading times

    private Vector3 forwardVector;

    #endregion

    #region Turn settings

    public float maxTurnSpeed = 70f;
    public float turnSpeedFactor = 1f;
    public float turnOppositeMultiplier = 6f;                       // can be used for smoothing left/right transition
    public float currentTurnSpeed = 0f;

    public float resetTurnFactor = 3f;

    #endregion

    #region Controls

    private bool forward;
    private bool brake;
    private bool left;
    private bool right;

    #endregion



    private void Start()
    {

        #region Speed Approximation calculation

        speedCurveApproximation = new Dictionary<int, float>();

        int speed = 0;
        float lastFactor = 0f;
        while (speed <= (speedCurve.Evaluate(1f) * maxSpeed))
        {
            float factor = GetFactorForSpeed(speed, ref lastFactor);
            lastFactor = factor;
            speedCurveApproximation.Add(speed, factor);
            speed += 1;
            //Debug.Log("<color=green>added "+ speed +" = "+ factor +"</color>");
        }
        //Debug.Log("speed 238 m/s (or 23.8 unity units/s) is achieved at ratio " + GetRatioForSpeed(238));


        #endregion
    }


    private void Update()
    {
        #region Input Keys
        if (controllable)
        {
            forward = Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.UpArrow);
            brake = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
            left = Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow);
            right = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
        }
        #endregion

        #region Acceleration + Speed control

        if (forward)
        {
            accelFactor += Time.deltaTime / timeToFullAcceleration;                             // timeToFullAcceleration to reach max acceleration
            accelFactor = Mathf.Clamp01(accelFactor);
            speedFactor += accelCurve.Evaluate(accelFactor) * Time.deltaTime / timeToFullspeed; // timeToFullspeed to reach max speed (at max acceleration)
            speedFactor = Mathf.Clamp01(speedFactor);
            currentSpeed = speedCurve.Evaluate(speedFactor) * maxSpeed;
        }
        if (brake)
        {
            accelFactor -= Time.deltaTime / timeToFullAcceleration;
            accelFactor = Mathf.Clamp01(accelFactor);
            currentSpeed = Mathf.Lerp(currentSpeed, 0f, Time.deltaTime * 1f);                   // this will do for now.
            speedFactor = GetRatioForSpeed((int)(currentSpeed));                                // get the approximated speed factor
            speedFactor = Mathf.Clamp01(speedFactor);
        }
        if (!forward && !brake)
        {
            accelFactor -= Time.deltaTime / timeToFullAcceleration;                             // timeToFullAcceleration to loose acceleration. could be another number
            accelFactor = Mathf.Clamp01(accelFactor);
            currentSpeed = Mathf.Lerp(currentSpeed, 0f, Time.deltaTime / 4f);                   // this will do for now.
            speedFactor = GetRatioForSpeed((int)(currentSpeed));                                // get the approximated speed factor
            speedFactor = Mathf.Clamp01(speedFactor);
        }
        #endregion

        #region Turn control

        if (left)
        {
            currentTurnSpeed -= (currentTurnSpeed > 0) ?  turnSpeedFactor * turnOppositeMultiplier :  turnSpeedFactor;
            currentTurnSpeed = Mathf.Clamp(currentTurnSpeed, -maxTurnSpeed, maxTurnSpeed);
        }
        if (right)
        {
            currentTurnSpeed += (currentTurnSpeed < 0) ?  turnSpeedFactor * turnOppositeMultiplier :  turnSpeedFactor;
            currentTurnSpeed = Mathf.Clamp(currentTurnSpeed, -maxTurnSpeed, maxTurnSpeed);
        }
        if (!left && !right)
        {
            currentTurnSpeed *= 1f - Time.deltaTime * resetTurnFactor;                          // we gradually reduce turning speed;
        }

        #endregion

        transform.position += transform.forward * (currentSpeed * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + (currentTurnSpeed * Time.deltaTime), transform.eulerAngles.z);
    }


    #region Speed Approximation methods

    private float GetFactorForSpeed(int speed, ref float factor)
    {
        float speedFromCurve = speedCurve.Evaluate(factor) * maxSpeed;
        while (speed > speedFromCurve)
        {
            factor += speedCurveApproximationPrecision;
            speedFromCurve = speedCurve.Evaluate(factor) * maxSpeed;
        }
        return factor;
    }

    private float GetRatioForSpeed(int speed)
    {
        while (!speedCurveApproximation.ContainsKey(speed))
        {
            speed -= 1;
            if (speed == 0) return 0f;
        }
        //Debug.Log("returning the closest lower speed ratio : " + speed);
        return speedCurveApproximation[speed];
    }

    #endregion

    #region Public methods

    public void EnableControls()
    {
        controllable = true;
    }
    public void DisableControls()
    {
        controllable = false;
    }

    #endregion
}
