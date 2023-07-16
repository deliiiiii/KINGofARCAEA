using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public GameObject canvas;

    public float delay = 0.2f;
    public GameObject content_MyHandCard;
    public GameObject panel_DiscardedCards;
    public GameObject panel_Notice_Back;
    public Text text_NoticeThrowCard;
    public Text text_CircleNum;
    public Text text_Notice;
    public Button button_Start_Game;//开始游戏
    public Button button_YieldCard;//打出按钮
    public Button button_FinishYieldCard;//结束出牌按钮
    public Button button_ThrowCard;//结束出牌按钮
    public Button button_Confirm_Selection;//确认选定玩家
    public Button button_GiveUp_Selection;//放弃选定玩家

    private void Awake()
    {
        instance = this;
        button_Start_Game.onClick.AddListener(UIStartGame);
        button_YieldCard.onClick.AddListener(UIYieldCard);
        button_FinishYieldCard.onClick.AddListener(UIFinishYieldCard);
        button_ThrowCard.onClick.AddListener(UIThrowCard);
        button_Confirm_Selection.onClick.AddListener(UIConfirmSelection);
        button_GiveUp_Selection.onClick.AddListener(UIGiveUpSelection);
    }

    // Update is called once per frame
    void Update()
    {
        if(canvas.activeSelf)
        {
            Debug.Log(GameManager.state_);
            switch (GameManager.state_)
            {
                
                case GameManager.Temp_STATE.STATE_YIELD_CARDS:
                    {
                        if((Empty.index_CurrentPlayer - 1) == Empty.instance.GetIndex_in_list_netId((int)Empty.instance.netId))
                        {
                            button_YieldCard.gameObject.SetActive(true);
                            button_FinishYieldCard.gameObject.SetActive(true);
                        }
                        else
                        {
                            button_YieldCard.gameObject.SetActive(false);
                            button_FinishYieldCard.gameObject.SetActive(false);
                        }
                        
                        
                        button_ThrowCard.gameObject.SetActive(false);
                        text_NoticeThrowCard.gameObject.SetActive(false);
                        break;
                    }
                case GameManager.Temp_STATE.STATE_THROW_CARDS:
                    {
                        if ((Empty.index_CurrentPlayer - 1) == Empty.instance.GetIndex_in_list_netId((int)Empty.instance.netId))
                        {
                            button_ThrowCard.gameObject.SetActive(true);
                            text_NoticeThrowCard.gameObject.SetActive(true);
                        }
                        else
                        {
                            button_ThrowCard.gameObject.SetActive(false);
                            text_NoticeThrowCard.gameObject.SetActive(false);
                        }
                        button_YieldCard.gameObject.SetActive(false);
                        button_FinishYieldCard.gameObject.SetActive(false);
                        
                        //Debug.Log("THROW CARDS !!!");
                        break;
                    }
                case GameManager.Temp_STATE.STATE_SELECTING_TARGETPLAYER:
                    {
                        button_ThrowCard.gameObject.SetActive(false);
                        text_NoticeThrowCard.gameObject.SetActive(false);
                        button_YieldCard.gameObject.SetActive(false);
                        button_FinishYieldCard.gameObject.SetActive(false);
                        break;
                    }
                default:
                    {
                        //button_YieldCard.gameObject.SetActive(false);
                        button_FinishYieldCard.gameObject.SetActive(false);
                        button_ThrowCard.gameObject.SetActive(false);
                        text_NoticeThrowCard.gameObject.SetActive(false);
                        break;
                    }
            }
        }
        
        
    }
    public void UIStartGame()
    {
        Empty.instance.ClientStartGame();
        //button_Start_Game.gameObject.SetActive(false);
    }
    public void UIYieldCard()
    {
        Empty.instance.ClientYieldCard();
    }
    public void UIFinishYieldCard()
    {
        GameManager.state_ = GameManager.Temp_STATE.STATE_THROW_CARDS;
        Empty.instance.Client_ThrowCard_EndJudge((int)Empty.instance.netId);
    }
    public void UIThrowCard()
    {
        Empty.instance.ClientThrowCard();
    }

    public void UIConfirmSelection()
    {
        List<int> temp_list_index_offender= new();//临时受击者列表
        for(int i=0;i<Empty.list_netId.Count;i++)
        {
            if (UIPlayerManager.list_player[i].GetComponent<Player>().panel_Selected.activeSelf)
            {
                temp_list_index_offender.Add(Empty.list_netId[i]);
            }
        }
        if(Empty.instance.selectedCard.GetComponent<HandCard>().count_offender != temp_list_index_offender.Count)
        {
            text_Notice.text = "选择数量不正确! 请重新选择";
            panel_Notice_Back.SetActive(true);
            return;
        }

        Empty.instance.ClientRealizeHandCard(temp_list_index_offender);
        UIPlayerManager.instance.Hide_Button_Select();
    }

    public void UIGiveUpSelection()
    {
        Empty.instance.ClientRealizeHandCard(new List<int> {-1});
        UIPlayerManager.instance.Hide_Button_Select();
    }
    public void DiscardScorecard(int index,Vector2 v, Quaternion q)//将牌放在弃牌区
    {
        GameObject temp = ScoreCardManager.instance.GetScoreCardByIndex(index);
        temp = Instantiate(temp, panel_DiscardedCards.transform);
        temp.GetComponent<Button>().interactable = false;
        temp.transform.localPosition = v;
        temp.transform.localRotation = q;
        temp.SetActive(true);
    }
    public void DiscardHandcard(int index, Vector2 v, Quaternion q)//将牌放在弃牌区
    {
        GameObject temp = HandCardManager.instance.GetHandCardByIndex(index);
        temp.GetComponent<GrandCard>().used = true;
        temp.GetComponent<HandCard>().panel_New.SetActive(false);
        temp = Instantiate(temp, panel_DiscardedCards.transform);
        //temp.GetComponent<Button>().interactable = false;
        temp.GetComponent<Button>().interactable = true;
        temp.transform.localPosition = v;
        temp.transform.localRotation = q;
        temp.SetActive(true);
    }

    public void ClearHandCards_and_ScoreCard()
    {
        ClearChild(content_MyHandCard.transform);
        if (Empty.instance.scoreCard)
        {
            Destroy(Empty.instance.scoreCard);
            Empty.instance.scoreCard = null;
        }
           
    }

    public void ClearAllHandCards()
    {
        ClearChild(content_MyHandCard.transform);
    }
    public void ClearChild(Transform t_parent)
    {
        Transform t_child;

        for (int i = 0; i < t_parent.transform.childCount; i++)
        {
            t_child = t_parent.transform.GetChild(i);
            //Destroy(t_child.gameObject);
            t_child.gameObject.SetActive(false);
        }

    }

    public void Delay_ShowStartGame()
    {
        if(!Empty.instance)
        {
            Debug.Log("Delay_ShowStartGame()");
            Invoke(nameof(Delay_ShowStartGame), delay);
            return;
        }
        if (Empty.list_netId.Count == 0)
        {
            Debug.Log("Delay_ShowStartGame()");
            Invoke(nameof(Delay_ShowStartGame), delay);
            return;
        }
        if (Empty.list_netId[0] == (int)Empty.instance.netId)
        {
            button_Start_Game.gameObject.SetActive(true);
        }
        else
        {
            button_Start_Game.gameObject.SetActive(false);
        }
    }
}
