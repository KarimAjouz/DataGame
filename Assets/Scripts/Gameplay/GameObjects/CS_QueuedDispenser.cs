using UnityEngine;

public class CS_QueuedDispenser : MonoBehaviour
{

    [SerializeField]
    private Collider m_OutputCollider = null;
    
    [SerializeField]
    private GameObject m_PrefabToDispense = null;
    
    private AudioSource m_AudioSource = null;
    private int m_QueuedAmount = 0;

    private int m_CollisionCount = 0;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_OutputCollider = GetComponent<Collider>();
        if (!m_OutputCollider)
        {
            Debug.LogError("No collider exists on this game object");
            return;
        }
        
        if (!m_PrefabToDispense)
        {
            Debug.LogError("No PrefabToDispense Defined");
            return;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (m_QueuedAmount > 0 && m_CollisionCount == 0)
        {
            Instantiate(m_PrefabToDispense, transform.position, Quaternion.identity);
            m_QueuedAmount--;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        m_CollisionCount++;
    }
    
    void OnTriggerExit(Collider other)
    {
        m_CollisionCount--;
    }

    public void QueueItem()
    {
        m_QueuedAmount++;
    }
}
