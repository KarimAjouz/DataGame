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

public class CS_ChatLogBuilder : MonoBehaviour
{
    [SerializeField]
    [ReadOnly]
    private List<NNarrativeDataTypes.FChatLineTimeChunk> ChatLog;

    [SerializeField]
    [ReadOnly]
    private NNarrativeDataTypes.FChatRoom ChatRoom;

    // Start is called before the first frame update
    public void Start()
    {
        ChatLog = new List<NNarrativeDataTypes.FChatLineTimeChunk>();
    }

    public void PopulateChatLogLines(TextAsset InLines, JArray ChatLogArray)
    {
        if (InLines.IsObjectNullOrEmpty() || InLines.text.IsNullOrEmpty())
        {
            Debug.LogError("ERROR: InLines can not be null or empty!");
            return;
        }

        ChatLog.Clear();

        string PeopleRow = InLines.text.Split("\n")[0];
        string[] PeopleInChatLog = PeopleRow.Split(",");

        List<string> People = new List<string>();

        ChatRoom.People.Clear();

        // First we look for all the present characters in the chatlog
        // #TODO [KA] (03.10.2024): Replace the string usage of people with a structed out usage of people that gets the data from a separate CSV.
        foreach (string Person in PeopleInChatLog)
        {
            if (Person.Equals("TIME"))
            {
                continue;
            }
            ChatRoom.People.Add(Person.Split("\r")[0]);
        }

        foreach (JObject chatObject in ChatLogArray.Children())
        {
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

            foreach (string Person in ChatRoom.People)
            {
                string Dialogue = (string)chatObject[Person];

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
                        Temp.CharacterName = Person;

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
                ChatLog.Add(NewTimeChunk);
            }
        }
    }

}
