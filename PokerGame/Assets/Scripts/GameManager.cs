using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static event Action<GameState> OnGameStateChanged; // !!! Reset static event if it need to be used in multiple scenes
    [SerializeField] private Button _newRoundButton;

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
        //Debug.Log(_currentGameState);
    }

    private void Start()
    {
        PlayerManager.OnPlayerFolded += PlayerManager_OnPlayerFolded;
        _newRoundButton.onClick.AddListener(StartGameRound);
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
        BetManager.Instance.ResetForTheNewRound();
        ResetAllPlayersRoundStatus(); //resetting flop/inactive status
        SetGameState(GameState.NewRound);
        //SetPlayerStacks();       
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

    private void ResetAllPlayersRoundStatus()
    {
        foreach (var player in Players)
        {
            player.ResetForTheNewRound();
        }
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