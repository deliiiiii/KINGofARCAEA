using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
public class Player : NetworkBehaviour
{
    public static Player instance;
    public int my_netID;
    public int totalScore;
    
    public Text text_Index_Player;
    public string name_Player;
    public Text text_Name_Player;
    public Text Text_CardNum;
    public GameObject panel_You;
    public GameObject panel_Select;
    public GameObject panel_Selected;
    public GameObject panel_UnSelected;
    public GameObject panel_ToSelect;
    public GameObject panel_ToUnSelect;
    public GameObject image_Holder;
    public GameObject image_MyTurn;
    public GameObject content_State;
    public GameObject panel_Notice_StateCard;
    public Text Text_notice_State;
    public Toggle toggle_score_3; 
    public Toggle toggle_score_1; 
    public Toggle toggle_score_0;
    public Text text_totalScore;
    public Text text_RoundScore;
    public List<Toggle> list_toggle_score;

    public GameObject card_1005_GetLeftHand;
    public GameObject card_1005_GetRightHand;
    public Text card_1005_GiveWhom;
    //public Player(int index_Player, string name_player)
    //{
    //    totalScore = totalMove = 0;
    //    count_MyHandCard = 0;
    //    count_RoundUsedCard = 0;
    //    count_TotalUsedCard = 0;
    //    this.index_Player = index_Player;
    //    this.name_Player = name_player;
    //}
    private void Awake()
    {
        instance = this;

        list_toggle_score = new()
        {
            toggle_score_3,
            toggle_score_1,
            toggle_score_0,
        };
        for (int i=0 ; i < list_toggle_score.Count; i++)
        {
            list_toggle_score[i].onValueChanged.AddListener((bool value) => RefreshText_RoundScore_by_toggle());
        }

    }

    public void RefreshText_RoundScore_by_toggle()
    {
        int count_isOn = 0, index_last_isOn = -1, i = 0;
        for (; i<list_toggle_score.Count;i++)
        {
            if (list_toggle_score[i].isOn)
            {
                count_isOn++;
                index_last_isOn = i;
            }
        }
        if(count_isOn == 1)
        {
            switch (index_last_isOn)
            {
                case 0:
                    text_RoundScore.text = "3";
                    break;
                case 1:
                    text_RoundScore.text = "1";
                    break;
                case 2:
                    text_RoundScore.text = "0";
                    break;
                default:
                    break;
            }
        }
        else
        {
            text_RoundScore.text = "?";
        }
    }

    public void RefreshText_RoundScore_by_scoreCard(int score)
    {
        text_RoundScore.text = score.ToString();
        switch (score)
        {
            case 3:case 4:
                toggle_score_3.isOn = true;
                toggle_score_1.isOn = false;
                toggle_score_0.isOn = false;
                break;
            case 1:
                toggle_score_3.isOn = false;
                toggle_score_1.isOn = true;
                toggle_score_0.isOn = false;
                break;
            case 0:
                toggle_score_3.isOn = false;
                toggle_score_1.isOn = false;
                toggle_score_0.isOn = true;
                break;
            default:
                break;
        }
    }

    public void Refresh_list_toggle_score(bool[] list_toggle_isOn)
    {
        for(int i=0;i< list_toggle_score.Count;i++)
        {
            list_toggle_score[i].isOn = list_toggle_isOn[i];
        }
        //RefreshText_RoundScore_by_toggle();
    }

    public void Clear_toggle_isOns()
    {
        for (int i=0; i < list_toggle_score.Count; i++)
        {
            list_toggle_score[i].isOn = false;
        }
    }

    public void AddStateCard(int id_attacker,List<int> list_index_offender, int index_Card)
    {
        GameObject temp =  Instantiate(StateCardManager.instance.GetStateCardByIndex(index_Card), content_State.transform);
        temp.GetComponent<StateCard>().list_index_offender= list_index_offender;
        temp.GetComponent<StateCard>().notice_State = temp.GetComponent<StateCard>().notice_State.Replace("%", temp.GetComponent<StateCard>().ChangeIndexToString());
        temp.GetComponent<StateCard>().id_attacker = id_attacker;
        temp.SetActive(true);
        
    }
    //public void DrawHandCards(int num,int index)//0开始
    //{
    //    Debug.Log("index " + index + " draw " + num + "hand cards");
    //    for(int i=0;i<num;i++)
    //    {
    //        ///////HandCardManager.instance.DrawOneCard(index);
    //        UIPlayerManager.list_player[index].GetComponent<Player>().Text_CardNum.text =  (int.Parse(UIPlayerManager.list_player[index].GetComponent<Player>().Text_CardNum.text)+1).ToString();
    //    }
    //}
    //public void DrawScoreCards(int num, int index)//0开始
    //{
    //    Debug.Log("index " + index + " draw " + num + "score cards");
    //    for (int i = 0; i < num; i++)
    //    {
    //        //ScoreCardManager.instance.DrawOneCard(index);
    //    }
    //}


    //public void ThrowCard_Judge(int index)
    //{
    //    if(int.Parse(Text_CardNum.text) <= 4)
    //    {
    //        Empty.instance.ClientNewTurn();
    //    }
    //}
    //public void ThrowCard(int index)
    //{
    //    selectedCard.GetComponent<HandCard>().CloseDetail();
    //    Debug.Log("丢弃序号" + selectedCard.GetComponent<HandCard>().index_Card);
    //    UIPlayerManager.list_player[index].GetComponent<Player>().Text_CardNum.text = (int.Parse(UIPlayerManager.list_player[index].GetComponent<Player>().Text_CardNum.text) - 1).ToString();
    //    ///////UIManager.instance.CallClient_UIDiscardCard(selectedCard);
    //    Destroy(selectedCard);
    //    ThrowCard_Judge(index);
    //}
}
