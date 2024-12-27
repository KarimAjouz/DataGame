using System.Collections.Generic;
using ChoETL;
using Michsky.DreamOS;
using UnityEngine;
using NNarrativeDataTypes;

public class CS_DynamicChatManager : MonoBehaviour
{
    private CS_ChatLogBuilder _chatLogBuilder;
    
    private MessagingManager _messagingManager;

    [SerializeField] 
    private Dictionary<int, List<FNarrativeTimedEvent>> NarrativeEvents = new Dictionary<int, List<FNarrativeTimedEvent>>();

    public void Start()
    {
        _chatLogBuilder = FindFirstObjectByType<CS_ChatLogBuilder>();

        if (_chatLogBuilder == null)
        {
            Debug.LogError("No Chat Log Builder Found!");
        }
        
        _messagingManager = FindFirstObjectByType<MessagingManager>();

        if (_messagingManager == null)
        {
            Debug.LogError("No Messaging Manager Found!");
        }

        TEST_PushMessage();
    }

    private void TEST_PushMessage()
    {
        CS_TimeManager timeManager = FindFirstObjectByType<CS_TimeManager>();

        if (timeManager == null)
        {
            Debug.LogError("No Time Manager Found!");
            return;
        }
        
        PushIndividualMessage(
            "TEST",
            "This is a test message", 
            "Test Author", 
            timeManager.GetDisplayTimeString()
        );
        
    }

    private void BuildChatEvents()
    {
        if (_chatLogBuilder == null)
        {
            Debug.LogError("No Chat Log Builder Found!");
        }
        
        List<FChatRoom> Rooms = _chatLogBuilder.GetChatRooms();

        foreach (FChatRoom ChatRoom in Rooms)
        {
            foreach (FChatLineTimeChunk ChatLineTimeChunk in ChatRoom.ChatLog)
            {
                // This is where we create timed narrative events and push them to the master list.
                // #TODO [KA] (26.12.2024): Build some more cohesive system and move the master list of events out
                // #TODO                    to the primary TimedNarrativeEventManager/StoryManager
                
                foreach (FChatLine Line in ChatLineTimeChunk.ChatLines)
                {
                    if (!NarrativeEvents.ContainsKey(ChatLineTimeChunk.TimeStamp))
                    {
                        NarrativeEvents.Add(
                            ChatLineTimeChunk.TimeStamp, 
                            new List<FNarrativeTimedEvent>()
                        );
                    }
                    else
                    {
                        NarrativeEvents[ChatLineTimeChunk.TimeStamp].Add(
                            new FNarrativeTimedEvent(
                                ENarrativeEventType.ChatMessage, 
                                ChatRoom.RoomName,
                                Line.CharacterName, 
                                Line.Line, 
                                ChatLineTimeChunk.TimestampString
                            )
                        );
                    }
                }
            }
        }
    }

    private void PushIndividualMessage(string InChatTitle, string InText, string InAuthor, string InTime)
    {
        if (_messagingManager == null)
        {
            Debug.LogError("No Messaging Manager Found!");
            return;
        }
        
        ChatLayoutPreset layout = _messagingManager.chatViewer.Find(InChatTitle).GetComponent<ChatLayoutPreset>();

        if (layout == null)
        {
            Debug.LogError("No Layout Preset Found!");
            return;
        }
        
        // #TODO [KA] (26.12.2024): Localisation!
        _messagingManager.CreateCustomMessageFromAuthor(
            layout, 
            InText, 
            InAuthor, 
            InTime
        );
        
    }

    public void PushChatPostsToMessages(int CurrentHour, int CurrentMinute)
    {
        if (_messagingManager == null)
        {
            Debug.LogError("No Messaging Manager Found!");
        }

        int CurrentTime = CurrentHour + 100 + CurrentMinute;
        
        List<FNarrativeTimedEvent> EventsToPost = NarrativeEvents[CurrentTime];
        foreach (FNarrativeTimedEvent Event in EventsToPost)
        {
            if (Event.EventType != ENarrativeEventType.ChatMessage)
            {
                continue;
            }

            PushIndividualMessage(Event.EventSourceTitle, Event.EventText, Event.EventAuthor, Event.EventTimestampText);
        }
    }


}
