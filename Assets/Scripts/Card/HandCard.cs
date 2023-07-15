using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandCard : GrandCard
{
    public bool isAttackCard;//攻击类(A)手牌
    public bool isExchangeCard;//交换类(E)手牌
    public bool isTimingCard;//延时类(T)手牌
    public int count_offender;//受击者应该有的数量
    public GameObject panel_HandCardDetail;
    public GameObject panel_HandCardDetail_ReadOnly;
    public GameObject panel_New;
    public Image image_HandCard;
    public Image image_HandCard_ReadOnly;
    public List<int> index_attacker = new List<int>();////待添加 攻击者序号列表
    public List<int> index_offender = new List<int>();////待添加 受击者序号列表
    public HandCard(int index, int count,bool isAttackCard, int count_offender, bool isExchangeCard, bool isTimingCard)
    {
        base.index_Card = index;
        this.grossCount = count;
        this.isAttackCard = isAttackCard;
        this.count_offender = count_offender;
        this.isExchangeCard = isExchangeCard;
        this.isTimingCard = isTimingCard;
    }

    public void ShowDetail()
    {
        panel_New.SetActive(false);
        if ( GameManager.state_ != GameManager.Temp_STATE.STATE_SELECTING_TARGETPLAYER)
        {
            Empty.instance.selectedCard = gameObject;
        }

        if(gameObject.GetComponent<GrandCard>().used == true)//已用过
        {
            panel_HandCardDetail_ReadOnly.SetActive(true);
            panel_HandCardDetail.SetActive(false);
            image_HandCard_ReadOnly.sprite = gameObject.GetComponent<Image>().sprite;
        }
        else
        {
            panel_HandCardDetail_ReadOnly.SetActive(false);
            panel_HandCardDetail.SetActive(true);
            image_HandCard.sprite = gameObject.GetComponent<Image>().sprite;
        }

    }
    public void CloseDetail()
    {
        panel_HandCardDetail.SetActive(false);
    }
    
}
