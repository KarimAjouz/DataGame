using System.Collections; using System.Collections.Generic; using UnityEngine; using UnityEngine.EventSystems;
using UnityEngine.UI; using UnityEngine.UIElements;
using Whilefun.FPEKit;


public class CS_UI_DisplayController : MonoBehaviour {     [SerializeField, Range(1, 5)]     private float ScrollFactor;      [SerializeField]     private CS_UI_MousePointer MousePointerObj;      //private CS_UI_MousePointer m_DisplayPointer;      Scrollbar m_Scrollbar;     EventSystem m_EventSystem;          private Canvas m_Canvas;          Vector3 LowerLeftRelativePos;     Vector3 UpperRightRelativePos;      // Start is called before the first frame update     void Start()     {         m_Scrollbar = GetComponentInChildren<Scrollbar>();         m_EventSystem = GetComponent<EventSystem>();                  m_Canvas = GetComponent<Canvas>();          Vector3[] OutCorners = new Vector3[4];         m_Canvas.GetComponent<RectTransform>().GetWorldCorners(OutCorners);          Transform ParentTransform = transform.parent.gameObject.transform;          LowerLeftRelativePos = transform.InverseTransformPoint(OutCorners[0]);         UpperRightRelativePos = transform.InverseTransformPoint(OutCorners[2]);          //m_DisplayPointer = GetComponentInChildren<CS_UI_MousePointer>();      }      void Update()
    {

    }
      public void UpScroll()     {         m_Scrollbar.value += 0.1f * ScrollFactor;     }     public void DownScroll()     {         m_Scrollbar.value -= 0.1f * ScrollFactor;     }      public void SetPointerLocation(Vector3 InPos)
    {
        Vector3 relativePos = transform.InverseTransformPoint(InPos);

        MousePointerObj.SetPointerPos(relativePos);
    }

    // c#
    float Remap(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    } 
} 