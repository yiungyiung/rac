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

    
    float x, y;
    void FixedUpdate()
    {
        x=SimpleInput.GetAxis("Horizontal");
        y=SimpleInput.GetAxis("Vertical");
        Vector3 mov = -transform.right*x + -transform.forward*y;
        chari.Move(mov*speed);

    }
}
