using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;//调用函数和变量!
    public int count_Player;
    public int index_Round;//局数，1局 = 2圈
    public int count_Circle;//圈数
    public int init_draw_num;//初始手牌数
    

    public enum STATE
    {
        STATE_GAME_IDLING,
        STATE_GAME_STARTED,
        STATE_DRAW_CARDS,
        STATE_JUDGE_CARDS,
        STATE_YIELD_CARDS,
        STATE_THROW_CARDS
    }
    public static STATE state_ = STATE.STATE_GAME_IDLING;
    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        instance.count_Player = PlayerManager.list_player_info.Count;
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        switch (state_)
        {
            case STATE.STATE_GAME_IDLING:
                break;
            case STATE.STATE_GAME_STARTED:
                break;
            case STATE.STATE_DRAW_CARDS:
                break;
            case STATE.STATE_JUDGE_CARDS:
                break;
            case STATE.STATE_YIELD_CARDS:
                break;
            case STATE.STATE_THROW_CARDS:
                break;
            default:
                break;
        }
    }

    public void Initialize()
    {
        index_Round = 0;
        instance.init_draw_num = 4;
        PlayerManager.instance.Initialize();
        ScoreCardManager.instance.RefillScoreCards();
        HandCardManager.instance.Initialize();
        //GameCardManager.instance.Initialize();
    }

    public void StartGame()
    {
        state_ = STATE.STATE_GAME_STARTED;
        NewRound(0);//0代表第一局之前
    }
    public void NewRound(int state)
    {
        Debug.Log("1CurrentHolder = " + PlayerManager.index_CurrentHolder);
        instance.count_Circle = 0;
        UIManager.instance.text_CircleNum.text = instance.count_Circle.ToString();
        if (instance.index_Round == 0)//第一局之前
        {
            
            PlayerManager.index_CurrentHolder = 1;
            Debug.Log("2CurrentHolder = " + PlayerManager.index_CurrentHolder);
            PlayerManager.list_player[PlayerManager.index_CurrentHolder - 1].image_Holder.SetActive(true);
            //index_Round = 1;
            for (int i = 0; i < instance.count_Player; i++)
            {
                PlayerManager.list_player[i].DrawCards(4,i);
            }
        }
        else
        {
            PlayerManager.list_player[PlayerManager.index_CurrentHolder - 1].image_Holder.SetActive(false);
            PlayerManager.index_CurrentHolder++;
            Debug.Log("3CurrentHolder = " + PlayerManager.index_CurrentHolder);
            index_Round++;
            if (PlayerManager.index_CurrentHolder > instance.count_Player)
            {
                SummaryGame();
            }
            PlayerManager.list_player[PlayerManager.index_CurrentHolder - 1].image_Holder.SetActive(true);
        }
        NewTurn();

    }
    public void NewTurn()
    {
        if(index_Round == 0)
        {
            Debug.Log("0CurrentPlayer" + PlayerManager.index_CurrentPlayer);
            index_Round = 1;
            PlayerManager.index_CurrentPlayer = 1;
            Debug.Log("1CurrentPlayer" + PlayerManager.index_CurrentPlayer);
            PlayerManager.instance.MyTurn();
            return;
        }
        
        PlayerManager.instance.PassTurn();
        Debug.Log("2CurrentPlayer" + PlayerManager.index_CurrentPlayer);

        if (PlayerManager.index_CurrentPlayer > instance.count_Player)
        {
            PlayerManager.index_CurrentPlayer = 1;
            Debug.Log("3CurrentPlayer" + PlayerManager.index_CurrentPlayer);
        }

        if(PlayerManager.index_CurrentPlayer == PlayerManager.index_CurrentHolder)
        {
            instance.count_Circle++;
            UIManager.instance.text_CircleNum.text = instance.count_Circle.ToString();
        }
        
        if (instance.count_Circle == 2)
        {
            NewRound();
            return;
        }else
        {
            PlayerManager.instance.MyTurn();
        }
        
    }
    
    public void SummaryGame()
    {
        Debug.Log("GAME OVER !!!");
    }
}
