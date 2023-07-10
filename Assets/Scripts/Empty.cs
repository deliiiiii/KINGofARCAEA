using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Telepathy;
using Unity.Collections.LowLevel.Unsafe;
using System.Linq;
using System.Data.Common;

public class Empty : NetworkBehaviour
{
    //[SyncVar(hook = nameof(OnChange_count_player))]
    public int count_player = 0;
    //[SyncVar(hook = nameof(OnChange_list_netId))]
    public static List<int> list_netId = new();
    public static List<string> list_playerName = new();
    public float delay = 0.2f;
    //public int index_instance;
    //public float delay = 0.033f;

    public static Empty instance;// { get; private set; }
   
    //protected virtual void Awake()
    //{
    //    if (isLocalPlayer)
    //    {
    //        instance = this;
    //        
    //    }
            

    //}
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
    //[Server]
    //public void ServerSetName(string name)
    //{
    //    Debug.Log("[Server] 加入名 :" + name);
    //    list_playerName.Add(name);
    //    Debug.Log("[Server] list_playerName = " + GetContent_string(list_playerName));
    //    RpcClearName();
    //    for(int i=0;i<list_playerName.Count;i++)
    //    {
    //        RpcSetName(list_playerName[i]);
    //    }
    //}
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
        PlayerManager.instance.RefreshPlayer(list_netId, list_playerName);
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

        PlayerManager.instance.RefreshPlayer(list_netId, list_playerName);
        RpcRefreshPlayer();
    }
        
    
    //[Client]
    //public void OnChange_count_player(int oldValue,int newValue)
    //{
    //Debug.Log("[Client] count_player = " + newValue);
    //}
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
        PlayerManager.instance.RefreshPlayer(list_netId, list_playerName);
    }

    public bool CheckRepeatedNetId(int checked_netId)
    {
        for(int i=0;i<list_netId.Count;i++)
        {
            if (list_netId[i] == checked_netId) return true;
        }
        return false;
    }
    //[ClientRpc]
    //public void RpcClearName()
    //{
    //    list_playerName.Clear();
    //}
    //[ClientRpc]
    //void RpcSetName(string name)
    //{
    //    list_playerName.Add(name);
    //    Debug.Log("[Client] list_playerName = " + GetContent_string(list_playerName));
    //}
    //[Client]
    //public void OnChange_list_netId(List<int> oldValue, List<int> newValue)
    //{
    //    //Debug.Log("[Client] list_netId.Count = " + list_netId.Count);
    //    //Debug.Log("[Client] list_netId = " + GetContent_int(newValue));
    //}
    [Client]
    public void ClientAddPlayer(int added_netId,string added_name)
    {
        //Debug.Log("ClientAddPlayer  netId = " + netId + " || instance.netId = " + instance.netId);
        Debug.Log("[Client] 加入 :" + added_netId + " " + added_name);
        CmdAddPlayer(added_netId, added_name);
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
        PlayerManager.list_netId.Add(added_netId);
        PlayerManager.instance.RefreshPlayer();
        //Debug.Log("isClient = " + isClient);
        //Debug.Log("isServer = " + isServer);
        RpcAddPlayer(PlayerManager.list_netId);
    }
    [Server]
    public void ServerRemovePlayer(int added_netId)
    {
        Debug.Log("[Server] ServerRemovePlayer()");
        int i = 0;
        for (; i < PlayerManager.list_netId.Count; i++)
        {
            if (PlayerManager.list_netId[i] == added_netId) break;
        }
        if (i == PlayerManager.list_netId.Count)
        {
            Debug.LogError("未找到id来删除!!");
            return;
        }
        //Debug.Log("i = " + i);
        PlayerManager.list_netId.RemoveAt(i);
        PlayerManager.instance.RefreshPlayer();
        
        Debug.Log("前 RpcRemovePlayer(i)--- Empty.instance = " + Empty.instance.name);
        TEST_2();
        RpcRemovePlayer(PlayerManager.list_netId);
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
        PlayerManager.instance.ClearPlayer();
        PlayerManager.list_netId = list_netId;
        //if (!temp_netID) Debug.LogError("TNND ");
        //if(!temp_player)
        //{
        //    Debug.Log("DON'T have temp_player");
        //    Invoke(nameof(RpcAddPlayer), delay);
        //    return;
        //}
        PlayerManager.instance.RefreshPlayer();

    }


    [ClientRpc]
    public void RpcRemovePlayer(List<int> list_netId)
    {
        //if (!temp_netID) Debug.LogError("TNND ");
        Debug.Log("[Client] RpcRemovePlayer()");
        PlayerManager.list_netId = list_netId;
        //if(!temp_player)
        //{
        //    Debug.Log("DON'T have temp_player");
        //    Invoke(nameof(RpcAddPlayer), delay);
        //    return;
        //}
        PlayerManager.instance.RefreshPlayer();
        Debug.Log("list_netId.Count = " + PlayerManager.list_player.Count);


        ////MyNetworkManager.instance.My_OnServerDisconnect();
        //MyNetworkManager.instance.base_OnClientDisconnect();
        //MyNetworkManager.instance.base_OnStopClient();
        //MyNetworkManager.instance.base_OnServerDisConnect(conn);

        //MyNetworkManager.instance.NonePara_base_OnServerDisConnect();
        //////////////////////////////////////////////////
    }
    */
}
