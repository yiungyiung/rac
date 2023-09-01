using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    Rigidbody rb;
    [SerializeField]
    float speed;
    [SerializeField]
    Transform orientation;
    Vector3 movedir;
    public float hori,verti;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        hori=Input.GetAxisRaw("Horizontal");
        verti=Input.GetAxisRaw("Vertical");
        move();
        Vector3 vel=new Vector3(rb.velocity.x,0, rb.velocity.z);
        if (vel.magnitude>speed)
        {
            Vector3 newVel=vel.normalized*speed;
            rb.velocity=new Vector3(newVel.x,rb.velocity.y,newVel.z);
        }
    }

    private void move()
    {
        movedir=orientation.forward*verti+orientation.right*hori;
        rb.AddForce(movedir.normalized*speed*10f,ForceMode.Force);
        
    }
}
