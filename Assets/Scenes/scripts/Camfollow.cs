using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camfollow : MonoBehaviour
{
    [SerializeField]
    Transform camobject;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        camobject.position = transform.position;
    }
}
