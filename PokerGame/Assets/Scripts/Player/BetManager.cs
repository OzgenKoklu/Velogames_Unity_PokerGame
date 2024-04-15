using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetManager : MonoBehaviour
{
    public static BetManager Instance { get; private set; }

    public int CurrentHighestBetAmount
    {
        get => _currentHighestBetAmount;
        set => _currentHighestBetAmount = value;
    }
    private int _currentHighestBetAmount;

    private GameManager.GameState _currentState;

    private void Awake()
    {
        Instance = this;
    }

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
            _currentHighestBetAmount = DealerManager.Instance.GetBigBlind().BetAmount;
        }
    }

    public void SetBet(PlayerManager player, int betAmount)
    {
        player.BetAmount += betAmount;
        //Debug.Log(player.name + " bet " + betAmount);
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
        DealerManager.Instance.OnDealerChanged -= DealerManager_OnDealerChanged;
    }
}
