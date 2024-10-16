using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CS_SplitFlapDisplay : MonoBehaviour
{
    [SerializeField]
    [Min(1)]
    private int NumCharacters = 1;

    [SerializeField]
    private bool bIsInputField = false;

    [SerializeField]
    private string DisplayText;

    [SerializeField]
    private GameObject DisplayCharacterPrefab;

    [SerializeField]
    public string AvailableCharacters;

    [SerializeField]
    private Vector3 CharOffset;

    private List<GameObject> CharacterDisplays;

    [SerializeField]
    public AudioSource ReleaseAudio;

    [SerializeField]
    public AudioSource CatchAudio;

    [SerializeField]
    public CS_SplitFlapDisplayFontHolder FontHolder;

    private float runningClock = 5.0f;
    private bool ShouldTest = true;


    // Start is called before the first frame update
    void Start()
    {
        CharacterDisplays = new List<GameObject>();
        foreach (Transform child in transform)
            CharacterDisplays.Add(child.gameObject);

    }

    private void Update()
    {

        if (runningClock < 0.0f && ShouldTest)
        {
            SetDisplayText(DisplayText);
            //runningClock = 2.0f;
            ShouldTest = false;
        }
        else
            runningClock -= Time.deltaTime;
    }

    public void RegenerateDisplay()
    {
        List<CS_SplitFlapCharacter> DisplayChars = new List<CS_SplitFlapCharacter>(GetComponentsInChildren<CS_SplitFlapCharacter>());
        if(CharacterDisplays == null)
        {
            CharacterDisplays = new List<GameObject>();
        }

        while(DisplayChars.Count > 0) 
        { 
            DestroyImmediate(DisplayChars[0].gameObject); 
            DisplayChars.RemoveAt(0);
        }

        for (int i = 0;  i < NumCharacters; i++) 
        {
            GameObject newChar = Instantiate(DisplayCharacterPrefab, this.transform);
            newChar.transform.Translate(CharOffset * transform.localScale.x * i); 
            CharacterDisplays.Add(newChar);
        }
    }

    public void SetDisplayText(string InText)
    {
        for (int i = 0; i < CharacterDisplays.Count; i++)
        {
            if(i < InText.Length)
            {
                int CharIndex = AvailableCharacters.IndexOf(InText[i]);
                CharacterDisplays[i].GetComponent<CS_SplitFlapCharacter>().SetDisplayIndex(CharIndex);
            }
        }
    }
}
