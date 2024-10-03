using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

namespace NNarrativeDataTypes
{
    // #TODO [KA] (29.09.2024): Rename this it sucks
    [System.Serializable]
    public struct FChatRoom
    {
        [SerializeField]
        public List<string> People;
    }

    // #TODO [KA] (29.09.2024): Rename this it sucks
    [System.Serializable]
    public struct FChatLineTimeChunk
    {
        [HideInInspector]
        public string TimestampString;

        [SerializeField]
        public int TimeStamp;

        [SerializeField]
        public List<FChatLine> ChatLines;
    }

    [System.Serializable]
    public struct FChatLine
    {
        [SerializeField]
        public int PrintOrder;

        [SerializeField]
        public string CharacterName;

        [SerializeField]
        public string Line;
    }
}



public class CS_StoryManager : MonoBehaviour
{

    public void Start()
    {
        Debug.Log("Meme");
    }

}
