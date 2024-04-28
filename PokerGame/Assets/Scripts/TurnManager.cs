using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }
    public event Action<PlayerManager> OnPlayerTurn;

    [SerializeField] private TextMeshProUGUI _playerMoveInfoText;

    public PlayerManager CurrentPlayer { get; private set; }
    private int _currentPlayerIndex;

    public bool IsPreFlop
    {
        get => _isPreFlop;
    }
    [SerializeField] private bool _isPreFlop;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
    }

    private void GameManager_OnGameStateChanged(GameManager.GameState state)
    {
        switch (state)
        {
            case GameManager.GameState.PreFlop:
                _isPreFlop = true;
                ResetAllPlayersActiveStatus(); //resetting flop/inactive status
                SetFirstPlayer(IsPreFlop); //true for IsPreFlop
                GameManager.Instance.SetGameState(GameManager.GameState.PlayerTurn);
                break;
            case GameManager.GameState.PlayerTurn:
                OnPlayerTurn?.Invoke(CurrentPlayer);
                break;
            case GameManager.GameState.Flop:

                break;
            case GameManager.GameState.PostFlop:
                _isPreFlop = false;
                ResetTurnStatus();
                SetFirstPlayer(IsPreFlop); //false for IsPreFlop
                //PlayerTurn'e gecis bu sefer Poker Deck Manager'da. Bu yaklasımı sevmiyorum
                break;
            case GameManager.GameState.Turn:
                break;
            case GameManager.GameState.PostTurn:
                _isPreFlop = false;
                ResetTurnStatus();
                SetFirstPlayer(IsPreFlop); //false for IsPreFlop
                //PlayerTurn'e gecis bu sefer Poker Deck Manager'da. Bu yaklasımı sevmiyorum
                break;
            case GameManager.GameState.River:
                break;
            case GameManager.GameState.PostRiver:
                _isPreFlop = false;
                ResetTurnStatus();
                SetFirstPlayer(IsPreFlop); //false for IsPreFlop
                //PlayerTurn'e gecis bu sefer Poker Deck Manager'da. Bu yaklasımı sevmiyorum
                break;
            case GameManager.GameState.Showdown:
                BetManager.Instance.DevideIntoPots();
                PokerHandEvaluator.WinningHandResults winningHandResult = PokerHandEvaluator.Instance.SelectTheWinnerForTheShowdown();
                CardVisualsManager.Instance.FlipAllCards();
                CardVisualsManager.Instance.GetToShowdownPosition();
                HandleWinningHandResult(winningHandResult);
                break;
            case GameManager.GameState.PotDistribution:
                break;
            case GameManager.GameState.GameOver:
                break;
            default:
                break;
        }
    }

    private void SetFirstPlayer(bool isPreFlop)
    {
        var activePlayers = GameManager.Instance.ActivePlayers;
        PlayerManager firstPlayer;

        if (isPreFlop)
        {
            // Pre-flop: Start after the big blind
            firstPlayer = DealerManager.Instance.GetFirstPlayerAfterBigBlind();
        }
        else
        {
            // Post-flop: Start from the small blind or the first active player to the left of the dealer button
            firstPlayer = DealerManager.Instance.GetFirstActivePlayerFromDealer();
            Debug.Log("After the flop, first player selected as: " + firstPlayer);
        }

        CurrentPlayer = firstPlayer;
        _currentPlayerIndex = activePlayers.IndexOf(CurrentPlayer);

        return;
    }

    private void ResetAllPlayersActiveStatus()
    {
        var activePlayers = GameManager.Instance.ActivePlayers;
        foreach (var player in activePlayers)
        {
            player.IsPlayerActive = true;
            player.HasActedSinceLastRaise = false;
            player.IsPlayerAllIn = false;
        }
    }

    private void ExecuteAIMovePostFlop()
    {
        CurrentPlayer.PlayersAction = CurrentPlayer.PlayerHand.AiBotActionPostFlop();
        //_previousPlayerAction = CurrentPlayer.PlayerAction; //dont forget to reset it to fold or Null after each betting round ends.
        _playerMoveInfoText.text = CurrentPlayer.name + " Made the move: " + CurrentPlayer.PlayersAction;

        //ChangePlayerTurn();
    }

    public void ChangePlayerTurn(bool isPreviousPlayerFolded)
    {
        if (GameManager.Instance.GetState() != GameManager.GameState.PlayerTurn) return;
        BetManager.Instance.CollectBets(CurrentPlayer);


        CurrentPlayer.IsPlayerTurn = false;

        if (IsBettingRoundConcludable())
        {
            // Proceed to collect bets into the pot, move to the next stage
            
            //BetManager.Instance.CurrentHighestBetAmount = 0; bunu sildik çünkü highest bet'i showdown'a kadar tutucaz. (ki o bet'e erisebilene kadar raise/call etsinelr)

            switch (GameManager.Instance.GetMainGameState())
            {
                case GameManager.GameState.PreFlop:
                    GameManager.Instance.SetGameState(GameManager.GameState.Flop);
                    return;
                case GameManager.GameState.PostFlop:
                    GameManager.Instance.SetGameState(GameManager.GameState.Turn);
                    return;
                case GameManager.GameState.PostTurn:
                    GameManager.Instance.SetGameState(GameManager.GameState.River);
                    return;
                case GameManager.GameState.PostRiver:
                    GameManager.Instance.SetGameState(GameManager.GameState.Showdown);
                    return;
                case GameManager.GameState.Showdown:
                    GameManager.Instance.SetGameState(GameManager.GameState.PotDistribution);
                    return;
                case GameManager.GameState.PotDistribution:
                    GameManager.Instance.SetGameState(GameManager.GameState.GameOver);
                    return;
                case GameManager.GameState.GameOver:
                    GameManager.Instance.SetGameState(GameManager.GameState.NewRound);
                    return;
            }
        }

        // Check if the previous player folded to make sure the next player is the correct one
        if (isPreviousPlayerFolded)
        {
            _currentPlayerIndex = (_currentPlayerIndex) % GameManager.Instance.ActivePlayers.Count;
            CurrentPlayer = GameManager.Instance.ActivePlayers[_currentPlayerIndex];

            // Check if the player is all in and skip their turn if they are
            if (CurrentPlayer.IsPlayerAllIn == true)
            {
                _currentPlayerIndex = (_currentPlayerIndex + 1) % GameManager.Instance.ActivePlayers.Count;
                CurrentPlayer = GameManager.Instance.ActivePlayers[_currentPlayerIndex];
            }
        }
        else
        {
            _currentPlayerIndex = (_currentPlayerIndex + 1) % GameManager.Instance.ActivePlayers.Count;
            CurrentPlayer = GameManager.Instance.ActivePlayers[_currentPlayerIndex];

            // Check if the player is all in and skip their turn if they are
            if (CurrentPlayer.IsPlayerAllIn == true)
            {
                _currentPlayerIndex = (_currentPlayerIndex + 1) % GameManager.Instance.ActivePlayers.Count;
                CurrentPlayer = GameManager.Instance.ActivePlayers[_currentPlayerIndex];
            }
        }

        CurrentPlayer.IsPlayerTurn = true;
        GameManager.Instance.SetGameState(GameManager.GameState.PlayerTurn);
    }

    // DÜZELTİLECEK!!!!!!!!!
    //bu fonksiyonun deck'Le bi ilgisi yok o yüzden aslinda game manager'a tasinması mantikli olabilir. buradan yapilacak seylerin oradan yapilması dogru olabilir.
    private void HandleWinningHandResult(PokerHandEvaluator.WinningHandResults winningHandResult)
    {
        string winningHandType = winningHandResult.WinningHandType;
        string winningHandCardCodes = winningHandResult.WinningCardCodes;
        var winningPlayerList = winningHandResult.WinnerList;
        Debug.Log("Winning hand type: " + winningHandType + "- Winner List (playerManagerList) : " + winningPlayerList + " - Winning Hand(5Cards) Ranks: " + winningHandCardCodes);

        Debug.Log("Is there tie: " + winningHandResult.IsTie);
        //Show ile UI'da winning hand gösterecek bi mesaj. ShowWinningHand and player Name(indexten çikartilir) 

        //Main pot side pot ayarlamalari yapilacak. 
        int totalPotToSlipt = BetManager.Instance.TempPot;
        if (winningHandResult.IsTie)
        {
            //pot split (not doing side / main pot for now)
            int splitAmount = totalPotToSlipt / winningPlayerList.Count;

            foreach (var player in winningPlayerList)
            {
                player.TotalStackAmount += splitAmount;
            }
        }
        else
        {
            winningPlayerList[0].TotalStackAmount += totalPotToSlipt;
        }


        BetManager.Instance.TempPot = 0; // set pot to zero.
        List<CardSO> WinningCardList = winningHandResult.WinningCardList;
        bool isItATie = winningHandResult.IsTie;
        CardVisualsManager.Instance.HighlightHand(WinningCardList, winningHandCardCodes, isItATie);
    }

    private bool IsBettingRoundConcludable()
    {
        // Check if there has been any bet made
        if (BetManager.Instance.CurrentHighestBetAmount == 0)
        {
            // If no bet has been made, the round can conclude if everyone has had a chance to act and chosen to check.
            return AreAllActivePlayersChecked();
        }

        // Check if all active players have their bets equal to the highest current bet
        if (!BetManager.Instance.AreAllActivePlayersBetsEqual())
        {
            return false;
        }

        // Check if the last player to raise has had other players act after them
        return AreAllActivePlayersChecked();
    }

    private void ResetTurnStatus()
    {
        var activePlayers = GameManager.Instance.ActivePlayers;
        foreach (var player in activePlayers)
        {
            player.ResetTurnStatus();
        }
    }


    private bool AreAllActivePlayersChecked()
    {
        foreach (var player in GameManager.Instance.ActivePlayers)
        {
            if (player.IsPlayerActive && player.HasActedSinceLastRaise == false)
            {
                if (player.IsPlayerAllIn)
                {
                    continue;
                }
                return false;
            }
        }
        return true;
    }
}