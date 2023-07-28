using Mirror;
//using System;
using System.Collections.Generic;
using Telepathy;
using UnityEngine;

public class Empty : NetworkBehaviour
{
    public static Empty instance;

    public static List<int> list_netId = new();//玩家Id列表
    public static List<string> list_playerName = new();//玩家姓名列表

    public static int index_Round;//局数，1局 = 2圈
    public static int index_Circle;//圈数
    public static bool isSwitchHolder;//换主持人
    public static int init_draw_num;//初始手牌数

    /// <summary>
    /// Empty <- UIPlayerManager
    /// </summary>
    public static int index_CurrentPlayer = 0;//从1开始数
    public static int index_CurrentHolder = 0;//从1开始数


    /// <summary>
    /// Empty <- Player
    /// </summary>
    //public int totalScore;
    //public List<int> roundScore = new();////待添加
    //public int totalMove;
    public List<int> turnMove = new();////待添加
    public int count_MyHandCard;
    //public int count_RoundUsedCard;
    //public int count_TotalUsedCard;
    //public GameObject last_selectedCard;
    public GameObject selectedCard;
    public GameObject scoreCard;
    //public List<GameObject> handCards = new();


    /// <summary>
    /// Card
    /// </summary>
    public int index_Shown;
    public List<int> list_Card_1002_ScoreCard = new();
    public List<int> list_Card_1002_netId_ScoreCard = new();
    public List<int> list_Card_1005or1006_ScoreCard = new();
    public List<int> list_Card_1005or1006_netId_ScoreCard = new();
    public int temp_Card_1002_id_attacker;
    public List<int> temp_Card_1002_list_index_offender = new();
    public List<int> temp_card_1005or1006_list_index_offender;
    public int temp_Card_1007_checkCount;
    public int temp_Card_1007_id_attacker;
    public int temp_1008_index_holder;

    public List<int> temp_list_index_offender = new();
    /// <summary>
    /// State 
    /// </summary>
    public List<StateCard> list_stateCards = new();


    public float delay = 0.2f;




    public void Awake()
    {
        Delay_set_instance();
    }


    public void Delay_set_instance()
    {
        //Debug.Log("Delay_set_instance()");
        if(isLocalPlayer && this.isServer && this.isClient && this.netId == 1)
        {
            instance = this;
            Debug.Log("AWAKE instance.netId = " + instance.netId);
            return;
        }
        if (isLocalPlayer && this.netId == 1)
        {
            return;
        }
        if(isLocalPlayer || (!isLocalPlayer&&this.netId == 1))
        {
            instance = this;
            Debug.Log("AWAKE instance.netId = " + instance.netId);
            return;
        }
        if (!instance)
        {
            //if (this.netId == 1 && !this.isServer)
            //{
            //    return;
            //}
            //Debug.Log("Delay_set_instance ");
            Invoke(nameof(Delay_set_instance), 0.8f);
            return;
        }
    }
    
