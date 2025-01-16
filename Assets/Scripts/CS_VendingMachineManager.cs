using AYellowpaper.SerializedCollections;
using Michsky.DreamOS;
using UnityEngine;

using NStoreDataTypes;
using Unity.VisualScripting;

public class CS_VendingMachineManager : MonoBehaviour
{
    [SerializeField]
    private CS_PromptDisplayHandler m_DepositedCashDisplay;
    
    [SerializeField]
    private CS_SplitFlapDisplay m_KeyCodeDisplay;
    
    [SerializeField]
    private CS_PromptDisplayHandler m_ItemNameDisplay;
    
    [SerializeField]
    private CS_PromptDisplayHandler m_ItemPriceDisplay;
    
    [SerializeField]
    private CS_PromptDisplayHandler m_ItemCountDisplay;
    
    [SerializeField]
    private CS_PromptDisplayHandler m_TotalPriceDisplay;
    
    [SerializeField] 
    private int m_DepositedCashCount = 0;
    
    [SerializeField] 
    private int m_RequestedItemCount = 0;

    [SerializeField] 
    private int m_TotalPurchasePrice = 0;
    
    [SerializeField] 
    private CS_Socket m_CashSocket;

    [SerializeField] 
    private CS_Keypad m_Keypad;

    [SerializeField] 
    private CS_StoreManager m_StoreManager;

    [SerializeField]
    private SerializedDictionary<int, FStoreItem> m_VendingMachineInventory;

    [SerializeField]
    private int m_KeypadValue;

    [SerializeField]
    private int m_QueuedItemCode = 0;
    
    [SerializeField]
    private CS_ItemDeliverer m_ItemDeliverer;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (m_CashSocket == null)
        {
            Debug.LogError("Cash socket is null");
            return;
        }

        if (m_ItemDeliverer == null)
        {
            Debug.LogError("Item deliverer is null");
            return;
        }
        
        m_CashSocket.OnSocket.AddListener(ConsumeCashFromSocket);
        
        m_Keypad.OnKeyInput.AddListener(AddKeypadInput);
        m_Keypad.OnKeypadClear.AddListener(ClearKeypad);
        m_Keypad.OnKeySubmission.AddListener(TryKeycode);

