using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;
using Whilefun.FPEKit;

public class CS_PostItNote : MonoBehaviour
{
    Rigidbody m_rb;
    Collider m_collider;

    [SerializeField]
    GameObject GhostGO;


    [SerializeField]
    float WallOffset = 0.01f;

    bool bPickedUp = false;

    Vector3 GhostPosition = Vector3.zero;
    Vector3 GhostRotation = Vector3.zero;

    Camera m_PlayerCam;

    [SerializeField] 
    private LayerMask _layerMask;

    // Start is called before the first frame update
    void Start()
    {
        m_rb = GetComponent<Rigidbody>();
        m_collider = GetComponent<Collider>();
        m_PlayerCam = FPEPlayer.Instance.GetComponentInChildren<Camera>();
        if (m_PlayerCam == null)
        {
            Debug.LogError("m_PlayerCam could not be initialised!");
        }

        if (GhostGO == null)
        {
            Debug.LogError("GhostGO is not populated!");
        }
    }

    private void Update()
    {
        if(bPickedUp)
        {
            UpdateGhost();
        }
    }

    private void UpdateGhost()
    {
        RaycastHit hit;
        Physics.Raycast(m_PlayerCam.transform.position, m_PlayerCam.transform.forward, out hit, 1.0f, _layerMask);
        
        if(hit.collider == null)
        {
            GhostGO.SetActive(false);
            return;
        }

        Debug.Log("ObjName: " + hit.collider.gameObject.name);

        GhostPosition = hit.point;
        GhostRotation = hit.normal;
        GhostGO.transform.rotation = Quaternion.FromToRotation(Vector3.back, hit.normal);

        GhostGO.transform.position = GhostPosition + (WallOffset * GhostGO.transform.forward);

        GhostGO.SetActive(true);

    }

    public void OnPickup()
    {
        m_rb.useGravity = true;
        bPickedUp = true;
    }

    public void OnThrow()
    {
        bPickedUp = false;
        if (GhostGO.activeSelf)
        {

            transform.rotation = GhostGO.transform.rotation;
            transform.position = GhostGO.transform.position;

            m_rb.velocity = Vector3.zero;

            transform.SetParent(null);

            m_rb.freezeRotation = true;
            m_rb.useGravity = false;
            m_rb.isKinematic = true;
            m_collider.isTrigger = true;

            Debug.Log("careful now!");

        }
        else
        {
            m_rb.useGravity = true;
            m_rb.isKinematic = false;
            m_collider.isTrigger = false;

            Debug.Log("YEET!");
        }

        GhostGO.SetActive(false);
    }
}
