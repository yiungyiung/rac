using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cammovement : MonoBehaviour
{   
    [SerializeField]
    float sensx;
    [SerializeField]
    float sensy;
    [SerializeField]
    string m1;
    [SerializeField]
    string m2;
    public Transform body;
    float xrot=0,yrot=0;
    public float gz, gx;
    void Start()
    {
        //Cursor.lockState=CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
    float mousex=SimpleInput.GetAxisRaw(m1)*Time.deltaTime*sensx*10;
     float mousey=SimpleInput.GetAxisRaw(m2)*Time.deltaTime*sensy*10;
    
     xrot-=mousey;
     xrot=Mathf.Clamp(xrot,-90f,90f);

    transform.localRotation=Quaternion.Euler(xrot, 0,0);
    body.Rotate(Vector3.up*mousex);
    

    }
}
