using ChoETL;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_ProfileInput : MonoBehaviour
{

    [SerializeField]
    private CS_Socket ReadSocket;

    [SerializeField]
    private CS_Socket WriteSocket;

    [SerializeField]
    private CS_Dock_SFDisplayManager InputDisplay;

    // Start is called before the first frame update
    void Start()
    {
        if (ReadSocket.IsNull())
        {
            Debug.LogError("ReadSocket is invalid!");
            return;
        }
        if (WriteSocket.IsNull())
        {
            Debug.LogError("WriteSocket is invalid!");
            return;
        }
    }

    public void WriteToSocket()
    {
        if(WriteSocket == null)
        {
            return;
        }

        GameObject PunchcardGO = WriteSocket.GetSocketedGO();
        if(PunchcardGO.IsNull())
        {
            Debug.LogError("Socket has no assigned GameObject!");
            return;
        }

        CS_PunchCard Punchcard = PunchcardGO.GetComponent<CS_PunchCard>();

        if (Punchcard.IsNull())
        {
            Debug.LogError("Attempted to write to invalid punchcard!");
            return;
        }

        Punchcard.SetCharacterData(InputDisplay.MakeCharacterDataFromDisplays());
    }

    public void ReadFromSocket()
    {
        if (ReadSocket == null)
        {
            return;
        }

        GameObject PunchcardGO = ReadSocket.GetSocketedGO();
        if (PunchcardGO.IsNull())
        {
            Debug.LogError("Socket has no assigned GameObject!");
            return;
        }

        CS_PunchCard Punchcard = PunchcardGO.GetComponent<CS_PunchCard>();

        if (Punchcard.IsNull())
        {
            Debug.LogError("Attempted to write to invalid punchcard!");
            return;
        }
        InputDisplay.ReadCharacterDataToDisplays(Punchcard.GetCharacterData());
    }

}
