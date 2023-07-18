using Mirror;
//using System;
using System.Collections.Generic;
using Telepathy;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Empty : NetworkBehaviour
{
    public static Empty instance;

    public static List<int> list_netId = new();//玩家Id列表
    public static List<string> list_playerName = new();//玩家姓名列表


    /// <summary>
    /// Empty <- GameManager
    /// </summary>
    public enum Temp_STATE
    {
        STATE_GAME_IDLING,
        STATE_GAME_STARTED,
        STATE_GAME_SUMMARY,
        STATE_DRAW_CARDS,
        STATE_JUDGE_CARDS,
        STATE_YIELD_CARDS,
        STATE_THROW_CARDS
    }
    public static Temp_STATE state_ = Temp_STATE.STATE_GAME_IDLING;
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
    public List<int> roundScore = new();////待添加
    public int totalMove;
    public List<int> turnMove = new();////待添加
    public int count_MyHandCard;
    //public int count_RoundUsedCard;
    //public int count_TotalUsedCard;
    public int last_index_yieldedCard;
    public GameObject last_selectedCard;
    public GameObject selectedCard;
    public GameObject scoreCard;
    //public List<GameObject> handCards = new();


    /// <summary>
    /// Card
    /// </summary>
    public List<int> list_Card_1002_ScoreCard = new();
    public int temp_card_1002_id_attacker;

    public float delay = 0.2f;




    private void Awake()
    {
        Delay_set_instance();
    }


    public void Delay_set_instance()
    {
        //Debug.Log("Delay_set_instance()");
        if (isLocalPlayer)
        {
            instance = this;
            //Debug.Log("AWAKE instance.netId = " + instance.netId);
        }
        if (!instance)
        {
            Invoke(nameof(Delay_set_instance), delay);
        }
    }
    [Command]
    public void CmdAddPlayer(int added_netId, string added_name)
    {
        //Debug.Log("CmdAddPlayer()");
        ServerAddPlayer(added_netId, added_name);
    }
    [Command]
    public void CmdStartGame()
    {
        ServerStartGame();
    }
    [Command]
    public void CmdDiscardScoreCard(int index_ScoreCard)
    {
        RpcDiscardScoreCard(index_ScoreCard);
    }
    [Command]
    public void CmdDiscardHandCard(int onlineID, int index)
    {
        RpcDiscardHandCard(onlineID, index);
    }
    [Command]
    public void CmdDrawHandCards(int onlineID, int times)
    {
        RpcDrawHandCards(onlineID, times);
    }
    [Command]
    public void CmdDrawScoreCard(int onlineID, bool canDiscard)
    {
        RpcDrawScoreCard(onlineID,canDiscard);
    }
    [Command]
    public void CmdNewTurn()
    {
        ServerNewTurn();
    }
    [Command]
    public void CmdGetHisAllHandCards(int id_attacker, List<int> list_index_offender)//1001 代打
    {
        RpcGetHisAllHandCards(id_attacker, list_index_offender);
    }
    [Command]
    public void CmdGiveMyAllHandCards(int id_attacker, List<int> list_index_handCard)//1001 代打
    {
        RpcReceiveHisAllHandCards(id_attacker, list_index_handCard);
    }
    [Command]
    public void CmdClearAllHandCards(int onlineID)//1001 代打
    {
        RpcClearAllHandCards(onlineID);
    }
    [Command]
    public void CmdDrawHandCards_Specific(int onlineID, List<int> list_index_handCard)//1001 代打
    {
        RpcDrawHandCards_Specific(onlineID, list_index_handCard);
    }
    [Command]
    public void CmdDrawScoreCard_Specific(int onlineID, int score)//1003 指点江山
    {
        RpcDrawScoreCard_Specific(onlineID, score);
    }
    [Command]
    public void CmdCard_1002_CollectAllScoreCards(int id_attacker)
    {
        instance.list_Card_1002_ScoreCard.Clear();
        for (int i = 0; i < list_netId.Count; i++)
        {
            bool isOver = (i == list_netId.Count - 1);
            RpcCard_1002_CollectAllScoreCards(id_attacker, list_netId[i]);
        }
    }
    [Command]
    public void CmdCard_1002_AddScoreCard(int id_attacker, int score/*, bool isover*/)
    {
        instance.list_Card_1002_ScoreCard.Add(score);
        Debug.Log("1002   #" + GetContent_int(instance.list_Card_1002_ScoreCard));
        instance.ServerDelay_RpcCard_1002_Show_Panel_SelectScoreCard();
        instance.temp_card_1002_id_attacker = id_attacker;
    }
    [Command]
    public void CmdCard_1002_NextTurn(int id_turn, int index_last_Selected)
    {
        RpcCard_1002_NextTurn(id_turn, index_last_Selected);
    }
    [Command]
    public void CmdCard_1003_GetHisScoreCard(int id_attacker, List<int> list_index_offender)
    {
        RpcCard_1003_GetHisScoreCard(id_attacker, list_index_offender);
    }
    [Command]
    public void CmdCard_1003_ReceiveScoreCard(int id_attacker,int score)
    {
        RpcCard_1003_ReceiveScoreCard(id_attacker,score);
    }
    [Command]
    public void CmdCard_1004_GetHisScoreCard( List<int> list_index_offender)
    {
        RpcCard_1004_GetHisScoreCard(list_index_offender);
    }
    [Command]
    public void CmdCard_1004_Show_Panel(int id_offender,int score)
    {
        RpcCard_1004_Show_Panel(id_offender,score);
    }
    //[Command]
    //public void CmdYieldCard()
    //{
    //    RpcYieldCard();
    //}
    //[Command]
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
        state_ = Temp_STATE.STATE_GAME_STARTED;
        index_Round = 0;
        init_draw_num = 4;
        isSwitchHolder = false;
        index_CurrentHolder = index_CurrentPlayer = 0;

        ScoreCardManager.instance.RefillScoreCards();
        HandCardManager.instance.RefillHandCards();
        RpcInitialize(ScoreCardManager.list_index, HandCardManager.list_index);

        ServerNewRound();

    }
    [Server]
    public void ServerNewRound()
    {
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

        }
        else
        {
            RpcSetHolder(index_CurrentHolder - 1, false);
            index_CurrentHolder++;
            if (index_CurrentHolder > list_netId.Count)
            {
                ///////SummaryGame();
                return;
            }
            RpcSetHolder(index_CurrentHolder - 1, true);
        }

        ServerNewTurn();
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

            }
            if (index_Circle == 3)
            {
                ServerNewRound();
                return;
            }
        }
        RpcSetIndex(index_Circle, index_CurrentPlayer, index_CurrentHolder, index_Round);
        RpcMyTurn(index_CurrentPlayer - 1);
    }

    [Server]
    public void ServerDelay_RpcCard_1002_Show_Panel_SelectScoreCard()
    {

        if (instance.list_Card_1002_ScoreCard.Count != list_netId.Count)
        {
            //Debug.Log("[Server]Delay_RpcCard_1002_Show_Panel_SelectScoreCard");
            Invoke(nameof(ServerDelay_RpcCard_1002_Show_Panel_SelectScoreCard), delay);
            return;
        }
        List<int> randomed_list_scoreCard = RandomList(instance.list_Card_1002_ScoreCard);
        instance.RpcCard_1002_Show_Panel_SelectScoreCard(instance.temp_card_1002_id_attacker, randomed_list_scoreCard);
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
        instance.count_MyHandCard = 0;
        instance.roundScore.Clear();
        instance.turnMove.Clear();
        instance.totalMove = 0;
        for (int j = 0; j < list_netId.Count; j++)
        {
            instance.roundScore.Add(0);
        }
        ScoreCardManager.instance.RefreshScoreCards(list_index_ScoreCards);
        HandCardManager.instance.RefreshHandCards(list_index_HandCards);
        UIManager.instance.ClearHandCards_and_ScoreCard();
        UIPlayerManager.instance.RefreshPlayer(list_netId, list_playerName);
        //GameCardManager.list_instance.RefillHandCards();
        
        


    }
    [ClientRpc]
    public void RpcDrawScoreCard(int onlineID,bool canDiscard)
    {
        if (instance.netId !=onlineID)
        {
            ScoreCardManager.instance.Sync_DrawOneCard();
        }
        else
        {
            ScoreCardManager.instance.DrawOneCard(canDiscard);
        }
    }
    [ClientRpc]
    public void RpcDrawScoreCard_Specific(int onlineID, int score)//1003 指点江山
    {
        if((int)instance.netId == onlineID)
        {
            ScoreCardManager.instance.DrawOneCard_Specific(score);
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
            if (instance.netId != onlineID)
            {
                HandCardManager.instance.Sync_DrawOneCard();
            }
            else
            {
                HandCardManager.instance.DrawOneCard();
                
            }
            
            UIPlayerManager.list_player[index].GetComponent<Player>().Text_CardNum.text = (int.Parse(UIPlayerManager.list_player[index].GetComponent<Player>().Text_CardNum.text)+1).ToString();
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
            if (instance.netId != onlineID)
            {
                //HandCardManager.instance.Sync_DrawOneCard_Specific(); //并没有实现
            }
            else
            {
                HandCardManager.instance.DrawOneCard_Specific(list_index_handCard[j]);

            }

            UIPlayerManager.list_player[index].GetComponent<Player>().Text_CardNum.text = (int.Parse(UIPlayerManager.list_player[index].GetComponent<Player>().Text_CardNum.text) + 1).ToString();
        }
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
        GameManager.state_ = GameManager.Temp_STATE.STATE_YIELD_CARDS;
        if (list_netId[index] == instance.netId)
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
    }

    [ClientRpc]
    public void RpcGetHisAllHandCards(int id_attacker, List<int> list_index_offender)
    {
        if (list_index_offender.Contains((int)instance.netId))
        {
            int count_myHandCards = HandCardManager.instance.GetCountOfMyHandCards();
            Debug.Log("我的手牌数 = " + instance.count_MyHandCard);
            instance.ClientGiveMyAllHandCards(id_attacker,HandCardManager.instance.GetIndexesOfMyHandCards());
            

            instance.ClientClearAllHandCards((int)instance.netId);
            instance.ClientDrawHandCards((int)instance.netId, count_myHandCards);
        }
    }
    [ClientRpc]
    public void RpcReceiveHisAllHandCards(int id_attacker, List<int> list_index_handCard)
    {
        if(instance.netId == id_attacker)
        {
            instance.ClientDrawHandCards_Specific((int)instance.netId, list_index_handCard);
        }
    }
    [ClientRpc]
    public void RpcClearAllHandCards(int onlineID)
    {
        int index_player = GetIndex_in_list_netId(onlineID);
        UIPlayerManager.list_player[index_player].GetComponent<Player>().Text_CardNum.text = "0";
    }
    [ClientRpc]
    public void RpcCard_1002_CollectAllScoreCards(int id_attacker,int index_offender/*,bool isover*/)
    {
        if (instance.netId != index_offender) return;
        instance.scoreCard.SetActive(false);
        instance.CmdCard_1002_AddScoreCard(id_attacker,instance.scoreCard.GetComponent<ScoreCard>().score/*, isover*/);
        Destroy(instance.scoreCard);
    }
    [ClientRpc]
    public void RpcCard_1002_Show_Panel_SelectScoreCard(int id_attacker,List<int> list_scoreCard)
    {
        UIManager.instance.UICard_1002_ShowPanel(id_attacker, list_scoreCard);
        //if (instance.netId != id_attacker) return;
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
        UIManager.instance.UICard_1002_NextTurn(last_id_turn,list_netId[index_id_turn], index_last_Selected);
    }
    [ClientRpc]
    public void RpcCard_1003_GetHisScoreCard(int id_attacker,List<int> list_index_offender)
    {
        if (list_index_offender[0] == ((int)instance.netId))
        {
            if (list_index_offender[0] == id_attacker)//选择自己相当于remake
            {
                instance.ClientDrawScoreCard((int)instance.netId, true);
            }
            else
            {
                instance.CmdCard_1003_ReceiveScoreCard(id_attacker, instance.scoreCard.GetComponent<ScoreCard>().score);
                instance.ClientDrawScoreCard((int)instance.netId, false);
            }
            
        }
        
    }
    [ClientRpc]
    public void RpcCard_1003_ReceiveScoreCard(int id_attacker,int score)
    {
        if(instance.netId == id_attacker)
        {
            instance.ClientDrawScoreCard_Specific((int)instance.netId,score);
        }
    }
    [ClientRpc]
    public void RpcCard_1004_GetHisScoreCard( List<int> list_index_offender)
    {
        if (list_index_offender[0] == ((int)instance.netId))
        {
            instance.CmdCard_1004_Show_Panel(list_index_offender[0],instance.scoreCard.GetComponent<ScoreCard>().score);
        }
    }
    [ClientRpc]
    public void RpcCard_1004_Show_Panel(int id_offender, int score)
    {
        UIManager.instance.UICard_1004_ShowPanel(id_offender, score);
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
        instance.CmdDrawScoreCard_Specific(onlineID,score);
    }
    [Client]
    public void ClientNewTurn()
    {
        instance.CmdNewTurn();
    }
    [Client]
    public void ClientYieldCard()
    {
        instance.selectedCard.GetComponent<HandCard>().CloseDetail();
        if(instance.selectedCard.GetComponent<HandCard>().count_offender != 0)
        {
            UIPlayerManager.instance.Show_Button_Select();
        }
        else
        {
            instance.ClientRealizeHandCard(new List<int> { (int)instance.netId});
        }


        instance.count_MyHandCard--;
        instance.ClientDiscardHandCard((int)instance.netId, instance.selectedCard.GetComponent<HandCard>().index_Card);
        instance.selectedCard.SetActive(false);

        
    }
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
        //Debug.Log("剩余手牌数 = " + instance.count_MyHandCard);
        if (instance.count_MyHandCard <= 4)
        {
            instance.ClientNewTurn();
        }
    }
    [Client]
    public void ClientRealizeHandCard(List<int> list_index_offender)//1001 代打
    {
        if (list_index_offender[0]!= -1)//不是放弃选择
        {
            switch (instance.selectedCard.GetComponent<HandCard>().index_Card)////手牌新增
            {
                case 1001://代打
                    Debug.Log("代打");
                    instance.CmdGetHisAllHandCards((int)instance.netId, list_index_offender);
                    break;
                case 1002://天下第一音游祭
                    Debug.Log("天下第一音游祭");
                    instance.CmdCard_1002_CollectAllScoreCards((int)instance.netId);
                    break;
                case 1003://指点江山
                    Debug.Log("指点江山");
                    instance.CmdCard_1003_GetHisScoreCard((int)instance.netId, list_index_offender);
                    break;
                case 1004://观看手元
                    Debug.Log("观看手元");
                    instance.CmdCard_1004_GetHisScoreCard(list_index_offender);
                    break;
                case 1005://神之左手
                    Debug.Log("神之左手");
                    break;
                case 1006://鬼之右手
                    Debug.Log("鬼之右手");
                    break;
                case 1007://音游窝
                    Debug.Log("音游窝");
                    break;
                case 1008://音游王
                    Debug.Log("音游王");
                    break;
                case 1009://联机
                    Debug.Log("联机");
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
                    break;
                case 3004://从头开始
                    Debug.Log("从头开始");
                    instance.ClientDrawScoreCard((int)instance.netId,true);
                    break;
            }
        }
        

        GameManager.state_ = GameManager.Temp_STATE.STATE_YIELD_CARDS;

        int count_turn = instance.turnMove.Count;
        instance.turnMove[count_turn - 1]++;
        Debug.Log("instance.turnMove[count_turn - 1] =" + instance.turnMove[count_turn - 1]);
        instance.totalMove++;
        if (instance.turnMove[count_turn - 1] >= 3)
        {
            Debug.Log("准备弃牌");
            UIManager.instance.UIFinishYieldCard();
        }
        Destroy(instance.selectedCard);
    }
    [Client]
    public void ClientGiveMyAllHandCards(int id_attacker, List<int> list_index_handCard)//1001 代打
    {
        Debug.Log("ClientGiveMyAllHandCards 应接受ID = " + id_attacker);
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
    public void Call_ClientCard_1002_NextTurn(GameObject scoreCard)
    {
        instance.ClientCard_1002_NextTurn(scoreCard);
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







    /*
    [Command]
    public void CmdAddPlayer(int added_netId)
    {
        Debug.Log("[Server] CmdAddPlayer()");
        ServerAddPlayer(added_netId);
    }
    [Command]//↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓
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
