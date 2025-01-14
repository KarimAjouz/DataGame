using ChoETL;
using NNarrativeDataTypes;
using NaughtyAttributes;
using UnityEngine;

public enum EPunchCardType
{
    PCT_NONE,
    PCT_Character,
    PCT_Trait,
    count
}

public class CS_PunchCard : MonoBehaviour
{
    [SerializeField]
    public EPunchCardType PunchCardType = EPunchCardType.PCT_NONE;
    
    [SerializeField]
    private FCharacterData StoredCharacterData;

    [SerializeField]
    [ReadOnly]
    private CS_TraitSelector StoredTrait;


    // Start is called before the first frame update
    void Start()
    {
        switch (PunchCardType)
        {
            case EPunchCardType.PCT_Character:
                if(StoredCharacterData.IsNull())
                    StoredCharacterData = new FCharacterData();
                return;
            case EPunchCardType.PCT_Trait:
                if(StoredTrait.IsNull())
                    StoredTrait = gameObject.GetComponent<CS_TraitSelector>();
                return;
        }
    }

    public void SetCharacterData(FCharacterData characterData)
    {
        StoredCharacterData = characterData;
    }

    public FCharacterData GetCharacterData()
    {
        return StoredCharacterData;
    }

    public FCharacterTraitId GetTraitId()
    {
        return StoredTrait.MakeIdFromTrait();
    }

    public string GetDisplayText()
    {
        switch (PunchCardType)
        {
            case EPunchCardType.PCT_Character:
                return StoredCharacterData.GetCharacterName();
            case EPunchCardType.PCT_Trait:
                return StoredTrait.TraitValueDisplayNames[StoredTrait.TraitId];
            default:
                return "N/A";
        }
    }
}