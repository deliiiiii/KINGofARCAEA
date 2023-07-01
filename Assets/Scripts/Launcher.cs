using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Launcher : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    public GameObject content_Player;
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("WELCOME £¡£¡£¡");
        Debug.Log(PhotonNetwork.JoinOrCreateRoom("Room",new Photon.Realtime.RoomOptions() { MaxPlayers = 4},default));
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("INSTANTIATE1");
        GameObject g = PhotonNetwork.Instantiate("Player_1", new Vector3 (0,0,0),Quaternion.identity,0);
        g.transform.parent = gameObject.transform.parent;
        Debug.Log("INSTANTIATE2");
    }
}
