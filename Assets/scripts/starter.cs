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
    public void startgame()
    {
        if(isServer)
        {
            rpcstart();
        }
    }

    [ClientRpc]
    public void rpcstart()
    {
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
