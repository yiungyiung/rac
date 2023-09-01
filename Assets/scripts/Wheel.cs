using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//This component is responsible for visual and aural effects including
//1. Mesh Rotation
//2. Skid Sound
//3. Skid angle etc calculation

[RequireComponent(typeof (WheelCollider))]
public class Wheel : MonoBehaviour
{
    public Rigidbody body;
    public WheelCollider col;
    public float slipAngle;
    public Transform wheelMesh;
    public Vector3 meshOffset;
    public float ForwardSlip;
    private void Start()
    {
        body = GetComponentInParent<Rigidbody>();
        col = GetComponent<WheelCollider>();
    }
    void FixedUpdate()
    {
        col.ConfigureVehicleSubsteps(1, 50, 50);
        col.GetWorldPose(out Vector3 pos, out Quaternion rot);

        //set position
        if(wheelMesh)
            wheelMesh.SetPositionAndRotation(pos, rot);
        //get slip angle
        col.GetGroundHit(out WheelHit hit);
        Vector3 Vdir = body.GetPointVelocity(hit.point).normalized;
        Debug.DrawLine(hit.point, hit.point + Vdir, Color.red);
        Vector3 SteerDir = Vector3.Cross(rot * Vector3.right, col.transform.up);

        Debug.DrawLine(hit.point, hit.point + SteerDir, Color.green);
        slipAngle = Vector3.Angle(Vdir, SteerDir);

        ForwardSlip = hit.forwardSlip;
    }
}
