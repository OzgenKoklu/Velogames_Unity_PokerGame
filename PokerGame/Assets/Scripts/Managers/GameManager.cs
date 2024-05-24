using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static event Action<GameState> OnGameStateChanged; // !!! Reset static event if it need to be used in multiple scenes
    public event Action OnTournamentStarted;
    public event Action OnMainPlayerWinsTheTournament;
    public event Action OnTournamentEnded;
    public enum GameState
    {
        NewRound,       // Setup for a new round of poker
        PreFlop,        // Deals two hole cards face down to each player. Before the flop (first three community cards) is dealt. 
        Flop,           // The flop is dealt
        PostFlop,       // Betting round after the flop
        Turn,           // The turn (fourth community card) is dealt
        PostTurn,       // Betting round after the turn
        River,          // The river (fifth community card) is dealt
        PostRiver,      // Final betting round after the river
        Showdown,       // Players reveal their hands to determine the winner
        PotDistribution,// The pot is distributed to the winner(s)
        EveryoneFolded, // Everyone folded except one player
        PlayerTurn,     // A player's turn to act
        GameOver        // The game is over
    }

    private GameState _currentMainGameState;
    private GameState _currentGameState;
    private bool _isGameStarted = false;
    private int _roundCount;

    public List<PlayerManager> Players => _players;
    [SerializeField] private List<PlayerManager> _players;
    public List<PlayerManager> ActivePlayers => _activePlayers;
    [SerializeField] private List<PlayerManager> _activePlayers;

    public PlayerManager MainPlayer => _mainPlayer;
    [SerializeField] private PlayerManager _mainPlayer;

    private void Awake()
    {
        Instance = this;
        _roundCount = 0;
    }

    private void Start()
    {
        PlayerManager.OnPlayerFolded += PlayerManager_OnPlayerFolded;
        PokerDeckManager.Instance.OnCardDealingComplete += PokerDeckManager_OnCardDealingComplete;
        StartGame();
    }

    private void PokerDeckManager_OnCardDealingComplete()
    {
        SetGameState(GameState.PlayerTurn);
    }

    public void SetTimeScale(bool isGamePaused)
    {
        Time.timeScale = isGamePaused ? 0 : 1;
    }

    private void PlayerManager_OnPlayerFolded(PlayerManager foldedPlayer)
    {
        _activePlayers.Remove(foldedPlayer);
    }

    public void StartGame()
    {
        _isGameStarted = true;
        OnTournamentStarted?.Invoke();
        StartGameRound();
    }

    public bool IsGameStarted() => _isGameStarted;

    public void StartGameRound()
    {
        BetManager.Instance.ResetForTheNewRound();
        ResetAllPlayersRoundStatus(); //resetting flop/inactive status

        bool continueGame = CheckIfEnoughPlayersAreRemaining();

        if (continueGame)
        {
            _roundCount++;
            SetGameState(GameState.NewRound);
        }
        else
        {
            Debug.Log("Time To Conclude the game.  Winner: " + _activePlayers[0].PlayerName);

            /// !!! REPLAY GAME BUTTON - WINNER ANOUNCEMENT - SAVE PLAYER DATA ETCETC
            /// !!! DIKKAT
            if (_activePlayers[0] == MainPlayer)
            {
                OnMainPlayerWinsTheTournament?.Invoke();
            }

            OnTournamentEnded?.Invoke();

        }
    }

    public void SetGameState(GameState newState)
    {
        // Check if all players have folded except one and set the game state to EveryoneFolded and give the main pot to the remaining player
        if (ActivePlayers.Count == 1)
        {
            _currentGameState = GameState.EveryoneFolded;

            _currentMainGameState = GameState.EveryoneFolded;
            OnGameStateChanged?.Invoke(_currentGameState);
            Debug.Log("Everyone folded...");
            return;
        }

        if (newState != GameState.PlayerTurn)
        {
            _currentMainGameState = newState;
        }
        _currentGameState = newState;

        OnGameStateChanged?.Invoke(_currentGameState);
    }

    public bool CheckIfEnoughPlayersAreRemaining() => ActivePlayers.Count > 1;
    public int GetBettingRoundCount() => _roundCount;

    private void ResetAllPlayersRoundStatus()
    {
        _activePlayers.Clear();
        foreach (var player in Players)
        {
            player.ResetForTheNewRound();
            if (player.IsPlayerActive)
            {
                _activePlayers.Add(player);
            }
        }
    }
    public bool IsAnyPlayerAllIn()
    {
        return Players.Any(player => player.IsPlayerAllIn);
    }
    public GameState GetState() => _currentGameState;
    public GameState GetMainGameState() => _currentMainGameState;

    private void OnDestroy()
    {
        ResetStaticData();
        PlayerManager.OnPlayerFolded -= PlayerManager_OnPlayerFolded;
        Instance = null;
    }

    public static void ResetStaticData() => OnGameStateChanged = null;
}