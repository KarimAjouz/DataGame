using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using NaughtyAttributes;

using UnityEngine;
using UnityEditor;
using ChoETL;
using Newtonsoft.Json.Linq;
using System.Linq;
using Michsky.DreamOS;
using NNarrativeDataTypes;
// ReSharper disable PossibleNullReferenceException

public class CS_ChatLogBuilder : MonoBehaviour
{
    [SerializeField]
    [ReadOnly]
    private FChatRoom WorkingChatRoom;

    [SerializeField]
    private MessagingManager _messagingManager;

    [SerializeField]
    private MessagingChat BaseChatAsset;
    
    [SerializeField]
    private List<FChatRoom> ChatRooms = new List<FChatRoom>();
    
    [SerializeField]
    private bool bUseWorkingRoom = false;

    // Start is called before the first frame update
    public void Start()
    {
        if (bUseWorkingRoom)
        {
            WorkingChatRoom.ChatLog = new List<FChatLineTimeChunk>();
        }
    }

    public void PushChatItemToMessageManager(string InTitle, int inChatId, MessagingChat InChatAssetDefault, bool inIsVisByDefault = true)
    {
        if (_messagingManager.IsNull())
        {
            Debug.LogWarning("No Messaging Manager Found!");
            return;
        }
        
        MessagingManager.ChatItem NewChatItem = new MessagingManager.ChatItem
        {
            isTimeEventChat = true,
            chatTitle = InTitle,
            chatId = inChatId,            
            chatAsset = InChatAssetDefault,
            isVisible = inIsVisByDefault
        };

        _messagingManager.chatList.Add(NewChatItem);
    }
    
