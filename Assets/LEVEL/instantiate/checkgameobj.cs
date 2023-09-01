using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkgameobj : MonoBehaviour
{
    public string tag;

    private bool isloaded;

    private bool shouldload = true;

    public GameObject prefabToInstantiate;

    void start()
    {
 tag=gameObject.name;
    }
    void update()
    {
        GameObject[] foundObjects = GameObject.FindGameObjectsWithTag(tag);
        if (foundObjects.Length > 1)
        {
            for (int i = 0; i < foundObjects.Length - 1; i++)
            {
                Destroy(foundObjects[i]);
            }
        }
    }

    void load()
    {
        if (!isloaded && shouldload)
        {
            Instantiate(prefabToInstantiate,
            new Vector3(0f, 0f, 0f),
            Quaternion.identity);
            isloaded = true;
            shouldload = false;
        }
    }

    void unload()
    {
        if (isloaded && !shouldload)
        {
            GameObject[] foundObjects = GameObject.FindGameObjectsWithTag(tag);
            if (foundObjects.Length > 0)
            {
                for (int i = 0; i < foundObjects.Length; i++)
                {
                    Destroy(foundObjects[i]);
                }
            }
            isloaded = false;
            shouldload = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "pla")
        {
            load();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "pla")
        {
            unload();
        }
    }
}
