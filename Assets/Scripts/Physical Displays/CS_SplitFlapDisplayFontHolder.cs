using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_SplitFlapDisplayFontHolder : MonoBehaviour
{

    [SerializeField]
    [SerializedDictionary ("KeyCode", "Sprite")]
    private SerializedDictionary<char, Sprite> FontSprites;

    public Sprite GetSpriteForChar(char InChar)
    {
        return FontSprites[InChar];
    }
}