    public ref List<FChatRoom> GetChatRooms()
    {
        return ref ChatRooms;
    }

    
    public void PopulateChatLogLinesFromString(string InLines, JArray ChatLogArray, int InChatId)
    {
        CS_CharacterListBuilder CharacterHolder = gameObject.GetComponent<CS_CharacterListBuilder>();
        
        if(!CharacterHolder)
        {
            Debug.LogError("Character Holder is null!");
            return;
        }
        
        if (InLines.IsNullOrEmpty())
        {
            Debug.LogError("ERROR: InLines can not be null or empty!");
            return;
        }

        if (InChatId == 0)
        {
            ChatRooms.Clear();
            _messagingManager.chatList.Clear();
        }

        
        // We are using the debug WorkingChatRoom
        if (bUseWorkingRoom)
        {
            WorkingChatRoom.ChatLog.Clear();
            WorkingChatRoom.Characters.Clear();
            WorkingChatRoom.RoomId = 0;
            WorkingChatRoom.RoomName = "TEST";

            string PeopleRow = InLines.Split("\n")[0];
            string[] PeopleInChatLog = PeopleRow.Split(",");
            ChatRooms.Clear();

            // First we look for all the present characters in the chatlog
            // #TODO [KA] (03.10.2024): Replace the string usage of people with a structed out usage of people that gets the data from a separate CSV.
            foreach (string Person in PeopleInChatLog)
            {
                if (Person.Equals("TIME"))
                {
                    continue;
                }

                WorkingChatRoom.Characters.Add(CharacterHolder.GetCharacterIndexFromName(Person.Split("\r")[0]));
            }

            foreach (var jToken in ChatLogArray.Children())
            {
                var chatObject = (JObject)jToken;

                if (chatObject.IsNullOrEmpty())
                {
                    Debug.LogWarning("WARNING: chatObject is null!");
                    continue;
                }

                FChatLineTimeChunk NewTimeChunk = new FChatLineTimeChunk();

                string TimestampString = (string)chatObject["TIME"];

                if (TimestampString == "")
                {
                    continue;
                }

                NewTimeChunk.TimestampString = TimestampString;

                TimestampString = TimestampString.Replace(":", "");

                NewTimeChunk.TimeStamp = int.Parse(TimestampString);
                NewTimeChunk.ChatLines = new List<FChatLine>();

                foreach (FCharacterData Character in WorkingChatRoom.Characters)
                {
                    string Dialogue = (string)chatObject[Character.GetCharacterName()];

                    if (Dialogue.IsNullOrEmpty())
                    {
                        continue;
                    }

                    foreach (string DialogueLine in Dialogue.Split("\n"))
                    {
                        if (DialogueLine.IsNullOrEmpty())
                        {
                            continue;
                        }

                        if (DialogueLine.StartsWith("#"))
                        {
                            FChatLine Temp = new FChatLine();
                            string CleanedDialogueLine = DialogueLine.Replace("#", "");
                            Temp.PrintOrder = DialogueLine.Length - CleanedDialogueLine.Length;
                            Temp.CharacterName = Character.GetCharacterName();

                            if (CleanedDialogueLine.StartsWith(" ") && CleanedDialogueLine.Length > 1)
                            {
                                Temp.Line = CleanedDialogueLine.Substring(1);
                            }
                            else
                            {
                                Temp.Line = CleanedDialogueLine;
                            }

                            NewTimeChunk.ChatLines.Add(Temp);
                        }
                        else
                        {
                            NewTimeChunk.ChatLines[NewTimeChunk.ChatLines.Count - 1].Line.Concat(DialogueLine);
                        }
                    }
                }

                if (NewTimeChunk.ChatLines.Count > 0)
                {
                    WorkingChatRoom.ChatLog.Add(NewTimeChunk);
                }
            }

            ChatRooms.Add(WorkingChatRoom);
            
            PushChatItemToMessageManager(WorkingChatRoom.RoomName, WorkingChatRoom.RoomId, BaseChatAsset);
        }
        else
        {
            FChatRoom NewRoom = new FChatRoom();
            NewRoom.ChatLog = new List<FChatLineTimeChunk>();
            NewRoom.Characters = new List<FCharacterData>();
            NewRoom.RoomId = InChatId;
            NewRoom.RoomName = "ROOM: " + InChatId;
            
            string PeopleRow = InLines.Split("\n")[0];
            string[] PeopleInChatLog = PeopleRow.Split(",");

            // First we look for all the present characters in the chatlog
            // #TODO [KA] (03.10.2024): Replace the string usage of people with a structed out usage of people that gets the data from a separate CSV.
            foreach (string Person in PeopleInChatLog)
            {
                if (Person.Equals("TIME"))
                {
                    continue;
                }

                NewRoom.Characters.Add(CharacterHolder.GetCharacterIndexFromName(Person.Split("\r")[0]));
            }

            foreach (var jToken in ChatLogArray.Children())
            {
                var chatObject = (JObject)jToken;

                if (chatObject.IsNullOrEmpty())
                {
                    Debug.LogWarning("WARNING: chatObject is null!");
                    continue;
                }

                FChatLineTimeChunk NewTimeChunk = new FChatLineTimeChunk();

                string TimestampString = (string)chatObject["TIME"];

                if (TimestampString.IsNullOrEmpty())
                {
                    continue;
                }

                NewTimeChunk.TimestampString = TimestampString;
                TimestampString = TimestampString.Replace(":", "");

                NewTimeChunk.TimeStamp = int.Parse(TimestampString);
                NewTimeChunk.ChatLines = new List<FChatLine>();

                foreach (FCharacterData Character in NewRoom.Characters)
                {
                    string Dialogue = (string)chatObject[Character.GetCharacterName()];

                    if (Dialogue.IsNullOrEmpty())
                    {
                        continue;
                    }

                    foreach (string DialogueLine in Dialogue.Split("\n"))
                    {
                        if (DialogueLine.IsNullOrEmpty())
                        {
                            continue;
                        }

                        if (DialogueLine.StartsWith("#"))
                        {
                            FChatLine Temp = new FChatLine();
                            string CleanedDialogueLine = DialogueLine.Replace("#", "");
                            Temp.PrintOrder = DialogueLine.Length - CleanedDialogueLine.Length;
                            Temp.CharacterName = Character.GetCharacterName();

                            if (CleanedDialogueLine.StartsWith(" ") && CleanedDialogueLine.Length > 1)
                            {
                                Temp.Line = CleanedDialogueLine.Substring(1);
                            }
                            else
                            {
                                Temp.Line = CleanedDialogueLine;
                            }

                            NewTimeChunk.ChatLines.Add(Temp);
                        }
                        else
                        {
                            NewTimeChunk.ChatLines[NewTimeChunk.ChatLines.Count - 1].Line.Concat(DialogueLine);
                        }
                    }
                }

                if (NewTimeChunk.ChatLines.Count > 0)
                {
                    NewRoom.ChatLog.Add(NewTimeChunk);
                }
            }

            ChatRooms.Add(NewRoom);
            
            PushChatItemToMessageManager(NewRoom.RoomName, NewRoom.RoomId, BaseChatAsset);
        }
    }
    
