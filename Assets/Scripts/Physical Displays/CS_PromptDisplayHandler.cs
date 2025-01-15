using ChoETL;
using NNarrativeDataTypes;
using UnityEngine;

public class CS_PromptDisplayHandler : MonoBehaviour
{
    // #TODO [KA]: 
    [SerializeField]
    private CS_SplitFlapDisplay m_PromptDisplay;
    
    [SerializeField]
    private CS_SplitFlapDisplay m_InputDisplay;

    [SerializeField] 
    private CS_Socket ReadSocket;

    private string InputPromptText;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void SetInputPromptText(string InText)
    {
        InputPromptText = InText;
        m_PromptDisplay.SetDisplayText(InputPromptText);
    }

    public void InitDisplay(string InInputPrompt, string InInputFormat)
    {
        m_PromptDisplay.SetDisplayText(InInputPrompt);
        m_InputDisplay.InitDisplay(InInputFormat);
    }

    public void ResetDisplay()
    {
        m_InputDisplay.ResetDisplay();
    }

    public CS_SplitFlapDisplay GetInputDisplay()
    {
        return m_InputDisplay;
    }
    
    public void UpdateFromInputSocket()
    {
        GameObject PunchcardGO = ReadSocket.GetSocketedGO();
        if(PunchcardGO.IsNull())
        {
            m_InputDisplay.ResetDisplay();
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
            m_InputDisplay.SetDisplayText(Punchcard.GetDisplayText());
        }
    }

    public FCharacterTraitId ReadTraitFromInputSocket()
    {
        GameObject PunchcardGO = ReadSocket.GetSocketedGO();
        if(PunchcardGO.IsNull())
        {
            return new FCharacterTraitId();
        }

        CS_PunchCard Punchcard = PunchcardGO.GetComponent<CS_PunchCard>();

        if (Punchcard.IsNull())
        {
            return new FCharacterTraitId();
        }

        return Punchcard.PunchCardType == EPunchCardType.PCT_Trait ? Punchcard.GetTraitId() : new FCharacterTraitId();
    }

    public void ClearFromInputSocket()
    {
        CS_Dock_SFDisplayManager DisplayManager = GetComponentInParent<CS_Dock_SFDisplayManager>();
        ResetDisplay();
    }
}