    [Command(requiresAuthority = false)]
    public void CmdSetState(GameManager.Temp_STATE state)
    {
        if (GameManager.instance.state_ == state) return;
        Debug.Log(GameManager.instance.state_ = state);
        //GameManager.instance.state_ = state;
        instance.RpcSetState(state);
    }
    [Command(requiresAuthority = false)]
    public void CmdAddPlayer(int added_netId, string added_name)
    {
        //Debug.Log("CmdAddPlayer()");
        ServerAddPlayer(added_netId, added_name);
    }
    [Command(requiresAuthority = false)]
    public void CmdDelayShowStartGame()
    {
        instance.RpcShowStartGame();
    }
    [Command(requiresAuthority = false)] 
    public void CmdStartGame()
    {
        ServerStartGame();
    }
    [Command(requiresAuthority = false)]
    public void CmdDiscardScoreCard(int index_ScoreCard)
    {
        RpcDiscardScoreCard(index_ScoreCard);
    }
    [Command(requiresAuthority = false)]
    public void CmdDiscardHandCard(int onlineID, int index)
    {
        RpcDiscardHandCard(onlineID, index);
    }
    [Command(requiresAuthority = false)]
    public void CmdDrawHandCards(int onlineID, int times)
    {
        RpcDrawHandCards(onlineID, times);
    }
    [Command(requiresAuthority = false)]
    public void CmdDrawScoreCard(int onlineID, bool canDiscard)
    {
        RpcDrawScoreCard(onlineID,canDiscard);
    }
    [Command(requiresAuthority = false)]
    public void CmdNewTurn()
    {
        instance.ServerNewTurn();
    }
    [Command(requiresAuthority = false)]
    public void CmdGetHisAllHandCards(int id_attacker, List<int> list_index_offender)//1001 代打
    {
        RpcGetHisAllHandCards(id_attacker, list_index_offender);
    }
    [Command(requiresAuthority = false)]
    public void CmdGiveMyAllHandCards(int id_attacker, List<int> list_index_handCard)//1001 代打
    {
        RpcReceiveHisAllHandCards(id_attacker, list_index_handCard);
    }
    [Command(requiresAuthority = false)]
    public void CmdClearAllHandCards(int onlineID)//1001 代打
    {
        RpcClearAllHandCards(onlineID);
    }
    [Command(requiresAuthority = false)]
    public void CmdDrawHandCards_Specific(int onlineID, List<int> list_index_handCard)//1001 代打
    {
        RpcDrawHandCards_Specific(onlineID, list_index_handCard);
    }
    [Command(requiresAuthority = false)]
    public void CmdDrawScoreCard_Specific(int onlineID, int score)//1003 指点江山
    {
        //RpcDrawScoreCard_Specific(onlineID, score);
    }
    [Command(requiresAuthority = false)]
    public void CmdCard_1002_CollectAllScoreCards(int id_attacker, List<int> list_index_offender)
    {
        instance.list_Card_1002_ScoreCard.Clear();
        instance.list_Card_1002_netId_ScoreCard.Clear();
        for (int i = 0; i < list_index_offender.Count; i++)
        {
            //bool isOver = (i == list_netId.Count - 1);
            RpcCard_1002_CollectAllScoreCards(list_index_offender[i]);
        }
        instance.temp_Card_1002_id_attacker = id_attacker;
        instance.temp_Card_1002_list_index_offender = list_index_offender;
        instance.ServerDelay_RpcCard_1002_Show_Panel_SelectScoreCard();
    }
    [Command(requiresAuthority = false)]
    public void CmdCard_1002_AddScoreCard(int score, int index_offender)
    {
        instance.list_Card_1002_ScoreCard.Add(score);
        instance.list_Card_1002_netId_ScoreCard.Add(list_netId[index_offender]);
        Debug.Log("1002   #" + GetContent_int(instance.list_Card_1002_ScoreCard));
        
    }
    [Command(requiresAuthority = false)]
    public void CmdCard_1002_NextTurn(int id_turn, int index_last_Selected)
    {
        RpcCard_1002_NextTurn(id_turn, index_last_Selected);
    }
    [Command(requiresAuthority = false)]
    public void CmdCard_1003_GetHisScoreCard(int id_attacker, List<int> list_index_offender,int index_Card)
    {
        RpcCard_1003_GetHisScoreCard(id_attacker, list_index_offender,index_Card);
    }
    [Command(requiresAuthority = false)]
    public void CmdCard_1003_ReceiveScoreCard(int id_attacker, List<int> list_index_offender,int score)
    {
        RpcCard_1003_ReceiveScoreCard(id_attacker, list_index_offender,score);
    }
    [Command(requiresAuthority = false)]
    public void CmdCard_1004_GetHisScoreCard(List<int> list_index_offender)
    {
        RpcCard_1004_GetHisScoreCard(list_index_offender);
    }
    [Command(requiresAuthority = false)]
    public void CmdCard_1004_Show_Panel(int index_offender,int score)
    {
        RpcCard_1004_Show_Panel(index_offender,score);
    }
    [Command(requiresAuthority = false)]
    public void CmdCard_1005_GetLeftSuspectedScore(List<int> list_index_offender)
    {
        RpcCard_1005_GetLeftSuspectedScore(list_index_offender);
    }
    [Command(requiresAuthority = false)]
    public void CmdCard_1005or1006_CollectAllScoreCards(List<int> list_index_offender,int index_card)
    {
        instance.list_Card_1005or1006_ScoreCard.Clear();
        instance.list_Card_1005or1006_netId_ScoreCard.Clear();
        for (int i = 0; i < list_netId.Count; i++)
        {
            //bool isOver = (i == list_netId.Count - 1);
            RpcCard_1005or1006_CollectAllScoreCards(list_index_offender, list_netId[i]);
        }
        instance.temp_card_1005or1006_list_index_offender = list_index_offender;
        switch(index_card)
        {
            case 1005:
                instance.ServerDelay_RpcCard_1005_GetLeftScoreCard();
                break;
            case 1006:
                instance.ServerDelay_RpcCard_1006_GetRightScoreCard();
                break;
            default:break;
        }
    }
    [Command(requiresAuthority = false)]
    public void CmdCard_1005or1006_AddScoreCard(int onlineId, int score/*, bool isover*/)
    {
        instance.list_Card_1005or1006_netId_ScoreCard.Add(onlineId);
        instance.list_Card_1005or1006_ScoreCard.Add(score);
        Debug.Log("加入分数值" + score + "  id  = " + onlineId) ;
    }
    [Command(requiresAuthority = false)]
    public void CmdCard_1006_GetRightSuspectedScore(List<int> list_index_offender)
    {
        RpcCard_1006_GetRightSuspectedScore(list_index_offender);
    }
    [Command(requiresAuthority = false)]
    public void CmdCard_1007_CollectScoreCards(int id_attacker, List<int> list_index_offender)
    {
        instance.list_Card_1005or1006_ScoreCard.Clear();
        instance.list_Card_1005or1006_netId_ScoreCard.Clear();
        instance.temp_Card_1007_checkCount = list_index_offender.Count;
        //int temp_index_CurrentPlayer = index_CurrentPlayer;
        //int my_index = GetIndex_in_list_netId(id_attacker);
        //if(index_Circle < 2)
        //{
        //    Debug.Log("  һȦ my_index = " + my_index);
        //    for (int i = 0; i < list_netId.Count; i++)
        //    {
        //        if (i == my_index) continue;
        //        if(!list_index_offender.Contains(i)) continue;
        //        instance.temp_Card_1007_checkCount++;
        //        RpcCard_1007_CollectScoreCards(list_netId[i]);
        //    }
        //}
        //else if(index_Circle == 2)
        //{
        //    for(int i = my_index + 1; ;i++)
        //    {
        //        if (i == list_netId.Count) i = 0;

        //        if (i == index_CurrentHolder - 1) break;
        //        if (!list_index_offender.Contains(i)) continue;
        //        instance.temp_Card_1007_checkCount++;
        //        RpcCard_1007_CollectScoreCards(list_netId[i]);
        //    }
        //}
        for (int i = 0; i < list_index_offender.Count; i++)
        {
            RpcCard_1007_CollectScoreCards(list_netId[list_index_offender[i]]);
        }
        Debug.Log("#1007 checkcount " + instance.temp_Card_1007_checkCount);
        instance.temp_Card_1007_id_attacker = id_attacker;
        instance.ServerDelay_ClientCard_1007_ShowPanel();
    }
    [Command(requiresAuthority = false)]
    public void CmdCard_1008_AddBeforeRealize(int index_Card,int id_attacker,List<int> list_index_offender)
    {

        StateCard added = new();
        added.index_Card = index_Card;
        added.id_attacker = id_attacker;
        added.list_index_offender = list_index_offender;
        instance.list_stateCards.Add(added);

    }
    [Command(requiresAuthority = false)]
    public void CmdCard_1008_Realize(int index_holder)
    {
        instance.temp_1008_index_holder = index_holder;
        instance.ServerDelay_ClientCard_1008_Realize();
        //instance.RpcCard_1008_BeforeRealize(index_holder);
    }
    //[Command(requiresAuthority = false)]
    //public void CmdCard_1008_Realize()
    //{
    //    instance.RpcCard_1008_Realize();
    //}
    [Command(requiresAuthority = false)]
    public void CmdCard_1008_ShowPanel(int id_attacker, int id_offender, int score)
    {
        RpcCard_1008_ShowPanel(id_attacker, id_offender, score);
    }
    [Command(requiresAuthority = false)]
    public void CmdCheckCard_2001and2002(int index_Card,int id_attacker, List<int> list_index_offender)
    {
        instance.ServerSetState(GameManager.Temp_STATE.STATE_WHETHERDEFEND);
        instance.RpcCard_2001and2002_ShowPanel(index_Card, id_attacker, list_index_offender);

    }
    [Command(requiresAuthority = false)]
    public void CmdCard_2001and2002_NextTurn(int index_Card,int id_attacker, int id_turn, List<int> list_index_offender)
    {
        instance.RpcCard_2001and2002_NextTurn(index_Card, id_attacker,id_turn, list_index_offender);
    }
    [Command(requiresAuthority = false)]
    public void CmdCard_2001and2002_RefreshList(List<int> list_index_offender)
    {
        instance.RpcCard_2001and2002_RefreshList(list_index_offender);
    }
    [Command(requiresAuthority = false)]
    public void CmdAddStateCard(int id_attacker, List<int> list_index_offender,int index_Card)
    {
        RpcAddStateCard(id_attacker, list_index_offender, index_Card);
    }
    [Command(requiresAuthority = false)]
    public void CmdShowLastYieldCard(string name_attacker, int index_Card, List<int> list_index_offender)
    {
        RpcShowLastYieldCard(name_attacker, index_Card, list_index_offender);
    }
    [Command(requiresAuthority = false)]
    public void CmdRefreshLastYieldCard(List<int> list_index_offender)
    {
        instance.RpcRefreshLastYieldCard(list_index_offender);
    }
    [Command(requiresAuthority = false)]
    public void CmdCloseLastYieldCard()
    {
        instance.RpcCloseLastYieldCard();
    }
    //[Command(requiresAuthority = false)]
    //public void CmdYieldCard()
    //{
    //    RpcYieldCard();
    //}
    //[Command(requiresAuthority = false)]
    //public void CmdThrowCard()
    //{
    //    RpcThrowCard();
    //}
    [Server]
    public void ServerAddPlayer(int added_netId, string added_name)
    {
        if (CheckRepeatedNetId(added_netId))
        {
            Debug.Log("id重复！");
            return;
        }
        //Debug.Log("ServerAddPlayer  netId = " + netId + " || instance.netId = " + instance.netId);
        if (added_netId == 1) return;
        list_netId.Add(added_netId);

        list_playerName.Add(added_name);



        RpcClearPlayer();
        for (int i = 0; i < list_netId.Count; i++)
        {
            RpcAddPlayer(list_netId[i], list_playerName[i]);
        }
        UIPlayerManager.instance.RefreshPlayer(list_netId, list_playerName);
        RpcRefreshPlayer();
        //Debug.Log("[Server] list_netId = " + GetContent_int(list_netId));
        //Debug.Log("[Server] list_playerName = " + GetContent_string(list_playerName));
    }
    [Server]
    public void ServerRomovePlayer(int removed_netId)
    {
        //Debug.Log("ServerRomovePlayer  netId = " + netId + " || instance.netId = " + instance.netId);
        if (removed_netId == 1) return;
        int removed_index = -1;
        for (int i = 0; i < list_netId.Count; i++)
        {
            if (list_netId[i] == removed_netId)
            {
                removed_index = i;
                break;
            }
        }
        if (removed_index == -1)
        {
            Debug.LogError("未找到ID来删除!!");
            return;
        }



        Debug.Log("[Server] 离开 " + removed_netId + " " + list_playerName[removed_index]);
        list_netId.RemoveAt(removed_index);
        list_playerName.RemoveAt(removed_index);
        //Debug.Log("[Server] list_netId = " + GetContent_int(list_netId));
        //Debug.Log("[Server] list_playerName = " + GetContent_string(list_playerName));
        RpcClearPlayer();
        for (int i = 0; i < list_netId.Count; i++)
        {
            RpcAddPlayer(list_netId[i], list_playerName[i]);
        }

        UIPlayerManager.instance.RefreshPlayer(list_netId, list_playerName);
        RpcRefreshPlayer();
    }
    [Server]
    public void ServerStartGame()
    {
        
        index_Round = 0;
        init_draw_num = 4;
        instance.list_stateCards.Clear();
        isSwitchHolder = false;
        index_CurrentHolder = index_CurrentPlayer = 0;

        ScoreCardManager.instance.RefillScoreCards();
        HandCardManager.instance.RefillHandCards();
        instance.RpcInitialize(ScoreCardManager.list_index, HandCardManager.list_index);
        instance.ServerSetState(GameManager.Temp_STATE.STATE_TURNING_ROUND);
        instance.ServerDelay_NewRound();
    }
    [Server]
    public void ServerDelay_NewRound()
    {
        if (GameManager.instance.state_ !=  GameManager.Temp_STATE.STATE_TURNING_ROUND)
        {
            //Debug.Log("[Server]Delay_NewRound");
            Invoke(nameof(ServerDelay_NewRound), 0.75f);
            return;
        }
        ServerNewRound();
    }
    [Server]
    public void ServerNewRound()
    {
        instance.RpcClearUsedCards();
        index_Round++;
        index_Circle = 1;
        isSwitchHolder = true;
        UIManager.instance.text_CircleNum.text = index_Circle.ToString();
        
        if (index_Round == 1)//第一局之前
        {
            index_CurrentHolder = 1;
            RpcSetHolder(index_CurrentHolder - 1, true);

            for (int i = 0; i < list_netId.Count; i++)
            {
                RpcDrawScoreCard(list_netId[i],true);////判空
                RpcDrawHandCards(list_netId[i], init_draw_num);////判空
            }
            instance.ServerSetState(GameManager.Temp_STATE.STATE_TURNING_TURN);
            instance.ServerNewTurn();
            isSwitchHolder = false;
        }
        else
        {
            instance.list_Card_1002_ScoreCard.Clear();
            instance.list_Card_1002_netId_ScoreCard.Clear();
            for (int i = 0; i < list_netId.Count; i++)
            {
                instance.RpcCard_1002_CollectAllScoreCards(i);
            }
            //Empty.instance.RpcSetState(GameManager.Temp_STATE.STATE_BUSYCONNECTING);
            instance.ServerDelay_RpcStartGainScore();
            instance.ServerDelay_NextRound();
        }
    }
    [Server]
    public void ServerDelay_NextRound()
    {
        if(GameManager.instance.state_ != GameManager.Temp_STATE.STATE_TURNING_ROUND)
        {
            //Debug.Log("[Server]Delay_NextRound");
            Invoke(nameof(ServerDelay_NextRound), 0.75f);
            return;
        }
        Debug.Log("[Server]Delay_NextRound");
        instance.RpcClearAllSuspectedCardOnNewRound();
        instance.RpcClearStates(2);/////////位置好不好？ 2:新一局
        RpcSetHolder(index_CurrentHolder - 1, false);
        index_CurrentHolder++;
        if (index_CurrentHolder > list_netId.Count)
        {
            instance.ServerSummaryGame();
            return;
        }
        RpcSetHolder(index_CurrentHolder - 1, true);
        for (int i = 0; i < list_netId.Count; i++)
        {
            RpcDrawScoreCard(list_netId[i], true);////判空
        }
        instance.ServerNewTurn();
        isSwitchHolder = false;
    }
    [Server]
    public void ServerNewTurn()
    {
        if (isSwitchHolder)
        {
            if (index_Round == 1)//第一局
            {
                index_CurrentPlayer = 1;
            }
            else if (index_Round != 1)//非第一局
            {
                RpcPassTurn(index_CurrentPlayer - 1);
                index_CurrentPlayer = index_CurrentHolder;
            }
        }
        else
        {
            RpcPassTurn(index_CurrentPlayer - 1);
            index_CurrentPlayer++;
            if (index_CurrentPlayer > list_netId.Count)
            {
                index_CurrentPlayer = 1;
            }
            if (index_CurrentPlayer == index_CurrentHolder)
            {
                index_Circle++;
                instance.ServerSetState(GameManager.Temp_STATE.STATE_JUDGE_CARDS);
                UIPlayerManager.instance.Card_1008_Collect(index_CurrentPlayer - 1);//从当前主持人开始获取1008状态卡
                
                instance.ServerDelay_NewCircle();
                return;
            }
        }
        RpcSetIndex(index_Circle, index_CurrentPlayer, index_CurrentHolder, index_Round);
        RpcMyTurn(index_CurrentPlayer - 1);
        instance.RpcClearStates(0,index_CurrentPlayer - 1);/////////位置好不好？ 0:
    }
    [Server]
    public void ServerDelay_NewCircle()
    {
        if(GameManager.instance.state_ != GameManager.Temp_STATE.STATE_TURNING_TURN)
        {
            Invoke(nameof(ServerDelay_NewCircle), 0.3f);
            return;
        }
        instance.RpcClearStates(1);/////////位置好不好？ 1:新一轮
        if (index_Circle == 3)
        {
            ServerNewRound();
            return;
        }
        RpcSetIndex(index_Circle, index_CurrentPlayer, index_CurrentHolder, index_Round);
        RpcMyTurn(index_CurrentPlayer - 1);
        instance.RpcClearStates(0, index_CurrentPlayer - 1);/////////位置好不好？ 0:
    }
    [Server]
    public void ServerDelay_RpcCard_1002_Show_Panel_SelectScoreCard()
    {

        if (instance.list_Card_1002_ScoreCard.Count != instance.temp_Card_1002_list_index_offender.Count)
        {
            //Debug.Log("[Server]Delay_RpcCard_1002_Show_Panel_SelectScoreCard");
            Invoke(nameof(ServerDelay_RpcCard_1002_Show_Panel_SelectScoreCard), delay);
            return;
        }
        Debug.Log("#1002 分数牌列表 = " + GetContent_int(instance.list_Card_1002_ScoreCard));
        List<int> randomed_list_scoreCard = RandomList(instance.list_Card_1002_ScoreCard);
        instance.RpcCard_1002_Show_Panel_SelectScoreCard(instance.temp_Card_1002_id_attacker, instance.temp_Card_1002_list_index_offender, randomed_list_scoreCard);
    }
    [Server]
    public void ServerDelay_RpcCard_1005_GetLeftScoreCard()
    {
        if (instance.list_Card_1005or1006_ScoreCard.Count != list_netId.Count)
        {
            //Debug.Log("[Server]Delay_RpcCard_1005_GetLeftScoreCard");
            Invoke(nameof(ServerDelay_RpcCard_1005_GetLeftScoreCard), delay);
            return;
        }
        
        //Debug.Log("#1005 分数牌列表 = " +  GetContent_int(instance.list_Card_1005or1006_ScoreCard));
        //Debug.Log("分数牌主人的id列表 = " +  GetContent_int(instance.list_Card_1005or1006_netId_ScoreCard));
        //Debug.Log("受击者的id列表 = " +  GetContent_int(instance.temp_card_1005or1006_list_index_offender));
        instance.RpcCard_1005_GetLeftScoreCard(instance.temp_card_1005or1006_list_index_offender,instance.list_Card_1005or1006_ScoreCard,instance.list_Card_1005or1006_netId_ScoreCard);
    }
    [Server]
    public void ServerDelay_RpcCard_1006_GetRightScoreCard()
    {
        if (instance.list_Card_1005or1006_ScoreCard.Count != list_netId.Count)
        {
            //Debug.Log("[Server]Delay_RpcCard_1006_GetRightScoreCard");
            Invoke(nameof(ServerDelay_RpcCard_1006_GetRightScoreCard), delay);
            return;
        }
        
        Debug.Log("分数牌列表 = " + GetContent_int(list_Card_1005or1006_ScoreCard));
        instance.RpcCard_1006_GetRightScoreCard(instance.temp_card_1005or1006_list_index_offender, instance.list_Card_1005or1006_ScoreCard, instance.list_Card_1005or1006_netId_ScoreCard);
    }
    [Server]
    public void ServerDelay_ClientCard_1007_ShowPanel()
    {
        if (list_Card_1005or1006_ScoreCard.Count != instance.temp_Card_1007_checkCount)
        {
            //Debug.Log("Delay 1007");
            Invoke(nameof(ServerDelay_ClientCard_1007_ShowPanel), delay);
            return;
        }
        instance.RpcCard_1007_ShowPanel(instance.temp_Card_1007_id_attacker, instance.list_Card_1005or1006_ScoreCard, instance.list_Card_1005or1006_netId_ScoreCard);
    }
    [Server]
    public void ServerDelay_ClientCard_1008_Realize()
    {
        //Debug.Log("#1008 index_holder = " + instance.temp_1008_index_holder);
        
        bool found_id = false;
        //Debug.Log("#1008 状态牌数量 =" + instance.list_stateCards.Count);
        for (int i = 0; i < instance.list_stateCards.Count; i++)
        {
            if (instance.list_stateCards[i].id_attacker == Empty.list_netId[instance.temp_1008_index_holder] && (instance.list_stateCards[i].index_Card == 1008))
            {
                found_id = true;
                instance.RpcCard_1003_GetHisScoreCard(instance.list_stateCards[i].id_attacker, instance.list_stateCards[i].list_index_offender,1008);
                instance.list_stateCards.RemoveAt(i);
                //Debug.Log("#1008 移除第 " + i + "张状态牌");
            }
        }
        if (!found_id)
        {
            instance.temp_1008_index_holder++;
            if (instance.temp_1008_index_holder == list_netId.Count)
            {
                instance.temp_1008_index_holder = 0;
            }
            if (instance.temp_1008_index_holder == index_CurrentHolder - 1)
            {
                //Debug.Log("#1008 结束");
                instance.RpcCard_1008_ClosePanel();
                instance.ServerSetState(GameManager.Temp_STATE.STATE_TURNING_TURN);
                return;
            }
        }
        Invoke(nameof(ServerDelay_ClientCard_1008_Realize), 5f);
    }
    [Server]
    public void ServerDelay_RpcStartGainScore()
    {
        if (instance.list_Card_1002_ScoreCard.Count != list_netId.Count)
        {
            Debug.Log("[Server]Delay_RpcCard_1002_Show_Panel_SelectScoreCard");
            Invoke(nameof(ServerDelay_RpcStartGainScore), delay);
            return;
        }
        Debug.Log("分数列表 = " + GetContent_int(instance.list_Card_1002_ScoreCard) + " 分数id列表 = " + GetContent_int(instance.list_Card_1002_netId_ScoreCard));
        instance.RpcStartGainScore(index_CurrentHolder - 1,instance.list_Card_1002_ScoreCard,instance.list_Card_1002_netId_ScoreCard);
    }
    [Server]
    public void ServerSummaryGame()
    {
        instance.ServerSetState(GameManager.Temp_STATE.STATE_GAME_SUMMARY);
        instance.RpcShowStartGame();
    }
    [Server]
    public void ServerSetState(GameManager.Temp_STATE state)
    {
        GameManager.instance.state_ = state;
        instance.RpcSetState(state);
    }
    [ClientRpc]
    public void RpcShowStartGame()
    {
        UIManager.instance.Delay_ShowStartGame();
    }
    [ClientRpc]
    public void RpcClearPlayer()
    {
        list_netId.Clear();
        list_playerName.Clear();
    }
    [ClientRpc]
    public void RpcAddPlayer(int added_netId, string added_name)
    {
        //Debug.Log("[Client] ServerAddPlayer()");
        list_netId.Add(added_netId);
        list_playerName.Add(added_name);
        //Debug.Log("[Client] list_netId = " + GetContent_int(list_netId));
    }
    [ClientRpc]
    public void RpcRefreshPlayer()
    {
        UIPlayerManager.instance.RefreshPlayer(list_netId, list_playerName);
    }
    [ClientRpc]
    public void RpcInitialize(List<int> list_index_ScoreCards,List<int> list_index_HandCards)
    {
        UIManager.instance.button_Start_Game.gameObject.SetActive(false);
        instance.count_MyHandCard = 0;
        //instance.roundScore.Clear();
        instance.turnMove.Clear();
        //instance.totalMove = 0;
        //for (int j = 0; j < list_netId.Count; j++)
        //{
        //    instance.roundScore.Add(0);
        //}
        ScoreCardManager.instance.RefreshScoreCards(list_index_ScoreCards);
        HandCardManager.instance.RefreshHandCards(list_index_HandCards);
        UIManager.instance.ClearHandCards_and_ScoreCard();
        UIPlayerManager.instance.RefreshPlayer(list_netId, list_playerName);
        //GameCardManager.list_instance.RefillHandCards();
    }
    [ClientRpc]
    public void RpcDrawScoreCard(int onlineID,bool canDiscard)
    {
        if ((int)instance.netId !=onlineID)
        {
            ScoreCardManager.instance.Sync_DrawOneCard();
        }
        else
        {
            ScoreCardManager.instance.DrawOneCard(canDiscard);
            
        }
    }
    [ClientRpc]
    public void RpcDrawHandCards(int onlineID,int times)
    {
        int index = GetIndex_in_list_netId(onlineID);
        //for (int i = 0; i < list_netId.Count; i++)
        //{
        //    if (list_netId[i] == onlineID)
        //    {
        //        onlineID = i;
        //        break;
        //    }
        //}
        for (int j = 0; j < times;j++)
        {
            if ((int)instance.netId != onlineID)
            {
                HandCardManager.instance.Sync_DrawOneCard();
            }
            else
            {
                HandCardManager.instance.DrawOneCard();
                
            }
            
            UIPlayerManager.list_player[index].GetComponent<Player>().Text_CardNum.text = (int.Parse(UIPlayerManager.list_player[index].GetComponent<Player>().Text_CardNum.text)+1).ToString();
        }
        if ((int)instance.netId == onlineID)
        {
            UIManager.instance.UIAnimDrawHandCard(times,false);
        }
    }
    [ClientRpc]
    public void RpcDrawHandCards_Specific(int onlineID, List<int> list_index_handCard)
    {
        int index = GetIndex_in_list_netId(onlineID);
        //for (int i = 0; i < list_netId.Count; i++)
        //{
        //    if (list_netId[i] == onlineID)
        //    {
        //        onlineID = i;
        //        break;
        //    }
        //}
        for (int j = 0; j < list_index_handCard.Count; j++)
        {
            if ((int)instance.netId != onlineID)
            {
                //HandCardManager.instance.Sync_DrawOneCard_Specific(); //并没有实现
            }
            else
            {
                HandCardManager.instance.DrawOneCard_Specific(list_index_handCard[j]);

            }
            UIPlayerManager.list_player[index].GetComponent<Player>().Text_CardNum.text = (int.Parse(UIPlayerManager.list_player[index].GetComponent<Player>().Text_CardNum.text) + 1).ToString();
        }
        if ((int)instance.netId == onlineID)
        {
            UIManager.instance.UIAnimDrawHandCard(list_index_handCard.Count, true);
        }
        Empty.instance.CmdSetState(GameManager.Temp_STATE.STATE_ONENDREALIZING_CARDS);
    }
    [ClientRpc]
    public void RpcDiscardScoreCard(int onlineID)
    {
        UIManager.instance.DiscardScorecard(onlineID,new Vector2(Random.Range(-500f, 500f), Random.Range(-200f, 300f)), Quaternion.Euler(0, 0, Random.Range(0f, 360f)));
    }
    [ClientRpc]
    public void RpcDiscardHandCard(int onlineID,int index_Card)
    {
        int index_player = GetIndex_in_list_netId(onlineID);
        UIPlayerManager.list_player[index_player].GetComponent<Player>().Text_CardNum.text = (int.Parse(UIPlayerManager.list_player[index_player].GetComponent<Player>().Text_CardNum.text) - 1).ToString();
        UIManager.instance.DiscardHandcard(index_Card, new Vector2(Random.Range(-500f, 500f), Random.Range(-200f, 300f)), Quaternion.Euler(0, 0, Random.Range(0f, 360f)));
    }
    [ClientRpc]
    public void RpcSetHolder(int index, bool state)
    {
        UIPlayerManager.list_player[index].GetComponent<Player>().image_Holder.SetActive(state);
    }
    [ClientRpc]
    public void RpcPassTurn(int index)
    {
        UIPlayerManager.instance.PassTurn(index);
    }
    [ClientRpc]
    public void RpcMyTurn(int index)
    {
        UIManager.instance.panel_HandCardDetail.SetActive(false);
        //instance.CmdSetState(GameManager.Temp_STATE.STATE_YIELD_CARDS);
        if (list_netId[index] == (int)instance.netId)
        {
            UIPlayerManager.instance.MyTurn(index);
        }
        else
        {
            UIPlayerManager.instance.Sync_MyTurn(index);
        }
    }
    [ClientRpc]
    public void RpcSetIndex(int num_circle,int index_currentPlayer,int index_currentHolder,int index_round)
    {
        UIManager.instance.text_CircleNum.text = num_circle.ToString();
        index_CurrentPlayer = index_currentPlayer;
        index_CurrentHolder = index_currentHolder;
        index_Round = index_round;
        index_Circle = num_circle;
    }
    [ClientRpc]
    public void RpcGetHisAllHandCards(int id_attacker, List<int> list_index_offender)
    {
        if (list_index_offender.Contains(GetIndex_in_list_netId((int)instance.netId)))
        {
            int count_myHandCards = HandCardManager.instance.GetCountOfMyHandCards();
            Debug.Log("#1001 我的手牌数 = " + instance.count_MyHandCard);
            instance.ClientGiveMyAllHandCards(id_attacker,HandCardManager.instance.GetIndexesOfMyHandCards());
            

            instance.ClientClearAllHandCards((int)instance.netId);
            instance.ClientDrawHandCards((int)instance.netId, count_myHandCards);
        }
    }
    [ClientRpc]
    public void RpcReceiveHisAllHandCards(int id_attacker, List<int> list_index_handCard)
    {
        //GameManager.state_ = GameManager.Temp_STATE.STATE_BUSYCONNECTING;
        if ((int)instance.netId == id_attacker)
        {
            instance.ClientDrawHandCards_Specific((int)instance.netId, list_index_handCard);
            //instance.ClientOnEndRealizeHandCard();
        }
    }
    [ClientRpc]
    public void RpcClearAllHandCards(int onlineID)
    {
        int index_player = GetIndex_in_list_netId(onlineID);
        UIPlayerManager.list_player[index_player].GetComponent<Player>().Text_CardNum.text = "0";
    }
    [ClientRpc]
    public void RpcCard_1002_CollectAllScoreCards(int index_offender/*,bool isover*/)
    {
        if ((int)instance.netId != list_netId[index_offender]) return;
        instance.scoreCard.SetActive(false);
        instance.CmdCard_1002_AddScoreCard(instance.scoreCard.GetComponent<ScoreCard>().score, index_offender);
        Destroy(instance.scoreCard);
    }
    [ClientRpc]
    public void RpcCard_1002_Show_Panel_SelectScoreCard(int id_attacker,List<int> list_index_offender,List<int> list_scoreCard)
    {
        UIManager.instance.UICard_1002_ShowPanel(id_attacker, list_index_offender, list_scoreCard);
        //if ((int)instance.netId != id_attacker) return;
    }
    [ClientRpc]
    public void RpcCard_1002_NextTurn(int last_id_turn, int index_last_Selected)
    {
        int index_id_turn = GetIndex_in_list_netId(last_id_turn);
        index_id_turn++;
        if(index_id_turn == list_netId.Count)
        {
            index_id_turn = 0;
        }
        UIManager.instance.UICard_1002_NextTurn(last_id_turn, list_netId[index_id_turn], index_last_Selected);
    }
    [ClientRpc]
    public void RpcCard_1003_GetHisScoreCard(int id_attacker,List<int> list_index_offender,int index_Card)
    {
        if (list_index_offender.Count == 0)
        {
            return;
        }
        if (list_index_offender[0] == GetIndex_in_list_netId((int)instance.netId))
        {
            if (index_Card == 1008)
            {
                Debug.Log("#1008 JUDGE");
                instance.CmdCard_1003_ReceiveScoreCard(id_attacker, list_index_offender, instance.scoreCard.GetComponent<ScoreCard>().score);
                return; 
            }
            if (list_index_offender[0] == GetIndex_in_list_netId(id_attacker))//选择自己相当于remake
            {
                instance.ClientDrawScoreCard((int)instance.netId, true);
                Empty.instance.CmdSetState(GameManager.Temp_STATE.STATE_ONENDREALIZING_CARDS);
            }
            else
            {
                instance.CmdCard_1003_ReceiveScoreCard(id_attacker, list_index_offender, instance.scoreCard.GetComponent<ScoreCard>().score);
                instance.ClientDrawScoreCard((int)instance.netId, false);
            }
            
        }
        
    }
    [ClientRpc]
    public void RpcCard_1003_ReceiveScoreCard(int id_attacker, List<int> list_index_offender, int score)
    {
        if((int)instance.netId == id_attacker)
        {
            ScoreCardManager.instance.DrawOneCard_Specific(score);
            //instance.ClientDrawScoreCard_Specific((int)instance.netId,score);
            if(GameManager.instance.state_ == GameManager.Temp_STATE.STATE_JUDGE_CARDS) 
            {
                instance.CmdCard_1008_ShowPanel(id_attacker, list_netId[list_index_offender[0]], score);
                return; 
            }
            Empty.instance.CmdSetState(GameManager.Temp_STATE.STATE_ONENDREALIZING_CARDS);
        }
    }
    [ClientRpc]
    public void RpcCard_1004_GetHisScoreCard(List<int> list_index_offender)
    {
        if (list_index_offender[0] == GetIndex_in_list_netId((int)instance.netId))
        {
            instance.CmdCard_1004_Show_Panel(list_index_offender[0],instance.scoreCard.GetComponent<ScoreCard>().score);
        }
    }
    [ClientRpc]
    public void RpcCard_1004_Show_Panel(int index_offender, int score)
    {
        UIManager.instance.UICard_1004_ShowPanel(index_offender, score);
    }
    [ClientRpc]
    public void RpcCard_1005_GetLeftSuspectedScore(List<int> list_index_offender)
    {
        UIPlayerManager.instance.Card_1005_GetLeftSuspectedScore(list_index_offender);
    }
    [ClientRpc]
    public void RpcCard_1005or1006_CollectAllScoreCards(List<int> list_index_offender, int index_offender)
    {
        if ((int)instance.netId != index_offender) return;
        instance.CmdCard_1005or1006_AddScoreCard((int)instance.netId,instance.scoreCard.GetComponent<ScoreCard>().score);
        //if (!list_index_offender.Contains((int)instance.netId)) return;
        //instance.scoreCard.SetActive(false);
        //Destroy(instance.scoreCard);
    }
    [ClientRpc]
    public void RpcCard_1005_GetLeftScoreCard(List<int> list_index_offender,List<int> list_Score,List<int>list_onlineId_of_ScoreCard)
    {
        if (list_index_offender.Count == 0 || list_index_offender.Count == 1)
        {
            UIManager.instance.UINotice_Card_1005_LackPeople();
            return;
        }
        if (!list_index_offender.Contains(GetIndex_in_list_netId((int)instance.netId)))
        {
            Debug.Log("防出去了");
            return;
        }

        int my_index = GetIndex_in_list_netId((int)instance.netId);
        int last_index = my_index - 1;
        if(last_index < 0 )
        {
            last_index = list_netId.Count - 1;
        }
        while (!list_index_offender.Contains(last_index))
        {
            last_index--;
            if (last_index < 0)
            {
                last_index = list_netId.Count - 1;
            }
            if (last_index < -10)
            {
                Debug.Log("死循环了");
                return;
            }
        }
        
        int index_trueId = -1;
        for(int i=0;i<list_onlineId_of_ScoreCard.Count;i++)
        {
            if (list_onlineId_of_ScoreCard[i] == list_netId[last_index])
            {
                index_trueId = i;
                break;
            }
        }
        Debug.Log("my_index" + my_index + " last_index = " + last_index + " 分数列表" + GetContent_int(list_Score) + " 真正分数位置 " + index_trueId);
        ScoreCardManager.instance.Card_1002_ReGetScoreCard(ScoreCardManager.instance.GetScoreCardByScore(list_Score[index_trueId]));
        Empty.instance.CmdSetState(GameManager.Temp_STATE.STATE_ONENDREALIZING_CARDS);
    }
    [ClientRpc]
    public void RpcCard_1006_GetRightScoreCard(List<int> list_index_offender, List<int> list_Score, List<int> list_onlineId_of_ScoreCard)
    {
        if (list_index_offender.Count == 0 || list_index_offender.Count == 1)
        {
            UIManager.instance.UINotice_Card_1005_LackPeople();
            return;
        }
        if (!list_index_offender.Contains(GetIndex_in_list_netId((int)instance.netId)))
        {
            Debug.Log("防出去了");
            return;
        }

        int my_index = GetIndex_in_list_netId((int)instance.netId);
        int next_index = my_index + 1;
        if (next_index >= list_netId.Count)
        {
            next_index = 0;
        }
        while (!list_index_offender.Contains(next_index))
        {
            next_index++;
            if (next_index >= list_netId.Count)
            {
                next_index = 0;
            }
            if (next_index > 990)
            {
                Debug.Log("死循环了");
                return;
            }
        }

        int index_trueId = -1;
        for (int i = 0; i < list_onlineId_of_ScoreCard.Count; i++)
        {
            if (list_onlineId_of_ScoreCard[i] == list_netId[next_index])
            {
                index_trueId = i;
                break;
            }
        }
        //Debug.Log("my_index" + my_index + " next_index = " + next_index + " 分数列表" + GetContent_int(list_Score) + " 真正分数位置 " + index_trueId);
        ScoreCardManager.instance.Card_1002_ReGetScoreCard(ScoreCardManager.instance.GetScoreCardByScore(list_Score[index_trueId]));
        Empty.instance.CmdSetState(GameManager.Temp_STATE.STATE_ONENDREALIZING_CARDS);
    }
    [ClientRpc]
    public void RpcCard_1006_GetRightSuspectedScore(List<int> list_index_offender)
    {
        UIPlayerManager.instance.Card_1006_GetRightSuspectedScore(list_index_offender);
    }
    [ClientRpc]
    public void RpcCard_1007_CollectScoreCards(int index_offender)
    {
        if ((int)instance.netId != index_offender) return;
        instance.CmdCard_1005or1006_AddScoreCard((int)instance.netId, instance.scoreCard.GetComponent<ScoreCard>().score);
    }
    [ClientRpc]
    public void RpcCard_1007_ShowPanel(int id_attacker , List<int> list_score, List<int> list_id_of_score)
    {
        if ((int)instance.netId != id_attacker) return;
        UIManager.instance.UICard_1007_ShowPanel(list_score, list_id_of_score);
    }
    [ClientRpc]
    public void RpcCard_1008_ShowPanel(int id_attacker, int id_offender, int score)
    {
        UIManager.instance.UICard_1008_ShowPanel(id_attacker, id_offender, score);
    }
    [ClientRpc]
    public void RpcCard_1008_ClosePanel()
    {
        UIManager.instance.UICard_1008_ClosePanel();
    }
    [ClientRpc]
    public void RpcCard_2001and2002_ShowPanel(int index_Card, int id_turn, List<int> list_index_offender)
    {
        UIManager.instance.UICard_200X_ShowPanel(index_Card, id_turn,id_turn, list_index_offender);
    }
    [ClientRpc]
    public void RpcCard_2001and2002_NextTurn(int index_Card, int id_attacker,int id_turn, List<int> list_index_offender)
    {
        int index_id_turn = GetIndex_in_list_netId(id_turn);
        index_id_turn++;
        if (index_id_turn == list_netId.Count)
        {
            index_id_turn = 0;
        }
        UIManager.instance.UICard_200X_NextTurn(index_Card, id_attacker, list_netId[index_id_turn], list_index_offender);
    }
    [ClientRpc]
    public void RpcCard_2001and2002_RefreshList(List<int> list_index_offender)
    {
        instance.temp_list_index_offender = list_index_offender;
        instance.CmdSetState(GameManager.Temp_STATE.STATE_REALIZING_CARDS);
    }
    [ClientRpc]
    public void RpcAddStateCard(int id_attacker, List<int> list_index_offender, int index_Card)
    {
        UIPlayerManager.instance.AddStateCard(id_attacker, list_index_offender, index_Card);
    }
    [ClientRpc]
    public void RpcStartGainScore(int index_Shown,List<int> list_score, List<int> list_id_of_score)
    {
        instance.index_Shown = index_Shown;
        instance.list_Card_1002_ScoreCard = list_score;
        instance.list_Card_1002_netId_ScoreCard = list_id_of_score;
        ClientGainScore();
    }
    
