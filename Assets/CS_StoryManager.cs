using System.Collections;
using System.Collections.Generic;
using System.
using UnityEngine;

// #TODO [KA] (29.09.2024): Rename this it sucks
public struct ChatLineTimeChunk
{
    int TimeStamp;

    List<ChatLine> ChatLines;
}

public struct ChatLine
{
    int PrintOrder;
    string CharacterName;
    string Line;
}



public class CS_StoryManager : MonoBehaviour
{
    [SerializeField]
    public TextAsset TestChatlogCSV;

    [SerializeField]
    public List<ChatLineTimeChunk> ChatLog;

    public string ConvertCsvFileToJsonObject(TextAsset Lines)
    {
        var csv = new List<string[]>();
        var lines = File.ReadAllLines(path);

        foreach (string line in lines)
            csv.Add(line.Split(','));

        var properties = lines[0].Split(',');

        var listObjResult = new List<Dictionary<string, string>>();

        for (int i = 1; i < lines.Length; i++)
        {
            var objResult = new Dictionary<string, string>();
            for (int j = 0; j < properties.Length; j++)
                objResult.Add(properties[j], csv[i][j]);

            listObjResult.Add(objResult);
        }

        return JsonConvert.SerializeObject(listObjResult);
    }


    public void PopulateChatLogLines(TextAsset Lines)
    {
        ChatLog.Clear();

        string[] ChatLines = Lines.text.Split('\n');

        List<string> People = new List<string>();

        for(int i = 0; i < ChatLines.Length; i++) 
        {
            if (i == 0)
            {
                
                continue;
            }

            ChatLineTimeChunk NewTimeChunk;

            ChatLogLines.Add(ChatLines[i]);
        }
    }
}
