using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int totalScore;
    public List<int> roundScore = new List<int>();
    public int scoreCard;
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

    public Player(int index_Player, string name_player)
    {
        totalScore = 0;
        scoreCard = 0;
        count_HandCard = 0;
        count_RoundUsedCard = 0;
        count_TotalUsedCard = 0;
        this.index_Player = index_Player;
        this.name_Player = name_player;
    }

    private void Start()
    {
        
    }
    public void DrawCards(int num,int index)//0¿ªÊ¼
    {
        Debug.Log("index " + index + " draw " + num + " cards");
        for(int i=0;i<num;i++)
        {
            HandCardManager.instance.DrawOneCard(index);
            PlayerManager.list_player[index].Text_CardNum.text =  (int.Parse(PlayerManager.list_player[index].Text_CardNum.text)+1).ToString();
        }
    }
}