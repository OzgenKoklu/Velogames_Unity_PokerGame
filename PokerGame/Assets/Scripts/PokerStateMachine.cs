using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokerStateMachine : MonoBehaviour
{
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

    public static event Action<GameState> OnGameStateChanged;
    private GameState _currentGameState;

    private void ChangeState(GameState newState)
    {
        _currentGameState = newState;
        OnGameStateChanged?.Invoke(_currentGameState);
    }
}
