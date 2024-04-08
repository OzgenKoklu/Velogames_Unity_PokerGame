using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class DealerManager : MonoBehaviour
{
    public static DealerManager Instance { get; private set; }
    public Action<PlayerManager> OnDealerChanged;

    private int _currentDealerIndex;
    private int _smallBlindIndex;
    private int _bigBlindIndex;
    private int _firstPlayerIndexAfterBigBlind;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        _currentDealerIndex = 0;
        GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
    }

    private void GameManager_OnGameStateChanged(GameState gameState)
    {
        if (gameState == GameState.NewRound)
        {
            SetDealerPlayer();
        }
    }

    private void SetDealerPlayer()
    {
        var players = GameManager.Instance.Players;

        if (GameManager.Instance != null && players.Count > 0)
        {
            // Check if GameManager instance and Players list are valid
            Debug.Log("GameManager and Players list are valid");

            // Attempt to set the IsPlayerDealer property
            players[_currentDealerIndex].IsPlayerDealer = true;
            SetSmallAndBigBlind();
            Debug.Log("Player set as dealer");
            OnDealerChanged?.Invoke(players[_currentDealerIndex]);

            _currentDealerIndex++;
            // Check if the current dealer index is greater than the number of players
            if (_currentDealerIndex == players.Count)
            {
                _currentDealerIndex = 0;
            }
        }
        else
        {
            Debug.Log("GameManager instance is null or no players found");
        }
    }

    public PlayerManager GetDealerPlayer()
    {
        return GameManager.Instance.Players[_currentDealerIndex];
    }

    public int GetDealerPlayerIndex()
    {
        return _currentDealerIndex;
    }

    public PlayerManager GetSmallBlind()
    {
        return GameManager.Instance.Players[_smallBlindIndex];
    }

    public PlayerManager GetBigBlind()
    {
        return GameManager.Instance.Players[_bigBlindIndex];
    }

    public void SetSmallAndBigBlind()
    {
        var players = GameManager.Instance.Players;

        if (_currentDealerIndex + 1 >= players.Count)
        {
            _smallBlindIndex = 0;
        }
        else
        {
            _smallBlindIndex = _currentDealerIndex + 1;
        }

        if (_currentDealerIndex + 2 >= players.Count)
        {
            _bigBlindIndex = 1;
            _firstPlayerIndexAfterBigBlind = _bigBlindIndex + 1;
        }
        else
        {
            _bigBlindIndex = _currentDealerIndex + 2;
            _firstPlayerIndexAfterBigBlind = _bigBlindIndex + 1;
        }
    }

    public int GetFirstPlayerIndexAfterBigBlind()
    {
        return _firstPlayerIndexAfterBigBlind;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }
}
