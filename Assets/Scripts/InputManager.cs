using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    /*
    public static KeyCode button_A = KeyCode.A;
    public Text text_button_A;
    public static KeyCode button_ChangebuttonA = KeyCode.B;
    public static KeyCode button_ChangePlayer = KeyCode.C;
    bool canChangebutton_A = false;
    float time = 0;
    */
    void Start()
    {
            }

    void Update()
    {
        /////////////ÐÞ¸Ä¼üÎ»
        /*
        if(Input.GetKeyDown(InputManager.button_ChangebuttonA) && !canChangebutton_A)
        {
            canChangebutton_A = true;
        }
        else if(Input.GetKeyDown(InputManager.button_ChangePlayer))
        {
            PlayerManager.index_CurrentPlayer = (PlayerManager.index_CurrentPlayer+1)% PlayerManager.list_player.Count;
        }
        if (canChangebutton_A)
        {
            time += Time.deltaTime;
            if(time > 0.08f)
            {
                System.Array values = System.Enum.GetValues(typeof(KeyCode));
                foreach (KeyCode code in values)
                {
                    if (Input.GetKeyDown(code) && code!=button_ChangebuttonA)
                    {
                        button_A = code;
                        text_button_A.text = code.ToString();
                        canChangebutton_A = false;
                        time = 0;
                    }
                }
            }
        }
        */
    }
}
