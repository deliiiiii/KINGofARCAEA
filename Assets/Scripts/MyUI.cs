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
    void MySetActiveTrue()
    {
        gameObject.SetActive(true);
    }
    void MySetActive(bool state)
    {
        gameObject.SetActive(state);
    }
    void MySetState(GameManager.Temp_STATE state)
    {
        Empty.instance.CmdSetState(state);
    }
    void MySetStateClientOnly(GameManager.Temp_STATE state)
    {
        GameManager.instance.state_ = state;
    }
    void MyShowScoreCard()
    {
        Empty.instance.scoreCard.SetActive(true);
    }
    void MyDebug(string de)
    {
        Debug.Log(de);
    }
}
