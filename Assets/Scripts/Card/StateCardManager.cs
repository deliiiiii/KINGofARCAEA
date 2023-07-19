using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateCardManager : MonoBehaviour
{
    public static StateCardManager instance;
    public List<GameObject> stateCardsPrefab = new();//预制体列表（不重复）
    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject GetStateCardByIndex(int index)
    {
        for (int i = 0; i < stateCardsPrefab.Count; i++)
        {
            if (stateCardsPrefab[i].GetComponent<StateCard>().index_Card == index)
            {
                return stateCardsPrefab[i];
            }
        }
        Debug.LogError("未找到对应状态牌");
        return null;
    }
}
