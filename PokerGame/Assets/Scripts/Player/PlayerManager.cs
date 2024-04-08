using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public bool IsPlayerTurn
    {
        get => _isPlayerTurn;
        set => _isPlayerTurn = value;
    }
    [SerializeField] private bool _isPlayerTurn;

    public bool IsPlayerDealer
    {
        get => _isPlayerDealer;
        set => _isPlayerDealer = value;
    }
    [SerializeField] private bool _isPlayerDealer;

    public int BetAmount
    {
        get => _betAmount;
        set => _betAmount = value;
    }
    private int _betAmount;

    [SerializeField] private GameObject _dealerIcon;

    private void Start()
    {
        DealerManager.Instance.OnDealerChanged += OnDealerChanged;
        TurnManager.Instance.OnPlayerTurn += TurnManager_OnPlayerTurn;
        OnDealerChanged(this);
    }

    private void TurnManager_OnPlayerTurn(PlayerManager player)
    {
        if (player == this)
        {
            StartTurn();
        }
    }

    public void StartTurn()
    {
        TimerManager.Instance.StartTimer();
        Debug.Log("My turn: " + gameObject.name);
        // Enable input for this player
        // Implement logic for their actions (bet, fold, check, etc.)
    }

    private void OnDealerChanged(PlayerManager playerManager)
    {
        if (_isPlayerDealer && playerManager == this)
        {
            _dealerIcon.SetActive(true);
        }
        else
        {
            _isPlayerDealer = false;
            _dealerIcon.SetActive(false);
        }
    }
}
