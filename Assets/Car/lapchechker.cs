using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lapchechker : MonoBehaviour
{   
    public enum Type
{
    half,
    full
}
public Type type;

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag=="Player")
        {
            if(type==Type.half)
            {
                other.gameObject.GetComponent<PlayerLapTracker>().halfLapTriggerPassed =true;
            }
            else if(type==Type.full && other.gameObject.GetComponent<PlayerLapTracker>().halfLapTriggerPassed)
            {
                other.gameObject.GetComponent<PlayerLapTracker>().fullLapTriggerPassed=true;
            }
        }


}
}
