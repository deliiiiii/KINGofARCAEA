using Mirror;
using System.Collections.Generic;
using Telepathy;
using UnityEngine;
using UnityEngine.UI;

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
    public int totalScore;
    public List<int> roundScore = new();////待添加
    public int totalMove;
    public List<int> turnMove = new();////待添加
    public int count_HandCard;
    public int count_RoundUsedCard;
    public int count_TotalUsedCard;

    public GameObject selectedCard;
    public GameObject scoreCard;
    public List<GameObject> handCards = new();


    public float delay = 0.2f;


    public enum STATE
    {
        STATE_GAME_IDLING,
        STATE_GAME_STARTED,
        STATE_GAME_SUMMARY,
        STATE_DRAW_CARDS,
        STATE_JUDGE_CARDS,
        STATE_YIELD_CARDS,
        STATE_THROW_CARDS
    }
    public static STATE state = STATE.STATE_GAME_IDLING;

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
    public void CmdAddPlayer(int added_netId,string added_name)
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
    public void CmdDiscardScoreCard(int index)
    {
        RpcDiscardScoreCard(index);
    }
    [Command]
    public void CmdDrawHandCards(int onlineID, int times)
    {
        RpcDrawHandCards(onlineID, times);
    }
    [Command]
    public void CmdNewTurn()
    {
        ServerNewTurn();
    }
    [Server]
    public void ServerAddPlayer(int added_netId, string added_name)
    {
        if(CheckRepeatedNetId(added_netId))
        {
            Debug.Log("id重复！");
            return;
        }
        //Debug.Log("ServerAddPlayer  netId = " + netId + " || instance.netId = " + instance.netId);
        if (added_netId == 1) return;
        list_netId.Add(added_netId);
        list_playerName.Add(added_name);

        

        RpcClearPlayer();
        for(int i=0;i<list_netId.Count;i++)
        {
            RpcAddPlayer(list_netId[i], list_playerName[i]);
        }
        UIPlayerManager.instance.RefreshPlayer(list_netId, list_playerName);
        RpcRefreshPlayer();
        Debug.Log("[Server] list_netId = " + GetContent_int(list_netId));
        Debug.Log("[Server] list_playerName = " + GetContent_string(list_playerName));
    }
    [Server]
    public void ServerRomovePlayer(int removed_netId)
    {
        //Debug.Log("ServerRomovePlayer  netId = " + netId + " || instance.netId = " + instance.netId);
        if (removed_netId == 1) return;
        int removed_index = -1;
        for(int i=0;i<list_netId.Count;i++)
        {
            if (list_netId[i] == removed_netId)
            {
                removed_index = i;
                break;
            }
        }
        if(removed_index == -1)
        {
            Debug.LogError("未找到ID来删除!!");
            return;
        }



        Debug.Log("[Server] 离开 " + removed_netId + " " + list_playerName[removed_index]);
        list_netId.RemoveAt(removed_index);
        list_playerName.RemoveAt(removed_index);
        Debug.Log("[Server] list_netId = " + GetContent_int(list_netId));
        Debug.Log("[Server] list_playerName = " + GetContent_string(list_playerName));
        RpcClearPlayer();
        for (int i = 0; i < list_netId.Count; i++)
        {
            RpcAddPlayer(list_netId[i], list_playerName[i]) ;
        }

        UIPlayerManager.instance.RefreshPlayer(list_netId, list_playerName);
        RpcRefreshPlayer();
    }
    [Server]
    public void ServerStartGame()
    {
        state = STATE.STATE_GAME_STARTED;
        index_Round = 0;
        init_draw_num = 4;
        isSwitchHolder = false;
        index_CurrentHolder = index_CurrentPlayer = 0;

        ScoreCardManager.instance.RefillScoreCards();
        HandCardManager.instance.RefillHandCards();
        RpcInitialize(ScoreCardManager.list_index,HandCardManager.list_index);

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
            RpcSetHolder(index_CurrentHolder-1,true);

            for (int i = 0; i < list_netId.Count; i++)
            {
                RpcDrawScoreCard(list_netId[i]);////判空
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
                RpcSetCircleNum(index_Circle);
            }
            if (index_Circle == 3)
            {
                ServerNewRound();
                return;
            }
        }
        RpcMyTurn(index_CurrentPlayer - 1);
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
        Debug.Log("[Client] list_netId = " + GetContent_int(list_netId));
    }
    [ClientRpc]
    public void RpcRefreshPlayer()
    {
        UIPlayerManager.instance.RefreshPlayer(list_netId, list_playerName);
    }
    [ClientRpc]
    public void RpcInitialize(List<int> list_index_ScoreCards,List<int> list_index_HandCards)
    {
        instance.roundScore.Clear();
        for (int j = 0; j < list_netId.Count; j++)
        {
            instance.roundScore.Add(0);
        }
        ScoreCardManager.instance.RefreshScoreCards(list_index_ScoreCards);
        HandCardManager.instance.RefreshHandCards(list_index_HandCards);
        //GameCardManager.list_instance.RefillHandCards();
        
        


    }
    [ClientRpc]
    public void RpcDrawScoreCard(int onlineID)
    {
        if (instance.netId !=onlineID)
        {
            ScoreCardManager.instance.Sync_DrawOneCard();
        }
        else
        {
            ScoreCardManager.instance.DrawOneCard();
        }
    }
    [ClientRpc]
    public void RpcDrawHandCards(int onlineID,int times)
    {
        if (instance.netId != onlineID)
        {
            for(int j = 0; j < times;j++)
            {
                HandCardManager.instance.Sync_DrawOneCard();
            }
            
        }
        else
        {
            for (int j = 0; j < times; j++)
            {
                HandCardManager.instance.DrawOneCard();
            }
            
        }
    }
    [ClientRpc]
    public void RpcDiscardScoreCard(int index)
    {
        UIManager.instance.DiscardScorecard(index,new Vector2(Random.Range(-500f, 500f), Random.Range(-200f, 300f)), Quaternion.Euler(0, 0, Random.Range(0f, 360f)));
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
    public void RpcSetCircleNum(int num_circle)
    {
        UIManager.instance.text_CircleNum.text = num_circle.ToString();
    }
    [Client]
    public void ClientAddPlayer(int added_netId,string added_name)
    {
        //Debug.Log("ClientAddPlayer  netId = " + netId + " || instance.netId = " + instance.netId);
        Debug.Log("[Client] 加入 :" + added_netId + " " + added_name);
        CmdAddPlayer(added_netId, added_name);
    }
    [Client]
    public void ClientStartGame()
    {
        CmdStartGame();
    }
    [Client]
    public void ClientDiscardScoreCard(int index)
    {
        CmdDiscardScoreCard(index);
    }
    [Client]
    public void ClientDrawHandCards(int onlineID, int times)
    {
        CmdDrawHandCards(onlineID, times);
    }
    [Client]
    public void ClientNewTurn()
    {
        CmdNewTurn();
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