    [ClientRpc]
    public void RpcSetState(GameManager.Temp_STATE state)
    {
        if (GameManager.instance.state_ == state) return;
        //GameManager.instance.state_ = state;
        Debug.Log(GameManager.instance.state_ = state);
    }
    [ClientRpc]
    public void RpcClearStates(int circumstance)
    {
        UIPlayerManager.instance.ClearStates(circumstance);
    }
    [ClientRpc]
    public void RpcClearStates(int circumstance,int index_id)
    {
        UIPlayerManager.instance.ClearStates(circumstance,index_id);
    }
    [ClientRpc]
    public void RpcClearAllSuspectedCardOnNewRound()
    {
        List<int> temp_list_all_index = new();
        for(int i=0;i<list_netId.Count;i++)
        {
            temp_list_all_index.Add(i);
        }
        UIPlayerManager.instance.Card_1002_ClearSuspectedCard(temp_list_all_index);
    }
    [ClientRpc]
    public void RpcClearUsedCards()
    {
        UIManager.instance.ClearChild(UIManager.instance.panel_DiscardedCards.transform);
    }
    [ClientRpc]
    public void RpcShowLastYieldCard(string name_attacker, int index_Card, List<int> list_index_offender)
    {
        UIManager.instance.UIShowLastYieldCard(name_attacker, index_Card, list_index_offender);
    }
    [ClientRpc]
    public void RpcRefreshLastYieldCard(List<int> list_index_offender)
    {
        UIManager.instance.UIRefreshLastYieldCard(list_index_offender);
    }
    [ClientRpc]
    public void RpcCloseLastYieldCard()
    {
        UIManager.instance.UICloseLastYieldCard();
    }
    [Client]
    public void ClientAddPlayer(int added_netId,string added_name)
    {
        Debug.Log("[Client] 加入 :" + added_netId + " " + added_name);
        instance.CmdAddPlayer(added_netId, added_name);
    }
    [Client]
    public void ClientStartGame()
    {
        instance.CmdStartGame();
    }
    [Client]
    public void ClientDiscardScoreCard(int index_ScoreCard)
    {
        instance.CmdDiscardScoreCard(index_ScoreCard);
    }
    [Client]
    public void ClientDiscardHandCard(int onlineID,int index)
    {
        instance.CmdDiscardHandCard(onlineID,index);
    }
    [Client]
    public void ClientDrawHandCards(int onlineID, int times)
    {
        instance.CmdDrawHandCards(onlineID, times);
    }
    [Client]
    public void ClientDrawHandCards_Specific(int onlineID, List<int> list_index_handCard)
    {
        instance.CmdDrawHandCards_Specific(onlineID, list_index_handCard);
    }
    [Client]
    public void ClientDrawScoreCard(int onlineID, bool canDiscard)
    {
        instance.CmdDrawScoreCard(onlineID,canDiscard);
    }
    [Client]
    public void ClientDrawScoreCard_Specific(int onlineID, int score)
    {
        //instance.CmdDrawScoreCard_Specific(onlineID,score);
    }
    [Client]
    public void ClientNewTurn()
    {
        instance.CmdNewTurn();
    }
    [Client]
    public void ClientYieldCard()
    {
        //GameManager.instance.state_ = GameManager.Temp_STATE.STATE_YIELD_CARDS;
        //GameManager.instance.state_ = GameManager.Temp_STATE.STATE_BUSYCONNECTING;
        //instance.CmdSetState(GameManager.Temp_STATE.STATE_BUSYCONNECTING);


        //ClientDelay_AfterYieldCard();

        instance.temp_list_index_offender.Clear();
        instance.selectedCard.GetComponent<HandCard>().CloseDetail();
        
        if (instance.selectedCard.GetComponent<HandCard>().count_offender != 0)
        {
            instance.ClientShowLastYieldCard(list_playerName[GetIndex_in_list_netId((int)instance.netId)], instance.selectedCard.GetComponent<HandCard>().index_Card, instance.temp_list_index_offender);
            UIPlayerManager.instance.Show_Button_Select();
        }
        else
        {
            for (int i = 0; i < list_netId.Count; i++)
            {
                instance.temp_list_index_offender.Add(i);
            }
            if (instance.selectedCard.GetComponent<HandCard>().index_Card == 1007)
            {
                instance.temp_list_index_offender.Clear();
                int my_index = GetIndex_in_list_netId((int)instance.netId);
                if (index_Circle < 2)
                {
                    Debug.Log("第一圈 my_index = " + my_index);
                    for (int i = 0; i < list_netId.Count; i++)
                    {
                        if (i == my_index) continue;
                        instance.temp_list_index_offender.Add(i);
                        //if (!instance.temp_list_index_offender.Contains(i)) continue;
                        //instance.temp_Card_1007_checkCount++;
                        //RpcCard_1007_CollectScoreCards(list_netId[i]);
                    }
                }
                else if (index_Circle == 2)
                {
                    for (int i = my_index + 1; ; i++)
                    {
                        if (i == list_netId.Count) i = 0;
                        if (i == index_CurrentHolder - 1) break;
                        instance.temp_list_index_offender.Add(i);
                        //if (!temp_list_index_offender.Contains(i)) continue;
                        //instance.temp_Card_1007_checkCount++;
                        //RpcCard_1007_CollectScoreCards(list_netId[i]);
                    }
                }
            }
            if (instance.selectedCard.GetComponent<HandCard>().index_Card / 1000 == 3)
            {
                instance.temp_list_index_offender.Clear();
                instance.temp_list_index_offender.Add(GetIndex_in_list_netId((int)instance.netId));
            }
            instance.ClientShowLastYieldCard(list_playerName[GetIndex_in_list_netId((int)instance.netId)], instance.selectedCard.GetComponent<HandCard>().index_Card, instance.temp_list_index_offender);
            instance.CmdCheckCard_2001and2002(instance.selectedCard.GetComponent<HandCard>().index_Card, (int)instance.netId, instance.temp_list_index_offender);
            instance.ClientRealizeHandCard();
        }
        instance.count_MyHandCard--;
        instance.ClientDiscardHandCard((int)instance.netId, instance.selectedCard.GetComponent<HandCard>().index_Card);
        instance.selectedCard.SetActive(false);
    }
    [Client]
    public void ClientShowLastYieldCard(string name_attacker,int index_Card,List<int> list_index_offender)
    {
        CmdShowLastYieldCard(name_attacker, index_Card, list_index_offender);
    }
    [Client]
    public void ClientRefreshLastYieldCard(List<int> list_index_offender)
    {
        instance.CmdRefreshLastYieldCard(list_index_offender);
    }
    [Client]
    public void ClientCloseLastYieldCard()
    {
        instance.CmdCloseLastYieldCard();
    }
    //[Client]
    //public void ClientDelay_AfterYieldCard()
    //{
    //if(GameManager.instance.state_ != GameManager.Temp_STATE.STATE_BUSYCONNECTING)
    //{
    //    Invoke(nameof(ClientDelay_AfterYieldCard), 0.3f);
    //    return;
    //}
    //}
    [Client]
    public void ClientThrowCard()
    {
        instance.selectedCard.GetComponent<HandCard>().CloseDetail();
        Debug.Log("丢弃序号" + instance.selectedCard.GetComponent<HandCard>().index_Card);
        instance.count_MyHandCard--;
        instance.ClientDiscardHandCard((int)instance.netId, instance.selectedCard.GetComponent<HandCard>().index_Card);
        Destroy(instance.selectedCard);
        instance.Client_ThrowCard_EndJudge((int)instance.netId);
    }
    [Client]
    public void Client_ThrowCard_EndJudge(int onlineID)
    {
        //if(onlineID != (int)instance.netId) { return; }
        Debug.Log("需要弃牌？剩余手牌数 = " + instance.count_MyHandCard);
        if (instance.count_MyHandCard <= 4)
        {
            instance.CmdSetState(GameManager.Temp_STATE.STATE_TURNING_TURN);
            instance.ClientNewTurn();
        }
    }
    [Client]
    public void ClientRealizeHandCard()
    {
        if(GameManager.instance.state_ != GameManager.Temp_STATE.STATE_REALIZING_CARDS)
        {
            Invoke(nameof(ClientRealizeHandCard), 1f);
            return;
        }
        
        
        if(instance.selectedCard.GetComponent<HandCard>().isAttackCard)
        {
            instance.temp_list_index_offender = UIPlayerManager.instance.CheckStates(instance.temp_list_index_offender, 3002);
        }
        instance.ClientRefreshLastYieldCard(instance.temp_list_index_offender);
        if (instance.selectedCard.GetComponent<HandCard>().isAttackCard && instance.selectedCard.GetComponent<HandCard>().count_offender == 1 && instance.temp_list_index_offender.Count == 0)
        {
            UIManager.instance.UINotice_Defend();
            instance.ClientOnEndRealizeHandCard();
            return;
        }
        if (instance.selectedCard.GetComponent<HandCard>().isTimingCard)
        {
            instance.CmdAddStateCard((int)instance.netId, instance.temp_list_index_offender, instance.selectedCard.GetComponent<HandCard>().index_Card);
        }
        //if (list_index_offender[0]!= -1)//不是放弃选择
        //{
        switch (instance.selectedCard.GetComponent<HandCard>().index_Card)////手牌新增
            {
                case 1001://代打
                    Debug.Log("代打");
                    instance.CmdGetHisAllHandCards((int)instance.netId,instance.temp_list_index_offender);
                    break;
                case 1002://天下第一音游祭
                    Debug.Log("天下第一音游祭");
                    instance.CmdCard_1002_CollectAllScoreCards((int)instance.netId, instance.temp_list_index_offender);
                    break;
                case 1003://指点江山
                    Debug.Log("指点江山");
                    instance.CmdCard_1003_GetHisScoreCard((int)instance.netId, instance.temp_list_index_offender,1003);
                    break;
                case 1004://观看手元
                    Debug.Log("观看手元");
                    instance.CmdCard_1004_GetHisScoreCard(instance.temp_list_index_offender);
                    break;
                case 1005://神之左手
                    Debug.Log("神之左手");
                    //instance.temp_list_index_offender.Clear();
                    //for (int i=0;i<list_netId.Count;i++)
                    //{
                    //    instance.temp_list_index_offender.Add(i);
                    //}
                    instance.CmdCard_1005_GetLeftSuspectedScore(instance.temp_list_index_offender);
                    instance.CmdCard_1005or1006_CollectAllScoreCards(instance.temp_list_index_offender,1005);
                    break;
                case 1006://鬼之右手
                    Debug.Log("鬼之右手");
                    //instance.temp_list_index_offender.Clear();
                    //for (int i = 0; i < list_netId.Count; i++)
                    //{
                    //    instance.temp_list_index_offender.Add(i);
                    //}
                    instance.CmdCard_1006_GetRightSuspectedScore(instance.temp_list_index_offender);
                    instance.CmdCard_1005or1006_CollectAllScoreCards(instance.temp_list_index_offender, 1006);
                    break;
                case 1007://音游窝
                    Debug.Log("音游窝");
                    instance.CmdCard_1007_CollectScoreCards((int)instance.netId, instance.temp_list_index_offender);
                    break;
                case 1008://音游王
                    Debug.Log("音游王");
                    break;
                case 1009://联机
                    Debug.Log("联机");
                    instance.CmdCard_1005_GetLeftSuspectedScore(instance.temp_list_index_offender);
                    instance.CmdCard_1005or1006_CollectAllScoreCards(instance.temp_list_index_offender, 1005);
                    break;
                case 1010://自来熟
                    Debug.Log("自来熟");
                    break;

                case 2001://手癖
                    Debug.Log("手癖");
                    break;
                case 2002://降噪耳机
                    Debug.Log("降噪耳机");
                    break;
                case 2003://网络延迟
                    Debug.Log("网络延迟");
                    break;

                case 3001://看铺
                    Debug.Log("看铺");
                    break;
                case 3002://私人订制手台
                    Debug.Log("私人订制手台");
                    break;
                case 3003://底力提升
                    Debug.Log("底力提升");
                    instance.ClientDrawHandCards((int)instance.netId, 2);
                    Empty.instance.CmdSetState(GameManager.Temp_STATE.STATE_ONENDREALIZING_CARDS);
                    break;
                case 3004://从头开始
                    Debug.Log("从头开始");
                    instance.ClientDrawScoreCard((int)instance.netId,true);
                    Empty.instance.CmdSetState(GameManager.Temp_STATE.STATE_ONENDREALIZING_CARDS);
                    break;
            }
        //}
        //if(instance.selectedCard.GetComponent<HandCard>().index_Card !=1001)
        instance.ClientOnEndRealizeHandCard();



    }
    [Client]
    public void ClientOnEndRealizeHandCard()
    {
        
        if (GameManager.instance.state_ == GameManager.Temp_STATE.STATE_REALIZING_CARDS)
        {
            Invoke(nameof(ClientOnEndRealizeHandCard), delay);
            return;
        }
        Empty.instance.ClientCloseLastYieldCard();
        Empty.instance.ClientOnEndUIRealizeHandCard();
        //Empty.instance.CmdSetState(GameManager.Temp_STATE.STATE_YIELD_CARDS);
    }
    [Client]
    public void ClientOnEndUIRealizeHandCard()
    {
        if (GameManager.instance.state_ == GameManager.Temp_STATE.STATE_ONENDREALIZING_CARDS)
        {
            Invoke(nameof(ClientOnEndUIRealizeHandCard), delay);
            return;
        }
        int count_turn = Empty.instance.turnMove.Count;
        Empty.instance.turnMove[count_turn - 1]++;
        Debug.Log("已行动次数 =" + Empty.instance.turnMove[count_turn - 1]);
        //instance.totalMove++;
        if (Empty.instance.turnMove[count_turn - 1] >= 3)
        {
            Debug.Log("准备弃牌");
            UIManager.instance.UIFinishYieldCard();
            return;
        }
        Empty.instance.selectedCard.GetComponent<HandCard>().CloseDetail();
        Empty.instance.CmdSetState(GameManager.Temp_STATE.STATE_YIELD_CARDS);
    }
    [Client]
    public void ClientGiveMyAllHandCards(int id_attacker, List<int> list_index_handCard)//1001 代打
    {
        //Debug.Log("#1001 ClientGiveMyAllHandCards 应接受ID = " + id_attacker);
        CmdGiveMyAllHandCards(id_attacker, list_index_handCard);
    }
    [Client]
    public void ClientClearAllHandCards(int onlineID)
    {
        instance.count_MyHandCard = 0;
        UIManager.instance.ClearAllHandCards();
        instance.CmdClearAllHandCards(onlineID);
    }
    [Client]
    public void ClientCard_1002_NextTurn(GameObject scoreCard)
    {
        ScoreCardManager.instance.Card_1002_ReGetScoreCard(scoreCard);
        CmdCard_1002_NextTurn((int)instance.netId, scoreCard.transform.GetSiblingIndex());
    }
    
