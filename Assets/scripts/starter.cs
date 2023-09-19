using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class starter : NetworkBehaviour
{   

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
    }
}
