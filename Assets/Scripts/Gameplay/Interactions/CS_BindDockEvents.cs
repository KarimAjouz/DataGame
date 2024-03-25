using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

using Whilefun.FPEKit;

public class CS_BindDockEvents : MonoBehaviour
{
    private FPEInteractionManagerScript InteractionScript;
    private GameObject PlayerObject;
    private Camera PlayerCamera;

    [SerializeField]
    private Canvas DisplayCanvas;

    public float DockedFOV = 45;
    private float DefaultFOV = 60;

    public void AssignCam(GameObject InObj)
    {
        string ObjectName = InObj.name + "(Clone)";

        string PlayerObjName = "FPEPlayerController";
        PlayerObject = GameObject.Find(PlayerObjName);

        if (PlayerObject == null)
        {
            Debug.Log("ERROR: CS_BindDockEvents::AssignCam --> PlayerObject could not be initialised!");
            return;
        }

        InteractionScript = GameObject.Find(ObjectName).GetComponent<FPEInteractionManagerScript>();

        if (InteractionScript == null)
        {
            Debug.Log("ERROR: CS_BindDockEvents::AssignCam --> InteractionScript could not be initialised!");
            return;
        }

        PlayerCamera = PlayerObject.GetComponentInChildren<Camera>();

        if (PlayerCamera == null)
        {
            Debug.Log("ERROR: CS_BindDockEvents::AssignCam --> PlayerCam could not be initialised!");
            return;
        }
    }

    public void OnDockIn()
    {
        InteractionScript.zoomedFOV = DockedFOV;
        InteractionScript.b_Zoomed = true;

        //DisplayCanvas.worldCamera = PlayerCamera;

        Cursor.visible = true;

        gameObject.GetComponent<CS_PC_Controller>().bDocked = true;
    }

    public void OnDockOut()
    {
        //InteractionScript.zoomedFOV = DefaultFOV;
        InteractionScript.b_Zoomed = false;

        //DisplayCanvas.worldCamera = null;
        Cursor.visible = false;

        gameObject.GetComponent<CS_PC_Controller>().bDocked = false;
    }
}
