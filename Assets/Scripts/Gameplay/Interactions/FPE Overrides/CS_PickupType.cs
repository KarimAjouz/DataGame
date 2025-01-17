using NaughtyAttributes;
using NInteractionTypes;
using Gameplay.Interactions.FPE_Overrides;
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
    private bool Stackable = false;

    [SerializeField]
    [ShowIf("HasPutBack")]
    private PutBackSetup PutBackRules = new PutBackSetup();


    private Rigidbody m_rb;
    private Collider m_collider;

    private CS_Socket Socket = null;

    private void Start()
    {
        m_rb = GetComponent<Rigidbody>();
        m_collider = GetComponent<Collider>();
    }


    public void OnPickup()
    {
        m_rb.useGravity = true;

        if (Socket != null)
        {
            Socket.Unsocket();
            Socket = null;
        }

        if (transform.parent == null)
        {
            return;
        }
        
        CS_ItemDeliverer ItemDeliverer = transform.parent.GetComponent<CS_ItemDeliverer>();
        if (ItemDeliverer)
        {
            ItemDeliverer.PickupItem();
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

        CS_Socket FoundSocket = PutBackObj.GetComponent<CS_Socket>();
        if(FoundSocket != null) 
        {
            FoundSocket.SocketGO(gameObject);
            Socket = FoundSocket;
        }
        

        m_rb.freezeRotation = true;
        m_rb.useGravity = false;
        m_rb.isKinematic = true;
        m_collider.isTrigger = true;
    }
}
