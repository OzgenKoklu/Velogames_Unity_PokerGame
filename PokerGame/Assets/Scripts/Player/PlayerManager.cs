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

    [SerializeField] private GameObject _dealerIcon;

    private void Start()
    {
        DealerManager.Instance.OnDealerChanged += OnDealerChanged;
        OnDealerChanged(this);
    }

    public void StartTurn()
    {
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
