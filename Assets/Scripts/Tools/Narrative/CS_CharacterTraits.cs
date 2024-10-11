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
    public enum ECharacterTraitCategory
    {
        ETraitCategory_NONE,
        ETraitCategory_EDUCATION,
        ETraitCategory_OCCUPATIONSTATUS,
        ETraitCategory_OCCUPATIONFIELD,
        ETraitCategory_GENDERIDENTITY,
        ETraitCategory_SEXUALITY,
        ETraitCategory_LOCATION,
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
        private SerializedDictionary<ECharacterTraitCategory, List<FCharacterTraitId>> CharacterTraitsDictionary;

        [SerializeField]
        [ReadOnly]
        [SerializedDictionary("DisplayName", "CategoryID")]
        private SerializedDictionary<string, ECharacterTraitCategory> StringToTraitCategory;


        public FCharacterTraitDictionary(int n)
        {
            CharacterTraitsDictionary = new SerializedDictionary<ECharacterTraitCategory, List<FCharacterTraitId>>();
            StringToTraitCategory = new SerializedDictionary<string, ECharacterTraitCategory>();
        }

        public void AddUniqueItem(ECharacterTraitCategory InCategory, string CategoryString, FCharacterTraitId ItemId)
        {
            if(!CharacterTraitsDictionary.ContainsKey(InCategory))
            {
                CharacterTraitsDictionary.Add(InCategory, new List<FCharacterTraitId>());
                StringToTraitCategory.Add(CategoryString, InCategory);
                
            }

            if (!CharacterTraitsDictionary[InCategory].Contains(ItemId))
            {
                CharacterTraitsDictionary[InCategory].Add(ItemId);
            }
        }

        public List<string> GetAllCategoryStrings()
        {
            return new List<string>(StringToTraitCategory.Keys);
        }

        public bool HasCategory(ECharacterTraitCategory InCategory)
        {
            return CharacterTraitsDictionary.ContainsKey(InCategory);
        }

        public List<FCharacterTraitId> GetTraitsForCategory(ECharacterTraitCategory InCategory)
        {
            return CharacterTraitsDictionary[InCategory];
        }

        public FCharacterTraitId FindTrait(ECharacterTraitCategory InCategory, string InString)
        {
            if(!HasCategory(InCategory))
            {
                Debug.LogError("Category: " + InCategory + " Does not exist in the trait dictionary!");
                return new FCharacterTraitId();
            }

            foreach (FCharacterTraitId Trait in CharacterTraitsDictionary[InCategory])
            {
                if(Trait.DisplayName == InString)
                {
                    return Trait;
                }
            }
            return new FCharacterTraitId();
        }

        public ECharacterTraitCategory GetCategoryFromName(string CategoryName)
        {
            if(!StringToTraitCategory.ContainsKey(CategoryName))
            {
                Debug.LogError("FCharacterTraitId::GetCategoryFromName() --> StringToTraitCategory Dictionary does not contain key: " + CategoryName);
                return ECharacterTraitCategory.ETraitCategory_NONE;
            }
            return StringToTraitCategory[CategoryName];
        }

        public void ResetMap()
        {
            if(!CharacterTraitsDictionary.IsNull())
            {
                CharacterTraitsDictionary.Clear();
            }
            else
            {
                CharacterTraitsDictionary = new SerializedDictionary<ECharacterTraitCategory, List<FCharacterTraitId>>();
            }

            if(!StringToTraitCategory.IsNull())
            {
                StringToTraitCategory.Clear();
            }
            else
            {
                StringToTraitCategory = new SerializedDictionary<string, ECharacterTraitCategory>();
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

                TraitDictionary.AddUniqueItem((ECharacterTraitCategory)i, CategoriesInCSV[i].Split("\r")[0], new FCharacterTraitId(DisplayName, ItemId));
                bEmptyRow = false;
            }

            if(bEmptyRow)
            {
                break;
            }
        }
    }

    public FCharacterTraitId GetTrait(ECharacterTraitCategory InTraitCategory, string Value)
    {
        return TraitDictionary.FindTrait(InTraitCategory, Value);
    }

    public List<string> GetCategoryList()
    {
        return TraitDictionary.GetAllCategoryStrings();
    }
    public ECharacterTraitCategory GetCategory(string CategoryName) 
    {
        return TraitDictionary.GetCategoryFromName(CategoryName);
    }
}
