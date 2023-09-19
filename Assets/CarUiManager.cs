using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

[RequireComponent(typeof(VehiclePhysics))]
public class CarUiManager : NetworkBehaviour
{
    // Start is called before the first frame update

    Transform needleRot;
    public float needleRotMult;
    public float needleRotZero;
    Text gearLabel;
    float rpm;
    int currentGear;
    VehiclePhysics car;
    void Start()
    {
        if (!isLocalPlayer) return;

        needleRot = GameObject.Find("Canvas/Tachometer/Needle").transform;
        gearLabel = GameObject.Find("Canvas/Tachometer/Gear").GetComponent<Text>();
        car = GetComponent<VehiclePhysics>();

    }

    // Update is called once per frame
    void Update()
    {
        if(!isLocalPlayer) return;

        if(!car) car = GetComponent<VehiclePhysics>();
        if(!needleRot) needleRot = GameObject.Find("Canvas/Tachometer/Needle").transform;
        if(!gearLabel) gearLabel = GameObject.Find("Canvas/Tachometer/Gear").GetComponent<Text>();
        rpm = car.EngineRPM;
        currentGear = car.currentGear;
        needleRot.localEulerAngles = new Vector3(0, 0, needleRotZero + needleRotMult * rpm);
        gearLabel.text = currentGear == -1?"R": currentGear.ToString();
    }
}
