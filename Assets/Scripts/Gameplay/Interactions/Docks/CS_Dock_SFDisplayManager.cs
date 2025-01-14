using AYellowpaper.SerializedCollections;
using ChoETL;
using NCharacterTraitCategoryTypes;
using NNarrativeDataTypes;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;


[System.Serializable]
struct FDisplayTextData
{
    [SerializeField]
    public string InputPrompt;

    [SerializeField]
    public string FormatString;
}

[System.Serializable]
class CS_DataInputPage
{
    [SerializeField]
    string PageDisplayName;

    [SerializeField]
    ECharacterDataPageType PageType;

    [SerializeField]
    [SerializedDictionary ("Category", "DisplayRef")]
    SerializedDictionary<ECharacterTraitCategory, CS_PromptDisplayHandler> CategoryToDisplayMap;

    [SerializeField]
    [SerializedDictionary("Category", "InputPrompt")]
    SerializedDictionary<ECharacterTraitCategory, FDisplayTextData> CategoryToPromptMap;

    [SerializeField]
    private int ActiveDisplay;

    public CS_SplitFlapDisplay GetActiveDisplay()
    {
        return CategoryToDisplayMap.Values.Count > ActiveDisplay ? (CategoryToDisplayMap.GetValueAt(ActiveDisplay) as CS_PromptDisplayHandler)?.GetInputDisplay() : null;
    }

    public int GetDisplayCount()
    {
        return CategoryToDisplayMap.Count;
    }

    public ECharacterTraitCategory GetDisplayCategory(CS_PromptDisplayHandler InDisplay)
    {
        foreach (KeyValuePair<ECharacterTraitCategory, CS_PromptDisplayHandler> pair in CategoryToDisplayMap)
        {
            if(pair.Value == InDisplay)
                return pair.Key;
        }

        return ECharacterTraitCategory.ETraitCategory_NONE;
    }
    
    public void InitPage()
    {
        foreach (KeyValuePair<ECharacterTraitCategory, CS_PromptDisplayHandler> pair in CategoryToDisplayMap)
        {
            pair.Value.InitDisplay(CategoryToPromptMap[pair.Key].InputPrompt, CategoryToPromptMap[pair.Key].FormatString);
        } 
    }

    public void ResetPage()
    {
        foreach (KeyValuePair<ECharacterTraitCategory, CS_PromptDisplayHandler> pair in CategoryToDisplayMap)
        {
            pair.Value.ResetDisplay();
        }
    } 

    public void NextDisplay()
    {
        GetActiveDisplay().SetDisplayActive(false);
        ActiveDisplay++;
        ActiveDisplay = ActiveDisplay % GetDisplayCount();
        GetActiveDisplay().SetDisplayActive(true);
    }

    public void PreviousDisplay()
    {
        GetActiveDisplay().SetDisplayActive(false);
        ActiveDisplay--;
        ActiveDisplay +=  GetDisplayCount();
        ActiveDisplay %= GetDisplayCount();
        GetActiveDisplay().SetDisplayActive(true);
    }

    public void PopulateCharacterData(ref FCharacterData InOutData)
    {
        if(PageType == ECharacterDataPageType.ECharacterDataPageType_IDENTIFIER)
        {
            foreach (KeyValuePair<ECharacterTraitCategory, CS_PromptDisplayHandler> Pair in CategoryToDisplayMap)
            {
                CS_SplitFlapDisplay InputDisplay = Pair.Value.GetInputDisplay();
                switch (Pair.Key)
                {
                    case ECharacterTraitCategory.ETraitCategory_NAME when !InputDisplay.GetInputFieldData().IsEmpty():
                        InOutData.SetCharName(InputDisplay.GetInputFieldData());
                        break;
                    case ECharacterTraitCategory.ETraitCategory_WEBHANDLE when !InputDisplay.GetInputFieldData().IsEmpty():
                        InOutData.SetWebHandle(new FCharacterWebHandle(InputDisplay.GetInputFieldData()));
                        break;
                    case ECharacterTraitCategory.ETraitCategory_WEBID when !InputDisplay.GetInputFieldData().IsEmpty():
                    {
                        int Seg1 = int.Parse(InputDisplay.GetInputFieldData());
                        InOutData.SetWebId(new FWebIdHandle(Seg1, InputDisplay.GetInputFieldData()));
                        break;
                    }
                    case ECharacterTraitCategory.ETraitCategory_BIRTHDATE when !InputDisplay.GetInputFieldData().IsEmpty():
                        InOutData.SetDateOfBirth(new FDateHandle(InputDisplay.GetInputFieldData()));
                        break;
                    default:
                        FCharacterTraitId TraitId = Pair.Value.ReadTraitFromInputSocket();
                        if (!TraitId.DisplayName.IsNullOrEmpty())
                        {
                            InOutData.GetTraits().Add(Pair.Key, TraitId);
                        }
                        break;
                }
            }
        }

    }

