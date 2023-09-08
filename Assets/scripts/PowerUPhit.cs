using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUPhit : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag=="Player")
            other.gameObject.GetComponent<carpower>().powerup(0);
            Destroy(gameObject);
    }

    
}
