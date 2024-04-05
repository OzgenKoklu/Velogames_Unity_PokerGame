using System;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }
    public PlayerManager CurrentPlayer { get; private set; }
    private int _currentPlayerIndex;

    private void Awake()
    {
        Instance = this;
    }

    public void SetPlayerTurn()
    {
        if (GameManager.Instance.GetState() == GameManager.GameState.PreFlop && !CurrentPlayer)
        {
            _currentPlayerIndex = DealerManager.Instance.GetDealerPlayerIndex() + 2;
            CurrentPlayer = GameManager.Instance.Players[_currentPlayerIndex];
            GameManager.Instance.Players[_currentPlayerIndex].IsPlayerTurn = true;
            Debug.Log(CurrentPlayer.name + "'s turn!");
            return;
        }

        if (_currentPlayerIndex + 1 >= GameManager.Instance.Players.Count)
        {
            CurrentPlayer.IsPlayerTurn = false;
            _currentPlayerIndex = 0;
            CurrentPlayer = GameManager.Instance.Players[_currentPlayerIndex];
            GameManager.Instance.Players[_currentPlayerIndex].IsPlayerTurn = true;
            Debug.Log(CurrentPlayer.name + "'s turn!");
            return;
        }

        CurrentPlayer.IsPlayerTurn = false;
        _currentPlayerIndex++;
        CurrentPlayer = GameManager.Instance.Players[_currentPlayerIndex];
        GameManager.Instance.Players[_currentPlayerIndex].IsPlayerTurn = true;
        Debug.Log(CurrentPlayer.name + "'s turn!");

        // Wait for player to make a move for a while
        // ...
        // ...
    }

    // For Debugging Button...
    // This will be deleted.
    public void ChangePlayerTurn()
    {
        SetPlayerTurn();
    }
}
