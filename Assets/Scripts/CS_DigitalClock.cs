using System.Collections;
using System.Collections.Generic;
using TimeSyncSystem;
using TMPro;
using UnityEngine;

public class CS_DigitalClock : MonoBehaviour
{
    [SerializeField]
    TMP_Text TextComponent;

    CS_TimeManager TimeManager;

    // Start is called before the first frame update
    void Start()
    {
    }

    private void Awake()
    {

        TextComponent = GetComponentInChildren<TMP_Text>();

        TimeManager = GameObject.FindFirstObjectByType<CS_TimeManager>();

        TimeManager.GetSyncEvent().AddListener(OnTimeUpdated);
    }

    void OnTimeUpdated(FTimeSyncEvent InEvent)
    {
        int TimeDisplayText = InEvent.NewTimeStamp;
        int NumHours = (TimeDisplayText / 100);
        int NumMinutes = TimeDisplayText - (NumHours * 100);
        
        if(NumHours < 10)
            TextComponent.SetText("0" + NumHours.ToString() + ":" + NumMinutes.ToString());
        else
            TextComponent.SetText(NumHours.ToString() + ":" + NumMinutes.ToString());
    }
}
