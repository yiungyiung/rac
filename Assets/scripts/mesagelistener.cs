using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mesagelistener : MonoBehaviour
{
    public string[] data;
    public PlayerMovement pla;
    public cammovement gy;
    float xer=-2.25f,yer=6.15f ;
    // Invoked when a line of data is received from the serial device.
    void OnMessageArrived(string msg)
    {   
        //Debug.Log(msg);
        string[] data =msg.Split(',');
        if(data.Length<4){
            return;
        }
       
        float gyrox=(float.Parse(data[3])-0.06f);
        float gyroz=(float.Parse(data[4])+0.01f);
        //Debug.Log(gyrox+"|"+gyroz);
        if(data[2]=="0")
        {
        gy.gx=gyrox;
        gy.gz=gyroz;
        pla.hori=0;
        pla.verti=0;
        }
        else{
            gy.gx=0f;
            gy.gz=0f;
        pla.hori=-((int)((float.Parse(data[0])-xer)));
        pla.verti=-((int)((float.Parse(data[1])-yer))*2f);
        Debug.Log(pla.hori+"||"+pla.verti);
        }

    }

    void OnConnectionEvent(bool success)
    {
        if (success)
            Debug.Log("Connection established");
        else
            Debug.Log("Connection attempt failed or disconnection detected");
    }
}