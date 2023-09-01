using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckScene : MonoBehaviour
{
    // Start is called before the first frame update
    private bool isloaded;
    private bool shouldload=true;
    void Start()
    {
       if(SceneManager.sceneCount>0)
       {
        for(int i=0; i<SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if(scene.name==gameObject.name)
            {
                isloaded = true;
                shouldload=false;
            }
        }
       } 
    }

    // Update is called once per frame
    

    void load(){
        if(!isloaded&&shouldload)
        {
            SceneManager.LoadSceneAsync(gameObject.name,LoadSceneMode.Additive);
            isloaded = true;
            shouldload=false;
        }
    }

    void unload(){

        if(isloaded&&!shouldload)
        {
        SceneManager.UnloadSceneAsync(gameObject.name);
        isloaded = false;
        shouldload=true;
        }

    }
     void OnTriggerEnter(Collider other) {
        
        if(other.tag=="pla")
        {
            load();
        }
    }

    void OnTriggerExit(Collider other) {
        
        if(other.tag=="pla")
        {
            unload();
        }
    }
}
