using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaMove : MonoBehaviour
{
    [SerializeField]
    CharacterController chari;
    [SerializeField]
    float speed=12;
    [SerializeField]
    Transform orientation;

    
    public float x, y;
    void FixedUpdate()
    {
        y=Input.GetAxisRaw("Vertical");
        Vector3 mov = transform.right*x + orientation.forward*y;
        chari.Move(mov*speed);

    }
}
