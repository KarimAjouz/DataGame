using ChoETL;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Socketable : MonoBehaviour
{
    private GameObject SocketedTo = null;

    public virtual void SocketTo(GameObject go)
    {
        if (go.IsNull())
        {
            return;
        }

        Debug.Log("Socketing " + gameObject.name + " to: " + go.name);
        SocketedTo = go;
    }

    public virtual void Unsocket()
    {
        Debug.Log("Unsocketing: " + gameObject.name);
        SocketedTo = null;
    }
}
