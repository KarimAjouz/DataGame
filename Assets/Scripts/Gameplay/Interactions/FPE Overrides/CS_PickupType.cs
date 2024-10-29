using NaughtyAttributes;
using NInteractionTypes;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Whilefun.FPEKit;

[System.Serializable]
public struct PutBackSetup
{
    [SerializeField]
    public bool LockToPositionOnPutBack;
}


public class CS_PickupType : MonoBehaviour
{
    public EPickupObjectTypes PickupType = EPickupObjectTypes.OT_Generic;

    [SerializeField]
    bool HasPutBack = false;

    [SerializeField]
    [ShowIf("HasPutBack")]
    private PutBackSetup PutBackRules = new PutBackSetup();


    private Rigidbody m_rb;
    private Collider m_collider;

    private void Start()
    {
        m_rb = GetComponent<Rigidbody>();
        m_collider = GetComponent<Collider>();
    }


    public void OnPickup()
    {
        m_rb.useGravity = true;

        CS_Socket Socket = GetComponent<CS_Socket>();
        if (Socket != null)
        {
            Socket.Unsocket();
        }
    }

    public void OnPutBack()
    {
        if(!HasPutBack)
        {
            return;
        }

        GameObject PutBackObj = FPEInteractionManagerScript.Instance.GetCurrentPutBackObject();
        if (PutBackObj == null)
        {
            return;
        }

        CS_PutBackOverride AttemptedPutBackSocket = PutBackObj.GetComponent<CS_PutBackOverride>();
        if (AttemptedPutBackSocket == null)
        {
            return;
        }

        CS_Socket Socket = PutBackObj.GetComponent<CS_Socket>();
        if(Socket == null) 
        {
            return;
        }
        
        Socket.SocketGO(gameObject);

        //transform.SetParent(AttemptedPutBackSocket.SocketedTransform);
        transform.position = Socket.SocketedTransform.position;
        transform.rotation = Socket.SocketedTransform.rotation;

        m_rb.freezeRotation = true;
        m_rb.useGravity = false;
        m_rb.isKinematic = true;
        m_collider.isTrigger = true;
    }
}
