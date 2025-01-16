using NaughtyAttributes;
using TimeSyncSystem;
using UnityEngine;


enum ENarrativeEventType
{
    ChatMessage,
    ForumPost
}

[System.Serializable]
struct FNarrativeTimedEvent
{
    
    [ReadOnly]
    public readonly ENarrativeEventType EventType;
    
    [ReadOnly]
    public string EventSourceTitle;
    
    [ReadOnly]
    public string EventAuthor;
    
    [ReadOnly]
    public string EventText;
    
    [ReadOnly]
    public string EventTimestampText;

    public FNarrativeTimedEvent(ENarrativeEventType InEventType, string InEventSourceTitle, string InEventAuthor, string InEventText, string InEventTimestampText)
    {
        this.EventType = InEventType;
        this.EventSourceTitle = InEventSourceTitle;
        this.EventAuthor = InEventAuthor;
        this.EventText = InEventText;
        this.EventTimestampText = InEventTimestampText;
    }
}


// This is a class that takes time-relative narrative events and pings them to the DreamOS UI so that they can be displayed to the player
// It also manages systems where narrative events are fired by non-dream OS systems. 
public class CS_TimedNarrativeEventManager : MonoBehaviour
{
    private CS_TimeManager m_TimeManager;
    private CS_DynamicChatManager m_DynamicChatManager;
    private CS_DynamicPostManager m_DynamicPostManager;

    void Start()
    {
        m_TimeManager = FindFirstObjectByType<CS_TimeManager>();

        if (m_TimeManager == null)
        {
            Debug.LogError("No Time Manager found");
        }
        
        m_DynamicChatManager = FindFirstObjectByType<CS_DynamicChatManager>();

        if (m_DynamicChatManager == null)
        {
            Debug.LogError("No dynamic chat manager found");
        }
        
        m_DynamicPostManager = FindFirstObjectByType<CS_DynamicPostManager>();

        if (m_DynamicPostManager == null)
        {
            Debug.LogError("No dynamic post manager found");
        }
        
        m_TimeManager.GetSyncEvent().AddListener(ProcessNarrativeEventsFromTimeSyncEvent);

    }

    public void ProcessNarrativeEventsFromTimeSyncEvent(FTimeSyncEvent InTimeSyncEvent)
    {
        switch (InTimeSyncEvent.reason)
        {
        case FTimeSyncEvent.TimeUpdateReason.ADDED:

            for (int MinuteToProcess = InTimeSyncEvent.OldTimeStamp + 1;
                 MinuteToProcess <= InTimeSyncEvent.NewTimeStamp;
                 MinuteToProcess++)
            {
                m_DynamicChatManager.PushChatPostsToMessages(InTimeSyncEvent.NewTimeStamp);
            }
            
            break;
        case FTimeSyncEvent.TimeUpdateReason.REWIND: 
            break;
        }
        
    }
}
