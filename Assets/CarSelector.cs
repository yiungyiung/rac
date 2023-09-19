using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CarSelector : NetworkBehaviour
{
    [SerializeField]
    public GameObject[] Cars;

    [SyncVar(hook = nameof(OnCarChanged))]
    public int currentCar;

    public override void OnStartClient()
    {
        base.OnStartClient();
        UpdateCar(currentCar);
    }

    public void NextCar()
    {
        if (isLocalPlayer)
        {
            currentCar++;
            currentCar = currentCar % Cars.Length;
            CmdChangeCar(currentCar);
        }
    }

    public void PrevCar()
    {
        if (isLocalPlayer)
        {
            currentCar--;
            if (currentCar < 0) currentCar = Cars.Length - 1;
            CmdChangeCar(currentCar);
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdChangeCar(int newCar)
    {
        Debug.Log("Server is changing the car");
        if (isServer)
            currentCar = newCar;
    }

    [ClientRpc]
    private void RpcUpdateCars(int newCar)
    {
        Debug.Log("Updating car on client");
        UpdateCar(newCar);
    }

    private void OnCarChanged(int oldCar, int newCar)
    {   
        Debug.Log("Entered OnCarChanged");
        RpcUpdateCars(newCar);
    }

    private void UpdateCar(int newCar)
    {
        for (int i = 0; i < Cars.Length; i++)
        {
            Cars[i].SetActive(i == newCar);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) { return; }
        if (Input.GetKeyDown(KeyCode.N)) NextCar();
        if (Input.GetKeyDown(KeyCode.P)) PrevCar();
    }
}
