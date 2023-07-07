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
    public int listID;
    public int totalScore;
    public List<int> roundScore = new List<int>();////待添加
    public int totalMove;
    public List<int> turnMove = new List<int>();////待添加
    
    public int count_HandCard;
    public int count_RoundUsedCard;
    public int count_TotalUsedCard;
    public int index_Player;
    public Text text_Index_Player;
    public string name_Player;
    public Text text_Name_Player;
    public Text Text_CardNum;
    public GameObject image_Holder;
    public GameObject image_MyTurn;
    public GameObject selectedCard;
    public GameObject scoreCard;
    public Player(int index_Player, string name_player)
    {
        totalScore = totalMove = 0;
        count_HandCard = 0;
        count_RoundUsedCard = 0;
        count_TotalUsedCard = 0;
        this.index_Player = index_Player;
        this.name_Player = name_player;
    }
    private void Awake()
    {
        instance = this;
    }

    public void DrawHandCards(int num,int index)//0开始
    {
        Debug.Log("index " + index + " draw " + num + "hand cards");
        for(int i=0;i<num;i++)
        {
            HandCardManager.instance.DrawOneCard(index);
            PlayerManager.list_player[index].GetComponent<Player>().Text_CardNum.text =  (int.Parse(PlayerManager.list_player[index].GetComponent<Player>().Text_CardNum.text)+1).ToString();
        }
    }
    public void DrawScoreCards(int num, int index)//0开始
    {
        Debug.Log("index " + index + " draw " + num + "score cards");
        for (int i = 0; i < num; i++)
        {
            ScoreCardManager.instance.DrawOneCard(index);
        }
    }
    public void YieldCard(int index)
    {
        selectedCard.GetComponent<HandCard>().CloseDetail();
        Debug.Log("打出序号" + selectedCard.GetComponent<HandCard>().index_Card);
        PlayerManager.list_player[index].GetComponent<Player>().Text_CardNum.text = (int.Parse(PlayerManager.list_player[index].GetComponent<Player>().Text_CardNum.text) - 1).ToString();
        
        switch (selectedCard.GetComponent<HandCard>().index_Card)////手牌新增
        {
            case 1001://代打
                Debug.Log("代打");
                break;
            case 1002://天下第一音游祭
                Debug.Log("天下第一音游祭");
                break;
            case 1003://指点江山
                Debug.Log("指点江山");
                break;
            case 1004://观看手元
                Debug.Log("观看手元");
                break;
            case 1005://神之左手
                Debug.Log("神之左手");
                break;
            case 1006://鬼之右手
                Debug.Log("鬼之右手");
                break;
            case 1007://音游窝
                Debug.Log("音游窝");
                break;
            case 1008://音游王
                Debug.Log("音游王");
                break;
            case 1009://联机
                Debug.Log("联机");
                break;
            case 1010://自来熟
                Debug.Log("自来熟");
                break;

            case 2001://手癖
                Debug.Log("手癖");
                break;
            case 2002://降噪耳机
                Debug.Log("降噪耳机");
                break;
            case 2003://网络延迟
                Debug.Log("网络延迟");
                break;

            case 3001://看铺
                Debug.Log("看铺");
                break;
            case 3002://私人订制手台
                Debug.Log("私人订制手台");
                break;
            case 3003://底力提升
                Debug.Log("底力提升");
                DrawHandCards(2, PlayerManager.index_CurrentPlayer - 1);
                break;
            case 3004://从头开始
                Debug.Log("从头开始");
                DrawScoreCards(1, PlayerManager.index_CurrentPlayer - 1);
                break;
        }

        int count_turn = PlayerManager.list_player[PlayerManager.index_CurrentPlayer - 1].GetComponent<Player>().turnMove.Count;
        PlayerManager.list_player[PlayerManager.index_CurrentPlayer - 1].GetComponent<Player>().turnMove[count_turn - 1]++;
        PlayerManager.list_player[PlayerManager.index_CurrentPlayer - 1].GetComponent<Player>().totalMove++;
        if (PlayerManager.list_player[PlayerManager.index_CurrentPlayer - 1].GetComponent<Player>().turnMove[count_turn - 1] >= 3)
        {
            UIManager.instance.UIFinishYieldCard();
        }

        UIManager.instance.DiscardCard(selectedCard);
        Destroy(selectedCard);
    }
    
    public void ThrowCard_Judge(int index)
    {
        if(int.Parse(Text_CardNum.text) <= 4)
        {
            GameManager.instance.NewTurn();
        }
    }
    public void ThrowCard(int index)
    {
        selectedCard.GetComponent<HandCard>().CloseDetail();
        Debug.Log("丢弃序号" + selectedCard.GetComponent<HandCard>().index_Card);
        PlayerManager.list_player[index].GetComponent<Player>().Text_CardNum.text = (int.Parse(PlayerManager.list_player[index].GetComponent<Player>().Text_CardNum.text) - 1).ToString();
        UIManager.instance.DiscardCard(selectedCard);
        Destroy(selectedCard);
        ThrowCard_Judge(index);
    }
}
