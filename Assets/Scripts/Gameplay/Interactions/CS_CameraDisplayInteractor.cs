using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Whilefun.FPEKit;

public class CS_CameraDisplayInteractor : MonoBehaviour
{
    private bool bInDisplay = false;

    private Camera m_PlayerCamera = null;

    private GameObject m_Reticule = null;

    // Start is called before the first frame update
    void Start()
    {
        m_PlayerCamera = gameObject.GetComponent<Camera>();  
        
        if(m_PlayerCamera == null )
        {
            Debug.LogWarning("CS_CameraDisplayInteractor::Start --> Component is not attached to a gameobject with a Camera!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(bInDisplay)
        {
            RaycastHit OutHit;
            int Layer = 1 << LayerMask.NameToLayer("UI");
            if (Physics.Raycast(m_PlayerCamera.transform.position, m_PlayerCamera.transform.forward, out OutHit, 3.0f, ~LayerMask.NameToLayer("UI")))
            {
                Canvas canvas = OutHit.collider.GetComponent<Canvas>();
                if (canvas == null) 
                {
                    Debug.LogWarning("CS_CameraDisplayInteractor::Update --> Object tagged as UI has no Canvas component");
                    return;
                }

                CS_UI_DisplayController DisplayController = OutHit.collider.gameObject.GetComponent<CS_UI_DisplayController>();

                if (DisplayController == null)
                {
                    Debug.LogWarning("CS_CameraDisplayInteractor::Update --> Object tagged as UI has no DisplayController component");
                    return;
                }

                DisplayController.SetPointerLocation(OutHit.point);
                FindFirstObjectByType<CS_UI_HUDScript>().HideReticule();


                FPEInputManager InputManager = FPEInputManager.Instance;
                if (InputManager == null)
                {
                    Debug.LogWarning("CS_CameraDisplayInteractor::Update --> No InputManager could be found");
                    return;
                }
            }
            else
            {
                FindObjectOfType<CS_UI_HUDScript>().ShowReticule();
            }

        }
    }

    public void DockIn()
    {
        bInDisplay = true;
    }


    public void DockOut()
    {
        bInDisplay = false;
    }
}
