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
            int bettingRoundCount = GameManager.Instance.GetBettingRoundCount();
            SelectDealerIndex(bettingRoundCount);
            // Check if GameManager instance and Players list are valid
            Debug.Log("GameManager and Players list are valid");

            // Attempt to set the IsPlayerDealer property
            _currentDealer.IsPlayerDealer = true;
            SetSmallAndBigBlind();
            //Debug.Log("Player set as dealer");
            OnDealerChanged?.Invoke(_currentDealer);

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
        var BlindIndex = currentDealerIndex + 1;
        BlindIndex = BlindIndex % 5; // guarantees it wont be bigger than player count (0,1,2,3,4)

        //!!! write some conditions to support less than 3 active players. !!!

        do
        {
            _smallBlind = GameManager.Instance.Players[BlindIndex];
            BlindIndex++; //will be used to find big blind so incrementing it is ok.
            BlindIndex = BlindIndex % 5;
        } while (!_smallBlind.IsPlayerActive);

        do
        {
            _bigBlind = GameManager.Instance.Players[BlindIndex];
            BlindIndex++; //will be used to find firs player after big blind so incrementing is ok.
            BlindIndex = BlindIndex % 5;
        } while (!_bigBlind.IsPlayerActive);

        do
        {
            _firstPlayerAfterBigBlind = GameManager.Instance.Players[BlindIndex];
            BlindIndex++; //will be used to find firs player after big blind so incrementing is ok.
            BlindIndex = BlindIndex % 5;
        } while (!_firstPlayerAfterBigBlind.IsPlayerActive);

        OnSmallBlindChanged?.Invoke(_smallBlind);
        OnBigBlindChanged?.Invoke(_bigBlind);
    }

    public PlayerManager GetFirstActivePlayerFromDealer()
    {
        bool isPlayerAvailableForTurn;
        var players = GameManager.Instance.Players;
        PlayerManager playerToCheck;
        PlayerManager selectedPlayer = null;

        int playerIndex = players.IndexOf(_smallBlind);
        int loopLimit = players.Count;
        int loopCounter = 0;

        do
        {
            playerToCheck = players[playerIndex];

            isPlayerAvailableForTurn = (!playerToCheck.IsPlayerAllIn && playerToCheck.IsPlayerActive);

            if (isPlayerAvailableForTurn)
            {
                selectedPlayer = playerToCheck;
                break;
            }

            playerIndex++;
            loopCounter++;
            playerIndex %= 5;

        } while (isPlayerAvailableForTurn == false && loopCounter < loopLimit);

        return selectedPlayer; // can return null, if null, means no one has to make any further move (
                               // e.g.  2 players left, one is all in the other isnt. Game should not get stuck at that point due to this being null.

        /*
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
        */
    }

    public PlayerManager GetFirstPlayerAfterBigBlind()
    {
        return _firstPlayerAfterBigBlind;
    }

    public void SelectDealerIndex(int bettingRoundCount)
    {
        if (_currentDealer != null) _currentDealer.IsPlayerDealer = false;

        int dealerIndex = bettingRoundCount - 1;
        dealerIndex = dealerIndex % 5;
        //Itterates through player index's until it finds a valid dealer
        do
        {
            _currentDealer = GameManager.Instance.Players[dealerIndex];
            dealerIndex++;
            dealerIndex = dealerIndex % 5;
        } while (!_currentDealer.IsPlayerActive);

        Debug.Log("current dealer player set as: " + _currentDealer.name);
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }

    private void OnDestroy()
    {
        Instance = null;
    }
}
