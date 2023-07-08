using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Telepathy;
using Unity.Collections.LowLevel.Unsafe;
using System.Linq;

public class Empty : NetworkBehaviour
{
    public static Empty instance;
    //[SyncVar(hook = nameof(OnChange_count_player))]
    public int count_player = 0;
    //[SyncVar(hook = nameof(OnChange_list_netId))]
    public List<int> list_netId = new();
    public float delay = 0.2f;
    //public int index_instance;
    //public float delay = 0.033f;
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
        }
        if(!instance)
        {
            Invoke(nameof(Delay_set_instance), delay);
        }
    }
    [Server]
    public void ServerAddPlayer(int netId)
    {
        if (netId == 1) return;
        //Debug.Log("[Server] ServerAddPlayer()");
        //count_player++;
        list_netId.Add(netId);
        Debug.Log("[Server] list_netId = " + GetContent(list_netId));
        RpcClearPlayer();
        for(int i=0;i<list_netId.Count;i++)
        {
            RpcAddPlayer(list_netId[i]);
        }
       
    }
    [Server]
    public void ServerRomovePlayer(int netId)
    {

        //Debug.Log("[Server] ServerRomovePlayer()");
        //count_player--;
        list_netId.Remove(netId);
        Debug.Log("[Server] list_netId = " + GetContent(list_netId));
        //Debug.Log("isServer = " + isServer + " ID = " + netId + " instance.ID = " + instance.netId);
        
        RpcClearPlayer();

        for (int i = 0; i < list_netId.Count; i++)
        {
            RpcAddPlayer(list_netId[i]);
        }
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
    }
    [ClientRpc]
    public void RpcAddPlayer(int netId)
    {
        //Debug.Log("[Client] ServerAddPlayer()");
        list_netId.Add(netId);
        Debug.Log("[Client] list_netId = " + GetContent(list_netId));
    }
    //[Client]
    //public void OnChange_list_netId(List<int> oldValue, List<int> newValue)
    //{
    //    //Debug.Log("[Client] list_netId.Count = " + list_netId.Count);
    //    //Debug.Log("[Client] list_netId = " + GetContent(newValue));
    //}

    public string GetContent(List<int> list)
    {
        string a="";
        for(int i=0;i< list.Count;i++)
        {
            a += list[i].ToString();
            a += " ";
        }
        return a;
    }















    /*
    [Command]
    public void CmdAddPlayer(int netId)
    {
        Debug.Log("[Server] CmdAddPlayer()");
        ServerAddPlayer(netId);
    }
    [Command]//↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓
    public void CmdRemovePlayer(int netId)
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
        ServerRemovePlayer(netId);
    }
    [Server]
    public void ServerAddPlayer(int netId)
    {
        Debug.Log("[Server] ServerAddPlayer()");
        PlayerManager.list_netId.Add(netId);
        PlayerManager.instance.RefreshPlayer();
        //Debug.Log("isClient = " + isClient);
        //Debug.Log("isServer = " + isServer);
        RpcAddPlayer(PlayerManager.list_netId);
    }
    [Server]
    public void ServerRemovePlayer(int netId)
    {
        Debug.Log("[Server] ServerRemovePlayer()");
        int i = 0;
        for (; i < PlayerManager.list_netId.Count; i++)
        {
            if (PlayerManager.list_netId[i] == netId) break;
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
