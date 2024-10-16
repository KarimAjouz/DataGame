using ChoETL;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;

public class CS_SplitFlapCharacter_SingleCard : MonoBehaviour
{
    [SerializeField]
    private TMP_Text FrontText;

    [SerializeField]
    private TMP_Text RearText;

    [SerializeField]
    [ReadOnly]
    public CS_SplitFlapCharacter ParentCharacter;

    Animation Anim;

    private void Start()
    {
        ParentCharacter = GetComponentInParent<CS_SplitFlapCharacter>();
        Anim = GetComponent<Animation>();
    }

    public void SetCard(char FrontCardStr, char RearCardString)
    {
        FrontText.text = FrontCardStr.ToString();
        RearText.text = RearCardString.ToString();
    }

    public void FireAnim()
    {
        Anim.Play();
    }

    public void ShouldFireAgain()
    {
        ParentCharacter.MidPointCheckAnim();
    }

    public void CompleteAnim()
    {
        ParentCharacter.CompleteSpinAnim();
    }
}
