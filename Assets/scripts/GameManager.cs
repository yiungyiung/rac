using UnityEngine;
using Mirror;
using System;
using TMPro;

public class GameManager : NetworkBehaviour
{
    [SyncVar]
    public float timer;
    public TMP_Text Position;
    public int dispPosition=0;
    [SyncVar]
    public int finishPosition = 0;
    uint calledid;
private bool hasUpdated = false;
    

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
        }
    }

    [Command(requiresAuthority = false)]
    public void Cmdposupdate(uint netid)
    {
        finishPosition=finishPosition+1;
        calledid=netid;
        Rpcupdatepos(finishPosition,netid);
    }
    
    [ClientRpc]
    public void Rpcupdatepos(int pos,uint netid){
        finishPosition=pos;
        calledid=netid; 
    }
    [ClientRpc]
    void RpcUpdateTimer(float newTime)
    {
        timer = newTime;
        Text.text = timer.ToString("0.00"); // Update the TextMeshProUGUI text
    }

    public int GetLap()
    {
        return finishPosition;
    }
    public uint GetID()
    {
        return calledid;
    }
    public float GetTime()
    {
        return timer;
    }
}
