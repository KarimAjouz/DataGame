using ChoETL;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.U2D;

public class CS_SplitFlapCharacter : MonoBehaviour
{
    [SerializeField]
    private CS_SplitFlapCharacter_SingleCard PendingCard;
    private CS_SplitFlapCharacter_SingleCard QueuedCard;
    private Vector3 PendingCardPos;
    private Quaternion PendingCardRot;

    [SerializeField]
    private CS_SplitFlapCharacter_SingleCard TopCard;
    private Vector3 TopCardPos;
    private Quaternion TopCardRot;

    [SerializeField]
    private CS_SplitFlapCharacter_SingleCard BottomCard;
    private Vector3 BotCardPos;
    private Quaternion BotCardRot;

    [SerializeField]
    private CS_SplitFlapCharacter_SingleCard CompletedCard;
    private Vector3 CompCardPos;
    private Quaternion CompCardRot;

    private int DisplayIndexCurrent = 0;
    private int DisplayIndexTarget = 0;

    private CS_SplitFlapDisplay SFDisplay;

    void Start()
    {
        SFDisplay = GetComponentInParent<CS_SplitFlapDisplay>();
        if (SFDisplay.IsObjectNullOrEmpty())
        {
            Debug.LogWarning("Could not get SplitFlapDisplay class from parent!");
        }

        int NumCards = SFDisplay.AvailableCharacters.Length;

        TopCard.SetCard(SFDisplay.AvailableCharacters[(DisplayIndexCurrent) % NumCards], SFDisplay.AvailableCharacters[(DisplayIndexCurrent + 1) % NumCards]);
        BottomCard.SetCard(SFDisplay.AvailableCharacters[(DisplayIndexCurrent + NumCards - 1) % NumCards], SFDisplay.AvailableCharacters[DisplayIndexCurrent % NumCards]);
        CompletedCard.SetCard(SFDisplay.AvailableCharacters[(DisplayIndexCurrent + NumCards - 2) % NumCards], SFDisplay.AvailableCharacters[(DisplayIndexCurrent + NumCards - 1) % NumCards]);
        PendingCard.SetCard(SFDisplay.AvailableCharacters[(DisplayIndexCurrent + 1) % NumCards], SFDisplay.AvailableCharacters[(DisplayIndexCurrent + 2) % NumCards]);
        
        TopCardPos = TopCard.transform.position;
        TopCardRot = TopCard.transform.rotation;

        BotCardPos = BottomCard.transform.position;
        BotCardRot = BottomCard.transform.rotation;

        PendingCardPos = PendingCard.transform.position;
        PendingCardRot = PendingCard.transform.rotation;

        CompCardPos = CompletedCard.transform.position;
        CompCardRot = CompletedCard.transform.rotation;


    }

    public void SetDisplayIndex(int InIndex)
    {
        DisplayIndexTarget = InIndex;
        MidPointCheckAnim();
    }

    public void MidPointCheckAnim()
    {
        

        if (DisplayIndexCurrent != DisplayIndexTarget)
        {
            TopCard.FireAnim();
            SpinCardAnim();
        }
    }

    public void SpinCardAnim()
    {
        DisplayIndexCurrent++;
        DisplayIndexCurrent = DisplayIndexCurrent % SFDisplay.AvailableCharacters.Length;

        QueuedCard = BottomCard;

        BottomCard = TopCard;
        TopCard = PendingCard;
        PendingCard = CompletedCard;
        CompletedCard = QueuedCard;

        QueuedCard = null;
        PendingCard.SetCard(SFDisplay.AvailableCharacters[(DisplayIndexCurrent + 1) % SFDisplay.AvailableCharacters.Length], SFDisplay.AvailableCharacters[(DisplayIndexCurrent + 2) % SFDisplay.AvailableCharacters.Length]);


        TopCard.transform.SetPositionAndRotation(TopCardPos, TopCardRot);
        PendingCard.transform.SetPositionAndRotation(PendingCardPos, PendingCardRot);
        SFDisplay.ReleaseAudio.Play();
    }
    public void CompleteSpinAnim()
    {
        CompletedCard.transform.SetPositionAndRotation(CompCardPos, CompCardRot);
        BottomCard.transform.SetPositionAndRotation(BotCardPos, BotCardRot);

        SFDisplay.CatchAudio.Play();
    }

    public void SetCardHighlightColour(Color InColour)
    {
        TopCard.SetColor(InColour);
        BottomCard.SetColor(InColour);
        PendingCard.SetColor(InColour);
        CompletedCard.SetColor(InColour);
    }
}
