using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowRotationScript : MonoBehaviour
{
    void Update()
    {
        Locations loc = GameObject.Find("NavigationNodes").GetComponent<Locations>();
        this.transform.eulerAngles = new Vector3(0, loc.GetArrowRot(), 0);
    }
}