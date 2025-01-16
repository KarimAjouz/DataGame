using UnityEngine;

public class CS_Currency : MonoBehaviour
{
    [SerializeField]
    private int m_CurrencyValue;

    public int GetValue()
    {
        return m_CurrencyValue;
    }
}
