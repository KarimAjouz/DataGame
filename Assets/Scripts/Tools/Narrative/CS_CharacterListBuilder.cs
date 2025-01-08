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
    private List<FCharacterData> CharacterData;
    
    [SerializeField]
    [ReadOnly]
    [SerializedDictionary("Input Name", "Category")]
    private SerializedDictionary<string, ECharacterTraitCategory> StringToTraitCategory;
        
    [SerializeField]
    [ReadOnly]
    [SerializedDictionary("Trait Category", "Type")]
    private SerializedDictionary<ECharacterTraitCategory, ECharacterTraitType> CategoryToTypeDictionary;

    public void PopulateCategoryAndTraitMaps(TextAsset InLines)
    {
        CS_CharacterTraits CharacterTraitComponent = gameObject.GetComponent<CS_CharacterTraits>();

        if(CharacterTraitComponent == null)
        {
            Debug.LogError("CS_CharacterListBuilder::PopulateCharacterList --> No CS_CharacterTraits component found!");
            return;
        }

        StringToTraitCategory.Clear();
        CategoryToTypeDictionary.Clear();
        
        
        string CategoryRow = InLines.text.Split("\n")[0];
        string[] CategoriesInCSV = CategoryRow.Split(",");

        for(int i = 0; i < CategoriesInCSV.Length; i++)
        {
            string CatName = CategoriesInCSV[i].Split("\r")[0];
            StringToTraitCategory.Add(CatName, (ECharacterTraitCategory)i);
            
            string TypeName = CatName.Split("_")[0];
            ECharacterTraitType TraitType = CharacterTraitComponent.TraitDictionary.GetTypeFromName(TypeName);
            if(!TraitType.Equals(ECharacterTraitType.ETraitType_NONE))
            {
                CategoryToTypeDictionary.Add((ECharacterTraitCategory)i, TraitType);
            }
        }
    }

    public void PopulateCharacterList (TextAsset InLines, JArray InCharacterListArray)
    {
        if (InLines.IsObjectNullOrEmpty() || InLines.text.IsNullOrEmpty())
        {
            Debug.LogError("ERROR: InLines can not be null or empty!");
            return;
        }

        int CharacterId = 0;
        CharacterData.Clear();

        foreach (JObject CharacterObject in InCharacterListArray.Children())
        {
            CS_CharacterTraits CharacterTraitComponent = gameObject.GetComponent<CS_CharacterTraits>();

            if(CharacterTraitComponent == null)
            {
                Debug.LogError("CS_CharacterListBuilder::PopulateCharacterList --> No CS_CharacterTraits component found!");
                return;
            }
            
            string CharacterNameString = (string)CharacterObject["NAME"];

            if(CharacterNameString.IsNullOrEmpty())
            {
                continue;
            }

            string WebHandleString = (string)CharacterObject["WebHandle"];

            string WebIdDisplay = (string)CharacterObject["WebID"];

            //string civ1str = (string)CharacterObject["CIVID1"];

            int WebID = int.Parse((string)CharacterObject["WebID"]);
            //int CivId2 = int.Parse((string)CharacterObject["CIVID2"]);
            //int CivId3 = int.Parse((string)CharacterObject["CIVID3"]);

            string CurrentLocationString = (string)CharacterObject["LOCATION"];
            string BirthplaceLocationString = (string)CharacterObject["BIRTHPLACE"];

            string DateOfBirthStr = (string)CharacterObject["BIRTHDATE"];

            int dd = int.Parse(DateOfBirthStr.Substring(0, 2));
            int mm = int.Parse(DateOfBirthStr.Substring(3, 2));
            int yyyy = int.Parse(DateOfBirthStr.Substring(6, 4));

            FDateHandle DoBHandle = new FDateHandle(dd, mm, yyyy);

            FCharacterWebHandle characterWebHandle = new FCharacterWebHandle(WebHandleString);

            FWebIdHandle civIdHandle = new FWebIdHandle(WebID/*, CivId2, CivId3,*/, WebIdDisplay);

            SerializedDictionary<ECharacterTraitCategory, FCharacterTraitId> CharacterTraits = new SerializedDictionary<ECharacterTraitCategory, FCharacterTraitId>();

            //CharacterTraits.Add(
            //    ECharacterTraitCategory.ETraitCategory_CURRENTLOCATION,
            //    CharacterTraitComponent.GetTrait
            //    (
            //        NCharacterTraitCategoryTypes.ECharacterTraitType.ETraitType_LOCATION,
            //        CurrentLocationString
            //    )
            //);

            //CharacterTraits.Add(
            //    ECharacterTraitCategory.ETraitCategory_BIRTHPLACE,
            //    CharacterTraitComponent.GetTrait
            //    (
            //        NCharacterTraitCategoryTypes.ECharacterTraitType.ETraitType_LOCATION, 
            //        BirthplaceLocationString
            //    )
            //);

            foreach (string TraitCategoryName in StringToTraitCategory.Keys)
            {
                if(TraitCategoryName.Equals("LOCATION"))
                {
                    continue;
                }
                string Value = (string)CharacterObject[TraitCategoryName];

                if(Value.IsNullOrEmpty())
                {
                    Debug.LogWarning("WARNING: Category: " + TraitCategoryName + " Was not found in CharacterJObject!");
                    continue;
                }

                ECharacterTraitCategory ImportedTraitCategory = StringToTraitCategory[TraitCategoryName];
                if (ImportedTraitCategory == ECharacterTraitCategory.ETraitCategory_NAME
                    || ImportedTraitCategory == ECharacterTraitCategory.ETraitCategory_WEBID
                    || ImportedTraitCategory == ECharacterTraitCategory.ETraitCategory_BIRTHDATE
                    || ImportedTraitCategory == ECharacterTraitCategory.ETraitCategory_WEBHANDLE
                    || ImportedTraitCategory == ECharacterTraitCategory.ETraitCategory_NONE
                   )
                {
                    continue;
                }
                if(!CategoryToTypeDictionary.ContainsKey(ImportedTraitCategory))
                {
                    Debug.Log("No value exists for trait category: " + TraitCategoryName);
                    continue;
                }

                // This is wrong needs to pass in category not trait type!

                FCharacterTraitId TraitToAdd = CharacterTraitComponent.GetTraitFromStringValue(CategoryToTypeDictionary[ImportedTraitCategory], Value);
                
                CharacterTraits.Add(ImportedTraitCategory, TraitToAdd);
            }

            CharacterData.Add(new FCharacterData(CharacterNameString, CharacterId, characterWebHandle, civIdHandle, DoBHandle, CharacterTraits));
            CharacterId++;
        }
    }
    
    public FCharacterData GetCharacterIndexFromName(string Name)
    {
        foreach (FCharacterData Character in CharacterData)
        {
            if(Character.GetCharacterName().Equals(Name))
                return Character;
        }

        Debug.LogWarning("WARNING: No character matching name: " + Name + " was found! \n Consider reimporting character data sheet!");
        return new FCharacterData();
    }

    public FCharacterData? GetCharacterDataFromWebHandle(FCharacterWebHandle InWebHandle)
    {
        foreach (FCharacterData Character in CharacterData)
        {
            if (Character.GetWebHandle() == InWebHandle)
            {
                return Character;
            }
        }

        return null;
    }
}
