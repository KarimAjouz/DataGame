using AYellowpaper.SerializedCollections;
using UnityEngine;

using NStoreDataTypes;

public class CS_VendingMachineManager : MonoBehaviour
{
    [SerializeField]
    private CS_SplitFlapDisplay m_DepositedCashDisplay;
    
    [SerializeField]
    private CS_SplitFlapDisplay m_KeyCodeDisplay;

    [SerializeField] 
    private int m_DepositedCashCount;

    [SerializeField] 
    private GameObject m_CashSlot;

    [SerializeField] 
    private CS_Keypad m_Keypad;

    [SerializeField] 
    private CS_StoreManager m_StoreManager;

    [SerializeField]
    private SerializedDictionary<int, FStoreItem> m_VendingMachineInventory;

    [SerializeField]
    private int m_KeypadValue;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_Keypad.OnKeyInput.AddListener(AddKeypadInput);
        m_Keypad.OnKeypadClear.AddListener(ClearKeypad);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void AddKeypadInput(int InKeypadInput)
    {
        if (m_KeypadValue > 99)
        {
            ClearKeypad();
        }
        else
        {
            m_KeypadValue *= 10;
            m_KeypadValue += InKeypadInput;
        }

        string KeypadText = m_KeypadValue.ToString();
        if (m_KeypadValue < 10)
        {
            KeypadText = KeypadText.Insert(0, "00");
        }

        if (m_KeypadValue < 100)
        {
            KeypadText = KeypadText.Insert(0, "0");
        }
        
        m_KeyCodeDisplay.SetDisplayText(KeypadText);
    }

    private void ClearKeypad()
    {
        m_KeypadValue = 0;
        m_KeyCodeDisplay.SetDisplayText("000");
    }
}
