using AYellowpaper.SerializedCollections;
using ChoETL;
using NaughtyAttributes;
using NCharacterTraitCategoryTypes;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

namespace NCharacterTraitCategoryTypes
{

    // Where all the individual trait categories come from - we will build character data pages from this information.
    public enum ECharacterTraitCategory
    {
        ETraitCategory_NONE,
        ETraitCategory_NAME,
        ETraitCategory_WEBHANDLE,
        ETraitCategory_WEBID,
        ETraitCategory_CURRENTLOCATION,
        ETraitCategory_BIRTHPLACE,
        ETraitCategory_BIRTHDATE,
        ETraitCategory_EDUCATION,
        ETraitCategory_OCCUPATIONSTATUS,
        ETraitCategory_OCCUPATIONFIELD,
        ETraitCategory_GENDERIDENTITY,
        ETraitCategory_SEXUALITY,
        COUNT
    }

    public enum ECharacterTraitType
    {
        ETraitType_NONE,
        ETraitType_EDUCATION,
        ETraitType_OCCUPATIONSTATUS,
        ETraitType_OCCUPATIONFIELD,
        ETraitType_GENDERIDENTITY,
        ETraitType_SEXUALITY,
        ETraitType_LOCATION,
        COUNT
    }

    // The types of character data pages that players need to collect.
    public enum ECharacterDataPageType
    {
        ECharacterDataPageType_IDENTIFIER,
        ECharacterDataPageType_PERSONALITY,
        COUNT
    }

    [System.Serializable]
    public struct FCharacterTraitId
    {
        [SerializeField]
        [ReadOnly]
        public string DisplayName;

        private int Id;


        public FCharacterTraitId(string InDisplayName, int InIId)
        {
            DisplayName = InDisplayName;
            Id = InIId;
        }
        public override bool Equals(object obj)
        {
            return obj is FCharacterTraitId InItem &&
                   Id == InItem.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public static bool operator ==(FCharacterTraitId left, FCharacterTraitId right)
        {
            return
                left.Id == right.Id;
        }

        public static bool operator !=(FCharacterTraitId left, FCharacterTraitId right)
        {
            return !(left == right);
        }
    }

    [System.Serializable]
    public struct FCharacterTraitDictionary
    {
        [SerializeField]
        [ReadOnly]
        [SerializedDictionary ("CategoryID", "Items")]
        private SerializedDictionary<ECharacterTraitType, List<FCharacterTraitId>> CharacterTraitsDictionary;

        [SerializeField]
        [ReadOnly]
        [SerializedDictionary("DisplayName", "CategoryID")]
        private SerializedDictionary<string, ECharacterTraitType> StringToTraitType;


        public FCharacterTraitDictionary(int n)
        {
            CharacterTraitsDictionary = new SerializedDictionary<ECharacterTraitType, List<FCharacterTraitId>>();
            StringToTraitType = new SerializedDictionary<string, ECharacterTraitType>();
        }

        public void AddUniqueItem(ECharacterTraitType InType, string TypeString, FCharacterTraitId ItemId)
        {
            if(!CharacterTraitsDictionary.ContainsKey(InType))
            {
                CharacterTraitsDictionary.Add(InType, new List<FCharacterTraitId>());
                StringToTraitType.Add(TypeString, InType);
                
            }

            if (!CharacterTraitsDictionary[InType].Contains(ItemId))
            {
                CharacterTraitsDictionary[InType].Add(ItemId);
            }
        }

        public List<string> GetAllTypeNames()
        {
            return new List<string>(StringToTraitType.Keys);
        }

        public bool HasCategory(ECharacterTraitType InCategory)
        {
            return CharacterTraitsDictionary.ContainsKey(InCategory);
        }

        public List<FCharacterTraitId> GetTraitsForCategory(ECharacterTraitType InCategory)
        {
            return CharacterTraitsDictionary[InCategory];
        }

        public FCharacterTraitId FindTrait(ECharacterTraitType InType, string InString)
        {
            if(!HasCategory(InType))
            {
                Debug.LogError("Trait Type: " + InType + " Does not exist in the trait dictionary!");
                return new FCharacterTraitId();
            }

            foreach (FCharacterTraitId Trait in CharacterTraitsDictionary[InType])
            {
                if(Trait.DisplayName == InString)
                {
                    return Trait;
                }
            }
            return new FCharacterTraitId();
        }

        public ECharacterTraitType GetTypeFromName(string TypeName)
        {
            if(!StringToTraitType.ContainsKey(TypeName))
            {
                Debug.LogError("FCharacterTraitId::GetCategoryFromName() --> StringToTraitCategory Dictionary does not contain key: " + TypeName);
                return ECharacterTraitType.ETraitType_NONE;
            }
            return StringToTraitType[TypeName];
        }

        public void ResetMap()
        {
            if(!CharacterTraitsDictionary.IsNull())
            {
                CharacterTraitsDictionary.Clear();
            }
            else
            {
                CharacterTraitsDictionary = new SerializedDictionary<ECharacterTraitType, List<FCharacterTraitId>>();
            }

            if(!StringToTraitType.IsNull())
            {
                StringToTraitType.Clear();
            }
            else
            {
                StringToTraitType = new SerializedDictionary<string, ECharacterTraitType>();
            }
        }
    }
}



public class CS_CharacterTraits : MonoBehaviour
{
    [ReadOnly]
    [SerializeField]
    public FCharacterTraitDictionary TraitDictionary;


    // Start is called before the first frame update
    void Start()
    {
        TraitDictionary = new FCharacterTraitDictionary(0);
    }

    public void PopulateCharacterTraits(TextAsset InSourceCSV, JArray CharacterTraitArray)
    {
        if(TraitDictionary.IsNull())
        {
            TraitDictionary = new FCharacterTraitDictionary(0);
        }

        TraitDictionary.ResetMap();

        string CategoryRow = InSourceCSV.text.Split("\n")[0];
        string[] CategoriesInCSV = CategoryRow.Split(",");

        foreach (JObject CharacterTraitBlock in CharacterTraitArray) 
        {
            bool bEmptyRow = true;
            int ItemId = (int)CharacterTraitBlock["ID"];

            for (int i = 1; i < CategoriesInCSV.Length; ++i)
            {
                string DisplayName = (string)CharacterTraitBlock[CategoriesInCSV[i].Split("\r")[0]];

                if(DisplayName.IsNullOrEmpty())
                {
                    continue;
                }

                TraitDictionary.AddUniqueItem((ECharacterTraitType)i, CategoriesInCSV[i].Split("\r")[0], new FCharacterTraitId(DisplayName, ItemId));
                bEmptyRow = false;
            }

            if(bEmptyRow)
            {
                break;
            }
        }
    }

    public FCharacterTraitId GetTrait(ECharacterTraitType InTraitType, string Value)
    {
        return TraitDictionary.FindTrait(InTraitType, Value);
    }

    public List<string> GetTraitTypeNames()
    {
        return TraitDictionary.GetAllTypeNames();
    }
    public ECharacterTraitType GetType(string TypeName) 
    {
        return TraitDictionary.GetTypeFromName(TypeName);
    }
}
