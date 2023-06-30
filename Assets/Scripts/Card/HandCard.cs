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
        panel_HandCardDetail.SetActive(true);
        image_HandCard.sprite = gameObject.GetComponent<Image>().sprite;
    }


}
