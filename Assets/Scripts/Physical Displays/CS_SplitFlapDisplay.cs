using ChoETL;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class CS_SplitFlapDisplay : MonoBehaviour
{
    [SerializeField]
    [Min(1)]
    private int NumCharacters = 1;

    [SerializeField]
    private bool bIsInputField = false;

    private string InputPromptText;

    private int ActiveCharIndex;

    [SerializeField]
    private string DisplayText;

    [SerializeField]
    private GameObject DisplayCharacterPrefab;

    [SerializeField]
    public string AvailableCharacters;

    [SerializeField]
    private Vector3 CharOffset;

    private List<CS_SplitFlapCharacter> CharacterDisplays;

    [SerializeField]
    public AudioSource ReleaseAudio;

    [SerializeField]
    public AudioSource CatchAudio;

    [SerializeField]
    public CS_SplitFlapDisplayFontHolder FontHolder;

    private float runningClock = 5.0f;
    private bool ShouldTest = true;

    [SerializeField]
    private Color ActiveColour;

    [SerializeField]
    private Color DefaultColour;

    [SerializeField]
    private Color HighlightColour;


    // Start is called before the first frame update
    void Start()
    {
        CharacterDisplays = new List<CS_SplitFlapCharacter>(GetComponentsInChildren<CS_SplitFlapCharacter>());

        if (bIsInputField)
            SetDisplayText(InputPromptText);

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
            CharacterDisplays = new List<CS_SplitFlapCharacter>();
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
            CharacterDisplays.Add(newChar.GetComponent<CS_SplitFlapCharacter>());
        }
    }

    public void InitDisplay(bool InIsInputDisplay, string InInputPrompt = null)
    {
        bIsInputField = InIsInputDisplay;
        if(bIsInputField)
        {
            SetInputPromptText(InInputPrompt);
        }
    }

    public void SetDisplayText(string InText)
    {
        if(bIsInputField)
        {
            SetInputPromptText(InputPromptText);

            for (int i = InputPromptText.Length; i < CharacterDisplays.Count; i++)
            {
                if (i < InText.Length)
                {
                    int CharIndex = AvailableCharacters.IndexOf(InText[i]);
                    CharacterDisplays[i].GetComponent<CS_SplitFlapCharacter>().SetDisplayIndex(CharIndex);
                }
            }
        }
        
    }

    public void SetInputPromptText(string InText)
    {
        InputPromptText = InText;
        if (CharacterDisplays.IsNull())
            return;

        if (InText.Length >= CharacterDisplays.Count)
        {
            Debug.LogWarning("InputPrompt text is as long as/longer than Display! No input will be available!");
        }
        

        for (int i = 0; i < CharacterDisplays.Count; i++)
        {
            if (i < InputPromptText.Length)
            {
                int CharIndex = AvailableCharacters.IndexOf(InText[i]);
                CharacterDisplays[i].GetComponent<CS_SplitFlapCharacter>().SetDisplayIndex(CharIndex);
            }
        }
    }

    public void ResetDisplay()
    {
        int count = 0;

        if(bIsInputField)
            count = InputPromptText.Length;

        for (int i = count; i < CharacterDisplays.Count; i++)
        {
            CharacterDisplays[i].GetComponent<CS_SplitFlapCharacter>().SetDisplayIndex(0);
        }
    }

    public void SetActiveCharacter(char InNewChar)
    {
        if (!bIsInputField)
        {
            Debug.LogWarning("Can't set character to display that is not an input field!");
            return;
        }

        if(ActiveCharIndex >= CharacterDisplays.Count)
        {
            return;
        }

        CharacterDisplays[ActiveCharIndex].SetDisplayIndex(AvailableCharacters.IndexOf(InNewChar));
    }

    public void AddInputChar(char InNewChar)
    {
        if(!bIsInputField)
        {
            Debug.LogWarning("Can't add character to display that is not an input field!");
            return;
        }

        if(!AvailableCharacters.Contains(InNewChar))
        {
            return;
        }

        if(InputPromptText.Length + DisplayText.Length > CharacterDisplays.Count) 
        {
            CharacterDisplays[ActiveCharIndex].SetDisplayIndex(AvailableCharacters.IndexOf(InNewChar));
            NextChar();
        }
    }

    public void SetDisplayActive(bool Active) 
    {
        if(Active)
        {
            for (int i = 0; i < CharacterDisplays.Count; i++)
            {
                foreach (CS_SplitFlapCharacter FlapCharacter in CharacterDisplays)
                {
                    FlapCharacter.SetCardHighlightColour(ActiveColour);
                }
            }

            int InitialIndex = 0;
            if(bIsInputField)
            {
                InitialIndex = InputPromptText.Length;

                CS_SplitFlapCharacter FlapCharacter = CharacterDisplays[InitialIndex];
                FlapCharacter.SetCardHighlightColour(HighlightColour);
            }
            ActiveCharIndex = InitialIndex;
        }
        else
        {
            for (int i = 0; i < CharacterDisplays.Count; i++)
            {
                foreach (CS_SplitFlapCharacter FlapCharacter in CharacterDisplays)
                {
                    FlapCharacter.SetCardHighlightColour(DefaultColour);
                }
            }
        }
    }

    public void FireBackspace()
    {
        if (ActiveCharIndex >= InputPromptText.Length)
            CharacterDisplays[ActiveCharIndex].SetDisplayIndex(0);
        PreviousChar();
    }

    public void NextChar()
    {
        CharacterDisplays[ActiveCharIndex].SetCardHighlightColour(ActiveColour);

        if (ActiveCharIndex < CharacterDisplays.Count - 1)
            ActiveCharIndex++;
        CharacterDisplays[ActiveCharIndex].SetCardHighlightColour(HighlightColour);

    }

    public void PreviousChar()
    {
        CharacterDisplays[ActiveCharIndex].SetCardHighlightColour(ActiveColour);

        if (ActiveCharIndex >= InputPromptText.Length)
            ActiveCharIndex--;

        CharacterDisplays[ActiveCharIndex].SetCardHighlightColour(HighlightColour);

    }
}
