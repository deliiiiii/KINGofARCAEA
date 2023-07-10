using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MyNetworkManager : NetworkManager
{
    
    public int temp_netId;
    public float delay = 0.2f;
    public GameObject my_player_prefab;



    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);
        temp_netId = (int)conn.identity.netId;
        //Debug.Log("[Server] ¼ÓÈë ID = " + temp_netId);
        //Delay_ServerAddPlayer();
    }

    //public void Delay_ServerAddPlayer()
    //{
    //    if(!Empty.instance)
    //    {
    //        Invoke(nameof(Delay_ServerAddPlayer), delay);
    //        return;
    //    }
    //    //Empty.instance.ServerAddPlayer(temp_netId);
    //}
    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        temp_netId = (int)conn.identity.netId;
        //Debug.Log("[Server] Àë¿ª ID = " + temp_netId);
        Delay_ServerRomovePlayer();
        base.OnServerDisconnect(conn);
    }

    public void Delay_ServerRomovePlayer()
    {
        if (!Empty.instance)
        {
            Invoke(nameof(Delay_ServerRomovePlayer), delay);
            return;
        }
        Empty.instance.ServerRomovePlayer(temp_netId);
    }
}
