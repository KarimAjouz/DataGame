using System.Collections; using System.Collections.Generic; using UnityEngine; using UnityEngine.EventSystems;
using UnityEngine.UI; using UnityEngine.UIElements;
using Whilefun.FPEKit;

public class CS_UI_DisplayController : MonoBehaviour {     [SerializeField, Range(1, 5)]     private float ScrollFactor;      [SerializeField]     private GameObject MousePointerObj;      //private CS_UI_MousePointer m_DisplayPointer;      private PointerEventData m_PointerEventData;      private GraphicRaycaster canvasRaycaster;
    private List<RaycastResult> list;      Scrollbar m_Scrollbar;     EventSystem m_EventSystem;          private Canvas m_Canvas;          Vector3 LowerLeftRelativePos;     Vector3 UpperRightRelativePos;     // Start is called before the first frame update     void Start()     {         m_Scrollbar = GetComponentInChildren<Scrollbar>();         m_EventSystem = GetComponent<EventSystem>();                  m_Canvas = GetComponent<Canvas>();          Vector3[] OutCorners = new Vector3[4];         m_Canvas.GetComponent<RectTransform>().GetWorldCorners(OutCorners);          Transform ParentTransform = transform.parent.gameObject.transform;         canvasRaycaster = GetComponent<GraphicRaycaster>();          list = new List<RaycastResult>();          LowerLeftRelativePos = transform.InverseTransformPoint(OutCorners[0]);         UpperRightRelativePos = transform.InverseTransformPoint(OutCorners[2]);          //m_DisplayPointer = GetComponentInChildren<CS_UI_MousePointer>();      }      public void UpScroll()     {         m_Scrollbar.value += 0.1f * ScrollFactor;     }     public void DownScroll()     {         m_Scrollbar.value -= 0.1f * ScrollFactor;     }      public void SetPointerLocation(Vector3 InPos)
    {
        Vector3 relativePos = transform.InverseTransformPoint(InPos);


        //float MappedPositionX = Remap(relativePos.x, LowerLeftRelativePos.x, UpperRightRelativePos.x, -210f, 205f);
        //float MappedPositionY = Remap(relativePos.y, LowerLeftRelativePos.y, UpperRightRelativePos.y, -235f, 240f);
        //float MappedPositionZ = Remap(relativePos.z, LowerLeftRelativePos.z, UpperRightRelativePos.z, 0, 0);

        //Vector3 MappedPos = new Vector3(MappedPositionX, MappedPositionY, MappedPositionZ);

        //Debug.Log("InputPos: " + InPos + "\n RelativePos: " + relativePos + " MappedPos: " + MappedPos);

        m_PointerEventData = new PointerEventData(m_EventSystem);
        m_PointerEventData.position = Input.mousePosition;

        MousePointerObj.GetComponent<RectTransform>().localPosition = relativePos;
    }

    // c#
    float Remap(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }

    public void SetPointerEventType(FPEInputManager.eFPEInput Input, bool bIsDown)
    {
        canvasRaycaster.Raycast(m_PointerEventData, list);
        foreach (RaycastResult hit in list)
        {
            if (Input == FPEInputManager.eFPEInput.FPE_INPUT_INTERACT)
            {
                CS_UI_TabButton button = hit.gameObject.GetComponent<CS_UI_TabButton>();
                if (button != null)
                {
                    if (bIsDown)
                    {
                        button.GetComponent<UnityEngine.UI.Button>().OnPointerDown(m_PointerEventData);
                    }
                    else
                    {
                        button.GetComponent<UnityEngine.UI.Button>().OnPointerUp(m_PointerEventData);
                    }
                }
            }
            else if (Input == FPEInputManager.eFPEInput.FPE_INPUT_EXAMINE)
            {
                if (bIsDown)
                {

                }
                else
                {

                }
            }
        }

        //Debug.Log("PointerEventFPEInput: " + Input);
        //Debug.Log(m_PointerEventData);
    } 
} 