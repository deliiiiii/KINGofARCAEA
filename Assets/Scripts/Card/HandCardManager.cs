using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class HandCardManager : MonoBehaviour
{
    public static HandCardManager instance;
    public int count_HandCard;//手牌总数（算重复）
    public List<HandCard> handCardsCatagory;//种类+数量列表（不重复）
    public List<HandCard> handCardsPrefab = new List<HandCard>();//预制体列表（不重复）
    public List<HandCard> handCardsStock = new List<HandCard>();//卡组仓库（重复）
    public Text text_CardNum;
    public GameObject list_MyHandCard;
    public GameObject scroll_GameCard;
    public static List<GameObject> list_Scroll_MyHandCard = new List<GameObject>();
    void Awake()
    {
        instance = this;
        handCardsCatagory = new List<HandCard>()
        {
            new HandCard(1,15,false,false),
        };

    }
    void Start()
    {
       
        
    }

    public void Initialize()
    {
        for(int i=0;i<GameManager.instance.count_Player;i++)
        {
            list_Scroll_MyHandCard.Add(Instantiate(scroll_GameCard, list_MyHandCard.transform));
            //while(list_Scroll_MyHandCard[i].transform.GetChild(0).GetChild(0).childCount != 0)
            {
                //Destroy(list_Scroll_MyHandCard[i].transform.GetChild(0).GetChild(0).gameObject);
            }
            list_Scroll_MyHandCard[i].SetActive(false);
        }
        RefillHandCards();
    }
    public void RefillHandCards()
    {
        for (int i = 0; i < handCardsCatagory.Count; i++)//种类数
        {
            count_HandCard += handCardsCatagory[i].grossCount;//一个种类重复数
        }
        for (int i = 0; i < handCardsCatagory.Count; i++)//种类数
        {
            handCardsStock.Clear();
            for (int j = 0; j < handCardsCatagory[i].grossCount; j++)//一个种类重复数
            {
                handCardsStock.Add(handCardsPrefab[handCardsCatagory[i].index_Card - 1]);
            }
        }
        text_CardNum.text = count_HandCard.ToString();
        handCardsStock = RandomList(handCardsStock);
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
        text_CardNum.text = (int.Parse(text_CardNum.text)-1).ToString();
        Debug.Log("index = " +index);
        HandCard p = Instantiate(handCardsStock[0], list_Scroll_MyHandCard[index].gameObject.transform.GetChild(0).GetChild(0));
        p.gameObject.SetActive(true);
        handCardsStock.RemoveAt(0);
    }
}
