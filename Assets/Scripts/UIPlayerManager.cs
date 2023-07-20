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
            
            if (list_netId[i] == (int)Empty.instance.netId)
            {
                playerPrefab.GetComponent<Player>().panel_You.SetActive(true);
            }
            else
            {
                playerPrefab.GetComponent<Player>().panel_You.SetActive(false);
            }

            list_player.Add(Instantiate(playerPrefab, content_Player.transform));

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
        Empty.instance.turnMove.Add(0);
    }

    public void Show_Button_Select()
    {
        Empty.instance.CmdSetState(GameManager.Temp_STATE.STATE_SELECTING_TARGETPLAYER);
        for(int i = 0; i < list_player.Count; i++)
        {
            list_player[i].GetComponent<Player>().panel_Select.SetActive(true);
            list_player[i].GetComponent<Player>().panel_Selected.SetActive(false);
            list_player[i].GetComponent<Player>().panel_UnSelected.SetActive(true);
            list_player[i].GetComponent<Player>().panel_ToSelect.SetActive(true);
            list_player[i].GetComponent<Player>().panel_ToUnSelect.SetActive(false);
        }
        UIManager.instance.button_Confirm_Selection.gameObject.SetActive(true);
        UIManager.instance.button_GiveUp_Selection.gameObject.SetActive(true);
        
    }

    public void Hide_Button_Select()
    {
        for (int i = 0; i < list_player.Count; i++)
        {
            list_player[i].GetComponent<Player>().panel_Select.SetActive(false);
        }
        UIManager.instance.button_Confirm_Selection.gameObject.SetActive(false);
        UIManager.instance.button_GiveUp_Selection.gameObject.SetActive(false);
    }

    public void ShowOrHide_OtherItems(bool state, int index_Card)
    {
        if (state)
        {
            switch (index_Card)
            {
                case 1005:
                    for (int i = 0; i < Empty.list_netId.Count; i++)
                    {
                        list_player[i].GetComponent<Player>().card_1005_GetLeftHand.SetActive(state);
                    }
                    break;
                case 1006:
                    for (int i = 0; i < Empty.list_netId.Count; i++)
                    {
                        list_player[i].GetComponent<Player>().card_1005_GetRightHand.SetActive(state);
                    }
                    break;
                default:
                    break;
            }
        }
        else
        {
            for (int i = 0; i < Empty.list_netId.Count; i++)
            {
                list_player[i].GetComponent<Player>().card_1005_GetLeftHand.SetActive(state);
            }
            for (int i = 0; i < Empty.list_netId.Count; i++)
            {
                list_player[i].GetComponent<Player>().card_1005_GetRightHand.SetActive(state);
            }
        }
    }

    public void AddStateCard(int id_attacker, List<int> list_index_offender, int index_Card)
    {
        int my_index = Empty.instance.GetIndex_in_list_netId(id_attacker);
        list_player[my_index].GetComponent<Player>().AddStateCard(id_attacker,list_index_offender, index_Card);
    }
    public void Show_Notice_State(GameObject temp)
    {
        int my_index = Empty.instance.GetIndex_in_list_netId(temp.GetComponent<StateCard>().id_attacker);
        list_player[my_index].GetComponent<Player>().Text_notice_State.text = temp.GetComponent<StateCard>().name_Card + "\n" + temp.GetComponent<StateCard>().notice_State;
        list_player[my_index].GetComponent<Player>().panel_Notice_StateCard.SetActive(true);
    }
    public void Card_1002_ClearAllSuspectedCard()
    {
        for (int i = 0; i < Empty.list_netId.Count; i++)
        {
            list_player[i].GetComponent<Player>().Clear_toggle_isOns();
        }
    }
    public void Card_1005_GetLeftSuspectedScore(List <int> list_index_offender)
    {
        if (list_index_offender.Count == 0 || list_index_offender.Count == 1) return;

        bool[] temp_end = new bool[3];
        bool[] last_isOn = new bool[3];
        int end_index = Empty.list_netId.Count - 1, temp_first_index = 0;
        while (!list_index_offender.Contains(temp_first_index))
        {
            temp_first_index++;
            if (temp_first_index > 10) return;
        }
        while (!list_index_offender.Contains(end_index))
        {
            end_index--;
            if (end_index < 0) return;
        }
        for (int i=0;i< list_player[end_index].GetComponent<Player>().list_toggle_score.Count; i++)
        {
            temp_end[i] = list_player[end_index].GetComponent<Player>().list_toggle_score[i].isOn;
        }
        
        for (int my_index = end_index; my_index >= 0; my_index = end_index)
        {
            end_index = my_index - 1;
            while (!list_index_offender.Contains(end_index))
            {
                end_index--;
                if (end_index  < 0) return;
            }

            Debug.Log("last index = " + end_index);
            for (int i=0;i< list_player[end_index].GetComponent<Player>().list_toggle_score.Count; i++)
            {
                last_isOn[i] = list_player[end_index].GetComponent<Player>().list_toggle_score[i].isOn;
            }
            list_player[my_index].GetComponent<Player>().Refresh_list_toggle_score(last_isOn);

            if (end_index == temp_first_index)
            {
                list_player[temp_first_index].GetComponent<Player>().Refresh_list_toggle_score(temp_end);
                break;
            }

        }
    }
    public void Card_1006_GetRightSuspectedScore(List<int> list_index_offender)
    {
        if (list_index_offender.Count == 0 || list_index_offender.Count == 1) return;

        bool[] temp_first = new bool[3];
        bool[] next_isOn = new bool[3];
        int first_index = 0, temp_last_index = Empty.list_netId.Count - 1;
        while (!list_index_offender.Contains(temp_last_index))
        {
            temp_last_index--;
            if (temp_last_index < 0) return;
        }
        while (!list_index_offender.Contains(first_index))
        {
            first_index++;
            if (first_index > 10) return;
        }
        for (int i = 0; i < list_player[first_index].GetComponent<Player>().list_toggle_score.Count; i++)
        {
            temp_first[i] = list_player[first_index].GetComponent<Player>().list_toggle_score[i].isOn;
        }
        for (int my_index = first_index; my_index < Empty.list_netId.Count - 1; my_index = first_index)
        {
            first_index = my_index + 1;
            while (!list_index_offender.Contains(first_index))
            {
                first_index++;
                if (first_index > 10) return;
            }

            for (int i = 0; i < list_player[first_index].GetComponent<Player>().list_toggle_score.Count; i++)
            {
                next_isOn[i] = list_player[first_index].GetComponent<Player>().list_toggle_score[i].isOn;
            }
            list_player[my_index].GetComponent<Player>().Refresh_list_toggle_score(next_isOn);

            if (first_index == temp_last_index)
            {
                list_player[temp_last_index].GetComponent<Player>().Refresh_list_toggle_score(temp_first);
                break;
            }

        }
    }

    public void Card_1008_Collect(int index_holder)
    {
       GameObject state = list_player[index_holder].GetComponent<Player>().content_State;
        for(int i=0;i<state.transform.childCount;i++)
        {
            if(state.transform.GetChild(i).gameObject.activeSelf)
            {
                if (state.transform.GetChild(i).gameObject.GetComponent<StateCard>().index_Card == 1008)
                {
                    StateCard added_Statecard = state.transform.GetChild(i).gameObject.GetComponent<StateCard>();
                    Empty.instance.CmdCard_1008_AddBeforeRealize(added_Statecard.index_Card,  added_Statecard.id_attacker, added_Statecard.list_index_offender);
                    //Empty.instance.list_stateCards.Add(added_Statecard);
                }
            }
        }
        index_holder++;
        if(index_holder == Empty.list_netId.Count)
        {
            index_holder = 0;
        }
        if(index_holder == Empty.index_CurrentHolder - 1)
        {
            Empty.instance.CmdCard_1008_Realize(index_holder);
            return;
        }
        Card_1008_Collect(index_holder);
    }
    
        
}
