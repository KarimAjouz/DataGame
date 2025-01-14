using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


using ChoETL;
using Newtonsoft.Json.Linq;
using System.Linq;
using Object = UnityEngine.Object;

public class CS_NarrativeImporter : MonoBehaviour
{
    [SerializeField] 
    public string ChatLogFolderPath;
    
    [SerializeField]
    public Object SourceCharacterTraitSheetCSV;
    
    [SerializeField]
    public Object SourceCharacterSheetCSV;
}


[CustomEditor(typeof(CS_NarrativeImporter))]
public class CS_NarrativeImporterEditor : Editor
{
    private string WorkingText;
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        CS_NarrativeImporter importer = (CS_NarrativeImporter)target;
        BeginChatLogImporter(importer);

        BeginCharacterDataImporter(importer);
        serializedObject.ApplyModifiedProperties();

    }
    
    private void ImportSingleLogFromCSV(TextAsset InSourceCSV)
    {
        CS_ChatLogBuilder ChatLogBuilder = GameObject.FindFirstObjectByType<CS_ChatLogBuilder>();
        if (ChatLogBuilder == null)
        {
            Debug.LogError("Could not find CS_ChatLogBuilder scrtipt!");
            return;
        }
        
        string ChatLogJsonString = ConvertCsvFileToJsonString(InSourceCSV);
        JArray ChatLogArray = GetJArrayFromJSON(ChatLogJsonString);

        ChatLogBuilder.PopulateChatLogLinesFromTextAsset(InSourceCSV, ChatLogArray);
    }

    private void BuildTextFileFromFileInfo(FileInfo InFileInfo)
    {
        char[] result;
        StringBuilder builder = new StringBuilder();

        using (StreamReader reader = File.OpenText(InFileInfo.FullName))
        {
            result = new char[reader.BaseStream.Length];
            reader.Read(result, 0, (int)reader.BaseStream.Length);
        }

        foreach (char c in result)
        {
            if (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || char.IsPunctuation(c))
            {
                builder.Append(c);
            }
        }
        WorkingText = builder.ToString();
    }

    private void ImportChatLogFromWorkingText(string InResourcePath, string InFileName, int InChatId)
    {
        CS_ChatLogBuilder ChatLogBuilder = GameObject.FindFirstObjectByType<CS_ChatLogBuilder>();
        if (ChatLogBuilder == null)
        {
            Debug.LogError("Could not find CS_ChatLogBuilder script!");
            return;
        }
        
        string ChatLogJsonString = ConvertCsvStringToJsonString(WorkingText, InResourcePath + "/JSONs/" + InFileName);
        
        JArray ChatLogArray = GetJArrayFromJSON(ChatLogJsonString);

        ChatLogBuilder.PopulateChatLogLinesFromString(WorkingText, ChatLogArray, InChatId);
    }
    
    private void ImportChatlogsFromCSVs(TextAsset InSourceCSV)
    {
        CS_ChatLogBuilder ChatLogBuilder = GameObject.FindFirstObjectByType<CS_ChatLogBuilder>();
        if (ChatLogBuilder == null)
        {
            Debug.LogError("Could not find CS_ChatLogBuilder scrtipt!");
            return;
        }

        string ChatLogJsonString = ConvertCsvFileToJsonString(InSourceCSV);
        JArray ChatLogArray = GetJArrayFromJSON(ChatLogJsonString);

        ChatLogBuilder.PopulateChatLogLinesFromTextAsset(InSourceCSV, ChatLogArray);
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

        CharacterListBuilder.PopulateCategoryAndTraitMaps(InSourceCSV);
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

    public void BeginChatLogImporter(CS_NarrativeImporter Importer)
    {
        EditorGUILayout.BeginHorizontal();
        
        Importer.ChatLogFolderPath = EditorGUILayout.TextField(Importer.ChatLogFolderPath);
        
        if (GUILayout.Button("Import all Chatlogs From CSV"))
        {
            CS_DynamicChatManager DynamicChatManager = FindFirstObjectByType<CS_DynamicChatManager>();

            if (DynamicChatManager == null)
            {
                Debug.LogError("Could not find CS_DynamicChatManager script!");
                return;
            }
            
            CS_ChatLogBuilder ChatLogBuilder = FindFirstObjectByType<CS_ChatLogBuilder>();
            if (ChatLogBuilder == null)
            {
                Debug.LogError("Could not find CS_ChatLogBuilder scrtipt!");
                return;
            }
            
            var DirInfo = new DirectoryInfo(Importer.ChatLogFolderPath + "/CSVs");
            var DirFiles = DirInfo.GetFiles();

            int TrackingId = 0;

            foreach (FileInfo fInfo in DirFiles)
            {
                if (fInfo.Extension == ".meta")
                {
                    continue;
                }


                BuildTextFileFromFileInfo(fInfo);
                ImportChatLogFromWorkingText(Importer.ChatLogFolderPath, fInfo.Name, TrackingId);
                TrackingId++;
            }

            DynamicChatManager.BuildChatEvents(ref ChatLogBuilder);
        }

        EditorGUILayout.EndHorizontal();
    }

    public void BeginCharacterDataImporter(CS_NarrativeImporter Importer)
    {
        EditorGUILayout.BeginHorizontal();
        
        Importer.SourceCharacterTraitSheetCSV = EditorGUILayout.ObjectField(Importer.SourceCharacterTraitSheetCSV, typeof(TextAsset), false);
        
        if (GUILayout.Button("Import all Character Traits From CSV"))
        {
            ImportCharacterTraitDataFromCSV((TextAsset)Importer.SourceCharacterTraitSheetCSV);
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        Importer.SourceCharacterSheetCSV = EditorGUILayout.ObjectField(Importer.SourceCharacterSheetCSV, typeof(TextAsset), false);

        if (GUILayout.Button("Import all Character Sheets From CSV"))
        {
            ImportCharacterListDataFromCSV((TextAsset)Importer.SourceCharacterSheetCSV);
        }

        EditorGUILayout.EndHorizontal();
    }
    /// #END: Editor button tooling.
    /// 

    /// #BEGIN: CSV Data Importing Logic
    
    
    public string ConvertCsvStringToJsonString(string Lines, string InJsonPath)
    {
        var CSVLines = Lines;

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

        GenerateJSONLog(sb.ToString(), InJsonPath);

        return sb.ToString();
    }

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
        string path = InAssetPath.Split(".")[0] + "_JSON" + ".txt";
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
