using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreCardManager : MonoBehaviour
{
    public static ScoreCardManager instance;
    public int count_ScoreCard;//分数牌总数（算重复）
    public List<ScoreCard> scoreCardsCatagory;//种类+数量列表（不重复）
    public List<ScoreCard> scoreCardsPrefab = new List<ScoreCard>();//预制体列表（不重复）
    public List<ScoreCard> scoreCardsStock = new List<ScoreCard>();//卡组仓库（重复）
    public Text text_CardNum;
    void Awake()
    {
        instance = this;
        scoreCardsCatagory = new List<ScoreCard>()
        {
            new ScoreCard(1,3,6),
            new ScoreCard(2,1,5),
            new ScoreCard(3,0,5),
        };
    }
    void Start()
    {
        
    }

    public void RefillScoreCards()
    {
        count_ScoreCard = 0;
        for (int i = 0; i < scoreCardsCatagory.Count; i++)//种类数
        {
            count_ScoreCard += scoreCardsCatagory[i].grossCount;//一个种类重复数
        }
        scoreCardsStock.Clear();
        for (int i = 0; i < scoreCardsCatagory.Count; i++)//种类数
        {
            for (int j = 0; j < scoreCardsCatagory[i].grossCount; j++)//一个种类重复数
            {
                scoreCardsStock.Add(scoreCardsPrefab[scoreCardsCatagory[i].index_Card - 1]);
            }
        }
        text_CardNum.text = count_ScoreCard.ToString();
        scoreCardsStock = RandomList(scoreCardsStock);
    }

    public List<T> RandomList<T>(List<T> inList)
    {
        List<T> newList = new List<T>();
        int count = inList.Count;
        for (int i = 0; i < count; i++)
        {
            int temp = UnityEngine.Random.Range(0, inList.Count - 1);
            T tempT = inList[temp];
            newList.Add(tempT);
            inList.Remove(tempT);
        }
        //将最后一个元素再随机插入
        T tempT2 = newList[newList.Count - 1];
        newList.RemoveAt(newList.Count - 1);
        newList.Insert(UnityEngine.Random.Range(0, newList.Count), tempT2);
        inList = newList;
        return inList;
    }
    public void DrawOneCard(int index)
    {
        text_CardNum.text = (int.Parse(text_CardNum.text) - 1).ToString();
        ScoreCard p = Instantiate(scoreCardsStock[0], HandCardManager.list_Scroll_MyHandCard[index].transform.GetChild(1));
        p.gameObject.SetActive(true);
        scoreCardsStock.RemoveAt(0);/////判断空
    }
}
