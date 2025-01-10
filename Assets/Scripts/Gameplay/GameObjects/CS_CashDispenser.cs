using UnityEngine;

public class CS_CashDispenser : MonoBehaviour
{
    [SerializeField]
    private CS_QueuedDispenser m_CoinDispenser;
    
    
    [SerializeField]
    private CS_QueuedDispenser m_BillDispenser;

    private void Start()
    {
        if (!m_CoinDispenser)
        {
            Debug.LogError("No CoinDispenser assigned!");
        }
        
        if (!m_BillDispenser)
        {
            Debug.LogError("No BillDispenser assigned!");
        }
    }
    
    public void QueueRewards(int InAmount)
    {
        Debug.Log(InAmount);
        int amt = InAmount;
        while (amt - 10 > 0)
        {
            amt -= 10;
            m_BillDispenser.QueueItem();
        }
        
        while(amt > 0)
        {
            amt--;
            m_CoinDispenser.QueueItem();
        }
    }
}
