using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetManager : MonoBehaviour
{
    public static void SetBet(PlayerManager player, int betAmount)
    {
        if (GameManager.Instance.GetState() == GameManager.GameState.PreFlop)
        {
            // Set the bet amount for the Big Blind and Small Blind
            player.BetAmount = betAmount;
            Debug.Log(player.name + " bet " + betAmount);
        }
    }
}
