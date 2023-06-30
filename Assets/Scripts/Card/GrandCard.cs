using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrandCard : MonoBehaviour
{
    public int index_Card;//卡牌在图鉴的序号，不会重复
    public int grossCount;//这一类牌的总数
    public int usedRound;//被使用的回合
    public List<int> index_attacker = new List<int>();//攻击者序号列表
    public List<int> index_offender = new List<int>();//受击者序号列表
}
