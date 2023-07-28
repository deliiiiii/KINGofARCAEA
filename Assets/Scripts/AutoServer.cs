using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoServer : MonoBehaviour
{
    public MyNetworkManager manager;
    private void Awake()
    {
        manager = GetComponent<MyNetworkManager>();
    }
    void Start()
    {
        //DelayAutoStartServer();
        Test_2();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Test_2()
    {
        if (!manager || !NetworkServer.active)
        {
            Invoke(nameof(Test_2), 1f);
            return;
        }
        //manager.networkAddress = "localhost";
        manager.networkAddress = "123.60.91.26";
        manager.StartClient();
    }
    public void DelayAutoStartServer()
    {
        if (!manager)
        {
            Debug.Log("DelayAutoStartServer");
            Invoke(nameof(DelayAutoStartServer), 0.2f);
            return;
        }

        //manager.networkAddress = "123.60.91.26";
        manager.networkAddress = "localhost";
        manager.StopServer();
        //manager.StartHost();
        //manager.StartServer();
        DelayStartHost();
    }
    public void DelayStartHost()
    {
        if (NetworkServer.active)
        {
            Debug.Log("DelayStartHost");
            Invoke(nameof(DelayStartHost), 0.3f);
            return;
        }
        manager.StartHost();
    }
}
