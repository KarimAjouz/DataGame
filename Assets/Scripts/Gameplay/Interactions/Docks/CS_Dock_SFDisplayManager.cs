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
    SerializedDictionary<ECharacterTraitCategory, CS_SplitFlapDisplay> CategoryToDisplayMap;

    [SerializeField]
    [SerializedDictionary("Category", "InputPrompt")]
    SerializedDictionary<ECharacterTraitCategory, FDisplayTextData> CategoryToPromptMap;

    [SerializeField]
    private int ActiveDisplay;

    public CS_SplitFlapDisplay GetActiveDisplay()
    {
        return CategoryToDisplayMap.Values.Count > ActiveDisplay ? CategoryToDisplayMap.GetValueAt(ActiveDisplay) as CS_SplitFlapDisplay : null;
    }

    public int GetDisplayCount()
    {
        return CategoryToDisplayMap.Count;
    }

    public ECharacterTraitCategory GetDisplayCategory(CS_SplitFlapDisplay InDisplay)
    {
        foreach (KeyValuePair<ECharacterTraitCategory, CS_SplitFlapDisplay> pair in CategoryToDisplayMap)
        {
            if(pair.Value == InDisplay)
                return pair.Key;
        }

        return ECharacterTraitCategory.ETraitCategory_NONE;
    }
    
    public void InitPage()
    {
        foreach (KeyValuePair<ECharacterTraitCategory, CS_SplitFlapDisplay> pair in CategoryToDisplayMap)
        {
            pair.Value.InitDisplay(true, CategoryToPromptMap[pair.Key].InputPrompt, CategoryToPromptMap[pair.Key].FormatString);
        } 
    }

    public void ResetPage()
    {
        foreach (KeyValuePair<ECharacterTraitCategory, CS_SplitFlapDisplay> pair in CategoryToDisplayMap)
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
            foreach (KeyValuePair<ECharacterTraitCategory, CS_SplitFlapDisplay> Pair in CategoryToDisplayMap)
            {
                if(Pair.Key == ECharacterTraitCategory.ETraitCategory_NAME && !Pair.Value.GetInputFieldData().IsEmpty())
                {
                    InOutData.SetCharName(Pair.Value.GetInputFieldData());
                }

                if (Pair.Key == ECharacterTraitCategory.ETraitCategory_WEBHANDLE && !Pair.Value.GetInputFieldData().IsEmpty())
                {
                    InOutData.SetWebHandle(new FCharacterWebHandle(Pair.Value.GetInputFieldData()));
                }

                if (Pair.Key == ECharacterTraitCategory.ETraitCategory_WEBID && !Pair.Value.GetInputFieldData().IsEmpty())
                {
                    int Seg1 = int.Parse(Pair.Value.GetInputFieldData());
                    InOutData.SetWebId(new FWebIdHandle(Seg1, Pair.Value.GetInputFieldData()));
                }

                if (Pair.Key == ECharacterTraitCategory.ETraitCategory_BIRTHDATE && !Pair.Value.GetInputFieldData().IsEmpty())
                {
                    InOutData.SetDateOfBirth(new FDateHandle(Pair.Value.GetInputFieldData()));
                }
            }
        }

    }

    public void PupulateInputFields(FCharacterData InData)
    {
        if (PageType == ECharacterDataPageType.ECharacterDataPageType_IDENTIFIER)
        {
            foreach (KeyValuePair<ECharacterTraitCategory, CS_SplitFlapDisplay> Pair in CategoryToDisplayMap)
            {
                if (Pair.Key == ECharacterTraitCategory.ETraitCategory_NAME && !InData.GetCharacterName().IsNullOrEmpty())
                {
                    Pair.Value.SetInputFieldData(InData.GetCharacterName());
                }

                if (Pair.Key == ECharacterTraitCategory.ETraitCategory_WEBHANDLE && !InData.GetWebHandle().IsEmpty())
                {
                    Pair.Value.SetInputFieldData(InData.GetWebHandle().GetDisplayName());
                }

                if (Pair.Key == ECharacterTraitCategory.ETraitCategory_WEBID && !InData.GetWebId().IsEmpty())
                {
                    Pair.Value.SetInputFieldData(InData.GetWebId().GetDisplayName());
                }

                if (Pair.Key == ECharacterTraitCategory.ETraitCategory_BIRTHDATE && !InData.GetDateOfBirth().IsEmpty())
                {
                    Pair.Value.SetInputFieldData(InData.GetDateOfBirth().GetDisplayName());

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
    
    public string GetReadSocketDisplayStringForDisplay(CS_SplitFlapDisplay InDisplay)
    {
        if (InputSockets)
        {
            return InputSockets.GetDisplayStringForCategoryFromReadSocket(ControlledDisplayPages[ActivePage].GetDisplayCategory(InDisplay));
        }
        return "";
    }


    private void ProcessInputString()
    {
        string InputString = Input.inputString.ToUpper();
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

            if(InputString.Length > 1 && InputString.Substring(0, 2) == "\n")
            {
                NextDisplay();
                InputString = InputString.Remove(0, 2);
            }

            ControlledDisplayPages[ActivePage].GetActiveDisplay().AddInputChar(InputString[0]);
            InputString = InputString.Remove(0, 1);
        }
    }

    public FCharacterData MakeCharacterDataFromDisplays()
    {
        FCharacterData OutChar = new FCharacterData();

        ControlledDisplayPages[ActivePage].PopulateCharacterData(ref OutChar);

        return OutChar;
    }

    public void ReadCharacterDataToDisplays(FCharacterData CharacterData)
    {
        ControlledDisplayPages[ActivePage].PupulateInputFields(CharacterData);
    }
}
