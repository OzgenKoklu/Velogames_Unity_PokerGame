using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using pheval;

public class Testing : MonoBehaviour
{
    public string hand1;
    public string hand2;
    public string hand3;
    public string hand4;

    void Start()
    {
        hand1 = "2c3c4c5c6c4s8h";
        hand2 = "7c3c4c5c6c4s8h";
        hand3 = "9c3s4s5ctc4s8h";
        hand4 = "9c3c4c5ctc4c8h";

        var rank1 = pheval.Eval.Eval7String(hand1);
        var rank2 = pheval.Eval.Eval7String(hand2);
        var rank3 = pheval.Eval.Eval7String(hand3);
        var rank4 = pheval.Eval.Eval7String(hand4);

        Debug.Log("rank1: " + rank1 + " rank2: " + rank2 + " rank3: " + rank3);


        var category1 = pheval.Rank.GetCategory(rank1);
        var category2 = pheval.Rank.GetCategory(rank2);
        var category3 = pheval.Rank.GetCategory(rank3);
        var category4 = pheval.Rank.GetCategory(rank4);


        Debug.Log("1.el: " + category1 + " 2.el: " + category2 + " 3.el: " + category3 + " 4.el: " + category4);

        Debug.Log(pheval.Rank.DescribeRankShort(rank1));
        Debug.Log(pheval.Rank.DescribeRankShort(rank2));
        Debug.Log(pheval.Rank.DescribeRankShort(rank3));
        Debug.Log(pheval.Rank.DescribeRankShort(rank4));


    }
}
