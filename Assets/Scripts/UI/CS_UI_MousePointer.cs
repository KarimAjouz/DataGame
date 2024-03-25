using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Whilefun.FPEKit;

public enum EMousePointerEventType
{
    PtrEventType_NONE,
    PtrEventType_CLICKED,
    PtrEventType_PRESSED,
    PtrEventType_RELEASED,
    PtrEventType_DRAGGED,
    PtrEventType_DRAGGING
}


public struct FMousePointerData
{
    public Vector3 Position;
    public Vector3 PreviousPosition;
    public Vector3 PressedPosition;
    public float PositionFrameDeltaSquared;
    public float PressedCumulativeFrameDeltaSquared;
    public float DraggedDeltaThreshold;

    public float PressedTime;
    public float ClickedReleaseThreshold;
    public float DragDeltaThreshold;

    public bool bIsPressed;

    public EMousePointerEventType EventType;

    public void UpdatePosition(Vector3 InPosition)
    {
        PreviousPosition = Position;
        Position = InPosition;

        PositionFrameDeltaSquared = (Position - PreviousPosition).sqrMagnitude;

        if (bIsPressed)
        {
            PressedCumulativeFrameDeltaSquared += PositionFrameDeltaSquared;
            PressedTime += Time.deltaTime;
            if (PressedTime > ClickedReleaseThreshold)
            {
                if (PositionFrameDeltaSquared > DragDeltaThreshold)
                    EventType = EMousePointerEventType.PtrEventType_DRAGGING;
                else if (PressedCumulativeFrameDeltaSquared != 0.0f)
                    EventType = EMousePointerEventType.PtrEventType_DRAGGED;
            }
        }
        else
        {
            EventType = EMousePointerEventType.PtrEventType_NONE;
        }
    }

    public void OnPressed(List<IPointerClickHandler> ClickHandlers, PointerEventData InPointerEventData)
    {
        PressedPosition = Position;
        PressedTime = 0.0f;
        bIsPressed = true;
        PressedCumulativeFrameDeltaSquared = 0;

        EventType = EMousePointerEventType.PtrEventType_PRESSED;
    }

    public void OnReleased(List<IPointerClickHandler> ClickHandlers, PointerEventData InPointerEventData)
    {
        bIsPressed = false;

        if (PressedTime > ClickedReleaseThreshold)
        {
            EventType = EMousePointerEventType.PtrEventType_RELEASED;
        }
        else if (PressedTime <= ClickedReleaseThreshold)
        {
            EventType = EMousePointerEventType.PtrEventType_CLICKED;
            foreach (IPointerClickHandler ClickHandler in ClickHandlers)
            {
                ClickHandler.OnPointerClick(InPointerEventData);
            }
        }
    }

}

public class CS_UI_MousePointer : MonoBehaviour
{
    List<IPointerClickHandler> m_ClickHandlerList;
    List<Selectable> m_SelectableList;
    
    private PointerEventData m_PointerEventData;     EventSystem m_EventSystem; 
    FMousePointerData m_PointerData;     RectTransform m_Transform;

    // Start is called before the first frame update
    void Start()
    {
        m_EventSystem = GetComponent<EventSystem>();
    
        InitialiseMousePointerData();

        m_ClickHandlerList = new List<IPointerClickHandler>();
        m_SelectableList = new List<Selectable>();

        m_PointerEventData = new PointerEventData(m_EventSystem);
    }

    // Update is called once per frame
    void Update()
    {
        HandleFPEInputs();
    }

    void InitialiseMousePointerData()
    {
        m_PointerData.Position = Input.mousePosition;

        m_PointerData.DraggedDeltaThreshold = 0.1f;

        m_PointerData.PressedTime = 0.0f;
        m_PointerData.ClickedReleaseThreshold = 0.2f;

        m_PointerData.bIsPressed = false;

        m_PointerData.EventType = EMousePointerEventType.PtrEventType_NONE;
    }

    public void SetPointerPos(Vector2 InPos)
    {
        transform.localPosition = InPos;

        m_PointerEventData.position = Input.mousePosition;

        m_PointerData.UpdatePosition(Input.mousePosition);


    }

    public void OnClick()
    {

    }

    private void OnTriggerEnter(Collider collision)
    {
        Selectable collidedSelectable = collision.gameObject.GetComponent<Selectable>();
        if (collidedSelectable != null)
        {
            m_SelectableList.Add(collidedSelectable);
            collidedSelectable.OnPointerEnter(m_PointerEventData);
        }

        IPointerClickHandler collidedClickHandler = collision.gameObject.GetComponent<IPointerClickHandler>();
        if (collidedClickHandler != null)
        {
            m_ClickHandlerList.Add(collidedClickHandler);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        Selectable collidedSelectable = collision.gameObject.GetComponent<Selectable>();
        if (collidedSelectable != null)
        {
            if (m_SelectableList.Contains(collidedSelectable))
            {
                m_SelectableList.Remove(collidedSelectable);
                collidedSelectable.OnPointerExit(m_PointerEventData);
            }
        }

        IPointerClickHandler collidedClickHandler = collision.gameObject.GetComponent<IPointerClickHandler>();
        if (collidedClickHandler != null)
        {
            if (m_ClickHandlerList.Contains(collidedClickHandler))
            {
                m_ClickHandlerList.Remove(collidedClickHandler);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
    }


    private void OnCollisionExit(Collision collision)
    {

        Debug.Log("Collider out");
    }

    private void HandleFPEInputs()
    {
        FPEInputManager InputManager = FPEInputManager.Instance;

        if (InputManager.GetButtonDown(FPEInputManager.eFPEInput.FPE_INPUT_INTERACT))
        {
            m_PointerData.OnPressed(m_ClickHandlerList, m_PointerEventData);
        }
        else if (InputManager.GetButtonUp(FPEInputManager.eFPEInput.FPE_INPUT_INTERACT))
        {
            m_PointerData.OnReleased(m_ClickHandlerList, m_PointerEventData);
        }
    }
}