    public void PopulateInputFields(FCharacterData InData)
    {
        if (PageType == ECharacterDataPageType.ECharacterDataPageType_IDENTIFIER)
        {
            foreach (KeyValuePair<ECharacterTraitCategory, CS_PromptDisplayHandler> Pair in CategoryToDisplayMap)
            {
                CS_SplitFlapDisplay InputDisplay = Pair.Value.GetInputDisplay();

                switch (Pair.Key)
                {
                    case ECharacterTraitCategory.ETraitCategory_WEBHANDLE when !InData.GetWebHandle().IsEmpty():
                    {
                        string DisplayText = InData.GetCharacterName();
                        InputDisplay.SetDisplayText(DisplayText);
                        InputDisplay.SetDefaultDisplayText(DisplayText);
                        break;
                    }
                    case ECharacterTraitCategory.ETraitCategory_WEBID when !InData.GetWebId().IsEmpty():
                    {
                        string DisplayText = InData.GetWebId().GetDisplayName();
                        InputDisplay.SetDisplayText(DisplayText);
                        InputDisplay.SetDefaultDisplayText(DisplayText);
                        break;
                    }
                    case ECharacterTraitCategory.ETraitCategory_BIRTHDATE when !InData.GetDateOfBirth().IsEmpty():
                    {
                        string DisplayText = InData.GetDateOfBirth().GetDisplayName();
                        InputDisplay.SetDisplayText(DisplayText);
                        InputDisplay.SetDefaultDisplayText(DisplayText);
                        break;
                    }
                }
            }
        }

    }

}


public class CS_Dock_SFDisplayManager : MonoBehaviour
{
    [SerializeField]
    private List<CS_DataInputPage> ControlledDisplayPages;

    [SerializeField]
    private int ActivePage = 0;

    [SerializeField] 
    private CS_ProfileInput InputSockets;

    private bool ComponentActive = false;

    // Start is called before the first frame update
    void Start()
    {
        InitialiseControlledDisplays();
    }

    // Update is called once per frame
    void Update()
    {
        if(ComponentActive)
        {
            ProcessInputString();

            if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                ControlledDisplayPages[ActivePage].PreviousDisplay();
            }

            if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                ControlledDisplayPages[ActivePage].NextDisplay();
            }

            if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                ControlledDisplayPages[ActivePage].GetActiveDisplay().PreviousChar();
            }

            if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                ControlledDisplayPages[ActivePage].GetActiveDisplay().NextChar();
            }
        }
    }

    private void InitialiseControlledDisplays()
    {
        if(ControlledDisplayPages.Count < ActivePage)
        {
            Debug.LogWarning("Trying to access control display page: " + ActivePage + " Where it doesn't exist!");
            return;
        }

        ControlledDisplayPages[ActivePage].InitPage();

    }

    private void NextDisplay()
    {
        ControlledDisplayPages[ActivePage].NextDisplay();
    }

    private void PreviousDisplay()
    {
        ControlledDisplayPages[ActivePage].PreviousDisplay();
    }

    public void OnDockEnter()
    {
        ComponentActive = true;
        ControlledDisplayPages[ActivePage].GetActiveDisplay().SetDisplayActive(true);
    }

    public void OnDockExit()
    {
        ComponentActive = false;
        ControlledDisplayPages[ActivePage].GetActiveDisplay().SetDisplayActive(false);
    }

    public void ClearScreen()
    {
        ControlledDisplayPages[ActivePage].ResetPage();
    }
    
    public string GetReadSocketDisplayStringForDisplay(CS_PromptDisplayHandler InDisplayHandler)
    {
        if (InputSockets)
        {
            return InputSockets.GetDisplayStringForCategoryFromReadSocket(ControlledDisplayPages[ActivePage].GetDisplayCategory(InDisplayHandler));
        }
        return "";
    }


    private void ProcessInputString()
    {
        string InputString = Input.inputString;
        while (!InputString.IsEmpty())
        {
            
            //First we process all instances of backspace inside the string.
            while (InputString.Contains("\b", 0))
            {
                int bspInd = InputString.IndexOf("\b");
                if(bspInd == 0)
                {
                    ControlledDisplayPages[ActivePage].GetActiveDisplay().FireBackspace();

                    if (InputString.Length > 2)
                        InputString = InputString.Substring(2, InputString.Length - 2);
                    else
                        break;
                }
                else
                {
                    InputString = InputString.Remove(bspInd - 1, 3);
                }
            }

            // Then process all instances of the 'Return' key.
            if(InputString.Length > 1 && InputString.Substring(0, 2) == "\n")
            {
                NextDisplay();
                InputString = InputString.Remove(0, 2);
                //continue;
            }
            
            string NextChar = InputString.Substring(0, 1);
            if (NextChar.IsAlpha())
            {
                NextChar = NextChar.ToUpper();
            }
            
            ControlledDisplayPages[ActivePage].GetActiveDisplay().AddInputChar(NextChar[0]);
            InputString = InputString.Remove(0, 1);
        }
    }

    public FCharacterData MakeCharacterDataFromDisplays()
    {
        FCharacterData OutChar = new FCharacterData();
        OutChar.InitTraits();

        ControlledDisplayPages[ActivePage].PopulateCharacterData(ref OutChar);

        return OutChar;
    }

    public void ReadCharacterDataToDisplays(FCharacterData CharacterData)
    {
        ControlledDisplayPages[ActivePage].PopulateInputFields(CharacterData);
    }
}
