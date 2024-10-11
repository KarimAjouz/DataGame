using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using NNarrativeDataTypes;
using Unity.VisualScripting;
using NCharacterTraitCategoryTypes;
using AYellowpaper.SerializedCollections;

namespace NNarrativeDataTypes
{
    // #BEGIN: Tagged Item Data

    [System.Serializable]
    public struct FDateHandle
    {
        public string DisplayName;

        private int dd;
        private int mm;
        private int yyyy;

        public FDateHandle(int InDd, int InMm, int InYyyy)
        {
            dd = InDd;
            mm = InMm;
            yyyy = InYyyy;

            DisplayName = dd.ToString() + "/" + mm.ToString() + "/" + yyyy.ToString();
        }

        public override bool Equals(object obj)
        {
            return obj is FDateHandle handle &&
                   dd == handle.dd &&
                   mm == handle.mm &&
                   yyyy == handle.yyyy;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(dd, mm, yyyy);
        }

        public static bool operator == (FDateHandle left, FDateHandle right) 
        {
            return 
                left.dd == right.dd &&
                left.mm == right.mm &&
                left.yyyy == right.yyyy;
        }

        public static bool operator !=(FDateHandle left, FDateHandle right)
        {
            return !(left == right);
        }
    }
    // #END: Tagged Item Data


    // #BEGIN: Chatroom Data Types

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
    // #END: Chatroom Data Types


    // #BEGIN: Character Data Types

    [System.Serializable]
    public struct FCharacterData
    {
        [SerializeField]
        private String CharacterName;

        [SerializeField]
        private int CharacterId;

        [SerializeField]
        private FCharacterWebHandle WebHandle;

        [SerializeField]
        private FCivIdHandle CivIdHandle;

        [SerializeField]
        private FDateHandle DateOfBirth;

        [SerializeField]
        [SerializedDictionary]
        private SerializedDictionary<string, FCharacterTraitId> Traits;

        public FCharacterData(
            string InName,
            int InId, 
            FCharacterWebHandle InWebHandle, 
            FCivIdHandle InCivIdHandle, 
            FDateHandle InDateOfBirth,
            SerializedDictionary<string, FCharacterTraitId> InTraits
            )
        {
            this.CharacterName = InName;
            this.CharacterId = InId;
            this.WebHandle = InWebHandle;
            this.CivIdHandle = InCivIdHandle;
            this.DateOfBirth = InDateOfBirth;
            this.Traits = InTraits;
        }
    }

    [System.Serializable]
    public struct FCharacterWebHandle
    {
        [SerializeField]
        private string DisplayName;

        public FCharacterWebHandle(string InDisplayName)
        {
            DisplayName = InDisplayName;
        }
    }


    [System.Serializable]
    public struct FCivIdHandle
    {
        private int Seg1;
        private int Seg2;
        private int Seg3;

        [SerializeField]
        string DisplayName;

        public FCivIdHandle(int InSeg1, int InSeg2, int InSeg3, string InDisplayName)
        {
            Seg1 = InSeg1;
            Seg2 = InSeg2;
            Seg3 = InSeg3;

            DisplayName = InDisplayName;
        }

        public override bool Equals(object obj)
        {
            return obj is FCivIdHandle handle &&
                   this == (FCivIdHandle)obj;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Seg1, Seg2, Seg3);
        }

        public static bool operator == (FCivIdHandle lhs, FCivIdHandle rhs)
        {
            return 
                lhs.Seg1 == rhs.Seg1 && 
                lhs.Seg2 == rhs.Seg2 && 
                lhs.Seg3 == rhs.Seg3;
        }
        public static bool operator != (FCivIdHandle lhs, FCivIdHandle rhs)
        {
            return !(lhs == rhs);
        }
    }


    // #END: Character Data Types

}



public class CS_StoryManager : MonoBehaviour
{

    public void Start()
    {
    }

}
