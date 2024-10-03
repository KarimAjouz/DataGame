using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class CS_TimeManager : MonoBehaviour
{
    [SerializeField]
    private int m_StartingTime = 0;

    private int m_CurrentTime;

    [SerializeField]
    private TimeSyncSystem.FTimeSyncEvent m_SyncEvent = null;

    public void SetTime(int TimeInMinutes)
    {
        m_SyncEvent.reason = TimeInMinutes <= m_CurrentTime ? TimeSyncSystem.FTimeSyncEvent.TimeUdpateReason.REWIND : TimeSyncSystem.FTimeSyncEvent.TimeUdpateReason.ADDED;

        m_CurrentTime = TimeInMinutes;

        m_SyncEvent.Invoke();
    }

    public int GetTime()
    {
        return m_CurrentTime;
    }

    public void AddTime(int TimeInMinutes)
    {
        m_CurrentTime += TimeInMinutes;
        m_SyncEvent.reason = TimeSyncSystem.FTimeSyncEvent.TimeUdpateReason.ADDED;
        m_SyncEvent.Invoke();
    }

    public int GetDisplayTime()
    {
        int OutTime = 0;
        OutTime += ((m_CurrentTime / 60) * 100) % 2400;
        OutTime += m_CurrentTime % 60;
        return OutTime;
    }

    public TimeSyncSystem.FTimeSyncEvent GetSyncEvent()
    {
        return m_SyncEvent;
    }


    // Start is called before the first frame update
    void Start()
    {
        SetTime(m_StartingTime);
    }
}
