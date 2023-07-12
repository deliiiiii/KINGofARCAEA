using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
public class UIPlayerManager : MonoBehaviour
{
    public static UIPlayerManager instance;
    //public static List<Player> list_player_info = new List<Player>();
    public static List<GameObject> list_player = new();
    public GameObject playerPrefab;
    public static int index_CurrentPlayer = 0;//从1开始数
    public static int index_CurrentHolder = 0;//从1开始数

    public GameObject content_Player;
    void Awake()
    {
        instance = this;
        //list_player_info.Clear();
        //list_player_info.Add(new Player(1, "Andy"));
        //list_player_info.Add(new Player(2, "Bob"));
        //list_player_info.Add(new Player(3, "F**k"));
    }

    //public void Initialize()
    //{
        //index_CurrentHolder = index_CurrentPlayer = 0;
        //list_player.Clear();
        //ClearChild(content_Player.transform);
        
        //for (int i = 0 ; i < GameManager.instance.count_Player ; i++)
        //{
            //list_player.Add(playerPrefab);
            //list_player[i].GetComponent<Player>().index_Player = i + 1;
            //list_player[i].GetComponent<Player>().text_Index_Player.text = list_player_info[i].index_Player.ToString();
            //list_player[i].GetComponent<Player>().text_Name_Player.text = list_player_info[i].name_Player.ToString();
            //list_player[i].GetComponent<Player>().roundScore.Clear();
            //for (int j = 0; j < GameManager.instance.count_Player; j++)
            //{
            //    list_player[i].GetComponent<Player>().roundScore.Add(0);
            //}
            //list_player[i] = Instantiate(list_player[i],content_Player.transform);
        //}
    //}
    public void ClearChild(Transform t_parent)
    {
        Transform t_child;
        for (int i = 0; i < t_parent.transform.childCount; i++)
        {
            t_child = t_parent.transform.GetChild(i);
            Destroy(t_child.gameObject);
        }
    }

    public void RefreshPlayer(List<int> list_netId,List<string> list_name)
    {
        //Transform t_parent;
        //for (int i = 0; i < content_Player.transform.childCount; i++)
        //{
        //    t_parent = content_Player.transform.GetChild(i);
        //    Destroy(t_parent.gameObject);
        //}


        for (int i = 0; i< content_Player.transform.childCount; i++)
        {
            Destroy(content_Player.transform.GetChild(i).gameObject);
        }
        list_player.Clear();
        for (int i = 0; i < list_netId.Count; i++)
        {
            playerPrefab.GetComponent<Player>().my_netID = list_netId[i];
            playerPrefab.GetComponent<Player>().text_Index_Player.text = (i+1).ToString();
            playerPrefab.GetComponent<Player>().name_Player = list_name[i];
            playerPrefab.GetComponent<Player>().text_Name_Player.text = list_name[i];
            list_player.Add (Instantiate(playerPrefab, content_Player.transform));
        }
    }


    public void PassTurn(int index)
    {
        //HandCardManager.list_Scroll_MyHandCard[index_CurrentPlayer - 1].SetActive(false);
        list_player[index].GetComponent<Player>().image_MyTurn.SetActive(false);
    }
    public void Sync_MyTurn(int index)
    {
        list_player[index].GetComponent<Player>().image_MyTurn.SetActive(true);
    }
    public void MyTurn(int index)
    {
        list_player[index].GetComponent<Player>().image_MyTurn.SetActive(true);
        ////////GameManager.state = GameManager.Temp_STATE.STATE_DRAW_CARDS;
        Empty.instance.ClientDrawHandCards((int)Empty.instance.netId,2);
        ////////GameManager.state_ = GameManager.Temp_STATE.STATE_YIELD_CARDS;
    }
}
