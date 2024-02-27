using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CS_TabbedDisplay : MonoBehaviour
{

    private int PageCount = 0;
    private int CurrentPageIndex = 0;


    [SerializeField]
    private GameObject PageContainer;

    [SerializeField]
    private GameObject Toolbar;

    [SerializeField]
    private GameObject ButtonPrefab;

    [SerializeField]
    private GameObject DefaultPage;

    [SerializeField]
    public List<CS_UI_TabPage> Pages;

    // Start is called before the first frame update
    void Start()
    {
        Pages = new List<CS_UI_TabPage>();
        foreach (CS_UI_TabPage Page in PageContainer.GetComponentsInChildren<CS_UI_TabPage>(true))
        {
            AddNewPage(Page);
        }
        SetPageIndex(0);
    }

    public void SetPageIndex(int InPageIndex)
    {
        if(CurrentPageIndex >= Pages.Count)
        {
            Debug.LogWarning("CS_TabbedDisplay::SetPageIndex --> Pages does not contain index: " + InPageIndex);
            return;
        }

        Pages[CurrentPageIndex].gameObject.SetActive(false);
        Pages[InPageIndex].gameObject.SetActive(true);

        CurrentPageIndex = InPageIndex;
    }

    void AddNewPage(CS_UI_TabPage PageGO)
    {
        if(Pages.Contains(PageGO)) { return; }

        Pages.Add(PageGO);
        AddButtonForPage(PageGO);
    }


    void AddButtonForPage(CS_UI_TabPage PageGO)
    {
        GameObject newButtonGO = Instantiate(ButtonPrefab, Toolbar.transform);
        CS_UI_TabButton newButton = newButtonGO.GetComponent<CS_UI_TabButton>();


        foreach (TMPro.TMP_Text text in newButtonGO.GetComponentsInChildren<TMPro.TMP_Text>())
        {
            text.text = PageGO.PageName;
        }

        newButtonGO.GetComponent<Image>().color = PageGO.PageColor;

        newButton.SetPage(PageGO.gameObject, Pages.Count - 1);
    }

}
