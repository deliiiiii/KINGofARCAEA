using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCardManager : MonoBehaviour
{
    public int count_ScoreCard;//分数牌总数（算重复）
    public List<ScoreCard> scoreCardsCatagory;//种类+数量列表（不重复）
    public List<ScoreCard> scoreCardsPrefab = new List<ScoreCard>();//预制体列表（不重复）
    public List<ScoreCard> scoreCardsStock = new List<ScoreCard>();//卡组仓库（重复）
    void Awake()
    {
        scoreCardsCatagory = new List<ScoreCard>()
        {
            new ScoreCard(1,3,6),
            new ScoreCard(2,1,5),
            new ScoreCard(3,0,5),
        };
    }
    void Start()
    {
        for(int i=0;i<scoreCardsCatagory.Count;i++)//种类数
        {
            count_ScoreCard += scoreCardsCatagory[i].grossCount;//一个种类重复数
        }
        for(int i=0;i< scoreCardsCatagory.Count; i++)//种类数
        {
            for (int j = 0; j < scoreCardsCatagory[i].grossCount;j++)//一个种类重复数
            {
                scoreCardsStock.Add(scoreCardsPrefab[scoreCardsCatagory[i].index_Card - 1]);
            }
        }

    }
}
