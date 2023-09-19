using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    Camera cam;
    public Transform target;
    public float accelSensitivity = -0.05f;
    public float sideSensitivity = 0.05f;
    public VehiclePhysics car;
    public Vector3 offset;
    public float fovMultiplier;
    public float baseFOV = 60f;
    public float camStiffness = 10f;
    public float maxAccel;
    public float shakeMultiplier;
    Vector3 finalPosition;
    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    Vector3 RandomVector()
    {
        return new Vector3(UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f));
    }
    void FixedUpdate()
    {
        //something cool

        //get jerk
        if(target!=null)
        {
        Vector3 jerk = car.jerk;

        //get the gforces
        Vector3 gForces =Vector3.ClampMagnitude( car.gForces, maxAccel);
        float forwardGForces = gForces.z;
        float sideGForces = gForces.x;
        Vector3 basePosition = target.position + target.TransformDirection(offset + forwardGForces * accelSensitivity * Vector3.forward ) + 
                                                target.TransformDirection(offset + sideSensitivity * sideGForces * Vector3.right);
        
        finalPosition =Vector3.Lerp(finalPosition, basePosition + RandomVector() * shakeMultiplier * jerk.magnitude, Time.deltaTime * camStiffness);
        transform.position = finalPosition;

        transform.LookAt(target.position);

        cam.fieldOfView =Mathf.Lerp(cam.fieldOfView, baseFOV + fovMultiplier * forwardGForces,Time.deltaTime * 10);

        }

    }
    public void setTarget(GameObject targett)
     {
         target = targett.transform;
         car=target.GetComponent<VehiclePhysics>();

     }
}
