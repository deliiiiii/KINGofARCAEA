using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandCardManager : MonoBehaviour
{
    public static HandCardManager instance;
    public int count_HandCard;//手牌总数（算重复）
    public List<HandCard> handCards_info;//种类+数量列表（不重复）
    public List<GameObject> handCardsPrefab = new();//预制体列表（不重复）
    public static List<GameObject> handCardsStock = new();//卡组仓库（重复）

    public static List<int> list_index = new();

    public Text text_CardNum;
    public GameObject content_MyHandCard;
    void Awake()
    {
        instance = this;
        handCards_info = new List<HandCard>()
        {
            new HandCard(1001,2,true,1,false,false),//代打
            new HandCard(1002,2,false,0,true,false),//天下第一音游祭
            new HandCard(1003,16/*4*/,true,1,false,false),//指点江山
            new HandCard(1004,4,true,1,false,false),//观看手元
            new HandCard(1005,4,false,0,true,false),//神之左手
            new HandCard(1006,4,false,0,true,false),//鬼之右手
            new HandCard(1007,4,true,0,false,false),//音游窝
            new HandCard(1008,4,true,1,false,true),//音游王
            new HandCard(1009,4,true,2,true,false),//联机
            new HandCard(1010,4,true,1,false,false),//自来熟

            new HandCard(2001,6,false,0,false,false),//手癖
            new HandCard(2002,6,false,0,false,false),//降噪耳机
            new HandCard(2003,6,false,0,false,false),//网络延迟

            new HandCard(3001,4,false,0,false,false),//看铺
            new HandCard(3002,4,false,0,false,true),//私人订制手台
            new HandCard(3003,4,false,0,false,false),//底力提升
            new HandCard(3004,4,false,0,false,false),//从头开始
        };

    }
    void Start()
    {
        for (int i = 0; i < handCards_info.Count; i++)////手牌新增属性
        {
            handCardsPrefab[i].GetComponent<HandCard>().index_Card = handCards_info[i].index_Card;
            handCardsPrefab[i].GetComponent<HandCard>().grossCount = handCards_info[i].grossCount;
            handCardsPrefab[i].GetComponent<HandCard>().isAttackCard = handCards_info[i].isAttackCard;
            handCardsPrefab[i].GetComponent<HandCard>().count_offender = handCards_info[i].count_offender;
            handCardsPrefab[i].GetComponent<HandCard>().isExchangeCard = handCards_info[i].isExchangeCard;
            handCardsPrefab[i].GetComponent<HandCard>().isTimingCard = handCards_info[i].isTimingCard;
        }
    }

    public void RefreshHandCards(List<int> list)
    {
        list_index = list;
        count_HandCard = list.Count;
        handCardsStock.Clear();
        for (int i = 0; i < list.Count; i++)
        {
            int index = -1;
            for (int j = 0; j < handCards_info.Count; j++)
            {
                if (list[i] == handCards_info[j].index_Card)
                {
                    index = j;
                    break;
                }
            }
            handCardsStock.Add(handCardsPrefab[index]);
        }
        text_CardNum.text = count_HandCard.ToString();
    }
    public void RefillHandCards()
    {
        /*
        //for (int i = 1; i < list_MyHandCard.transform.childCount; i++)
        //{
        //    Destroy(list_MyHandCard.transform.GetChild(i).gameObject);
        //}
        //list_Scroll_MyHandCard.Clear();
        //for (int i = 0; i < GameManager.instance.count_Player; i++)
        //{
        //    list_Scroll_MyHandCard.Add(Instantiate(scroll_GameCard, list_MyHandCard.transform));

        //    list_Scroll_MyHandCard[i].SetActive(false);
        //}
        */
        count_HandCard = 0;
        for (int i = 0; i < handCards_info.Count; i++)//种类数
        {
            count_HandCard += handCards_info[i].grossCount;//一个种类重复数
        }
        handCardsStock.Clear();
        for (int i = 0; i < handCards_info.Count; i++)//种类数
        {
            for (int j = 0; j < handCards_info[i].grossCount; j++)//一个种类重复数
            {
                handCardsStock.Add(handCardsPrefab[i]);
            }
        }
        text_CardNum.text = count_HandCard.ToString();
        handCardsStock = RandomList(handCardsStock);


        list_index.Clear();
        for (int i = 0; i < handCardsStock.Count; i++)
        {
            list_index.Add(handCardsStock[i].GetComponent<HandCard>().index_Card);
        }
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
    public GameObject GetHandCardByIndex(int index)
    {
        for (int i = 0; i < handCardsPrefab.Count; i++)
        {
            if (handCardsPrefab[i].GetComponent<HandCard>().index_Card == index)
            {
                return handCardsPrefab[i];
            }
        }
        Debug.LogError("未找到对应手牌");
        return null;
    }
    public void Sync_DrawOneCard()
    {
        count_HandCard -= 1;
        text_CardNum.text = count_HandCard.ToString();
        //
        //
        //
        handCardsStock.RemoveAt(0);/////判断空
        list_index.RemoveAt(0);
    }
    public void DrawOneCard()
    {
        count_HandCard -= 1;
        text_CardNum.text = count_HandCard.ToString();
        Empty.instance.count_MyHandCard++;
        GameObject temp = Instantiate(handCardsStock[0].gameObject, content_MyHandCard.transform);
        temp.GetComponent<GrandCard>().used = false;
        temp.GetComponent<HandCard>().panel_New.SetActive(true);
        temp.SetActive(true);

        handCardsStock.RemoveAt(0);/////判断空
        list_index.RemoveAt(0);

        int count = content_MyHandCard.transform.childCount;
        for (int i = 3;i<content_MyHandCard.transform.childCount;i++)
        {
            if (content_MyHandCard.transform.GetChild(i).gameObject.GetComponent<HandCard>().index_Card > content_MyHandCard.transform.GetChild(count-1).gameObject.GetComponent<HandCard>().index_Card)
            {
                content_MyHandCard.transform.GetChild(i).SetSiblingIndex(count-1);
                content_MyHandCard.transform.GetChild(count - 2).SetSiblingIndex(i);
            }
        }
    }
    public void DrawOneCard_Specific(int index_Card)
    {
        Empty.instance.count_MyHandCard++;
        GameObject temp = Instantiate(GetHandCardByIndex(index_Card), content_MyHandCard.transform);
        temp.GetComponent<GrandCard>().used = false;
        temp.GetComponent<HandCard>().panel_New.SetActive(true);
        temp.SetActive(true);

        int count = content_MyHandCard.transform.childCount;
        for (int i = 3; i < content_MyHandCard.transform.childCount; i++)
        {
            if (content_MyHandCard.transform.GetChild(i).gameObject.GetComponent<HandCard>().index_Card > content_MyHandCard.transform.GetChild(count - 1).gameObject.GetComponent<HandCard>().index_Card)
            {
                content_MyHandCard.transform.GetChild(i).SetSiblingIndex(count - 1);
                content_MyHandCard.transform.GetChild(count - 2).SetSiblingIndex(i);
            }
        }
    }
    public List<int> GetIndexesOfMyHandCards()
    {
        List<int> list = new();
        for(int i=0;i<content_MyHandCard.transform.childCount;i++)
        {
            if(content_MyHandCard.transform.GetChild(i).gameObject.activeSelf)
            {
                list.Add(content_MyHandCard.transform.GetChild(i).gameObject.GetComponent<HandCard>().index_Card);
            }
        }
        return list;
    }
    public int GetCountOfMyHandCards()
    {
        int count = 0;
        for(int i=0;i< content_MyHandCard.transform.childCount;i++)
        {
            if(content_MyHandCard.transform.GetChild(i).gameObject.activeSelf)
            {
                count++;
            }
        }
        return count;
    }
}
