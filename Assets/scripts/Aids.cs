using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
[RequireComponent(typeof(VehiclePhysics))]
public class Aids : NetworkBehaviour
{   
   [SerializeField]
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


    [Header("Rollover Protection")]
    public float pitchCorrection;
    public float rollCorrection;

    Transform FindRecursively(Transform parent, String name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name && child.gameObject.activeInHierarchy)
            {
                return child; // Found the child GameObject
            }

            Transform result = FindRecursively(child, name);
            if (result != null)
            {
                return result; // Found the child GameObject in the descendant hierarchy
            }
        }

        return null; // Child not found
    }

    void FixedUpdate()
    {
        if(!isLocalPlayer){return;}

        if (!FLWheel || !FLWheel.gameObject.activeInHierarchy) FLWheel = FindRecursively(transform, "FLCol").GetComponent<Wheel>();
        if (!BLWheel || !BLWheel.gameObject.activeInHierarchy) BLWheel = FindRecursively(transform, "BLCol").GetComponent<Wheel>();
        if (!FRWheel || !FRWheel.gameObject.activeInHierarchy) FRWheel = FindRecursively(transform, "FRCol").GetComponent<Wheel>();
        if (!BRWheel || !BRWheel.gameObject.activeInHierarchy) BRWheel = FindRecursively(transform, "BRCol").GetComponent<Wheel>();


        //Take raw inputs from the user. Brake to reverse switching also done here
        float rawThrottleInput = Mathf.Clamp01((isReverse?-1:1)* SimpleInput.GetAxis("Vertical"));
        float rawSteerInput = SimpleInput.GetAxis("Horizontal");
        float rawBrakeInput =Mathf.Clamp01((isReverse ? -1 : 1) * -SimpleInput.GetAxis("Vertical"));
        float handbrakeInput = SimpleInput.GetKey(KeyCode.Space)?1:0;
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
        car.brakeInput= rawBrakeInput+handbrakeInput;
        car.handbrakeInput = handbrakeInput;

        //apply forces for roll over correction

        float rollTorque =Vector3.SignedAngle( Vector3.up, car.transform.up,Vector3.ProjectOnPlane( car.transform.forward,Vector3.up)) * rollCorrection;
        float pitchTorque = Vector3.SignedAngle(Vector3.up, car.transform.up, Vector3.ProjectOnPlane(car.transform.right, Vector3.up)) * pitchCorrection;


        car.body.AddRelativeTorque(pitchTorque, 0, rollTorque);
    }
}
