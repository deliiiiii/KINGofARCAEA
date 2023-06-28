using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private int totalScore = 0;
    private List<int> roundScore = new List<int>();
    private int scoreCard = 0;
    private int count_HandCard = 0;
    private int count_RoundUsedCard = 0;
    private int count_TotalUsedCard = 0;
    public Text num;
}