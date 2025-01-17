using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using NNarrativeDataTypes;
using NCharacterTraitCategoryTypes;
using AYellowpaper.SerializedCollections;
using ChoETL;
using NaughtyAttributes;

namespace NNarrativeDataTypes
{
    public interface ICreditableCharacterData<in T>
    {
        public FCharacterDataCredit EvaluateCredit(T other, ECharacterTraitCategory InTraitCategory = ECharacterTraitCategory.ETraitCategory_NONE)
        {
            return new FCharacterDataCredit(ECharacterTraitCategory.ETraitCategory_NONE, ECreditType.CreditType_Invalid);
        }
    }
    
    
    // #BEGIN: Tagged Item Data

    [Serializable]
    public struct FDateHandle : 
        ICreditableCharacterData<FDateHandle>, 
        IEquatable<FDateHandle>
    {
        public string DisplayName;

        [SerializeField]
        private int dd;
        
        [SerializeField]
        private int mm;
        
        [SerializeField]
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

        public FCharacterDataCredit EvaluateCredit(FDateHandle InComparisonHandle, ECharacterTraitCategory InTraitCategory = ECharacterTraitCategory.ETraitCategory_BIRTHDATE)
        {
            if(InComparisonHandle == this)
            {
                return new FCharacterDataCredit(InTraitCategory, ECreditType.CreditType_Full);
            }

            if (InComparisonHandle.yyyy == yyyy)
            {
                return new FCharacterDataCredit(InTraitCategory, ECreditType.CreditType_Partial);
            }

            return new FCharacterDataCredit(InTraitCategory, ECreditType.CreditType_Invalid);
        }

        public bool Equals(FDateHandle other)
        {
            return DisplayName == other.DisplayName && dd == other.dd && mm == other.mm && yyyy == other.yyyy;
        }
    }
    // #END: Tagged Item Data


    // #BEGIN: Chatroom Data Types

    // #TODO [KA] (29.09.2024): Rename this it sucks
    [Serializable]
    public struct FChatRoom
    {
        [SerializeField] 
        public int RoomId;

        [SerializeField] 
        public string RoomName;
        
        [SerializeField]
        [ReadOnly]
        public List<FCharacterData> Characters;

        [SerializeField] 
        [ReadOnly]
        public List<FChatLineTimeChunk> ChatLog;
    }

    // #TODO [KA] (29.09.2024): Rename this it sucks
    [Serializable]
    public struct FChatLineTimeChunk
    {
        [HideInInspector]
        public string TimestampString;

        [SerializeField]
        public int TimeStamp;

        [SerializeField]
        public List<FChatLine> ChatLines;
    }

    [Serializable]
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

    [Serializable]
    public struct FCharacterData : IEquatable<FCharacterData>
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
            CharacterName = InName;
            CharacterId = InId;
            WebHandle = InWebHandle;
            WebIdHandle = InWebIdHandle;
            DateOfBirth = InDateOfBirth;
            Traits = InTraits;
        }

        public void InitTraits()
        {
            Traits = new SerializedDictionary<ECharacterTraitCategory, FCharacterTraitId>();
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

        public SerializedDictionary<ECharacterTraitCategory, FCharacterTraitId> GetTraits()
        {
            return Traits;
        }

        public bool Equals(FCharacterData other)
        {
            return CharacterId == other.CharacterId;
        }

        public override bool Equals(object obj)
        {
            return obj is FCharacterData other && Equals(other);
        }

        public override int GetHashCode()
        {
            return CharacterId;
        }

        public string GetDisplayStringForCategory(ECharacterTraitCategory InCategory)
        {
            switch (InCategory)
            {
                case ECharacterTraitCategory.ETraitCategory_BIRTHDATE:
                    return DateOfBirth.DisplayName;
                case ECharacterTraitCategory.ETraitCategory_WEBID:
                    return WebIdHandle.GetDisplayName();
                case ECharacterTraitCategory.ETraitCategory_WEBHANDLE:
                    return WebHandle.GetDisplayName();
                default:
                    if (Traits.ContainsKey(InCategory))
                    {
                        return Traits[InCategory].DisplayName;
                    }
                    return "";
            }
        }
    }

    [Serializable]
    public struct FCharacterWebHandle : IEquatable<FCharacterWebHandle>
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
                   this == handle;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(DisplayName);
        }

        public static bool operator ==(FCharacterWebHandle lhs, FCharacterWebHandle rhs)
        {
            return
                lhs.Equals(rhs);
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

        public bool Equals(FCharacterWebHandle other)
        {
            return string.Equals(DisplayName, other.DisplayName, StringComparison.CurrentCultureIgnoreCase);
        }
    }


    [Serializable]
    public struct FWebIdHandle : IEquatable<FWebIdHandle>
    {
        [SerializeField]
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
                   this == handle;
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

        public bool Equals(FWebIdHandle other)
        {
            return Seg1 == other.Seg1 && DisplayName == other.DisplayName;
        }
    }
    
    
    // The types of character data pages that players need to collect.
    public enum ECharacterDataPageType
    {
        ECharacterDataPageType_IDENTIFIER,
        ECharacterDataPageType_PERSONALITY,
        COUNT
    }

    [System.Serializable]
    public struct FCharacterTraitId : 
        ICreditableCharacterData<FCharacterTraitId>,
        IEquatable<FCharacterTraitId>
    {
        [SerializeField]
        [ReadOnly]
        public string DisplayName;
        
        [SerializeField]
        [ReadOnly]
        private ECharacterTraitType m_TraitType;

        [SerializeField]
        [ReadOnly]
        private int m_Id;


        public FCharacterTraitId(string InDisplayName, ECharacterTraitType InTraitType, int InId)
        {
            DisplayName = InDisplayName;
            m_TraitType = InTraitType;
            m_Id = InId;
        }
        public override bool Equals(object obj)
        {
            return obj is FCharacterTraitId InItem 
                   && m_Id == InItem.m_Id;
        }

        public bool Equals(int InId)
        {
            return m_Id == InId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(m_Id);
        }

        public static bool operator ==(FCharacterTraitId left, FCharacterTraitId right)
        {
            return
                left.m_Id == right.m_Id;
        }

        public static bool operator !=(FCharacterTraitId left, FCharacterTraitId right)
        {
            return !(left == right);
        }

        public bool Equals(FCharacterTraitId other)
        {
            return m_Id == other.m_Id;
        }

        // #BEGIN: ICreditableCharacterData Interface
        public FCharacterDataCredit EvaluateCredit(FCharacterTraitId InTraitId, ECharacterTraitCategory InCategory)
        {
            return 
                this == InTraitId ? 
                    new FCharacterDataCredit(InCategory, ECreditType.CreditType_Full) : 
                    new FCharacterDataCredit(ECharacterTraitCategory.ETraitCategory_NONE, ECreditType.CreditType_Invalid);
        }
        
        // #END: ICreditableCharacterData Interface
    }
    
    // #END: Character Data Types
    
    // #BEGIN: Profile scoring types
    
    public enum ECreditType
    {
        CreditType_Invalid,
        CreditType_Partial,
        CreditType_Full,
        COUNT
    }
    

    [Serializable]
    public struct FCharacterDataCredit : IEquatable<FCharacterDataCredit>
    {
        [SerializeField]
        public ECharacterTraitCategory m_CharacterType;
        
        [SerializeField]
        public ECreditType m_CreditType;

        public FCharacterDataCredit(ECharacterTraitCategory InCharacterType, ECreditType InCreditType)
        {
            m_CharacterType = InCharacterType;
            m_CreditType = InCreditType;
        }

        public static bool operator ==(FCharacterDataCredit lhs, FCharacterDataCredit rhs)
        {
            return lhs.m_CharacterType == rhs.m_CharacterType && lhs.m_CreditType == rhs.m_CreditType;
        }

        public static bool operator !=(FCharacterDataCredit lhs, FCharacterDataCredit rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(object obj)
        {
            return obj is FCharacterDataCredit Reward
                   && Reward == this;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(m_CharacterType, m_CreditType);
        }

        public bool Equals(FCharacterDataCredit other)
        {
            return m_CharacterType == other.m_CharacterType && m_CreditType == other.m_CreditType;
        }
    }
    
    // #END: Profile scoring types

}



