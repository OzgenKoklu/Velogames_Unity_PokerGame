using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealerManager : MonoBehaviour
{
    public static DealerManager Instance { get; private set; }
    public Action<PlayerManager> OnDealerChanged;
    private int _currentDealerIndex;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameManager.Instance.OnGameStarted += OnGameStarted;

        _currentDealerIndex = 0;
    }

    private void OnGameStarted()
    {
        SetDealerPlayer();
    }

    private void SetDealerPlayer()
    {
        if (GameManager.Instance != null && GameManager.Instance.Players.Count > 0)
        {
            // Check if GameManager instance and Players list are valid
            Debug.Log("GameManager and Players list are valid");

            // Attempt to set the IsPlayerDealer property
            GameManager.Instance.Players[_currentDealerIndex].IsPlayerDealer = true;
            Debug.Log("Player set as dealer");
            OnDealerChanged?.Invoke(GameManager.Instance.Players[_currentDealerIndex]);

            _currentDealerIndex++;
            // Check if the current dealer index is greater than the number of players
            if (_currentDealerIndex == GameManager.Instance.Players.Count)
            {
                _currentDealerIndex = 0;
            }
        }
        else
        {
            Debug.Log("GameManager instance is null or no players found");
        }
    }
}
