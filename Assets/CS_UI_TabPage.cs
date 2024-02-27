using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_UI_TabPage : MonoBehaviour
{
    [SerializeField]
    public string PageName = "NAME_None";

    [SerializeField]
    public Color PageColor = Color.blue;


    public  CS_UI_TabPage(string InName, Color InColor)
    {
        PageName = InName;
        PageColor = InColor;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

}