[SuppressMessage("ReSharper", "InconsistentNaming")]
public class CS_StoryManager : MonoBehaviour
{
    [SerializeField]
    [SerializedDictionary("RewardType", "Value")] 
    private SerializedDictionary<FCharacterDataCredit, int> m_CharacterDataCredit;

    private CS_CharacterListBuilder CharacterList;
    
    [SerializeField]
    private CS_CashDispenser m_CashDispenser;

    public void Start()
    {
        CharacterList = gameObject.GetComponent<CS_CharacterListBuilder>();
        if(CharacterList.IsNull())
        {
            Debug.LogWarning("Character List is invalid!");
        }

        if (!m_CashDispenser)
        {
            m_CashDispenser = FindFirstObjectByType<CS_CashDispenser>();
        }

        if (!m_CashDispenser)
        {
            Debug.LogError("Cash Dispenser is invalid!");
        }
        
    }

    public void SubmitProfile(FCharacterData InCharacterData)
    {
        m_CashDispenser.QueueRewards(GetRewardsForProfile(InCharacterData));
    }
    

    private int GetRewardsForProfile(FCharacterData InputProfile)
    {
        int OutReward = 0;

        if (InputProfile.GetWebHandle().IsEmpty())
        {
            return OutReward;
        }

        FCharacterData? ComparisonProfile = CharacterList.GetCharacterDataFromWebHandle(InputProfile.GetWebHandle());
        if (!ComparisonProfile.HasValue)
        {
            return OutReward;
        }

        if (ComparisonProfile.Value.GetWebId() != InputProfile.GetWebId())
        {
            return OutReward;
        }
        
        OutReward += GetRewardForCredit(new FCharacterDataCredit(ECharacterTraitCategory.ETraitCategory_WEBID, ECreditType.CreditType_Full));
        OutReward += GetRewardForCredit(InputProfile.GetDateOfBirth().EvaluateCredit(ComparisonProfile.Value.GetDateOfBirth()));

        if (InputProfile.GetTraits().IsNull())
        {
            return OutReward;
        }
        
        foreach (KeyValuePair<ECharacterTraitCategory, FCharacterTraitId> CategoryIdPair in InputProfile.GetTraits())
        {
            OutReward += GetRewardForCredit(CategoryIdPair.Value.EvaluateCredit(ComparisonProfile.Value.GetTraits()[CategoryIdPair.Key], CategoryIdPair.Key));
        }
        
        return OutReward;
    }
    
    private int GetRewardForCredit(FCharacterDataCredit InReward)
    {
        int outReward = 0;

        if (m_CharacterDataCredit.ContainsKey(InReward))
        {
            outReward = m_CharacterDataCredit[InReward];
        }

        return outReward;
    }
}
