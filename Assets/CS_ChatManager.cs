using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct FChatMessage
{
    // #TODO [KA] (29.09.2024): Replace OwnerName with an OwnerHandle from the story maanger.
    public string OwnerName;


    public string RawMessage;
    public string PrintMessage;

}

public class CS_ChatManager : MonoBehaviour
{
}
