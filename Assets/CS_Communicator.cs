using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Whilefun.FPEKit;

public class CS_Communicator : MonoBehaviour
{

    Rigidbody m_rb;
    Collider m_collider;
    // Start is called before the first frame update
    void Start()
    {
        m_rb = GetComponent<Rigidbody>();
        m_collider = GetComponent<Collider>();
    }


    public void OnPickup()
    {
        m_rb.useGravity = true;
    }

    public void OnPutBack()
    {

        GameObject PutBackObj = FPEInteractionManagerScript.Instance.GetCurrentPutBackObject();
        if (PutBackObj == null )
        {
            return;
        }

        CS_Socket InteractedSocket = PutBackObj.GetComponent<CS_Socket>();
        if (InteractedSocket == null)
        {
            return;
        }

        transform.SetParent(InteractedSocket.SocketedTransform);
        transform.localPosition = new Vector3();
        transform.localEulerAngles = new Vector3();

        m_rb.freezeRotation = true;
        m_rb.useGravity = false;
        m_rb.isKinematic = true;
        m_collider.isTrigger = true;
    }

    public void OnThrow()
    {
        m_rb.useGravity = true;
        m_rb.isKinematic = false;
        m_collider.isTrigger = false;
    }
}
