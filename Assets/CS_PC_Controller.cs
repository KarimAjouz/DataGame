using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Whilefun.FPEKit;

public class CS_PC_Controller : MonoBehaviour
{
    public bool bDocked = false;


    [SerializeField]
    private CS_UI_DisplayController m_DisplayController;

    // Start is called before the first frame update
    void Start()
    {
        if(m_DisplayController == null)
        {
            Debug.LogWarning("CS_PC_Controller::Start() --> m_DisplayController is unassigned!");
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(FPEInputManager.Instance.GetAxis(FPEInputManager.eFPEInput.KAZ_INPUT_SCROLLWHEEL) > 0.0f && bDocked)
        {
            UpScroll();
        }

        if (FPEInputManager.Instance.GetAxis(FPEInputManager.eFPEInput.KAZ_INPUT_SCROLLWHEEL) < 0.0f && bDocked)
        {
            DownScroll();
        }
    }

    public void UpScroll()
    {
        m_DisplayController.UpScroll();
    }

    public void DownScroll()
    {
        m_DisplayController.DownScroll();
    }
}
