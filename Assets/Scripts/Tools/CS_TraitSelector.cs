using NCharacterTraitCategoryTypes;
using NNarrativeDataTypes;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

class CS_TraitSelector : MonoBehaviour
{
    public ECharacterTraitType TraitType;
    public int TraitId;
    public string[] TraitValueDisplayNames;
    public CS_CharacterTraits TraitComponent;

    public FCharacterTraitId MakeIdFromTrait()
    {
        return new FCharacterTraitId(TraitValueDisplayNames[TraitId], TraitType, TraitId + 1);
    }
}

[CustomEditor(typeof(CS_TraitSelector))]
class CS_TraitSelectorEditor : Editor
{
    private void RefreshTraitComponent(ref CS_TraitSelector PunchCard, Object InTraitComponentObj)
    {
        PunchCard.TraitComponent = FindFirstObjectByType<CS_CharacterTraits>();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        CS_TraitSelector TraitSelector = (CS_TraitSelector)target;

        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();
        Object TraitComponentObject = EditorGUILayout.ObjectField(TraitSelector.TraitComponent, typeof(Object) , true);

        if (GUILayout.Button("Refresh TraitComponent"))
        {
            RefreshTraitComponent(ref TraitSelector, TraitComponentObject);
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        
        if (!TraitSelector.TraitComponent)
        {
            EditorGUILayout.HelpBox("Please refresh TraitComponent to display trait options", MessageType.Warning);
            EditorGUILayout.EndVertical();
            return;
        }
        
        EditorGUILayout.BeginHorizontal();
        
        
        
        TraitSelector.TraitType = (ECharacterTraitType)EditorGUILayout.EnumPopup(TraitSelector.TraitType);
        
        
        if (TraitSelector.TraitType is ECharacterTraitType.COUNT or ECharacterTraitType.ETraitType_NONE)
        {
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.HelpBox("Select a valid trait to display trait values", MessageType.Info);
            EditorGUILayout.EndVertical();
            return;
        }
        
        TraitSelector.TraitValueDisplayNames = TraitSelector.TraitComponent.TraitDictionary.GetTraitValueDisplayNames(TraitSelector.TraitType);
        
        TraitSelector.TraitId = EditorGUILayout.Popup(TraitSelector.TraitId, TraitSelector.TraitValueDisplayNames);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();

        
        serializedObject.ApplyModifiedProperties();
        
    }
    
}