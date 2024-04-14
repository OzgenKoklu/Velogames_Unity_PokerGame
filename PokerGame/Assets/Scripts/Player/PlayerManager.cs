using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public string PlayerName
    {
        get => _playerName;
        set => _playerName = value;
    }
    [SerializeField] private string _playerName;

    public TurnManager.PlayerAction PlayerAction
    {
        get => _playerAction;
        set => _playerAction = value;
    }
    [SerializeField] private TurnManager.PlayerAction _playerAction;

    public bool IsPlayerTurn
    {
        get { return _isPlayerTurn; }
        set
        {
            _isPlayerTurn = value;

            if (value == true)
            {
                _timebar.gameObject.SetActive(true);
            }
            else
            {
                _timebar.gameObject.SetActive(false);
            }
        }
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

    [SerializeField] private Slider _timebar;

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

    public Slider GetTimerSlider()
    {
        return _timebar;
    }
}
