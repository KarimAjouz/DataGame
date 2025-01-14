using ChoETL;
using System.Collections.Generic;
using NNarrativeDataTypes;
using UnityEngine;

public class CS_SplitFlapDisplay : MonoBehaviour
{
    [SerializeField]
    [Min(1)]
    private int NumCharacters = 1;

    [SerializeField] 
    private bool IsSocketInput = false;

    [SerializeField] 
    private CS_Socket ReadSocket;

    private string InputFormat;

    private int ActiveCharIndex;

    [SerializeField]
    private string DisplayText;

    [SerializeField] 
    private string DefaultDisplayText;

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

    [SerializeField]
    private Color ActiveColour;

    [SerializeField]
    private Color DefaultColour;

    [SerializeField]
    private Color HighlightColour;

    [SerializeField]
    private GameObject CharHolder;


    // Start is called before the first frame update
    void Start()
    {
        CharacterDisplays = new List<CS_SplitFlapCharacter>(CharHolder.GetComponentsInChildren<CS_SplitFlapCharacter>());

        if (InputFormat != null)
        {
            SetDisplayText(InputFormat);
        }

        if (IsSocketInput && ReadSocket == null)
        {
            ReadSocket = GetComponentInChildren<CS_Socket>();

            if (ReadSocket == null)
            {
                Debug.LogError("Field is marked as socket input but GO has no child CS_Socket!");
            }
        }
    }


    public void RegenerateDisplay()
    {
        if(CharacterDisplays == null)
        {
            CharacterDisplays = new List<CS_SplitFlapCharacter>();
        }

        if (CharHolder == null)
        {
            CharHolder = Instantiate(new GameObject(), transform);
            CharHolder.name = "Character Displays";
        }

        List<CS_SplitFlapCharacter> DisplayChars = new List<CS_SplitFlapCharacter>(CharHolder.GetComponentsInChildren<CS_SplitFlapCharacter>());
        
        while(DisplayChars.Count > 0) 
        { 
            DestroyImmediate(DisplayChars[0].gameObject); 
            DisplayChars.RemoveAt(0);
        }

        for (int i = 0;  i < NumCharacters; i++) 
        {
            GameObject newChar = Instantiate(DisplayCharacterPrefab, CharHolder.transform);
            newChar.transform.Translate(CharOffset * (transform.localScale.x * i)); 
            CharacterDisplays.Add(newChar.GetComponent<CS_SplitFlapCharacter>());
        }
    }

    public void InitDisplay(string InInputFormat)
    {
        SetDefaultDisplayText("");
        SetInputFormatText(InInputFormat);
    }

    public void SetDisplayText(string InText)
    {
        if(InText.IsNullOrEmpty()) 
        { 
            return; 
        }
        DisplayText = InText.ToUpper();

        for (int i = 0; i < CharacterDisplays.Count; i++)
        {
            if (i < DisplayText.Length && 
                (
                    InputFormat.IsNullOrEmpty() 
                    || InputFormat.Length <= i
                    || (i < InputFormat.Length 
                        && InputFormat[i].Equals(' ')
                        )
                )
               )
            {
                int CharIndex = AvailableCharacters.IndexOf(DisplayText[i]);
                CharacterDisplays[i].GetComponent<CS_SplitFlapCharacter>().SetDisplayIndex(CharIndex);
            }
        }
    }
    
    
    private void SetInputFormatText(string InText)
    {
        InputFormat = InText;
        if (CharacterDisplays.IsNull())
            return;

        if (InText.Length >= CharacterDisplays.Count)
        {
            Debug.LogWarning("InputFormat text is as long as/longer than Display! No input will be available!");
        }

        for (int i = 0; i < CharacterDisplays.Count; i++)
        {
            int CharIndex = 0;
            if (i < (InputFormat.Length))
            {
                CharIndex = AvailableCharacters.IndexOf(InText[i]);
            }
            CharacterDisplays[i].GetComponent<CS_SplitFlapCharacter>().SetDisplayIndex(CharIndex);
        }
    }

