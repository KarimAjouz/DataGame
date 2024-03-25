using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CS_UI_HUDScript : MonoBehaviour
{
    [SerializeField]
    private GameObject Reticule;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void HideReticule()
    {
        Reticule.SetActive(false);
    }

    public void ShowReticule()
    {
        Reticule.SetActive(true);
    }
}
