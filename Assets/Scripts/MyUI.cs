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
        if(state == GameManager.Temp_STATE.STATE_YIELD_CARDS)
        {
            

            int count_turn = Empty.instance.turnMove.Count;
            Empty.instance.turnMove[count_turn - 1]++;
            Debug.Log("已行动次数 =" + Empty.instance.turnMove[count_turn - 1]);
            //instance.totalMove++;
            if (Empty.instance.turnMove[count_turn - 1] >= 3)
            {
                Debug.Log("准备弃牌");
                UIManager.instance.UIFinishYieldCard();
                return;
            }
            Empty.instance.selectedCard.GetComponent<HandCard>().CloseDetail();
        }
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
