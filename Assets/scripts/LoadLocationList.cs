using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadLocationList : MonoBehaviour
{
    private TMP_Dropdown mText;
    private GameObject[] locations;
    private List<string> locationsTexts;
    
    public GameObject locationsObj;
    public GameObject srcDropDownObj;
    public GameObject dstDropDownObj;

    private string lastSrc = "";


    void Start()
    {
        locations = GameObject.FindGameObjectsWithTag("Navigate");
        locationsTexts = new List<string>();

        if (gameObject.name == "Source")
        {
            locationsTexts.Add("Player Location");
        }

        foreach (GameObject g in locations)
        {
            locationsTexts.Add(g.name);
        }

        mText = gameObject.GetComponent<TMP_Dropdown>();
        mText.ClearOptions();
        mText.AddOptions(locationsTexts);
    }

    public string GetCurrentSelected()
    {
        return locationsTexts[mText.value];
    }

    void SelectionUpdated()
    {
        string src = srcDropDownObj.GetComponent<LoadLocationList>().GetCurrentSelected();
        string dst = dstDropDownObj.GetComponent<LoadLocationList>().GetCurrentSelected();
        locationsObj.GetComponent<Locations>().CreatePath(src, dst);

        if (lastSrc != src)
        {
            locationsObj.GetComponent<Locations>().TeleportPlayerTo(src);
            lastSrc = src;
        }
    }
}
