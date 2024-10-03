using UnityEngine.Events;
using System;


namespace TimeSyncSystem
{
    [Serializable]
    public class FTimeSyncEvent : UnityEvent
    {
        public enum TimeUdpateReason
        {
            ADDED = 0,
            REWIND = 1
        }
        public TimeUdpateReason reason;
    }
}