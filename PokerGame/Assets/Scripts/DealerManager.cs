using System;
using UnityEngine;
using static GameManager;

public class DealerManager : MonoBehaviour
{
    public static DealerManager Instance { get; private set; }
    public event Action<PlayerManager> OnDealerChanged;
    public event Action<PlayerManager> OnSmallBlindChanged;
    public event Action<PlayerManager> OnBigBlindChanged;

    private PlayerManager _currentDealer;
    private PlayerManager _smallBlind;
    private PlayerManager _bigBlind;
    private PlayerManager _firstPlayerAfterBigBlind;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        _currentDealer = GameManager.Instance.ActivePlayers[0];
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
            // Debug.Log("GameManager and Players list are valid");

            // Attempt to set the IsPlayerDealer property
            var currentDealerIndex = players.IndexOf(_currentDealer);
            _currentDealer.IsPlayerDealer = true;
            SetSmallAndBigBlind();
            //Debug.Log("Player set as dealer");
            OnDealerChanged?.Invoke(_currentDealer);

            currentDealerIndex++;
            // Check if the current dealer index is greater than the number of players
            if (currentDealerIndex == players.Count)
            {
                currentDealerIndex = 0;
            }
        }
        else
        {
            Debug.Log("GameManager instance is null or no players found");
        }
    }

    public PlayerManager GetDealerPlayer()
    {
        return _currentDealer;
    }

    public PlayerManager GetSmallBlind()
    {
        return _smallBlind;
    }

    public PlayerManager GetBigBlind()
    {
        return _bigBlind;
    }

    public void SetSmallAndBigBlind()
    {
        var players = GameManager.Instance.Players;
        var currentDealerIndex = players.IndexOf(_currentDealer);

        if (currentDealerIndex + 1 >= players.Count)
        {
            _smallBlind = players[0];
        }
        else
        {
            _smallBlind = players[players.IndexOf(_currentDealer) + 1];
        }

        if (currentDealerIndex + 2 >= players.Count)
        {
            _bigBlind = players[1];
            _firstPlayerAfterBigBlind = players[players.IndexOf(_currentDealer) + 1];
        }
        else
        {
            _bigBlind = players[players.IndexOf(_currentDealer) + 2];
            _firstPlayerAfterBigBlind = players[players.IndexOf(_bigBlind) + 1];
        }

        OnSmallBlindChanged?.Invoke(_smallBlind);
        OnBigBlindChanged?.Invoke(_bigBlind);
    }

    public PlayerManager GetFirstActivePlayerFromDealer()
    {
        if (_smallBlind.IsPlayerActive && !_smallBlind.IsPlayerAllIn)
        {
            return _smallBlind;
        }
        else if (_bigBlind.IsPlayerActive && !_bigBlind.IsPlayerAllIn)
        {
            return _bigBlind;
        }
        else if (_firstPlayerAfterBigBlind.IsPlayerActive && !_firstPlayerAfterBigBlind.IsPlayerAllIn)
        {
            return _firstPlayerAfterBigBlind;
        }
        else
        {
            var players = GameManager.Instance.Players;
            var firstPlayerAfterBigBlindIndex = players.IndexOf(_firstPlayerAfterBigBlind);

            if (players[firstPlayerAfterBigBlindIndex + 1].IsPlayerActive && !players[firstPlayerAfterBigBlindIndex + 1].IsPlayerAllIn)
            {
                return players[firstPlayerAfterBigBlindIndex + 1];
            }
            else if (players[firstPlayerAfterBigBlindIndex + 2].IsPlayerActive && !players[firstPlayerAfterBigBlindIndex + 2].IsPlayerAllIn)
            {
                return players[firstPlayerAfterBigBlindIndex + 2];
            }
            else
            {
                return null;
                Debug.Log("No active player!");
            }
        }
    }

    public PlayerManager GetFirstPlayerAfterBigBlind()
    {
        return _firstPlayerAfterBigBlind;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }
}
