using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class CS_SplitFlapDisplayBuilder : MonoBehaviour
{
}



[CustomEditor(typeof(CS_SplitFlapDisplayBuilder))]
class CS_SplitFlapDisplayBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Generate Display"))
        {
            CS_SplitFlapDisplay DisplayComp = ((MonoBehaviour)target).GetComponent<CS_SplitFlapDisplay>();

            if (DisplayComp == null)
            {
                Debug.LogError("DisplayComp is invalid!");
                return;
            }
            
            DisplayComp.RegenerateDisplay();
        }

        EditorGUILayout.EndHorizontal();
    }
}