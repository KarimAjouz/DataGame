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
        if (GUILayout.Button("Import From CSV"))
        {
            ImportFromCSV();
        }
    }

    private void ImportFromCSV()
    {
        CS_StoryManager StoryManager = GameObject.FindObjectOfType<CS_StoryManager>();
        if (StoryManager == null)
        {
            Debug.LogError("Could not find NarrativeImporter scrtipt!");
            return;
        }

        if(StoryManager.TestChatlogCSV == null)
        {
            Debug.LogWarning("No CSV is defined!");
            return;
        }
        
        StoryManager.PopulateChatLogLines(StoryManager.TestChatlogCSV);
    }
}
