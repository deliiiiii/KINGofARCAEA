using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public static int index_SelectedHandCard;
    public static int index_Card_In_Hand;
    public GameObject panel_DiscardedCards;
    public Text text_CircleNum;
    public Button button_YieldCard;//打出按钮
    public Button button_FinishYieldCard;//结束出牌按钮
    public Button button_ThrowCard;//结束出牌按钮
    private void Awake()
    {
        instance = this;
        button_YieldCard.onClick.AddListener(UIYieldCard);
        button_FinishYieldCard.onClick.AddListener(UIFinishYieldCard);
        button_ThrowCard.onClick.AddListener(UIThrowCard);
    }
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(GameManager.state_);
        switch (GameManager.state_)
        {

            case GameManager.STATE.STATE_YIELD_CARDS:
                {
                    //button_YieldCard.gameObject.SetActive(true);
                    button_FinishYieldCard.gameObject.SetActive(true);
                    button_ThrowCard.gameObject.SetActive(false);
                    break;
                }
            case GameManager.STATE.STATE_THROW_CARDS:
                {
                    //button_YieldCard.gameObject.SetActive(false);
                    button_FinishYieldCard.gameObject.SetActive(false);
                    button_ThrowCard.gameObject.SetActive(true);
                    //Debug.Log("THROW CARDS !!!");
                    break;
                }
            default:
                {
                    //button_YieldCard.gameObject.SetActive(false);
                    button_FinishYieldCard.gameObject.SetActive(false);
                    button_ThrowCard.gameObject.SetActive(false);
                    break;
                }
        }
    }
    public void UIYieldCard()
    {
        PlayerManager.list_player[PlayerManager.index_CurrentPlayer - 1].GetComponent<Player>().YieldCard(PlayerManager.index_CurrentPlayer - 1);
    }
    public void UIFinishYieldCard()
    {
        GameManager.state_ = GameManager.STATE.STATE_THROW_CARDS;
        PlayerManager.list_player[PlayerManager.index_CurrentPlayer - 1].ThrowCard_Judge(PlayerManager.index_CurrentPlayer - 1);
    }
    public void UIThrowCard()
    {
        PlayerManager.list_player[PlayerManager.index_CurrentPlayer - 1].GetComponent<Player>().ThrowCard(PlayerManager.index_CurrentPlayer - 1);
    }
    public void DiscardCard(GameObject card)//将牌放在弃牌区
    {
        card = Instantiate(card, panel_DiscardedCards.transform);
        card.GetComponent<Button>().interactable = false;
        card.transform.localPosition = new Vector2(Random.Range(-500f,500f),Random.Range(-200f,300f));
        card.transform.localRotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
           
    }
    
}
