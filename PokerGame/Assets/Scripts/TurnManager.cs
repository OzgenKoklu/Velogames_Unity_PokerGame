using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _playerMoveInfoText;
    public event Action<PlayerManager> OnPlayerTurn;
    private List<PlayerAction> _aiPlayerActionsForTurn;

    public static TurnManager Instance { get; private set; }
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
        _aiPlayerActionsForTurn = new List<PlayerAction>();
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
                _isPreFlop = false;
                ResetTurnStatus();
                SetFirstPlayer(IsPreFlop); //false for IsPreFlop
                //PlayerTurn'e gecis bu sefer Poker Deck Manager'da. Bu yaklasımı sevmiyorum
                break;
            case GameManager.GameState.PostFlop:
                break;
            case GameManager.GameState.Turn:
                break;
            case GameManager.GameState.PostTurn:
                break;
            case GameManager.GameState.River:
                break;
            case GameManager.GameState.PostRiver:
                break;
            case GameManager.GameState.Showdown:
                //showdown'a katilmadan önce post floptan alinan oyuncu listeleri güncellenmeli, ona göre oyundan çikmamiş en yüksek el winning hand seçilmeli.
                // int handRank = PokerHandEvaluator.Instance.EvaluateHandRank(GetCardListWithCommunityCardsAdded()); şu şekil CardSO listeleri için handrank alınabiliyo.

                List<int> handRanksOfPlayersWhoAreStillinTheGame; //= PokerDeckManager.Instance.GetAllPlayerHands();

                // Aşağıdaki şekilde de bir liste içindeki kazanan el belirlenip winning hand result döndürülüyo.

                //PokerHandEvaluator.WinningHandResults winningHandResult = PokerHandEvaluator.Instance.SelectTheWinnerForTheShowdown(playerHandRankList);
                //HandleWinningHandResult(winningHandResult);
                break;
            case GameManager.GameState.PotDistribution:
                break;
            case GameManager.GameState.GameOver:
                break;
            default:
                break;
        }
    }

    private void ExecuteAIMovePostFlop()
    {
        CurrentPlayer.PlayersAction = CurrentPlayer.PlayerHand.AiBotActionPostFlop();
        //_previousPlayerAction = CurrentPlayer.PlayerAction; //dont forget to reset it to fold or Null after each betting round ends.
        _playerMoveInfoText.text = CurrentPlayer.name + " Made the move: " + CurrentPlayer.PlayersAction;

        ChangePlayerTurn();
    }

    private void SetFirstPlayer(bool isPreFlop)
    {
        var players = GameManager.Instance.Players;
        int firstPlayerIndex;

        if (isPreFlop)
        {
            // Pre-flop: Start after the big blind
            firstPlayerIndex = DealerManager.Instance.GetFirstPlayerIndexAfterBigBlind();
        }
        else
        {
            // Post-flop: Start from the small blind or the first active player to the left of the dealer button
            firstPlayerIndex = DealerManager.Instance.GetFirstActivePlayerIndexFromDealer();
            Debug.Log("After the flop, first player selected as: " + firstPlayerIndex);
        }

        _currentPlayerIndex = firstPlayerIndex;
        CurrentPlayer = players[_currentPlayerIndex];

        return;
    }

    private void ResetAllPlayersActiveStatus()
    {
        var players = GameManager.Instance.Players;
        foreach(var player in players)
        {
            player.IsPlayerActive = true;
            player.HasActedSinceLastRaise = false;
        }
    }

    public void ChangePlayerTurn()
    {
        if (GameManager.Instance.GetState() != GameManager.GameState.PlayerTurn) return;

        CurrentPlayer.IsPlayerTurn = false;
        int originalPlayerIndex = _currentPlayerIndex;

        if (IsBettingRoundConcludable())
        {
            // Proceed to collect bets into the pot, move to the next stage
            BetManager.Instance.CollectBets();
            GameManager.Instance.SetGameState(GameManager.GameState.Flop); // Or the next appropriate state
            return;
        }

        do
        {
            _currentPlayerIndex = (_currentPlayerIndex + 1) % GameManager.Instance.Players.Count;
            CurrentPlayer = GameManager.Instance.Players[_currentPlayerIndex];

            // Skip the players who have folded
            if (CurrentPlayer.IsPlayerActive)
            {
                CurrentPlayer.IsPlayerTurn = true;
                GameManager.Instance.SetGameState(GameManager.GameState.PlayerTurn);
                break;
            }
        } while (_currentPlayerIndex != originalPlayerIndex);  // Avoid infinite loops      
    }

    //bu fonksiyonun deck'Le bi ilgisi yok o yüzden aslinda game manager'a tasinması mantikli olabilir. buradan yapilacak seylerin oradan yapilması dogru olabilir.
    private void HandleWinningHandResult(PokerHandEvaluator.WinningHandResults winningHandResult)
    {
        Debug.Log("Winning hand type: " + winningHandResult.WinningHandType + "- Player Index(0,1,2,3,4), 0 is the player. : " + winningHandResult.WinningHandIndex + " - Winning Hand(5Cards) Ranks: " + winningHandResult.WinningCardCodes);
        string winningHandType = winningHandResult.WinningHandType;
        int winningHandPlayerIndex = winningHandResult.WinningHandIndex;

        //Show ile UI'da winning hand gösterecek bi mesaj. ShowWinningHand and player Name(indexten çikartilir) 

        string winningHandCardCodes = winningHandResult.WinningCardCodes;
        List<CardSO> WinningCardList = winningHandResult.WinningCardList;
        CardVisualsManager.Instance.HighlightHand(WinningCardList, winningHandCardCodes);
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
        var players = GameManager.Instance.Players;        
        foreach (var player in players)
        {
            player.IsPlayerTurn = false;
            player.HasActedSinceLastRaise = false;
        }
    }

    private bool AreAllActivePlayersChecked()
    {
        foreach (var player in GameManager.Instance.Players)
        {
            if (player.IsPlayerActive && player.HasActedSinceLastRaise == false)
            {
                return false;
            }
        }
        return true;
    }
}
