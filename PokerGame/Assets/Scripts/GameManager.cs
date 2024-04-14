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
        PreFlop,        // Before the flop (first three community cards) is dealt. Deals two hole cards face down to each player.
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

    private GameState _currentGameState;
    private bool _isGameStarted = false;

    public List<PlayerManager> Players => _players;
    [SerializeField] private List<PlayerManager> _players;

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

    private void Start()
    {
        StartGame();
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
    }

    public void SetGameState(GameState newState)
    {
        _currentGameState = newState;
        OnGameStateChanged?.Invoke(_currentGameState);
    }

    public GameState GetState()
    {
        return _currentGameState;
    }
}