    [Client]
    public void ClientGainScore()
    {
        Empty.instance.CmdSetState(GameManager.Temp_STATE.STATE_ADDING_SCORES);
        UIManager.instance.DiscloseScoreCard(instance.index_Shown, instance.list_Card_1002_ScoreCard,instance.list_Card_1002_netId_ScoreCard);
        instance.index_Shown++;
        if (instance.index_Shown == list_netId.Count)
        {
            instance.index_Shown = 0;
        }
        if (instance.index_Shown == index_CurrentHolder - 1)
        {
            Invoke(nameof(ClientDelay_EndGainScore), 5f);
            return;
        }
        Invoke(nameof(ClientGainScore), 5f);
    }
    [Client]
    public void ClientDelay_EndGainScore()
    {
        Empty.instance.CmdSetState(GameManager.Temp_STATE.STATE_TURNING_ROUND);
        UIManager.instance.EndDiscloseScoreCard();
    }


    public bool CheckRepeatedNetId(int checked_netId)
    {
        for(int i=0;i<list_netId.Count;i++)
        {
            if (list_netId[i] == checked_netId) return true;
        }
        return false;
    }
    public string GetContent_int(List<int> list)
    {
        string a="";
        for(int i=0;i< list.Count;i++)
        {
            a += list[i].ToString();
            a += " ";
        }
        return a;
    }
    public string GetContent_string(List<string> list)
    {
        string a = "";
        for(int i = 0; i < list.Count; i++)
        {
            a += list[i].ToString();
            a += " ";
        }
        return a;
    }
    public int GetIndex_in_list_netId(int netId)
    {
        for(int i=0;i<list_netId.Count;i++)
        {
            if(netId == list_netId[i]) return i;
        }
        return -1;
    }
    public string GetContent_by_IndexId(List<int> list_index_offender) 
    {
        string a = "";
        for(int i=0;i<list_index_offender.Count;i++)
        {
            a += list_playerName[list_index_offender[i]];
            if(i!= list_index_offender.Count -1)
            {
                a += "\n";
            }
        }
        return a;
    }
    public void Call_ClientCard_1002_NextTurn(GameObject scoreCard)
    {
        instance.ClientCard_1002_NextTurn(scoreCard);
    }
    public List<T> RandomList<T>(List<T> inList)
    {
        if (inList.Count == 0) return null;
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







    /*
    [Command(requiresAuthority = false)]
    public void CmdAddPlayer(int added_netId)
    {
        Debug.Log("[Server] CmdAddPlayer()");
        ServerAddPlayer(added_netId);
    }
    [Command(requiresAuthority = false)]//↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓
    public void CmdRemovePlayer(int added_netId)
    {
        //Debug.Log(NetworkClient.ready + " " + NetworkClient.active + "" + NetworkClient.isConnected);
        //NetworkClient.ready = true;
        //NetworkClient.connectState = ConnectState.Connected;
        //Debug.Log(NetworkClient.ready + " " + NetworkClient.active + "" + NetworkClient.isConnected);

        //if(!isServer)//↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑
        //{
        //    return;
        //}
        //Debug.Log("isServer = " + Empty.list_instance.isServer);
        //Debug.Log("isClient = " + Empty.list_instance.isClient);


        Debug.Log("[Server] CmdRemovePlayer()");
        ServerRemovePlayer(added_netId);
    }
    [Server]
    public void ServerAddPlayer(int added_netId)
    {
        Debug.Log("[Server] ServerAddPlayer()");
        UIPlayerManager.list_netId.Add(added_netId);
        UIPlayerManager.instance.RefreshPlayer();
        //Debug.Log("isClient = " + isClient);
        //Debug.Log("isServer = " + isServer);
        RpcAddPlayer(UIPlayerManager.list_netId);
    }
    [Server]
    public void ServerRemovePlayer(int added_netId)
    {
        Debug.Log("[Server] ServerRemovePlayer()");
        int i = 0;
        for (; i < UIPlayerManager.list_netId.Count; i++)
        {
            if (UIPlayerManager.list_netId[i] == added_netId) break;
        }
        if (i == UIPlayerManager.list_netId.Count)
        {
            Debug.LogError("未找到id来删除!!");
            return;
        }
        //Debug.Log("i = " + i);
        UIPlayerManager.list_netId.RemoveAt(i);
        UIPlayerManager.instance.RefreshPlayer();
        
        Debug.Log("前 RpcRemovePlayer(i)--- Empty.instance = " + Empty.instance.name);
        TEST_2();
        RpcRemovePlayer(UIPlayerManager.list_netId);
        Debug.Log("后 RpcRemovePlayer(i)--- Empty.instance = " + Empty.instance.name);
    }
    [ClientRpc]
    public void TEST_2()
    {
        Debug.Log("TEST_2");
    }
    [ClientRpc]
    public void RpcAddPlayer(List<int> list_netId)
    {

        Debug.Log("[Client] RpcAddPlayer()");
        UIPlayerManager.instance.ClearPlayer();
        UIPlayerManager.list_netId = list_netId;
        //if (!temp_netID) Debug.LogError("TNND ");
        //if(!temp_player)
        //{
        //    Debug.Log("DON'T have temp_player");
        //    Invoke(nameof(RpcAddPlayer), delay);
        //    return;
        //}
        UIPlayerManager.instance.RefreshPlayer();

    }


    [ClientRpc]
    public void RpcRemovePlayer(List<int> list_netId)
    {
        //if (!temp_netID) Debug.LogError("TNND ");
        Debug.Log("[Client] RpcRemovePlayer()");
        UIPlayerManager.list_netId = list_netId;
        //if(!temp_player)
        //{
        //    Debug.Log("DON'T have temp_player");
        //    Invoke(nameof(RpcAddPlayer), delay);
        //    return;
        //}
        UIPlayerManager.instance.RefreshPlayer();
        Debug.Log("list_netId.Count = " + UIPlayerManager.list_player.Count);


        ////MyNetworkManager.instance.My_OnServerDisconnect();
        //MyNetworkManager.instance.base_OnClientDisconnect();
        //MyNetworkManager.instance.base_OnStopClient();
        //MyNetworkManager.instance.base_OnServerDisConnect(conn);

        //MyNetworkManager.instance.NonePara_base_OnServerDisConnect();
        //////////////////////////////////////////////////
    }
    */
}
