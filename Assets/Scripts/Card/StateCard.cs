using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateCard : MonoBehaviour
{
    [Tooltip("0:本次行动结束时 1:本轮结束时 2:一轮后的自己出牌时 ")]
    public int category;

    public int index_Card;
    public string name_Card;
    public string notice_State;
    public int id_attacker;
    public List<int> list_index_offender;
    //public Text text_name_Card;
    
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public string ChangeIndexToString()
    {
        string a = "";
        for(int i=0;i<list_index_offender.Count;i++)
        {
            a += Empty.list_playerName[list_index_offender[i]];
            if(i != list_index_offender.Count -1) a += ", ";
        }
        return a;
    }
}
