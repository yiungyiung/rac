using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(VehiclePhysics))]
public class Aids : MonoBehaviour
{
   VehiclePhysics car;

    //Wheels of the car Front left, front right, back left, back right
    [Header("Wheels")]
    public Wheel FLWheel;
    public Wheel FRWheel;  
    public Wheel BLWheel;
    public Wheel BRWheel;

    [Header("Speed sensitive steering")]
    //steering sensitivity reduces with increased speed
    public float speedSensitivity = 0.1f;


    [Header("Gearshift parameters")]
    [Tooltip("RPM at which gear shifts up")]
    public float UpShiftRPM = 6000f;
    [Tooltip("RPM at which gear shifts down")]
    public float DownShiftRPM = 4000f;
    [Tooltip("Beyond this level of rear wheel traction loss, gears wont shift up.")]
    public float shiftMaxSkid = 0.2f;
    //Time paused after every gear shift decision
    float ShiftTime = 0.25f;
    float shiftTimer;
    //********************************

    //Is currently reverse gear?
    bool isReverse = false;

    [Header("Drift parameters")]
    [Tooltip("Maximum sideways friction")]
    public float rearIdealFriction = 2;

    [Tooltip("Minimum sideways friction.")]
    public float rearMinIdealFriction = 1.25f;

    [Tooltip("Higher the value, lesser the drift.")]
    public float driftFactor = 1.0f;

    private void Start()
    {
        car = GetComponent<VehiclePhysics>();
    }
    void FixedUpdate()
    {

        //Take raw inputs from the user. Brake to reverse switching also done here
        float rawThrottleInput = Mathf.Clamp01((isReverse?-1:1)* Input.GetAxis("Vertical"));
        float rawSteerInput = Input.GetAxis("Horizontal");
        float rawBrakeInput =Mathf.Clamp01((isReverse ? -1 : 1) * -Input.GetAxis("Vertical"));

        //Speed in meters per second
        float speed = car.GetComponent<Rigidbody>().velocity.magnitude;

       
        //Speed sensitive steer
        float steerInput = rawSteerInput/Mathf.Max(speed * speedSensitivity,1) ;

        //skid sensitive steer. Prevent over steer
        float flSlip = FLWheel.slipAngle;
        float frSlip = FRWheel.slipAngle;
        float blSlip = BLWheel.slipAngle;
        float brSlip = BRWheel.slipAngle;


        //DRIFT FAKER HAAAHAHAHA
        //based on the average slip angle difference between front and back, adjust rear wheel sideways friction value
        float frontAvg = (flSlip + frSlip) / 2;
        float rearAvg = (blSlip + brSlip) / 2;
    
        //calculate amount of friction to apply based on difference between front and rear slip
        float friction = Mathf.Clamp( (rearAvg - frontAvg) * driftFactor, rearMinIdealFriction, rearIdealFriction);
       
        //apply the frictions
        var blCurve = BLWheel.col.sidewaysFriction;
        blCurve.stiffness = friction;
        BLWheel.col.sidewaysFriction = blCurve;


        var brCurve = BRWheel.col.sidewaysFriction;
        brCurve.stiffness = friction;
        BRWheel.col.sidewaysFriction = brCurve;
        //***********************************************************************************


        //Autogearshifting
        float RPM = car.EngineRPM;
        float fwdSkid = BLWheel.ForwardSlip;
        shiftTimer -= Time.deltaTime;
        //brake to reverse stuff
        if (rawBrakeInput > 0.5f && speed < 1)
        {
            isReverse = !isReverse;
            if (isReverse) { car.Reverse(); } else { car.ShiftDrive(); }
        }
        //Automatic shifting
        if (shiftTimer< -1) shiftTimer= -1;
        if (shiftTimer <= 0 && fwdSkid < shiftMaxSkid && !isReverse)
        {
            if (RPM > UpShiftRPM)
            {
                car.ShiftUp();
                shiftTimer = ShiftTime;
            }
            else if (RPM < DownShiftRPM)
            {
                car.ShiftDown();
                shiftTimer = ShiftTime;
            }
        }
        //***********************************************************************

        //Final outputs
        car.SteerInput = steerInput;
        car.Throttle = rawThrottleInput;
        car.brakeInput= rawBrakeInput;
    }
}
