using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCardManager : MonoBehaviour
{
    public int count_HandCard;//手牌总数（算重复）
    public List<HandCard> handCardsCatagory;//种类+数量列表（不重复）
    public List<HandCard> handCardsPrefab = new List<HandCard>();//预制体列表（不重复）
    public List<HandCard> handCardsStock = new List<HandCard>();//卡组仓库（重复）
    void Awake()
    {
        handCardsCatagory = new List<HandCard>()
        {
            new HandCard(1,2,false,false),
        };

    }
    void Start()
    {
       
        for (int i = 0; i < handCardsCatagory.Count; i++)//种类数
        {
            count_HandCard += handCardsCatagory[i].grossCount;//一个种类重复数
        }
        for (int i = 0; i < handCardsCatagory.Count; i++)//种类数
        {
            //Debug.Log("i = " + i + "scoreCardsCatagory[i].count = " + scoreCardsCatagory[i].count);
            for (int j = 0; j < handCardsCatagory[i].grossCount; j++)//一个种类重复数
            {
                //Debug.Log("j = " + j);
                handCardsStock.Add(handCardsPrefab[handCardsCatagory[i].index_Card - 1]);
            }
        }
    }
}
