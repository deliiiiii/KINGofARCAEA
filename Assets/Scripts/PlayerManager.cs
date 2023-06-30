using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    public static List<Player> list_player_info = new List<Player>();
    public static List<Player> list_player = new List<Player>();
    public Player playerPrefab;

    public static int index_CurrentPlayer = 0;
    public static int index_CurrentHolder = 0;

    public GameObject content_Player;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        list_player_info.Clear();
        list_player_info.Add(new Player(1, "Andy"));
        list_player_info.Add(new Player(2, "Bob"));
        list_player_info.Add(new Player(3, "F**k"));
    }

    private void Start()
    {
        
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize()
    {
        index_CurrentHolder = index_CurrentPlayer = 0;
        list_player.Clear();
        for(int i=1;i<content_Player.transform.childCount;i++)
        {
            Destroy(content_Player.transform.GetChild(i).gameObject);
        }
        for (int i = 0 ; i < GameManager.instance.count_Player ; i++)
        {
            list_player.Add(playerPrefab);
            list_player[i].index_Player = i + 1;
            list_player[i].text_Index_Player.text = list_player_info[i].index_Player.ToString();
            list_player[i].text_Name_Player.text = list_player_info[i].name_Player.ToString();
            list_player[i].roundScore.Clear();
            for (int j = 0; j < GameManager.instance.count_Player; j++)
            {
                list_player[i].roundScore.Add(0);
            }
            list_player[i] = Instantiate(list_player[i],content_Player.transform);
        }
    }
    public void PassTurn()
    {
        HandCardManager.list_Scroll_MyHandCard[index_CurrentPlayer - 1].SetActive(false);
        list_player[index_CurrentPlayer - 1].image_MyTurn.SetActive(false);
    }
    public void MyTurn()
    {
        HandCardManager.list_Scroll_MyHandCard[index_CurrentPlayer - 1].SetActive(true);
        list_player[index_CurrentPlayer - 1].image_MyTurn.SetActive(true);
    }
}
