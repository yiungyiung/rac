using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class starter : NetworkBehaviour
{   [SerializeField]
    GameObject button;
    [SyncVar]
    public List <uint> players;
    public TMP_Text playersjoined;
    [SyncVar]
    public bool canStart;
    [SyncVar]
    public float startTime=3.5f;
    public TMP_Text StartText;
    public bool canStarted=false;
    public void Update()
    {
        if(isServer)
        {   
            if(canStarted){
            startTime=startTime-Time.deltaTime;
            if(startTime<=0)
            {
                rpcstart();
            }
            else if(startTime<0.2)
            {
                rpcprint("GO!");
            }
            else{
                rpcprint(startTime.ToString("0"));
            }
            }
        }
    }
    public void startgame()
    {
        if(isServer)
        {
            canStarted=true;
        }
    }

    [ClientRpc]
    public void rpcprint(string message)
    {
        StartText.text=message;
    }
    [ClientRpc]
    public void rpcstart()
    {   
        StartText.text="";
        gameObject.GetComponent<GameManager>().enabled = true;
        button.SetActive(false);
        canStart=true;
    }

    [Command(requiresAuthority = false)]
    public void Cmdupdatelist(uint netid)
    {   
       
        Rpcupdatelist(netid);
    }
    
    [ClientRpc]
    public void Rpcupdatelist(uint netid){
         if(!(players.Contains(netid))){
         players.Add(netid);
         playersjoined.text="Players: "+(players.Count).ToString()+"/4";}
    }
}
