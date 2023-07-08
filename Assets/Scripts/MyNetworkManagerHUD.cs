using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MyNetworkManagerHUD : MonoBehaviour
{
    

    public MyNetworkManager manager;

    public string last_input = string.Empty;

    List<GameObject> gameObjects;

    public GameObject canvas;

    public GameObject inputfield_IP;
    public Text input_IP ;
    public GameObject text_connecting;
    public Button start_Host;
    public Button start_Client;
    public Button stop_Host;
    public Button stop_Client;
    public Button cancel_connection_attempt;
   
    new void Awake()
    {
        manager = GetComponent<MyNetworkManager>();
        manager.networkAddress = last_input;
        gameObjects = new List<GameObject>()
        {
            inputfield_IP,
            text_connecting,
            start_Host.gameObject,
            start_Client.gameObject,
            stop_Host.gameObject,
            stop_Client.gameObject,
            cancel_connection_attempt.gameObject
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

        CheckState();
    }

    private void Update()
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
            stop_Client.gameObject.SetActive(true);
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
            last_input = manager.networkAddress = input_IP.text;
            if (manager.networkAddress.Length == 0)
            {
                Debug.Log("¿ÕµØÖ·!");
                return;
            }

            manager.StartHost();
        }  
    }
    void MyStartClient()
    {
        if (!NetworkClient.active)
        {
            last_input = manager.networkAddress = input_IP.text;
            if (manager.networkAddress .Length == 0)
            {
                Debug.Log("¿ÕµØÖ·!");
                return;
            }
            manager.StartClient();
        }
    }
    void MyStopClient()
    {
        if(NetworkClient.active || (NetworkServer.active && NetworkClient.isConnected))
        {
            manager.StopClient();
        }
    }
    void MyStopHost()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            manager.StopHost();
        }
    }
}
