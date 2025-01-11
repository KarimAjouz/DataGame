using System;
using System.Collections.Generic;
using UnityEngine;



public class CS_StoreManager : MonoBehaviour
{
    private enum EItemType
    {
        IT_None,
        IT_Item,
        IT_Upgrade,
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
        

        public bool IsValid()
        {
            return 
                ItemPrefab != null
                && Name != null;
        }
    }

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
}
