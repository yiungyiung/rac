using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class VehiclePhysics : NetworkBehaviour
{
    [HideInInspector]
    public Rigidbody body;

    [Header("Wheel Colliders")]
    public WheelCollider FLCol;
    public WheelCollider BLCol;
    public WheelCollider FRCol;
    public WheelCollider BRCol;
    [Space]

    [HideInInspector]
    public Vector3 gForces;
    [HideInInspector]
    public Vector3 jerk;


    Vector3 lastVel;
    Vector3 lastGForces;
    Vector3 currVel;

    [HideInInspector]
    public float FRSlipAngle;
    [HideInInspector]
    public float FLSlipAngle;

    [Header("Engine Sound Parameters")]
    public AudioSource engineSound;
    public float pitchX;

    [Header("Gearbox Parameters")]
    public int currentGear;
    public float finalGearRatio = 3.0f;
 
    public float[] gearRatios = {3.0f, 3.0f, 2.5f, 2.2f, 1.8f, 1.2f,0.8f};

    [Header("Engine Performance")]
    public float maxTorque = 300;
    public float EngineSmoothness = 10;

    [HideInInspector]
    public float EngineRPM;
    public float IdleRPM = 1000f;
    public float MaxRPM = 7000f;

    [Header("Differential Parameters")]
    public float DifferentialFactor;
    public float maxDifferentialTorque;

    [Header("Braking Parameters")]
    public float brakeTorque;
    
    [HideInInspector] 
    public float Throttle;
    public float MaxSteer;
    [HideInInspector] 
    public float SteerInput;
    [HideInInspector] 
    public float brakeInput;
    [HideInInspector]
    public float handbrakeInput;

    [Header("Center of mass")]
    public Transform COM;

    public override void OnStartLocalPlayer()
     {  
         Camera.main.GetComponent<CameraFollow>().setTarget(gameObject);
      }
    private void Start()
    {   if(!isLocalPlayer){
            engineSound.gameObject.SetActive(false);
            this.enabled = false;
            return;}
        body= GetComponent<Rigidbody>();
       
    }


    //gear shift up
    public void ShiftUp()
    {
        currentGear++;
        currentGear = Mathf.Clamp(currentGear,1,gearRatios.Length-1);
    }

    //gear shift down
    public void ShiftDown()
    {
        currentGear--;
        currentGear = Mathf.Clamp(currentGear, 1, gearRatios.Length-1);
    }

    //gear reverse
    public void Reverse()
    {
        currentGear = -1;
    }

    //gear drive
    public void ShiftDrive()
    {
        currentGear = 1;
    }


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
    {   if(!isLocalPlayer){return;}


        if (!FLCol || !FLCol.gameObject.activeInHierarchy) FLCol = FindRecursively(transform,"FLCol").GetComponent<WheelCollider>();
        if (!BLCol || !BLCol.gameObject.activeInHierarchy) BLCol = FindRecursively(transform, "BLCol").GetComponent<WheelCollider>();
        if (!FRCol || !FRCol.gameObject.activeInHierarchy) FRCol = FindRecursively(transform, "FRCol").GetComponent<WheelCollider>();
        if (!BRCol || !BRCol.gameObject.activeInHierarchy) BRCol = FindRecursively(transform, "BRCol").GetComponent<WheelCollider>();

        //update the center of mass
        if (COM != null)
        {
            body.centerOfMass = transform.InverseTransformPoint(COM.position);
        }

        //Steer angle
        FLCol.steerAngle= SteerInput * MaxSteer;
        FRCol.steerAngle = SteerInput * MaxSteer;

        //get rear rpm
        float BLRPM = BLCol.rpm;
        float BRRPM = BRCol.rpm;

        //The net axle rpm is the average rpm of both wheels [dIfFeReNtIaL BeHaViOuR]
        float RearSetRPM = (BLRPM + BRRPM)/2;

        //Force
        //To simulate LSD (not the drug, the limited slip differential) more force is applied to the slower spinning wheel
        //so find difference first
        float difference =  BRRPM - BLRPM;
        //now calculate the differential torques [i m not mechanical engineer btw]
        float differentialTorque = Mathf.Clamp(difference * DifferentialFactor, -maxDifferentialTorque, maxDifferentialTorque);

        //Now find engine torque

        //current gear ratio
        float currGearRatio = (currentGear == -1 ? -gearRatios[0] : gearRatios[currentGear]);
       

        //Calculate engine RPM from wheel speed
        EngineRPM =Mathf.Lerp(EngineRPM, Math.Max(IdleRPM, RearSetRPM * finalGearRatio * currGearRatio), Time.deltaTime * EngineSmoothness);
        //now set engine sound
        engineSound.pitch = EngineRPM * pitchX;

        //Calculate final torque
        float Torque = currGearRatio * finalGearRatio * Throttle * (EngineRPM < MaxRPM?maxTorque:0);

        //left wheel and right wheel torque
        float lTorque = Throttle *  Torque + differentialTorque;
        float rTorque = Throttle * Torque - differentialTorque;

        //finallyyyyyyyyyyyy apply the torque
        BLCol.motorTorque = lTorque;
        BRCol.motorTorque = rTorque;

        //brake.... normal stuff
        BLCol.brakeTorque = Mathf.Clamp01(brakeInput + handbrakeInput) * brakeTorque;
        BRCol.brakeTorque = Mathf.Clamp01(brakeInput + handbrakeInput) * brakeTorque;
        FLCol.brakeTorque = brakeInput * brakeTorque;
        FRCol.brakeTorque = brakeInput * brakeTorque;

        //getGForces... for camera movements
        currVel= body.velocity;
        gForces =transform.InverseTransformVector((currVel - lastVel)/Time.deltaTime);
        lastVel = currVel;

        //jerk (leave this, just timepass)
        jerk = (gForces - lastGForces)/Time.deltaTime;
        lastGForces= gForces;
    }
}
