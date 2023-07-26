using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyUI : MonoBehaviour
{
    // Start is called before the first frame update
    void MyDestroy()
    {
        Destroy(gameObject);
    }
    void MySetActiveFalse()
    {
        gameObject.SetActive(false);
    }
    void MySetState(GameManager.Temp_STATE state)
    {
        Debug.Log(Empty.instance.netId);
        Empty.instance.CmdSetState(state);
    }
}
