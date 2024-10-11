using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

using ChoETL;
using Newtonsoft.Json.Linq;
using NNarrativeDataTypes;
using NCharacterTraitCategoryTypes;
using AYellowpaper.SerializedCollections;

public class CS_CharacterListBuilder : MonoBehaviour
{
    [SerializeField]
    [ReadOnly]
    private List<NNarrativeDataTypes.FCharacterData> CharacterData;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void PopulateCharacterList (TextAsset InLines, JArray InCharacterListArray)
    {
        if (InLines.IsObjectNullOrEmpty() || InLines.text.IsNullOrEmpty())
        {
            Debug.LogError("ERROR: InLines can not be null or empty!");
            return;
        }

        CS_CharacterTraits CharacterTraitComponent = gameObject.GetComponent<CS_CharacterTraits>();

        if(CharacterTraitComponent == null)
        {
            Debug.LogError("CS_CharacterListBuilder::PopulateCharacterList --> No CS_CharacterTraits component found!");
            return;
        }

        int CharacterId = 0;
        CharacterData.Clear();

        foreach (JObject CharacterObject in InCharacterListArray.Children())
        {
            string CharacterNameString = (string)CharacterObject["NAME"];

            if(CharacterNameString.IsNullOrEmpty())
            {
                continue;
            }

            string WebHandleString = (string)CharacterObject["WEBHANDLE"];

            string CivIdDisplay = (string)CharacterObject["CIVID"];

            string civ1str = (string)CharacterObject["CIVID1"];

            int CivId1 = int.Parse((string)CharacterObject["CIVID1"]);
            int CivId2 = int.Parse((string)CharacterObject["CIVID2"]);
            int CivId3 = int.Parse((string)CharacterObject["CIVID3"]);

            string CurrentLocationString = (string)CharacterObject["LOCATION"];
            string BirthplaceLocationString = (string)CharacterObject["BIRTHPLACE"];

            string DateOfBirthStr = (string)CharacterObject["BIRTHDATE"];

            int dd = int.Parse(DateOfBirthStr.Substring(0, 2));
            int mm = int.Parse(DateOfBirthStr.Substring(3, 2));
            int yyyy = int.Parse(DateOfBirthStr.Substring(6, 4));

            FDateHandle DoBHandle = new FDateHandle(dd, mm, yyyy);

            FCharacterWebHandle characterWebHandle = new FCharacterWebHandle(WebHandleString);

            FCivIdHandle civIdHandle = new FCivIdHandle(CivId1, CivId2, CivId3, CivIdDisplay);

            SerializedDictionary<string, FCharacterTraitId> CharacterTraits = new SerializedDictionary<string, FCharacterTraitId>();

            CharacterTraits.Add(
                "CURRENT LOCATION",
                CharacterTraitComponent.GetTrait
                (
                    NCharacterTraitCategoryTypes.ECharacterTraitCategory.ETraitCategory_LOCATION,
                    CurrentLocationString
                )
            );

            CharacterTraits.Add(
                "BIRTHPLACE",
                CharacterTraitComponent.GetTrait
                (
                    NCharacterTraitCategoryTypes.ECharacterTraitCategory.ETraitCategory_LOCATION, 
                    BirthplaceLocationString
                )
            );

            foreach (string CategoryString in CharacterTraitComponent.GetCategoryList())
            {
                if(CategoryString.Equals("LOCATION"))
                {
                    continue;
                }
                string Value = (string)CharacterObject[CategoryString];

                if(Value.IsNullOrEmpty())
                {
                    Debug.LogWarning("WARNING: Category: " + CategoryString + " Was not found in CharacterJObject!");
                    continue;
                }
                CharacterTraits.Add(CategoryString, CharacterTraitComponent.GetTrait(CharacterTraitComponent.GetCategory(CategoryString), Value));
            }

            CharacterData.Add(new FCharacterData(CharacterNameString, CharacterId, characterWebHandle, civIdHandle, DoBHandle, CharacterTraits));
            CharacterId++;
        }
    }
}
