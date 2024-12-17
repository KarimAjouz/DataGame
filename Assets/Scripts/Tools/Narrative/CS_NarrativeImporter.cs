using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Rendering.Universal;
using UnityEngine;


using ChoETL;
using Newtonsoft.Json.Linq;
using System.Linq;

public class CS_NarrativeImporter : MonoBehaviour
{

}


[CustomEditor(typeof(CS_NarrativeImporter))]
public class CS_NarrativeImporterEditor : Editor
{
    [SerializeField]
    public Object SourceChatlogCSV;
    
    [SerializeField]
    public Object SourceCharacterTraitSheetCSV;
    
    [SerializeField]
    public Object SourceCharacterSheetCSV;

    public override void OnInspectorGUI()
    {
        BeginChatLogImporter();

        BeginCharacterDataImporter();
    }

    private void ImportChatlogsFromCSVs(TextAsset InSourceCSV)
    {
        CS_ChatLogBuilder ChatlogBuilder = GameObject.FindObjectOfType<CS_ChatLogBuilder>();
        if (ChatlogBuilder == null)
        {
            Debug.LogError("Could not find CS_ChatLogBuilder scrtipt!");
            return;
        }

        string ChatLogJsonString = ConvertCsvFileToJsonString(InSourceCSV);
        JArray ChatLogArray = GetJArrayFromJSON(ChatLogJsonString);

        ChatlogBuilder.PopulateChatLogLines(InSourceCSV, ChatLogArray);
    }

    private void ImportCharacterListDataFromCSV(TextAsset InSourceCSV)
    {
        CS_CharacterListBuilder CharacterListBuilder = GameObject.FindObjectOfType<CS_CharacterListBuilder>();
        if (CharacterListBuilder == null)
        {
            Debug.LogError("Could not find CS_CharacterListBuilder scrtipt!");
            return;
        }

        string CharacterListJSON = ConvertCsvFileToJsonString(InSourceCSV);
        JArray CharacterListArray = GetJArrayFromJSON(CharacterListJSON);

        CharacterListBuilder.PopulateCharacterList(InSourceCSV, CharacterListArray);
    }

    private void ImportCharacterTraitDataFromCSV(TextAsset InSourceCSV)
    {
        CS_CharacterTraits CharacterTraits = GameObject.FindObjectOfType<CS_CharacterTraits>();
        if (CharacterTraits == null)
        {
            Debug.LogError("Could not find CS_CharacterListBuilder scrtipt!");
            return;
        }

        string CharacterTraitJSON = ConvertCsvFileToJsonString(InSourceCSV);
        JArray CharacterTraitArray = GetJArrayFromJSON(CharacterTraitJSON);

        CharacterTraits.PopulateCharacterTraits(InSourceCSV, CharacterTraitArray);
    }


    /// #BEGIN: Editor button tooling.

    public void BeginChatLogImporter()
    {
        EditorGUILayout.BeginHorizontal();
        SourceChatlogCSV = EditorGUILayout.ObjectField(SourceChatlogCSV, typeof(TextAsset), true);

        if (GUILayout.Button("Import all Chatlogs From CSV"))
        {
            ImportChatlogsFromCSVs((TextAsset)SourceChatlogCSV);
        }

        EditorGUILayout.EndHorizontal();
    }

    public void BeginCharacterDataImporter()
    {
        EditorGUILayout.BeginHorizontal();
        SourceCharacterTraitSheetCSV = EditorGUILayout.ObjectField(SourceCharacterTraitSheetCSV, typeof(TextAsset), true);

        if (GUILayout.Button("Import all Character Traits From CSV"))
        {
            ImportCharacterTraitDataFromCSV((TextAsset)SourceCharacterTraitSheetCSV);
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        SourceCharacterSheetCSV = EditorGUILayout.ObjectField(SourceCharacterSheetCSV, typeof(TextAsset), true);

        if (GUILayout.Button("Import all Character Sheets From CSV"))
        {
            ImportCharacterListDataFromCSV((TextAsset)SourceCharacterSheetCSV);
        }

        EditorGUILayout.EndHorizontal();
    }
    /// #END: Editor button tooling.
    /// 

    /// #BEGIN: CSV Data Importing Logic

    public string ConvertCsvFileToJsonString(TextAsset Lines)
    {
        var csv = new List<string[]>();
        var CSVLines = Lines.text;

        StringBuilder sb = new StringBuilder();
        using (var p = ChoCSVReader.LoadText(CSVLines)
            .WithFirstLineHeader()
            .QuoteAllFields()
            .MayContainEOLInData()
            .ThrowAndStopOnMissingField(false)
            )
        {
            using (var w = new ChoJSONWriter(sb))
                w.Write(p);
        }

        GenerateJSONLog(sb.ToString(), AssetDatabase.GetAssetPath(Lines));

        return sb.ToString();
    }


    public JArray GetJArrayFromJSON(string InJsonText)
    {
        if (InJsonText.IsNullOrEmpty())
        {
            Debug.LogWarning("Warning: Cannot create JObject from Empty JsonText!");
            return null;
        }
        JArray OutArray = JArray.Parse(InJsonText);

        return OutArray;
    }

    public void GenerateJSONLog(string InString, string InAssetPath)
    {
        string path = InAssetPath.Split(".")[0] + "_JSON_" + ".txt";
        // This text is added only once to the file.
        if (!File.Exists(path))
        {
            // Create a file to write to.
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.WriteLine(InString);
            }
        }
        else
        {
            File.WriteAllText(path, InString);
        }
    }

    /// #END: CSV Data Importing Logic

}
