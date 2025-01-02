using UnityEngine.Events;
using System;


namespace TimeSyncSystem
{
    [Serializable]
    public class FTimeSyncEvent : UnityEvent<FTimeSyncEvent>
    {
        public enum TimeUpdateReason
        {
            ADDED = 0,
            REWIND = 1,
            NONE = 2
        }
        public TimeUpdateReason reason = TimeUpdateReason.NONE;

        public int OldTimeStamp = -1;
        public int NewTimeStamp = -1;
    }
}