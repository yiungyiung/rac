using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaMove : MonoBehaviour
{
    [SerializeField]
    public CharacterController chari;
    [SerializeField]
    float speed=12;
    [SerializeField]
    Transform orientation;

    public float x, y;

    void FixedUpdate()
    {
        float x = SimpleInput.GetAxisRaw("Horizontal");
        float y = SimpleInput.GetAxisRaw("Vertical");

        Vector3 mov = transform.right * x + orientation.forward * y;
        mov.y = 0;
        transform.position = new Vector3(transform.position.x,43.1f ,transform.position.z);
        chari.Move(mov * speed );
    }

}
