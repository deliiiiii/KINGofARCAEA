using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandCard : GrandCard
{
    public bool isAttackCard;//攻击类(A)手牌
    public bool isExchangeCard;//交换类(E)手牌
    public bool isTimingCard;//延时类(T)手牌
    public GameObject panel_HandCardDetail;
    public Image image_HandCard;
    public List<int> index_attacker = new List<int>();////待添加 攻击者序号列表
    public List<int> index_offender = new List<int>();////待添加 受击者序号列表
    public HandCard(int index, int count,bool isAttackCard, bool isExchangeCard, bool isTimingCard)
    {
        base.index_Card = index;
        this.grossCount = count;
        this.isAttackCard = isAttackCard;
        this.isExchangeCard = isExchangeCard;
        this.isTimingCard = isTimingCard;
    }

    public void ShowDetail()
    {
        UIPlayerManager.list_player[UIPlayerManager.index_CurrentPlayer - 1].GetComponent<Player>().selectedCard = gameObject;
        panel_HandCardDetail.SetActive(true);
        image_HandCard.sprite = gameObject.GetComponent<Image>().sprite;
        UIManager.index_SelectedHandCard = gameObject.GetComponent<HandCard>().index_Card;
        UIManager.index_Card_In_Hand = gameObject.transform.GetSiblingIndex();
    }
    public void CloseDetail()
    {
        panel_HandCardDetail.SetActive(false);
    }
    
}
