using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public static List<Player> list_player = new List<Player>();
    public static int index_CurrentPlayer = 0;
    public static int index_CurrentHolder = 0;
    public static int playerCount = 0;
    
    // Start is called before the first frame update
    void Awake()
    {
        for(int i=0;i < gameObject.transform.childCount; i++)
        {
            list_player.Add(gameObject.transform.GetChild(i).gameObject.GetComponent<Player>());
        }
        playerCount= list_player.Count;
    }

    private void Start()
    {

    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
