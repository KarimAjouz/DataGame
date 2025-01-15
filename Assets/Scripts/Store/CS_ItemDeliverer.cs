using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using NStoreDataTypes;

public class CS_ItemDeliverer : MonoBehaviour
{
    [SerializeField]
    private List<UnityEvent> ItemsDeliveredEvents = new List<UnityEvent>();
    
    [SerializeField]
    private List<UnityEvent> ItemsPickedUpEvents = new List<UnityEvent>();

    [SerializeField] 
    private GameObject ItemDeliveryParent;
    

    public void DeliverItem(FStoreItem InStoreItem)
    {
        if (ItemDeliveryParent.transform.childCount != 0)
        {
            Debug.LogWarning("ItemDeliveryParent is not empty!");
            return;
        }
        
        GameObject NewItem = Instantiate(InStoreItem.ItemPrefab, ItemDeliveryParent.transform);
        
        NewItem.transform.localPosition = InStoreItem.SpawnPositionOffset;
        NewItem.transform.localEulerAngles = InStoreItem.SpawnRotationOffset;

        foreach (UnityEvent deliverEvent in ItemsDeliveredEvents)
        {
            deliverEvent.Invoke();
        }
    }

    public void PickupItem()
    {
        if (ItemDeliveryParent.transform.childCount - 1 != 0)
        {
            return;
        }
        
        foreach (UnityEvent pickupEvent in ItemsPickedUpEvents)
        {
            pickupEvent.Invoke();
        }
    }
}