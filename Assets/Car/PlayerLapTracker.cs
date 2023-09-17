using UnityEngine;
using Mirror;
using TMPro;
public class PlayerLapTracker : NetworkBehaviour
{   
    public bool halfLapTriggerPassed = false;
    public bool fullLapTriggerPassed = false;
    [SerializeField]
    public TMP_Text LapCounter;
    [SerializeField]
    public TMP_Text LapTimer;
    int lapcounter = 0;
    [SerializeField]
    public GameManager gameManager;

    void Start()
    {   
        if(!isLocalPlayer){return;}
        gameManager= GameObject.FindWithTag("GameController").GetComponent<GameManager>();
        LapCounter=GameObject.Find("Canvas/LapCounter").GetComponent<TextMeshProUGUI>();
        LapTimer=GameObject.Find("Canvas/LapTimer").GetComponent<TextMeshProUGUI>();;    
        LapCounter.text="Lap:1/2";
            }

    void Update()
    {   
        if(!isLocalPlayer){return;}
        if ( halfLapTriggerPassed && fullLapTriggerPassed )
        {
            if(lapcounter==0)
            {
                LapCounter.text="Lap:2/2";
                LapTimer.text="Lap 1:"+gameManager.GetTime()+" s";
            }
             else if(lapcounter==1)
            {
                LapCounter.text="Finished";
                LapTimer.text="Lap 2:"+gameManager.GetTime()+" s";
            }
            halfLapTriggerPassed=false;
            fullLapTriggerPassed=false;
            lapcounter++;
            
        }
    }

}  