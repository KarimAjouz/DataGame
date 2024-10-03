using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Rendering.Universal;
using UnityEngine;

public class CS_NarrativeImporter : MonoBehaviour
{

}


[CustomEditor(typeof(CS_NarrativeImporter))]
public class CS_NarrativeImporterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Import all Chatlogs From CSV"))
        {
            ImportChatlogsFromCSVs();
        }
    }

    private void ImportChatlogsFromCSVs()
    {
        CS_ChatLogBuilder ChatlogBuilder = GameObject.FindObjectOfType<CS_ChatLogBuilder>();
        if (ChatlogBuilder == null)
        {
            Debug.LogError("Could not find NarrativeImporter scrtipt!");
            return;
        }

        if(ChatlogBuilder.TestChatlogCSV == null)
        {
            Debug.LogWarning("No CSV is defined!");
            return;
        }

        ChatlogBuilder.PopulateChatLogLines(ChatlogBuilder.TestChatlogCSV);
    }
}
