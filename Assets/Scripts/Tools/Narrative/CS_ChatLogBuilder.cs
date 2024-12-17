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
using NNarrativeDataTypes;

public class CS_ChatLogBuilder : MonoBehaviour
{
    [SerializeField]
    [ReadOnly]
    private NNarrativeDataTypes.FChatRoom WorkingChatRoom;
    
    [SerializeField]
    [ReadOnly]
    private List<FChatRoom> ChatRooms = new List<FChatRoom>();

    // Start is called before the first frame update
    public void Start()
    {
        WorkingChatRoom.ChatLog = new List<NNarrativeDataTypes.FChatLineTimeChunk>();
    }

    public ref List<FChatRoom> GetChatRooms()
    {
        return ref ChatRooms;
    }

    public void PopulateChatLogLines(TextAsset InLines, JArray ChatLogArray)
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

        WorkingChatRoom.ChatLog.Clear();
        WorkingChatRoom.Characters.Clear();

        string PeopleRow = InLines.text.Split("\n")[0];
        string[] PeopleInChatLog = PeopleRow.Split(",");
        

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
            
            NNarrativeDataTypes.FChatLineTimeChunk NewTimeChunk = new NNarrativeDataTypes.FChatLineTimeChunk();

            string TimestampString = (string)chatObject["TIME"];

            if (TimestampString == "")
            {
                continue;
            }

            NewTimeChunk.TimestampString = TimestampString;

            TimestampString = TimestampString.Replace(":", "");

            NewTimeChunk.TimeStamp = int.Parse(TimestampString);
            NewTimeChunk.ChatLines = new List<NNarrativeDataTypes.FChatLine>();

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
                        NNarrativeDataTypes.FChatLine Temp = new NNarrativeDataTypes.FChatLine();
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
    }

}
