using ChoETL;
using NNarrativeDataTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CS_PunchCard : MonoBehaviour
{
    [SerializeField]
    private FCharacterData StoredCharacterData;


    // Start is called before the first frame update
    void Start()
    {
        StoredCharacterData = new FCharacterData();
    }

    public void SetCharacterData(FCharacterData characterData)
    {
        StoredCharacterData = characterData;
    }

    public FCharacterData GetCharacterData()
    {
        return StoredCharacterData;
    }

}
