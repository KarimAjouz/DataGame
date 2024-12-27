using UnityEngine;


enum ENarrativeEventType
{
    ChatMessage,
    ForumPost
}
struct FNarrativeTimedEvent
{
    
    public readonly ENarrativeEventType EventType;

    public readonly string EventSourceTitle;
    public readonly string EventAuthor;
    public readonly string EventText;
    
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

}
