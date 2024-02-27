using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CS_UI_TabButton : MonoBehaviour
{
    private int PageIndex;
    private GameObject Page;

    private void Start()
    {

    }

    public void SetPage(GameObject InPage, int InPageIndex)
    {
        Page = InPage;
        PageIndex = InPageIndex;
        Button button = gameObject.GetComponent<Button>();

        button.onClick.AddListener(delegate
        {
            GetComponentInParent<CS_TabbedDisplay>().SetPageIndex(PageIndex);
        });
    }
}
