using System;
using System.Collections;
using System.Collections.Generic;
using Michsky.DreamOS;
using Unity.VisualScripting;
using UnityEngine;


public class CS_TimeManager : MonoBehaviour
{
    [SerializeField]
    private int m_StartingTime = 0;

    private int m_CurrentTime;

    [SerializeField]
    private TimeSyncSystem.FTimeSyncEvent m_SyncEvent;
    
    private CS_TimedNarrativeEventManager m_TimedNarrativeEventManager;
    
    private DateAndTimeManager m_DateAndTimeManager;

    private void SetTime(int TimeInMinutes)
    {
        m_SyncEvent.OldTimeStamp = m_CurrentTime;

        m_SyncEvent.reason = TimeInMinutes <= m_CurrentTime ? TimeSyncSystem.FTimeSyncEvent.TimeUpdateReason.REWIND : TimeSyncSystem.FTimeSyncEvent.TimeUpdateReason.ADDED;

        m_CurrentTime = TimeInMinutes;
        m_SyncEvent.NewTimeStamp = m_CurrentTime;
        
        PostDreamOsTime();

        m_TimedNarrativeEventManager.ProcessNarrativeEventsFromTimeSyncEvent(m_SyncEvent);
    }

    public int GetTime()
    {
        return m_CurrentTime;
    }

    private void PostDreamOsTime()
    {
        m_DateAndTimeManager.currentHour = GetDisplayTime() / 100;
        m_DateAndTimeManager.currentMinute = GetDisplayTime() % 60;
    }

    public void AddTime(int TimeInMinutes, bool UpdateOSTime = false)
    {
        m_SyncEvent.OldTimeStamp = m_CurrentTime;
        
        m_CurrentTime += TimeInMinutes;
        m_SyncEvent.reason = TimeSyncSystem.FTimeSyncEvent.TimeUpdateReason.ADDED;
        
        m_SyncEvent.NewTimeStamp = m_CurrentTime;
        
        if(UpdateOSTime)
            PostDreamOsTime();
        
        m_TimedNarrativeEventManager.ProcessNarrativeEventsFromTimeSyncEvent(m_SyncEvent);
    }

    private int GetDisplayTime()
    {
        int OutTime = 0;
        OutTime += ((m_CurrentTime / 60) * 100) % 2400;
        OutTime += m_CurrentTime % 60;
        return OutTime;
    }

    public string GetDisplayTimeString()
    {
        int DisplayTime = GetDisplayTime();
        string OutTimeString = (((m_CurrentTime / 60) * 100) % 2400).ToString() + ":" + (m_CurrentTime % 60).ToString();
        return OutTimeString;
    }

    public int GetTimeStampFromTime(int TimeInMinutes)
    {
        return (((m_CurrentTime / 60) * 100) % 2400) + (m_CurrentTime % 60);
    }
    

    public TimeSyncSystem.FTimeSyncEvent GetSyncEvent()
    {
        return m_SyncEvent;
    }
    
    // Start is called before the first frame update
    public void Start()
    {
        m_TimedNarrativeEventManager = FindFirstObjectByType<CS_TimedNarrativeEventManager>();
        if (m_TimedNarrativeEventManager == null)
        {
            Debug.LogError("No Timed Narrative Event manager found");
            return;
        }
        
        
        m_DateAndTimeManager = FindFirstObjectByType<DateAndTimeManager>();
        if (m_DateAndTimeManager == null)
        {
            Debug.LogError("No Michsky.DateAndTimeManager found");
            return;
        }
        
        

        m_CurrentTime = m_StartingTime - 1;
        SetTime(m_StartingTime);

    }

    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.Semicolon))
        {
            AddTime(1);
        }
    }
}
