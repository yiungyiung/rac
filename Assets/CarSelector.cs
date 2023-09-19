using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSelector : MonoBehaviour
{
    [SerializeField]
    public GameObject[] Cars;

    public int currentCar;


    public void NextCar()
    {
        currentCar++;
        currentCar = currentCar % Cars.Length;
    }
    public void PrevCar()
    {
        currentCar--;
        if (currentCar < 0) currentCar = Cars.Length - 1;
    }
    

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.N) ) NextCar();
        if (Input.GetKeyDown(KeyCode.P)) PrevCar();

       for (int i = 0; i < Cars.Length; i++)
        {
            if (i == currentCar) Cars[i].SetActive(true); else Cars[i].SetActive(false);
        }
    }
}
