using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using NNarrativeDataTypes;
using Unity.VisualScripting;
using NCharacterTraitCategoryTypes;
using AYellowpaper.SerializedCollections;
using ChoETL;

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

        public FDateHandle(string InDateStr)
        {
            string[] dateStrings = InDateStr.Split("/");
            string ddStr = dateStrings[0];
            string mmStr = dateStrings[1];
            string yyyyStr = dateStrings[2];
            dd = 0;
            mm = 0;
            yyyy = 0;

            if (!ddStr[1].Equals(' '))
                dd = int.Parse(ddStr);

            if (!mmStr[1].Equals(' '))
                mm = int.Parse(mmStr);

            if (yyyyStr.Length > 3 && !yyyyStr[3].Equals(' '))
                yyyy = int.Parse(yyyyStr);

            DisplayName = InDateStr;
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

        public bool IsEmpty()
        {
            return DisplayName.IsNullOrEmpty();
        }

        public string GetDisplayName()
        {
            return DisplayName;
        }

        public bool IsFullDoB()
        {
            return dd != 0 && mm != 0 && yyyy != 0;
        }

        public float CalculateScore(FDateHandle InComparisonHandle, ref int InErrorCount)
        {
            if(InComparisonHandle == this)
            {
                return 1.0f;
            }

            float OutScore = 0;

            if (dd != 0)
            {
                if(InComparisonHandle.dd == this.dd)
                {
                    OutScore++;
                }
                else
                {
                    InErrorCount++;
                }
            }

            if (mm != 0)
            {
                if (InComparisonHandle.mm == this.mm)
                {
                    OutScore++;
                }
                else
                {
                    InErrorCount++;
                }
            }

            if (yyyy != 0)
            {
                if (InComparisonHandle.yyyy == this.yyyy)
                {
                    OutScore++;
                }
                else
                {
                    InErrorCount++;
                }
            }

            return OutScore / 3.0f;
        }
    }
    // #END: Tagged Item Data


    // #BEGIN: Chatroom Data Types

    // #TODO [KA] (29.09.2024): Rename this it sucks
    [System.Serializable]
    public struct FChatRoom
    {
        [SerializeField] 
        public int RoomId;

        [SerializeField] 
        public string RoomName;
        
        [SerializeField]
        public List<FCharacterData> Characters;

        [SerializeField] 
        public List<FChatLineTimeChunk> ChatLog;
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
        private string CharacterName;

        [SerializeField]
        private int CharacterId;

        [SerializeField]
        private FCharacterWebHandle WebHandle;

        [SerializeField]
        private FWebIdHandle WebIdHandle;

        [SerializeField]
        private FDateHandle DateOfBirth;

        [SerializeField]
        [SerializedDictionary]
        private SerializedDictionary<ECharacterTraitCategory, FCharacterTraitId> Traits;

        public FCharacterData(
            string InName,
            int InId, 
            FCharacterWebHandle InWebHandle,
            FWebIdHandle InWebIdHandle, 
            FDateHandle InDateOfBirth,
            SerializedDictionary<ECharacterTraitCategory, FCharacterTraitId> InTraits
            )
        {
            this.CharacterName = InName;
            this.CharacterId = InId;
            this.WebHandle = InWebHandle;
            this.WebIdHandle = InWebIdHandle;
            this.DateOfBirth = InDateOfBirth;
            this.Traits = InTraits;
        }

        public void SetCharName(string InName)
        {
            CharacterName = InName;
        }

        public string GetCharacterName()
        {
            return CharacterName;
        }
        public void SetWebHandle(FCharacterWebHandle InWebHandle)
        {
            WebHandle = InWebHandle;
        }

        public FCharacterWebHandle GetWebHandle()
        {
            return WebHandle;
        }

        public void SetWebId(FWebIdHandle InWebId)
        {
            WebIdHandle = InWebId;
        }

        public FWebIdHandle GetWebId()
        {
            return WebIdHandle;
        }

        public void SetDateOfBirth(FDateHandle InDateOfBirth)
        {
            DateOfBirth = InDateOfBirth;
        }

        public FDateHandle GetDateOfBirth()
        {
            return DateOfBirth;
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

        public override bool Equals(object obj)
        {
            return obj is FCharacterWebHandle handle &&
                   this == (FCharacterWebHandle)obj;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(DisplayName);
        }

        public static bool operator ==(FCharacterWebHandle lhs, FCharacterWebHandle rhs)
        {
            return
                lhs.GetHashCode() == rhs.GetHashCode();
        }
        public static bool operator !=(FCharacterWebHandle lhs, FCharacterWebHandle rhs)
        {
            return !(lhs == rhs);
        }

        public bool IsEmpty()
        { 
            return DisplayName.IsNullOrEmpty(); 
        }

        public string GetDisplayName()
        {
            return DisplayName;
        }
    }


    [System.Serializable]
    public struct FWebIdHandle
    {
        private int Seg1;
        //private int Seg2;
        //private int Seg3;

        [SerializeField]
        string DisplayName;

        public FWebIdHandle(int InSeg1/*, int InSeg2, int InSeg3*/, string InDisplayName)
        {
            Seg1 = InSeg1;
            //Seg2 = InSeg2;
            //Seg3 = InSeg3;

            DisplayName = InDisplayName;
        }

        public override bool Equals(object obj)
        {
            return obj is FWebIdHandle handle &&
                   this == (FWebIdHandle)obj;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Seg1/*, Seg2, Seg3*/);
        }

        public static bool operator == (FWebIdHandle lhs, FWebIdHandle rhs)
        {
            return 
                lhs.Seg1 == rhs.Seg1 /*&& 
                lhs.Seg2 == rhs.Seg2 && 
                lhs.Seg3 == rhs.Seg3*/;
        }
        public static bool operator != (FWebIdHandle lhs, FWebIdHandle rhs)
        {
            return !(lhs == rhs);
        }

        public bool IsEmpty()
        {
            return DisplayName.IsNullOrEmpty();
        }

        public string GetDisplayName()
        {
            return DisplayName;
        }
    }


    // #END: Character Data Types

}



