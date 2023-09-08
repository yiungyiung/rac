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
        trailon();
    }


    void trailon(){
        trails.SetActive(true);
        Invoke("trailoff",10);
    }
    void trailoff(){
        trails.SetActive(false);
    }
}
