﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerAction { Fold, Check, Bet, Raise, Call }

public class PlayerManager : MonoBehaviour
{
    public float maxThinkTime = 10f; // Maximum time for make a move

    public string PlayerName
    {
        get => _playerName;
        set => _playerName = value;
    }
    [SerializeField] private string _playerName;

    public PlayerAction PlayersAction
    {
        get => _playerAction;
        set => _playerAction = value;
    }
    [SerializeField] private PlayerAction _playerAction;

    public PokerPlayerHand PlayerHand
    {
        get => _playerHand;
    }
    [SerializeField] private PokerPlayerHand _playerHand;

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

    public bool IsPlayerActive //folded or not
    {
        get => _isPlayerActive;
        set
        {
            _isPlayerActive = value;

            if (value == false)
            {
                _passivePlayerTint.gameObject.SetActive(true);
            }
            else
            {
                _passivePlayerTint.gameObject.SetActive(false);
            }
        }
    }
    [SerializeField] private bool _isPlayerActive;


    [SerializeField] private GameObject _passivePlayerTint;

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
        
        if(player == this && !IsPlayerActive)
        {
            //change turn imideatly if player folded
            TurnManager.Instance.ChangePlayerTurn();
        }

        if (player == this && player == GameManager.Instance.MainPlayer)
        {
            //if player is main player, set on UI objects for player input.

            if(player.BetAmount < BetManager.Instance.CurrentHighestBetAmount)
            {
                //player can  Fold, Call, Raise

                //can be used in the delegates / button text etc like Call (15$)
                var callBetAmount = BetManager.Instance.CurrentHighestBetAmount - player.BetAmount;
                //tuşa delegate olarak gönderilebilir. BetManager.Instance.SetBet(player, callBetAmount);


            }
            else
            {
                //player can Fold, Check, Bet
            }

            UiManager.Instance.SetActionButtonsForPlayer();

            StartCoroutine(TenSecondTimerForMainPlayer());

            
            //Burada previous Action'ı bizim playerın actionına eşitlememiz lazım
            // _previousPlayerAction = CurrentPlayer.PlayerAction;
            //check if player is last in this turn or not, if last, change state to flop, else, change player turn
        }
        else if (player == this)
        {
            StartCoroutine(AiBotMoveWithRandomWait());
        }
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

    public void FoldAction()
    {
        if(this == GameManager.Instance.MainPlayer)
        {
            PlayersAction = PlayerAction.Fold;
            Debug.Log("Player has made the move to: " + PlayersAction);
        }
    }

    public void CallAction()
    {
        if (this == GameManager.Instance.MainPlayer)
        {
            PlayersAction = PlayerAction.Call;
            Debug.Log("Player has made the move to: " + PlayersAction);
        }
    }

    public void BetAction()
    {
        if (this == GameManager.Instance.MainPlayer)
        {
            PlayersAction = PlayerAction.Bet;
            Debug.Log("Player has made the move to: " + PlayersAction);
        }
    }

    public void CheckAction()
    {
        if (this == GameManager.Instance.MainPlayer)
        {
            PlayersAction = PlayerAction.Check;
            Debug.Log("Player has made the move to: " + PlayersAction);
        }
    }

    public void RaiseAction()
    {
        if (this == GameManager.Instance.MainPlayer)
        {
            PlayersAction = PlayerAction.Raise;
            Debug.Log("Player has made the move to: " + PlayersAction);
        }
    }

    IEnumerator TenSecondTimerForMainPlayer()
    {
        float startTime = Time.time;

        // Initialize the timebar
        _timebar.maxValue = maxThinkTime;
        _timebar.value = maxThinkTime;

        while (Time.time - startTime < maxThinkTime)
        {
            // Update the timebar
            _timebar.value = maxThinkTime - (Time.time - startTime);
            yield return null; // Wait until next frame
        }

        // if (main player didn't make a move)
        //      if (can check):  => check
        //      else:            => fold
        UiManager.Instance.ResetFunctionsAndHideButtons();
        TurnManager.Instance.ChangePlayerTurn();
    }

    IEnumerator AiBotMoveWithRandomWait()
    {
        // Generate a random wait time between 0 to 10 seconds
        float waitTime = UnityEngine.Random.Range(2f, maxThinkTime);
        float startTime = Time.time;

        // Initialize the timebar
        _timebar.maxValue = maxThinkTime;
        _timebar.value = maxThinkTime;

        while (Time.time - startTime < waitTime)
        {
            // Update the timebar
            _timebar.value = maxThinkTime - (Time.time - startTime);
            yield return null; // Wait until next frame
        }

        // AI bot makes a move after the random wait time
        ExecuteAIMove();
    }

    private void ExecuteAIMove()
    {
        PlayersAction = PlayerHand.AiBotActionPreFlop();
        // Reset the previous player action to fold or null after each betting round ends
        Debug.Log(name + " Made the move: " + PlayersAction);

        if(PlayersAction == PlayerAction.Fold)
        {
            IsPlayerActive = false;
        }

        TurnManager.Instance.ChangePlayerTurn();
    }
}