    public void PopulateChatLogLinesFromTextAsset(TextAsset InLines, JArray ChatLogArray)
    {
        CS_CharacterListBuilder CharacterHolder = gameObject.GetComponent<CS_CharacterListBuilder>();
        
        if(!CharacterHolder)
        {
            Debug.LogError("Character Holder is null!");
            return;
        }
        
        if (InLines.IsObjectNullOrEmpty() || InLines.text.IsNullOrEmpty())
        {
            Debug.LogError("ERROR: InLines can not be null or empty!");
            return;
        }

        // We are using the debug WorkingChatRoom
        if (bUseWorkingRoom)
        {
            WorkingChatRoom.ChatLog.Clear();
            WorkingChatRoom.Characters.Clear();
            WorkingChatRoom.RoomId = 0;
            WorkingChatRoom.RoomName = "TEST";

            string PeopleRow = InLines.text.Split("\n")[0];
            string[] PeopleInChatLog = PeopleRow.Split(",");
            ChatRooms.Clear();

            // First we look for all the present characters in the chatlog
            // #TODO [KA] (03.10.2024): Replace the string usage of people with a structed out usage of people that gets the data from a separate CSV.
            foreach (string Person in PeopleInChatLog)
            {
                if (Person.Equals("TIME"))
                {
                    continue;
                }

                WorkingChatRoom.Characters.Add(CharacterHolder.GetCharacterIndexFromName(Person.Split("\r")[0]));
            }

            foreach (var jToken in ChatLogArray.Children())
            {
                var chatObject = (JObject)jToken;

                if (chatObject.IsNullOrEmpty())
                {
                    Debug.LogWarning("WARNING: chatObject is null!");
                    continue;
                }

                FChatLineTimeChunk NewTimeChunk = new FChatLineTimeChunk();

                string TimestampString = (string)chatObject["TIME"];

                if (TimestampString == "")
                {
                    continue;
                }

                NewTimeChunk.TimestampString = TimestampString;

                TimestampString = TimestampString.Replace(":", "");

                NewTimeChunk.TimeStamp = int.Parse(TimestampString);
                NewTimeChunk.ChatLines = new List<FChatLine>();

                foreach (FCharacterData Character in WorkingChatRoom.Characters)
                {
                    string Dialogue = (string)chatObject[Character.GetCharacterName()];

                    if (Dialogue.IsNullOrEmpty())
                    {
                        continue;
                    }

                    foreach (string DialogueLine in Dialogue.Split("\n"))
                    {
                        if (DialogueLine.IsNullOrEmpty())
                        {
                            continue;
                        }

                        if (DialogueLine.StartsWith("#"))
                        {
                            FChatLine Temp = new FChatLine();
                            string CleanedDialogueLine = DialogueLine.Replace("#", "");
                            Temp.PrintOrder = DialogueLine.Length - CleanedDialogueLine.Length;
                            Temp.CharacterName = Character.GetCharacterName();

                            if (CleanedDialogueLine.StartsWith(" ") && CleanedDialogueLine.Length > 1)
                            {
                                Temp.Line = CleanedDialogueLine.Substring(1);
                            }
                            else
                            {
                                Temp.Line = CleanedDialogueLine;
                            }

                            NewTimeChunk.ChatLines.Add(Temp);
                        }
                        else
                        {
                            NewTimeChunk.ChatLines[NewTimeChunk.ChatLines.Count - 1].Line.Concat(DialogueLine);
                        }
                    }
                }

                if (NewTimeChunk.ChatLines.Count > 0)
                {
                    WorkingChatRoom.ChatLog.Add(NewTimeChunk);
                }
            }

            ChatRooms.Add(WorkingChatRoom);
            
            PushChatItemToMessageManager(WorkingChatRoom.RoomName, WorkingChatRoom.RoomId, BaseChatAsset);
        }
        else
        {
            FChatRoom NewRoom = new FChatRoom();
            NewRoom.ChatLog.Clear();
            NewRoom.Characters.Clear();
            NewRoom.RoomId = 0;
            NewRoom.RoomName = "TEST";

            string PeopleRow = InLines.text.Split("\n")[0];
            string[] PeopleInChatLog = PeopleRow.Split(",");
            ChatRooms.Clear();

            // First we look for all the present characters in the chatlog
            // #TODO [KA] (03.10.2024): Replace the string usage of people with a structed out usage of people that gets the data from a separate CSV.
            foreach (string Person in PeopleInChatLog)
            {
                if (Person.Equals("TIME"))
                {
                    continue;
                }

                NewRoom.Characters.Add(CharacterHolder.GetCharacterIndexFromName(Person.Split("\r")[0]));
            }

            foreach (var jToken in ChatLogArray.Children())
            {
                var chatObject = (JObject)jToken;

                if (chatObject.IsNullOrEmpty())
                {
                    Debug.LogWarning("WARNING: chatObject is null!");
                    continue;
                }

                FChatLineTimeChunk NewTimeChunk = new FChatLineTimeChunk();

                string TimestampString = (string)chatObject["TIME"];

                if (TimestampString == "")
                {
                    continue;
                }

                NewTimeChunk.TimestampString = TimestampString;

                TimestampString = TimestampString.Replace(":", "");

                NewTimeChunk.TimeStamp = int.Parse(TimestampString);
                NewTimeChunk.ChatLines = new List<FChatLine>();

                foreach (FCharacterData Character in NewRoom.Characters)
                {
                    string Dialogue = (string)chatObject[Character.GetCharacterName()];

                    if (Dialogue.IsNullOrEmpty())
                    {
                        continue;
                    }

                    foreach (string DialogueLine in Dialogue.Split("\n"))
                    {
                        if (DialogueLine.IsNullOrEmpty())
                        {
                            continue;
                        }

                        if (DialogueLine.StartsWith("#"))
                        {
                            FChatLine Temp = new FChatLine();
                            string CleanedDialogueLine = DialogueLine.Replace("#", "");
                            Temp.PrintOrder = DialogueLine.Length - CleanedDialogueLine.Length;
                            Temp.CharacterName = Character.GetCharacterName();

                            if (CleanedDialogueLine.StartsWith(" ") && CleanedDialogueLine.Length > 1)
                            {
                                Temp.Line = CleanedDialogueLine.Substring(1);
                            }
                            else
                            {
                                Temp.Line = CleanedDialogueLine;
                            }

                            NewTimeChunk.ChatLines.Add(Temp);
                        }
                        else
                        {
                            NewTimeChunk.ChatLines[NewTimeChunk.ChatLines.Count - 1].Line.Concat(DialogueLine);
                        }
                    }
                }

                if (NewTimeChunk.ChatLines.Count > 0)
                {
                    NewRoom.ChatLog.Add(NewTimeChunk);
                }
            }

            ChatRooms.Add(NewRoom);
            
            PushChatItemToMessageManager(NewRoom.RoomName, NewRoom.RoomId, BaseChatAsset);
        }
    }

}
