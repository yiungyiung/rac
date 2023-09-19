using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carpower : MonoBehaviour
{   
    [SerializeField]
    public GameObject trails; 
    public void powerup(int index)
    {
        Debug.Log("Power up"+index);
        trails.GetComponent<Aids>().enabled = false;
    }


    void trailon(){
        Debug.Log("srt");
        Invoke("trailoff",10);
    }
    void trailoff(){
        Debug.Log("off");
    }
}
