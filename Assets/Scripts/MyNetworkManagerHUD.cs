using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MyNetworkManagerHUD : MonoBehaviour
{

    public MyNetworkManager manager;
    float delay = 0.2f;
    //float time_CheckState = 0.2f;
    //float timer_CheckState = 0f;
    //public bool stop_ALL_delay = false;
    public bool editedName = false;
    List<GameObject> gameObjects;

    public GameObject canvas;

    public GameObject inputfield_IP;
    public Text input_IP;
    public GameObject text_connecting;
    public Button start_Host;
    public Button start_Client;
    public Button stop_Host;
    public Button stop_Client;
    public Button cancel_connection_attempt;
    public GameObject inputfield_PlayerName;
    public Text input_PlayerName;
    //public Button hereWeGo;
    void Awake()
    {
        manager = GetComponent<MyNetworkManager>();
        gameObjects = new List<GameObject>()
        {
            inputfield_IP,
            text_connecting,
            start_Host.gameObject,
            start_Client.gameObject,
            stop_Host.gameObject,
            stop_Client.gameObject,
            cancel_connection_attempt.gameObject,
            inputfield_PlayerName,
            //hereWeGo.gameObject
        };

        
        start_Host.onClick.AddListener(MyStartHost);
        start_Host.onClick.AddListener(CheckState);
        start_Client.onClick.AddListener(MyStartClient);
        start_Client.onClick.AddListener(CheckState);
        stop_Host.onClick.AddListener(MyStopHost);
        stop_Host.onClick.AddListener(CheckState);
        stop_Client.onClick.AddListener(MyStopClient);
        stop_Client.onClick.AddListener(CheckState);
        cancel_connection_attempt.onClick.AddListener(MyStopClient);
        cancel_connection_attempt.onClick.AddListener(CheckState);
        //hereWeGo.onClick.AddListener(HereWeGo);
        CheckState();
    }
    void Update()
    {
        
    }

    void CheckState()
    {
        if (!NetworkClient.isConnected && !NetworkServer.active)
        {
            ShowStartButtons();
        }
        else
        {
            ShowStatusLabels();
        }
        ShowStopButtons();
    }
    void ClearAll()
    {
        
        for(int i=0;i<gameObjects.Count;i++)
        {
            if (!gameObjects[i]) continue;
            gameObjects[i].SetActive(false);
        }
    }

    void ShowStartButtons()
    {
        ClearAll();
        if (!NetworkClient.active)
        {
            start_Host.gameObject.SetActive(true);
            start_Client.gameObject.SetActive(true);
            inputfield_IP.SetActive(true);
            inputfield_PlayerName.SetActive(true);
        }
        else
        {
            text_connecting.SetActive(true);
            cancel_connection_attempt.gameObject.SetActive(true);
        }
    }
    void ShowStatusLabels()
    {
        ClearAll();
    }
    void ShowStopButtons()
    {
        
        // stop host if host mode
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            ClearAll();
            stop_Host.gameObject.SetActive(true);
            //stop_Client.gameObject.SetActive(true);
        }
        // stop client if client-only
        else if (NetworkClient.isConnected)
        {
            ClearAll();
            stop_Client.gameObject.SetActive(true);
        }
        else if (NetworkServer.active)
        {
            ClearAll();
            stop_Host.gameObject.SetActive(true);
        }
    }
    void MyStartHost()
    {
        if (!NetworkClient.active && (Application.platform != RuntimePlatform.WebGLPlayer))
        {
            manager.networkAddress = input_IP.text;
            if (manager.networkAddress.Length == 0)
            {
                manager.networkAddress = "localhost";
                //return;
            }

            manager.StartHost();
        }
    }
    void MyStartClient()
    {
        if (!NetworkClient.active)
        {
            manager.networkAddress = input_IP.text;
            
            if (manager.networkAddress.Length == 0)
            {
                manager.networkAddress = "localhost";
                //return;
            }
            if (input_PlayerName.text.Length == 0)
            {
                Debug.Log("¿ÕÐÕÃû!");
                return;
            }
            
            manager.StartClient();
            Delay_HereWeGo();
            
        }
    }
    void MyStopClient()
    {
        if(NetworkClient.active || (NetworkServer.active && NetworkClient.isConnected))
        {
            manager.StopClient();
            canvas.SetActive(false);
        }
    }
    void MyStopHost()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            manager.StopHost();
        }
    }
    //void Delay_EditName()
    //{
    //    if (stop_ALL_delay)
    //    {
    //        stop_ALL_delay = false;
    //        return;
    //    }
    //    if (!Empty.instance)
    //    {
    //        //Debug.Log("Delay_EditName()");
    //        Invoke(nameof(Delay_EditName), delay);
    //        return; 
    //    }
        
    //    Debug.Log("HUD ID = " + Empty.instance.netId);
    //    EditName();
    //}
    //void EditName()
    //{
    //    if(Empty.instance.isServer) { return; }
    //    ClearAll();
    //    inputfield_PlayerName.SetActive(true);
    //    stop_Client.gameObject.SetActive(true);
    //    hereWeGo.gameObject.SetActive(true);
    //}

    void Delay_HereWeGo()
    {
        if(!NetworkClient.active)
        {
            //Debug.Log("Delay_HereWeGo()");
            Invoke(nameof(Delay_HereWeGo), delay);
            return;
        }

        Debug.Log("AddName()");
        Delay_AddPlayer();
        
    }
    void Delay_AddPlayer()
    {
        if(!Empty.instance)
        {
            //Debug.Log("Delay_AddPlayer()");
            Invoke(nameof(Delay_AddPlayer), delay);
            return;
        }
        Debug.Log("ClientAddPlayer()");
        Empty.instance.ClientAddPlayer((int)Empty.instance.netId, input_PlayerName.text);
        ClearAll();
        canvas.SetActive(true);
        UIManager.instance.Delay_ShowStartGame();
        stop_Client.gameObject.SetActive(true);
    }
}
