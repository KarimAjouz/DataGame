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
    private TextMeshProUGUI FrontText;

    [SerializeField]
    private TextMeshProUGUI RearText;

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
        if(Anim.IsNull())
        {
            return;
        }

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

    public void SetColor(Color c)
    {
        FrontText.color = c;
        RearText.color = c;
    }

    public void CancelAnim()
    {
        Anim.Stop();
    }
}
