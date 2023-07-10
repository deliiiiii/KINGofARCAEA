using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public GameObject canvas;

    public float delay = 0.2f;
    public static int index_SelectedHandCard;
    public static int index_Card_In_Hand;
    public GameObject panel_DiscardedCards;
    public Text text_NoticeThrowCard;
    public Text text_CircleNum;
    public Button button_Start_Game;//开始游戏
    public Button button_YieldCard;//打出按钮
    public Button button_FinishYieldCard;//结束出牌按钮
    public Button button_ThrowCard;//结束出牌按钮
    private void Awake()
    {
        instance = this;
        button_Start_Game.onClick.AddListener(UIStartGame);
        button_YieldCard.onClick.AddListener(UIYieldCard);
        button_FinishYieldCard.onClick.AddListener(UIFinishYieldCard);
        button_ThrowCard.onClick.AddListener(UIThrowCard);
    }

    // Update is called once per frame
    void Update()
    {
        if(canvas.activeSelf)
        {
            switch (GameManager.state_)
            {

                case GameManager.STATE.STATE_YIELD_CARDS:
                    {
                        //button_YieldCard.gameObject.SetActive(true);
                        button_FinishYieldCard.gameObject.SetActive(true);
                        button_ThrowCard.gameObject.SetActive(false);
                        text_NoticeThrowCard.gameObject.SetActive(false);
                        break;
                    }
                case GameManager.STATE.STATE_THROW_CARDS:
                    {
                        //button_YieldCard.gameObject.SetActive(false);
                        button_FinishYieldCard.gameObject.SetActive(false);
                        button_ThrowCard.gameObject.SetActive(true);
                        text_NoticeThrowCard.gameObject.SetActive(true);
                        //Debug.Log("THROW CARDS !!!");
                        break;
                    }
                default:
                    {
                        //button_YieldCard.gameObject.SetActive(false);
                        button_FinishYieldCard.gameObject.SetActive(false);
                        button_ThrowCard.gameObject.SetActive(false);
                        text_NoticeThrowCard.gameObject.SetActive(false);
                        break;
                    }
            }
        }
        
        
    }
    public void UIStartGame()
    {
        Empty.instance.ClientStartGame();
        button_Start_Game.gameObject.SetActive(false);
    }
    public void UIYieldCard()
    {
        UIPlayerManager.list_player[UIPlayerManager.index_CurrentPlayer - 1].GetComponent<Player>().YieldCard(UIPlayerManager.index_CurrentPlayer - 1);
    }
    public void UIFinishYieldCard()
    {
        GameManager.state_ = GameManager.STATE.STATE_THROW_CARDS;
        UIPlayerManager.list_player[UIPlayerManager.index_CurrentPlayer - 1].GetComponent<Player>().ThrowCard_Judge(UIPlayerManager.index_CurrentPlayer - 1);
    }
    public void UIThrowCard()
    {
        UIPlayerManager.list_player[UIPlayerManager.index_CurrentPlayer - 1].GetComponent<Player>().ThrowCard(UIPlayerManager.index_CurrentPlayer - 1);
    }
    public void CallClient_UIDiscardCard(GameObject card)
    {
        Empty.instance.ClientDiscardCard(card);
    }
    public void DiscardCard(GameObject card,Vector2 v, Quaternion q)//将牌放在弃牌区
    {
        Empty.instance.ClientDiscardCard(card);
        card = Instantiate(card, panel_DiscardedCards.transform);
        card.GetComponent<Button>().interactable = false;
        card.transform.localPosition = v;
        card.transform.localRotation = q;
    }
    public void Delay_ShowStartGame()
    {
        if(Empty.list_netId.Count == 0)
        {
            Debug.Log("Delay_ShowStartGame()");
            Invoke(nameof(Delay_ShowStartGame), delay);
            return;
        }
        if (Empty.list_netId[0] == (int)Empty.instance.netId)
        {
            button_Start_Game.gameObject.SetActive(true);
        }
        else
        {
            button_Start_Game.gameObject.SetActive(false);
        }
    }
}
