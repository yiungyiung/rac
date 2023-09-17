using UnityEngine;
using Mirror;
using TMPro;

public class GameManager : NetworkBehaviour
{
    [SyncVar]
    public float timer;

    public TMP_Text Text; // Reference to your TextMeshProUGUI component

    void Start()
    {
        if (isServer)
        {
            timer = 0f;
        }
    }

    void Update()
    {
        if (isServer)
        {
            timer += Time.deltaTime;
            RpcUpdateTimer(timer);
            VehiclePhysics[] allCars = FindObjectsOfType<VehiclePhysics>();
            Debug.Log("Number of cars in the scene: " + allCars.Length);
        }
    }

    [ClientRpc]
    void RpcUpdateTimer(float newTime)
    {
        timer = newTime;
        Text.text = timer.ToString("0.00"); // Update the TextMeshProUGUI text
    }

    public float GetTime()
    {
        return timer;
    }
}
