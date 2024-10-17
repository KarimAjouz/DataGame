using ChoETL;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Dock_SFDisplayManager : MonoBehaviour
{
    [SerializeField]
    private List<CS_SplitFlapDisplay> ControlledDisplays;

    private int ActiveDisplay = 0;

    private bool ComponentActive = false;

    // Start is called before the first frame update
    void Start()
    {
        ControlledDisplays = new List<CS_SplitFlapDisplay>(GetComponentsInChildren<CS_SplitFlapDisplay>());
        ControlledDisplays[0].InitDisplay(true, "WEB:");
    }

    private void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(ComponentActive)
        {
            ProcessInputString();

            if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                PreviousDisplay();
            }

            if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                NextDisplay();
            }

            if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                ControlledDisplays[ActiveDisplay].PreviousChar();
            }

            if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                ControlledDisplays[ActiveDisplay].NextChar();
            }
        }
    }

    private void NextDisplay()
    {
        ControlledDisplays[ActiveDisplay].SetDisplayActive(false);
        ActiveDisplay = ++ActiveDisplay % ControlledDisplays.Count;
        ControlledDisplays[ActiveDisplay].SetDisplayActive(true);

    }

    private void PreviousDisplay()
    {
        ControlledDisplays[ActiveDisplay].SetDisplayActive(false);
        ActiveDisplay = (--ActiveDisplay + ControlledDisplays.Count) % ControlledDisplays.Count;
        ControlledDisplays[ActiveDisplay].SetDisplayActive(true);
    }

    public void OnDockEnter()
    {
        ComponentActive = true;
        ControlledDisplays[ActiveDisplay].SetDisplayActive(true);
    }

    public void OnDockExit()
    {
        ComponentActive = false;
        ControlledDisplays[ActiveDisplay].SetDisplayActive(false);
    }

    public void ClearDisplay()
    {
        foreach(CS_SplitFlapDisplay display in ControlledDisplays)
        {
            display.ResetDisplay();
        }
    }


    private void ProcessInputString()
    {
        string InputString = Input.inputString.ToUpper();
        while (!InputString.IsEmpty())
        {
            //First we process all instances of backspace inside the string.
            while (InputString.Contains("\b", 0))
            {
                int bspInd = InputString.IndexOf("\b");
                if(bspInd == 0)
                {
                    ControlledDisplays[ActiveDisplay].FireBackspace();

                    if (InputString.Length > 2)
                        InputString = InputString.Substring(2, InputString.Length - 2);
                    else
                        break;
                }
                else
                {
                    InputString = InputString.Remove(bspInd - 1, 3);
                }
            }

            if(InputString.Length > 1 && InputString.Substring(0, 2) == "\n")
            {
                NextDisplay();
                InputString = InputString.Remove(0, 2);
            }

            ControlledDisplays[ActiveDisplay].AddInputChar(InputString[0]);
            InputString = InputString.Remove(0, 1);
        }
    }
}
