using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public GameObject canvas;

    public bool card_1002_shouldCheckButtonInteractive = false;

    public List<int> card_1007_temp_list_score;
    public List<int> card_1007_temp_list_id_of_score;
    public int card_1007_temp_last_id;
    public bool card_1007_temp_circled;

    public float delay = 0.2f;
    public GameObject content_MyHandCard;
    public GameObject panel_DiscardedCards;
    public GameObject panel_Notice_Back;
    public GameObject panel_Card_1002;
    public GameObject panel_Card_1004;
    public GameObject panel_CardDetail_1004;
    public GameObject content_Card_1002;
    public GameObject panel_ScoreCard_Hidden;
    public GameObject panel_Wait;
    public GameObject panel_Card_1007;
    public GameObject panel_CardDetail_1007;
    public GameObject panel_DiscloseScore;
    public GameObject panel_DiscloseScoreDetail;
    public Text text_NoticeThrowCard;
    public Text text_CircleNum;
    public Text text_Notice;
    public Text text_name_1004;
    public Text text_name_1007;
    public Text text_name_DisclosedPlayer;
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
    void Update()
    {
        if(card_1002_shouldCheckButtonInteractive)
        {
            if(!UICard_1002_CheckButtonInteractive())
            {
                UICard_1002_ClosePanel();
            }
        }
        if (canvas.activeSelf)
        {
            //Debug.Log(GameManager.state_);
            switch (GameManager.instance.state_)
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
        Empty.instance.CmdSetState(GameManager.Temp_STATE.STATE_THROW_CARDS);
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
        GameManager.instance.state_ = GameManager.Temp_STATE.STATE_BUSYCONNECTING;
        Empty.instance.CmdSetState(GameManager.Temp_STATE.STATE_BUSYCONNECTING);
        Empty.instance.ClientRealizeHandCard(temp_list_index_offender);
        UIPlayerManager.instance.Hide_Button_Select();
    }

    public void UIGiveUpSelection()
    {
        Empty.instance.CmdSetState(GameManager.Temp_STATE.STATE_YIELD_CARDS);
        //Empty.instance.ClientRealizeHandCard(new List<int> {-1});
        UIPlayerManager.instance.Hide_Button_Select();

        //if(Empty.instance.selectedCard.GetComponent<HandCard>().index_Card == 1001)
        //{
            Empty.instance.ClientOnEndRealizeHandCard();
        //}
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


    public void UICard_1002_ShowPanel(int id_turn, List<int> list_scoreCard)
    {
        if (panel_Card_1002.activeSelf)
        {
            //Debug.LogError("重复调用!");
            return;//防止重复调用
        }
        if((int)Empty.instance.netId == id_turn)
        {
            panel_Wait.SetActive(false);
        }
        else
        {
            panel_Wait.SetActive(true);
        }
        ClearChild(content_Card_1002.transform);
        panel_Card_1002.SetActive(true);
        for(int i=0;i<Empty.list_netId.Count;i++)
        {
            GameObject temp = Instantiate(panel_ScoreCard_Hidden,content_Card_1002.transform);
            temp.SetActive(true);
            temp.AddComponent<ScoreCard>().score = list_scoreCard[i];
        }

        UIPlayerManager.instance.Card_1002_ClearAllSuspectedCard();
        
    }

    public void UICard_1002_NextTurn(int last_id_turn,int this_id_turn, int index_last_Selected)
    {
        if(!UICard_1002_CheckButtonInteractive())
        {
            UICard_1002_ClosePanel();
            return;
        }
        card_1002_shouldCheckButtonInteractive = true;
        if ((int)Empty.instance.netId == this_id_turn)
        {
            panel_Wait.SetActive(false);
        }
        else
        {
            panel_Wait.SetActive(true);
        }
        content_Card_1002.transform.GetChild(index_last_Selected).gameObject.GetComponent<Button>().interactable = false;
        content_Card_1002.transform.GetChild(index_last_Selected).GetChild(0).GetComponent<Text>().text = Empty.list_playerName[Empty.instance.GetIndex_in_list_netId(last_id_turn)];
    }
    public bool UICard_1002_CheckButtonInteractive()
    {
        for(int i=0;i< content_Card_1002.transform.childCount;i++)
        {
            if (!content_Card_1002.transform.GetChild(i).gameObject.activeSelf) continue;
            if (content_Card_1002.transform.GetChild(i).gameObject.GetComponent<Button>().interactable) return true;
        }
        return false;
    }
    public void UICard_1002_ClosePanel()
    {
        card_1002_shouldCheckButtonInteractive = false;
        panel_Wait.SetActive(false);
        panel_Card_1002.SetActive(false);
        //Debug.Log("?");
        Empty.instance.CmdSetState(GameManager.Temp_STATE.STATE_YIELD_CARDS);
    }
    public void UICard_1004_ShowPanel(int id_offender, int score)
    {
        text_name_1004.text = Empty.list_playerName[Empty.instance.GetIndex_in_list_netId(id_offender)];
        ClearChild(panel_CardDetail_1004.transform);
        GameObject temp = Instantiate(ScoreCardManager.instance.GetScoreCardByScore(score), panel_CardDetail_1004.transform);
        temp.SetActive(true);
        panel_Card_1004.SetActive(true);
        Empty.instance.CmdSetState(GameManager.Temp_STATE.STATE_YIELD_CARDS);
    }
    public void UICard_1007_ShowPanel(List<int>list_score,List<int>list_id_of_score)
    {
        Debug.Log("分数列表 = " + Empty.instance.GetContent_int(list_score) + " 分数id  = " + Empty.instance.GetContent_int(list_id_of_score));
        Debug.Log("empty id列表 = " + Empty.instance.GetContent_int(Empty.list_netId));
        card_1007_temp_list_score = list_score;
        card_1007_temp_list_id_of_score = list_id_of_score;
        if(list_id_of_score.Count == 0) 
        {
            panel_Notice_Back.SetActive(true);
            text_Notice.text = "没有符合条件的玩家！";
            Empty.instance.CmdSetState(GameManager.Temp_STATE.STATE_YIELD_CARDS);
            return;
        }
        int index_id = -1;
        int target_id = (int)Empty.instance.netId + 1;
        int max_id = -1;
        for(int i = 0;i<Empty.list_netId.Count;i++)
        {
            if (Empty.list_netId[i] > max_id)
            {
                max_id = Empty.list_netId[i];
            }
        }
        card_1007_temp_circled = false;
        while (!list_id_of_score.Contains(target_id))
        {
            target_id++;
            if (target_id > max_id)
            {
                card_1007_temp_circled = true;
                target_id = 0;
            }
        }
        if (card_1007_temp_circled && (target_id >= (int)Empty.instance.netId)) return;
        for (int i=0;i< list_id_of_score.Count;i++)
        {
            
            
            if (target_id == list_id_of_score[i])
            {
                index_id = i;
                break;
            }
        }
        Debug.Log("last_id = " + list_id_of_score[index_id]);
        card_1007_temp_last_id = list_id_of_score[index_id];
        text_name_1007.text = Empty.list_playerName[Empty.instance.GetIndex_in_list_netId(list_id_of_score[index_id])];
        ClearChild(panel_CardDetail_1007.transform);
        GameObject temp = Instantiate(ScoreCardManager.instance.GetScoreCardByScore(list_score[index_id]), panel_CardDetail_1007.transform);
        temp.SetActive(true);
        panel_Card_1007.SetActive(true);

        //card_1007_temp_circled = false;
    }
    public void UICard_1007_ShowNext()//编辑器已添加监听
    {
        int index_id = -1;
        int target_id = card_1007_temp_last_id + 1;
        int max_id = -1;
        for (int i = 0; i < Empty.list_netId.Count; i++)
        {
            if (Empty.list_netId[i] > max_id)
            {
                max_id = Empty.list_netId[i];
            }
        }
        while (!card_1007_temp_list_id_of_score.Contains(target_id))
        {
            target_id++;
            if (target_id > max_id)
            {
                if(card_1007_temp_circled)
                {
                    panel_Card_1007.SetActive(false);
                    Empty.instance.CmdSetState(GameManager.Temp_STATE.STATE_YIELD_CARDS);
                    return;
                }
                card_1007_temp_circled = true;
                target_id = 0;
            }
        }
        Debug.Log("Target ID = " + target_id);
        card_1007_temp_last_id = target_id;
        if (card_1007_temp_circled && (target_id >= (int)Empty.instance.netId))
        {
            Empty.instance.CmdSetState(GameManager.Temp_STATE.STATE_YIELD_CARDS);
            panel_Card_1007.SetActive(false);
            return;
        }
        for (int i = 0; i < card_1007_temp_list_id_of_score.Count; i++)
        {
            
            //if (target_id == Empty.index_CurrentHolder - 1)
            //{
            //    panel_Card_1007.SetActive(false);
            //    return;
            //}
            if (target_id == card_1007_temp_list_id_of_score[i])
            {
                index_id = i;
                break;
            }
        }
        if(index_id == -1)
        {
            panel_Card_1007.SetActive(false);
            Empty.instance.CmdSetState(GameManager.Temp_STATE.STATE_YIELD_CARDS);
            return;
        }
        Debug.Log("last_id = " + card_1007_temp_list_id_of_score[index_id]);
        text_name_1007.text = Empty.list_playerName[Empty.instance.GetIndex_in_list_netId(card_1007_temp_list_id_of_score[index_id])];
        ClearChild(panel_CardDetail_1007.transform);
        GameObject temp = Instantiate(ScoreCardManager.instance.GetScoreCardByScore(card_1007_temp_list_score[index_id]), panel_CardDetail_1007.transform);
        temp.SetActive(true);
    }
    public void UICard_1008_ShowPanel(int id_attacker,int id_offender,int score)
    {
        Debug.Log(id_attacker + "获得 " + id_offender+ " 的" + score+ "分");
    }
    public void DiscloseScoreCard(int index_Shown, List<int> list_score, List<int> list_id_of_score)
    {

        int id_shown = Empty.list_netId[index_Shown];
        int index_in_two_list = -1;
        for(int i=0;i< list_score.Count;i++)
        {
            if(list_id_of_score[i] == id_shown)
            {
                index_in_two_list = i;
            }
        }
        Debug.Log("展示！" + Empty.list_playerName[index_Shown] + "的分数是：" + list_score[index_in_two_list]);
        text_name_DisclosedPlayer.text = Empty.list_playerName[index_Shown];
        GameObject temp = Instantiate(ScoreCardManager.instance.GetScoreCardByScore(list_score[index_in_two_list]), panel_DiscloseScoreDetail.transform);
        temp.SetActive(true);
        panel_DiscloseScore.SetActive(true);
        int totalScore = int.Parse(UIPlayerManager.list_player[index_Shown].GetComponent<Player>().text_totalScore.text);
        totalScore += list_score[index_in_two_list];
        UIPlayerManager.list_player[index_Shown].GetComponent<Player>().text_totalScore.text = totalScore.ToString();
    }

    public void EndDiscloseScoreCard()
    {
        panel_DiscloseScore.SetActive(false);
        
    }
    public void ClearChild(Transform t_parent)
    {
        Transform t_child;

        for (int i = 0; i < t_parent.transform.childCount; i++)
        {
            t_child = t_parent.transform.GetChild(i);
            if(t_child.gameObject.name.Contains("lone)"))
            Destroy(t_child.gameObject);
            else t_child.gameObject.SetActive(false);
        }

    }

    public string GetContent_int(List<int> list)
    {
        string a = "";
        for (int i = 0; i < list.Count; i++)
        {
            a += list[i].ToString();
            a += " ";
        }
        return a;
    }
    public string GetContent_string(List<string> list)
    {
        string a = "";
        for (int i = 0; i < list.Count; i++)
        {
            a += list[i].ToString();
            a += " ";
        }
        return a;
    }


    public void FlashText(GameObject g)
    {
        g.GetComponent<Animator>().SetTrigger("Trigger_Flash");
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
