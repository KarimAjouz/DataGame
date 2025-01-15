using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CS_Socket : MonoBehaviour
{
    public Transform SocketedTransform;

    [SerializeField]
    private GameObject SocketedGO = null;

    [SerializeField]
    public UnityEvent OnSocket;
    [SerializeField]
    public UnityEvent OnUnsocket;

    public void SocketGO(GameObject go)
    {
        if(go == null)
        {
            Debug.LogError("Can not socket null GameObject!");
            return;
        }
        SocketedGO = go;

        OnSocket.Invoke();
    }

    public void Unsocket()
    {
        SocketedGO = null;
        OnUnsocket.Invoke();
    }

    public GameObject GetSocketedGO()
    {
        return SocketedGO;
    }

    public void DestroySocketedGO()
    {
        Destroy(SocketedGO);
    }

}