        InitializeItemListFromStoreManager();
        InitializeDisplays();
    }

    private void AddKeypadInput(int InKeypadInput)
    {
        if (m_KeypadValue > 99)
        {
            ClearKeypad();
        }
        
        m_KeypadValue *= 10; 
        m_KeypadValue += InKeypadInput;

        string KeypadText = m_KeypadValue.ToString();
        KeypadText = m_KeypadValue switch
        {
            < 10 => KeypadText.Insert(0, "00"),
            < 100 => KeypadText.Insert(0, "0"),
            _ => KeypadText
        };

        m_KeyCodeDisplay.SetDisplayText(KeypadText);
    }

    private void ClearKeypad()
    {
        m_KeypadValue = 0;
        m_KeyCodeDisplay.SetDisplayText("000");
    }

    private void InitializeDisplays()
    {
        m_ItemNameDisplay.SetInputPromptText("ITEM:");
        m_ItemPriceDisplay.SetInputPromptText("PRICE:");
        m_ItemCountDisplay.SetInputPromptText("COUNT:");
        m_TotalPriceDisplay.SetInputPromptText("TOTAL:");
        
        m_DepositedCashDisplay.SetInputPromptText("PIRCS:");
        
        UpdateCashDisplay();
        ClearItemDisplays();
    }
    
    private void InitializeItemListFromStoreManager()
    {
        m_VendingMachineInventory.Clear();
        foreach (FStoreItem StoreItem in m_StoreManager.GetItemList())
        {
            if (StoreItem.StoreType == EItemStoreType.ST_VendingMachine)
            {
                m_VendingMachineInventory.Add(StoreItem.PurchaseCode, StoreItem);
            }
        }
    }

    private void RequestFailed()
    {
        //Play failed audio.

        ClearKeypad();
    }

    private void SetDisplaysInitial(FStoreItem InStoreItem)
    {
        m_ItemNameDisplay.GetInputDisplay().SetDisplayText(InStoreItem.Name);
        m_ItemPriceDisplay.GetInputDisplay().SetDisplayText(InStoreItem.Price.ToString());
        UpdatePurchasePrice(1);
    }

    private void ClearItemDisplays()
    {
        m_ItemNameDisplay.GetInputDisplay().SetDisplayText("");
        m_ItemPriceDisplay.GetInputDisplay().SetDisplayText("");
        UpdatePurchasePrice(0);
    }

    private void UpdatePurchasePrice(int InNewPurchaseCount)
    {
        if (m_QueuedItemCode == 0)
        {
            m_ItemCountDisplay.GetInputDisplay().SetDisplayText("");
            m_TotalPriceDisplay.GetInputDisplay().SetDisplayText("");
            return;
        }
        
        m_RequestedItemCount = InNewPurchaseCount;
        m_TotalPurchasePrice = m_RequestedItemCount * m_VendingMachineInventory[m_QueuedItemCode].Price;

        m_ItemCountDisplay.GetInputDisplay().SetDisplayText(m_RequestedItemCount.ToString());
        m_TotalPriceDisplay.GetInputDisplay().SetDisplayText(m_TotalPurchasePrice.ToString());
    }

    private void QueueItem(FStoreItem InStoreItem)
    {
        if (!InStoreItem.IsValid())
        {
            return;
        }
        m_QueuedItemCode = InStoreItem.PurchaseCode;
        SetDisplaysInitial(InStoreItem);
    }
    
    private void AddCash(int InCash)
    {
        m_DepositedCashCount += InCash;
        UpdateCashDisplay();
    }

    public void SubtractCash(int InCash)
    {
        m_DepositedCashCount -= InCash;
        UpdateCashDisplay();
    }

    private void UpdateCashDisplay()
    {
        string CashDisplay = m_DepositedCashCount.ToString();
        while (CashDisplay.Length < 5)
        {
            CashDisplay = "0" + CashDisplay;
        }
        m_DepositedCashDisplay.GetInputDisplay().SetDisplayText(CashDisplay);
    }

    private void ConsumeCashFromSocket()
    {
        CS_Currency Currency = m_CashSocket.GetSocketedGO().GetComponent<CS_Currency>();

        if (Currency == null)
        {
            Debug.LogError("Deposited item has no CS_Currency component");
            return;
        }

        AddCash(Currency.GetValue());
        m_CashSocket.DestroySocketedGO();
    }
    
    private void TryKeycode()
    {
        if (!m_VendingMachineInventory.ContainsKey(m_KeypadValue))
        {
            RequestFailed();
            return;
        }
        QueueItem(m_VendingMachineInventory[m_KeypadValue]);
    }

    public void UpdateCount(int InCountDifference)
    {
        if (m_QueuedItemCode == 0
            ||m_RequestedItemCount + InCountDifference >= m_VendingMachineInventory[m_QueuedItemCode].MaxPurchaseAmount
            || m_RequestedItemCount + InCountDifference < 1)
        {
            return;
        }
        m_RequestedItemCount += InCountDifference;
        UpdatePurchasePrice(m_RequestedItemCount);
    }

    public void TryPurchaseItem()
    {
        if (m_QueuedItemCode == 0)
        {
            return;
        }
        
        FStoreItem Item = m_VendingMachineInventory[m_QueuedItemCode];
        if (m_DepositedCashCount <= Item.Price * m_RequestedItemCount)
        {
            RequestFailed();
            return;
        }
        
        m_DepositedCashCount -= Item.Price * m_RequestedItemCount;
        UpdateCashDisplay();
        
        m_ItemDeliverer.DeliverItems(Item, m_RequestedItemCount);
    }
    
    public void DequeueItem()
    {
        m_QueuedItemCode = 0;
        ClearItemDisplays();
    }
    
}