public class CS_StoryManager : MonoBehaviour
{
    CS_CharacterListBuilder CharacterList;

    public void Start()
    {
        CharacterList = gameObject.GetComponent<CS_CharacterListBuilder>();
        if(CharacterList.IsNull())
        {
            Debug.LogWarning("Character List is invalid!");
        }
    }

    public void ScoreProfile(FCharacterData InProfileToValidate)
    {
        List<FCharacterData> MatchingCharacters = CharacterList.GetMatchingCharacters(InProfileToValidate);

        if(MatchingCharacters.Count == 1)
        {
            float Score = ScoreProfile(InProfileToValidate, MatchingCharacters[0]);
        }

        if (MatchingCharacters.Count == 0)
        {
            // Run some logic to mark this as a failed submission that needs more info to identify a person
        }

        if(MatchingCharacters.Count > 1)
        {
            // Run some logic to mark this as a vague profile that needs more information to narrow it down to a person
        }
    }


    private float ScoreProfile(FCharacterData InputProfile,  FCharacterData ComparisonProfile)
    {
        float Score = 0.0f;
        int Errors = 0;

        if (!InputProfile.GetCharacterName().IsNullOrEmpty())
        {
            if (InputProfile.GetCharacterName() == ComparisonProfile.GetCharacterName())
            {
                Score++;
            }
            else
            {
                Errors++;
            }
        }

        if (!InputProfile.GetWebHandle().IsObjectNullOrEmpty())
        {
            if (InputProfile.GetWebHandle() == ComparisonProfile.GetWebHandle())
            {
                Score++;
            }
            else
            {
                Errors++;
            }
        }
        
        if (!InputProfile.GetWebId().IsObjectNullOrEmpty())
        {
            if (InputProfile.GetWebId() == ComparisonProfile.GetWebId())
            {
                Score++;
            }
            else
            {
                Errors++;
            }
        }

        Score += InputProfile.GetDateOfBirth().CalculateScore(ComparisonProfile.GetDateOfBirth(), ref Errors);

        return Errors >= 2 ? 0.0f : Score;
    }
}
