using ChoETL;
using System.Collections.Generic;
using UnityEngine;

public class CS_SplitFlapDisplay : MonoBehaviour
{
    [SerializeField]
    [Min(1)]
    private int NumCharacters = 1;

    [SerializeField]
    private bool bIsInputField = false;

    [SerializeField] 
    private bool IsSocketInput = false;

    [SerializeField] 
    private CS_Socket ReadSocket;

    private string InputPromptText;

    private string InputFormat;

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
            SetDisplayText(InputPromptText + InputFormat);

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
            newChar.transform.Translate(CharOffset * (transform.localScale.x * i)); 
            CharacterDisplays.Add(newChar.GetComponent<CS_SplitFlapCharacter>());
        }
    }

    public void InitDisplay(bool InIsInputDisplay, string InInputPrompt = null, string InInputFormat = null)
    {
        bIsInputField = InIsInputDisplay;
        if(bIsInputField)
        {
            SetInputPromptText(InInputPrompt);
            SetInputFormatText(InInputFormat);
        }
    }

    public void SetDisplayText(string InText)
    {
        if(InText.IsNullOrEmpty()) 
        { 
            return; 
        }

        if (!bIsInputField)
        {
            return;
        }
        
        SetInputPromptText(InputPromptText);

        for (int i = InputPromptText.Length; i < CharacterDisplays.Count; i++)
        {
            if (i < InText.Length && 
                (
                    InputFormat.Length == 0 
                    || (i < InputFormat.Length 
                        && InputFormat[i].Equals(' ')
                        )
                )
               )
            {
                int CharIndex = AvailableCharacters.IndexOf(InText[i]);
                CharacterDisplays[i].GetComponent<CS_SplitFlapCharacter>().SetDisplayIndex(CharIndex);
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
    public void SetInputFormatText(string InText)
    {
        InputFormat = InText;
        if (CharacterDisplays.IsNull())
            return;

        if (InText.Length + InputPromptText.Length > CharacterDisplays.Count)
        {
            Debug.LogWarning("InputPrompt text is as long as/longer than Display! No input will be available!");
        }

        if(InputFormat.Length == 0)
        {
            return;
        }

        for (int i = InputPromptText.Length; i < CharacterDisplays.Count; i++)
        {
            if (i < (InputPromptText.Length + InputFormat.Length))
            {
                int CharIndex = AvailableCharacters.IndexOf(InText[i - InputPromptText.Length]);
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

        if (ActiveCharIndex < CharacterDisplays.Count && (InputFormat.Length == 0 || ActiveCharIndex + 1 < InputFormat.Length + InputPromptText.Length))
        {
            ActiveCharIndex++;
        }

        if (ActiveCharIndex < InputFormat.Length + InputPromptText.Length && !InputFormat[ActiveCharIndex-InputPromptText.Length].Equals(' '))
        {
            ActiveCharIndex++;
        }

        CharacterDisplays[ActiveCharIndex].SetCardHighlightColour(HighlightColour);

    }

    public void PreviousChar()
    {
        CharacterDisplays[ActiveCharIndex].SetCardHighlightColour(ActiveColour);

        if (ActiveCharIndex > InputPromptText.Length)
        {
            ActiveCharIndex--;
        }

        if (InputFormat.Length != 0 && !InputFormat[ActiveCharIndex - InputPromptText.Length].Equals(' '))
        {
            ActiveCharIndex--;
        }

        CharacterDisplays[ActiveCharIndex].SetCardHighlightColour(HighlightColour);

    }

    public string GetInputFieldData()
    {
        bool HasFoundFinalInputChar = false;
        string OutString = "";
        for (int i = NumCharacters - 1; i >= InputPromptText.Length; i--)
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

    public void SetInputFieldData(string InString)
    {
        int count = 0;
        string Display = InString.ToUpper();

        if (bIsInputField)
            count = InputPromptText.Length;

        for (int i = count; i < CharacterDisplays.Count; i++)
        {
            int CharToSet = i - count < Display.Length ? AvailableCharacters.IndexOf(Display[i - count]) : 0;
            CharacterDisplays[i].GetComponent<CS_SplitFlapCharacter>().SetDisplayIndex(CharToSet);
        }
    }

    public void UpdateFromInputSocket()
    {
        GameObject PunchcardGO = ReadSocket.GetSocketedGO();
        if(PunchcardGO.IsNull())
        {            
            SetInputFieldData("");
            return;
        }

        CS_PunchCard Punchcard = PunchcardGO.GetComponent<CS_PunchCard>();

        if (Punchcard.IsNull())
        {
            Debug.LogError("Attempted to write to invalid punchcard!");
            return;
        }

        if (Punchcard.PunchCardType == EPunchCardType.PCT_Trait)
        {
            SetInputFieldData(Punchcard.GetDisplayText());
        }
    }

    public void ClearFromInputSocket()
    {
        CS_Dock_SFDisplayManager DisplayManager = GetComponentInParent<CS_Dock_SFDisplayManager>();
        SetInputFieldData(DisplayManager.GetReadSocketDisplayStringForDisplay(this));
    }
}
