using System;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static event Action<GameState> OnGameStateChanged;

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
        PlayerTurn,     // A player's turn to act
        GameOver        // The game is over
    }

    private GameState _currentMainGameState;
    private GameState _currentGameState;
    private bool _isGameStarted = false;

    public List<PlayerManager> Players => _players;
    [SerializeField] private List<PlayerManager> _players;
    public List<PlayerManager> ActivePlayers => _activePlayers;
    [SerializeField] private List<PlayerManager> _activePlayers;

    public PlayerManager MainPlayer => _mainPlayer;
    [SerializeField] private PlayerManager _mainPlayer;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        Debug.Log(_currentGameState);
    }

    private void Start()
    {
        PlayerManager.OnPlayerFolded += PlayerManager_OnPlayerFolded;
        StartGame();
    }

    private void PlayerManager_OnPlayerFolded(PlayerManager foldedPlayer)
    {
        _activePlayers.Remove(foldedPlayer);
    }

    public void StartGame()
    {
        _isGameStarted = true;
        StartGameRound();
    }

    public bool IsGameStarted()
    {
        return _isGameStarted;
    }

    public void StartGameRound()
    {
        SetGameState(GameState.NewRound);
        SetPlayerStacks();
    }

    public void SetGameState(GameState newState)
    {
        if (newState != GameState.PlayerTurn)
        {
            _currentMainGameState = newState;
        }
        _currentGameState = newState;
        OnGameStateChanged?.Invoke(_currentGameState);
    }

    private void SetPlayerStacks()
    {
        foreach (var player in _activePlayers)
        {
            //will need to change this.
            player.TotalStackAmount = 1000;
        }
    }

    public GameState GetState()
    {
        return _currentGameState;
    }

    public GameState GetMainGameState()
    {
        return _currentMainGameState;
    }
}
