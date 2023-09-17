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
    public TMP_Text Position;
    int lapcounter = 0;
    [SerializeField]
    public GameManager gameManager;
    public GameObject main;
    public int why;
    
    bool dispval=false;
    void Start()
    {   
        if(!isLocalPlayer){return;}
        gameManager= GameObject.FindWithTag("GameController").GetComponent<GameManager>();
        LapCounter=GameObject.Find("Canvas/LapCounter").GetComponent<TextMeshProUGUI>();
        LapTimer=GameObject.Find("Canvas/LapTimer").GetComponent<TextMeshProUGUI>();
        Position=GameObject.Find("Canvas/Position").GetComponent<TextMeshProUGUI>(); 
        LapCounter.text="Lap:1/2";
        LapTimer.text="Lap 1: ?";
            }

    void Update()
    {   
        if(!isLocalPlayer){return;}

        if(dispval){
            why=gameManager.GetLap();
            Position.text=why.ToString();
        }
        if ( halfLapTriggerPassed && fullLapTriggerPassed )
        {
            if(lapcounter==0)
            {
                LapCounter.text="Lap:2/2";
                LapTimer.text="Lap 1:"+gameManager.GetTime().ToString("0.00")+" s";
            }
             else if(lapcounter==1)
            {
                LapCounter.text="Finished";
                LapTimer.text="Lap 2:"+gameManager.GetTime().ToString("0.00")+" s";
            }
            halfLapTriggerPassed=false;
            fullLapTriggerPassed=false;
            lapcounter++;
                if(lapcounter>1)
                {   
                    gameManager.Cmdposupdate();
                    dispval=true;
                }


                
            }

        }

    
}  