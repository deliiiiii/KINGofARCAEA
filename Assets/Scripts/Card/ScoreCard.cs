using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCard : GrandCard
{
    public int score;
    public ScoreCard(int index, int score, int count)
    {
        base.index_Card = index;
        this.score = score;
        this.grossCount = count;
    }
}
