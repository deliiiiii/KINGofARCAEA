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
