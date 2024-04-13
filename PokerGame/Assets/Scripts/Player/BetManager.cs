using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetManager : MonoBehaviour
{
    private GameManager.GameState _currentState;

    private void Start()
    {
        GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
        DealerManager.Instance.OnDealerChanged += DealerManager_OnDealerChanged;
    }
    private void GameManager_OnGameStateChanged(GameManager.GameState state)
    {
        _currentState = state;
    }

    private void DealerManager_OnDealerChanged(PlayerManager dealerPlayer)
    {
        if (_currentState == GameManager.GameState.NewRound)
        {
            SetBet(DealerManager.Instance.GetSmallBlind(), 5);
            SetBet(DealerManager.Instance.GetBigBlind(), 10);
            
        }
    }

    public void SetBet(PlayerManager player, int betAmount)
    {
        player.BetAmount = betAmount;
        Debug.Log(player.name + " bet " + betAmount);
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
        DealerManager.Instance.OnDealerChanged -= DealerManager_OnDealerChanged;
    }
}
