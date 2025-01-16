using UnityEngine;
using Whilefun.FPEKit;

public class CS_VacuumTube : MonoBehaviour
{
    [SerializeField]
    private CS_Socket m_TubeSocket;
    
    
    [SerializeField]
    private CS_StoryManager m_StoryManager;

    private bool m_IsDoorClosed = true;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (m_TubeSocket == null)
        {
            m_TubeSocket = GetComponentInChildren<CS_Socket>();
        }

        if (!m_TubeSocket)
        {
            Debug.LogError("No TubeSocket assigned and no CS_Socket exists in children!");
            return;
        }
        
        
        if (m_StoryManager == null)
        {
            m_StoryManager = FindFirstObjectByType<CS_StoryManager>();
        }

        if (!m_StoryManager)
        {
            Debug.LogError("No CS_StoryManager assigned and no CS_StoryManager found in world!");
            return;
        }
    }

    public bool CanSubmit()
    {
        return m_IsDoorClosed && m_TubeSocket.GetSocketedGO() != null;
    }

    public void TrySubmission()
    {
        if (CanSubmit())
        {
            m_StoryManager.SubmitProfile(m_TubeSocket.GetSocketedGO().GetComponent<CS_PunchCard>().GetCharacterData());
        }
    }

    public void OnOpened()
    {
        m_IsDoorClosed = !m_IsDoorClosed;
    }
}
