using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reset : MonoBehaviour
{
    public LayerMask ground;
    public float raycastDistance = 10;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        rb = GetComponent<Rigidbody>(); 


    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, raycastDistance, ground))
        {
            initialPosition= transform.position;
            initialRotation = transform.rotation;
        }
        else
        {
            ResetPosition();
        }

        void ResetPosition()
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            transform.position = initialPosition;
            transform.rotation = initialRotation;
        }


    }
}