    public void SetDefaultDisplayText(string InText)
    {
        DefaultDisplayText = InText.ToUpper();
    }

    public void ResetDisplay()
    {
        SetDisplayText(DefaultDisplayText);
    }

    public void SetActiveCharacter(char InNewChar)
    {
        if(ActiveCharIndex >= CharacterDisplays.Count)
        {
            return;
        }

        CharacterDisplays[ActiveCharIndex].SetDisplayIndex(AvailableCharacters.IndexOf(InNewChar));
    }

    public void AddInputChar(char InNewChar)
    {
        if(!AvailableCharacters.Contains(InNewChar))
        {
            Debug.LogWarning("Can't add char: " + InNewChar+ " to display as it is not an available character!");
            return;
        }
        
        DisplayText += InNewChar;

        CharacterDisplays[ActiveCharIndex].SetDisplayIndex(AvailableCharacters.IndexOf(InNewChar));
        NextChar();
    }

    public void SetDisplayActive(bool Active, bool InIsInputDisplay = false)
    {
        Color DisplayColour = DefaultColour;
        if (Active)
        {
            DisplayColour = ActiveColour;
        }
        
        foreach (CS_SplitFlapCharacter FlapCharacter in CharacterDisplays)
        {
            FlapCharacter.SetCardHighlightColour(DisplayColour);
        }

        if (InIsInputDisplay && Active)
        {
            CS_SplitFlapCharacter FlapCharacter = CharacterDisplays[0];
            FlapCharacter.SetCardHighlightColour(HighlightColour);
        
            ActiveCharIndex = 0;
        }
    }

    public void FireBackspace()
    {
        CharacterDisplays[ActiveCharIndex].SetDisplayIndex(0);
        
        DisplayText = DisplayText.Substring(0, DisplayText.Length - 1);
        
        if (ActiveCharIndex > 0) 
            PreviousChar();
    }

    public void NextChar()
    {
        CharacterDisplays[ActiveCharIndex].SetCardHighlightColour(ActiveColour);

        //Go to the next character if it's available...
        if (ActiveCharIndex < CharacterDisplays.Count)
        {
            ActiveCharIndex++;
        }
        
        // If the next character display is overridden by the input formatting then go to the next character.
        if (ActiveCharIndex < CharacterDisplays.Count && (ActiveCharIndex < InputFormat.Length && !InputFormat[ActiveCharIndex].Equals(' ')))
        {
            ActiveCharIndex++;
        }
        
        CharacterDisplays[ActiveCharIndex].SetCardHighlightColour(HighlightColour);

    }

    public void PreviousChar()
    {
        CharacterDisplays[ActiveCharIndex].SetCardHighlightColour(ActiveColour);

        if (ActiveCharIndex > 0)
        {
            ActiveCharIndex--;
        }
        
        if (ActiveCharIndex > 0 && ActiveCharIndex > InputFormat.Length || !InputFormat[ActiveCharIndex].Equals(' '))
        {
            ActiveCharIndex++;
        }

        CharacterDisplays[ActiveCharIndex].SetCardHighlightColour(HighlightColour);

    }

    public string GetInputFieldData()
    {
        bool HasFoundFinalInputChar = false;
        string OutString = "";
        for (int i = NumCharacters - 1; i >= 0; i--)
        {
            if(HasFoundFinalInputChar)
            {
                OutString = OutString.Insert(0, AvailableCharacters[CharacterDisplays[i].GetDisplayIndex()].ToString());
            }
            else
            {
                if (CharacterDisplays[i].GetDisplayIndex() > 0)
                {
                    HasFoundFinalInputChar = true;
                    OutString = OutString.Insert(0, AvailableCharacters[CharacterDisplays[i].GetDisplayIndex()].ToString());
                }
            }
        }

        return OutString;
    }
}
