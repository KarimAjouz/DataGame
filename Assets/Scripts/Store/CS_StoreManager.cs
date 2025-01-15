using System;
using System.Collections.Generic;
using UnityEngine;
using NStoreDataTypes;

namespace NStoreDataTypes
{
    public enum EItemType
    {
        IT_None,
        IT_Item,
        IT_Upgrade,
        COUNT
    }

    public enum EItemStoreType
    {
        ST_None,
        ST_VendingMachine,
        ST_MailOrder,
        COUNT
    }
    
    [Serializable]
    public struct FStoreItem
    {
        [SerializeField]
        public string Name;
        
        [SerializeField]
        public int Price;

        [SerializeField] 
        public GameObject ItemPrefab;

        [SerializeField] 
        public Vector3 SpawnPositionOffset;
        
        [SerializeField] 
        public Vector3 SpawnRotationOffset;

        [SerializeField] 
        public EItemType ItemType;

        [SerializeField] 
        public EItemStoreType StoreType;

        [SerializeField] 
        public int PurchaseCode;

        [SerializeField] 
        public int MaxPurchaseAmount;
        

        public bool IsValid()
        {
            return 
                ItemPrefab != null
                && Name != null;
        }
    }
}

public class CS_StoreManager : MonoBehaviour
{
    

    [SerializeField] 
    private List<FStoreItem> StoreItems;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (FStoreItem StoreItem in StoreItems)
        {
            if (StoreItem.IsValid()) continue;
            Debug.LogError("Store contains invalid items!");
            return;
        }
    }

    public void Debug_BuyPunchcard()
    {
        CS_ItemDeliverer ItemDeliverer = FindFirstObjectByType<CS_ItemDeliverer>();

        if (!ItemDeliverer)
        {
            Debug.LogError("No ItemDeliverer!");
            return;
        }

        if (StoreItems.Count == 0)
        {
            Debug.LogError("Store contains no valid items!");
            return;
        }
        
        ItemDeliverer.DeliverItem(StoreItems[0]);
    }

    public List<FStoreItem> GetItemList()
    {
        return StoreItems;
    }
}
