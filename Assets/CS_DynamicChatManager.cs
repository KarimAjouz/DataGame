using System.Collections.Generic;
using Michsky.DreamOS;
using UnityEngine;
using NNarrativeDataTypes;

public class CS_DynamicChatManager : MonoBehaviour
{
    private CS_ChatLogBuilder _chatLogBuilder;
    
    private MessagingManager _messagingManager;
    
    [SerializeField]
    private ChatLayoutPreset ChatLayoutPreset;

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
    }
    

    public void PushChatPostsToMessages(int CurrentHour, int CurrentMinute)
    {
        if (_chatLogBuilder == null)
        {
            Debug.LogError("No Chat Log Builder Found!");
        }
        
        if (_messagingManager == null)
        {
            Debug.LogError("No Messaging Manager Found!");
        }
        
        List<FChatRoom> Rooms = _chatLogBuilder.GetChatRooms();

        foreach (FChatRoom Room in Rooms)
        {
            foreach (FChatLineTimeChunk ChatLineTimeChunk in Room.ChatLog)
            {
                if (CurrentHour * 100 + CurrentMinute != ChatLineTimeChunk.TimeStamp)
                {
                    continue;
                }
                
                // This is where we create and push the DreamOS messages
                foreach (FChatLine Line in ChatLineTimeChunk.ChatLines)
                {
                    _messagingManager.CreateCustomMessageFromAuthor(ChatLayoutPreset, Line.Line, Line.CharacterName, ChatLineTimeChunk.TimestampString);
                }
            }
        }
    }
}
