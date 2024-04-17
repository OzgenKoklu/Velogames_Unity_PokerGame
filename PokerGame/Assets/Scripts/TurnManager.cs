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
                ResetAllPlayersActiveStatus(); //resetting flop/inactive status
                SetFirstPlayer(true); //true for IsPreFlop
                GameManager.Instance.SetGameState(GameManager.GameState.PlayerTurn);
                break;
            case GameManager.GameState.PlayerTurn:
                OnPlayerTurn?.Invoke(CurrentPlayer);
                break;
            case GameManager.GameState.Flop:
                SetFirstPlayer(false); //false for IsPreFlop

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
        CurrentPlayer.IsPlayerTurn = true;
        
        return;
    }

    private void ResetAllPlayersActiveStatus()
    {
        var players = GameManager.Instance.Players;
        foreach(var player in players)
        {
            player.IsPlayerActive = true;
        }
    }

    public void ChangePlayerTurn()
    {
        if (IsBettingRoundConcludable())
        {
            //go into the next stage. Not always flop stage
            Debug.Log("State Changed, all players ready to go to the flop stage.");

            //also should take players money into the pot etc. The logic should be here.

            GameManager.Instance.SetGameState(GameManager.GameState.Flop);
            return;        
        };

        var players = GameManager.Instance.Players;

        CurrentPlayer.IsPlayerTurn = false;

        if (_currentPlayerIndex + 1 >= players.Count)
        {
            _currentPlayerIndex = 0;
            CurrentPlayer = players[_currentPlayerIndex];

            CurrentPlayer.IsPlayerTurn = true;
            GameManager.Instance.SetGameState(GameManager.GameState.PlayerTurn);

            return;
        }

        _currentPlayerIndex++;
        CurrentPlayer = players[_currentPlayerIndex];

        CurrentPlayer.IsPlayerTurn = true;
        GameManager.Instance.SetGameState(GameManager.GameState.PlayerTurn);
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
        //Burada dikkat. Sıra Big Blind'a geçince direkt conclude edebiliyo bir kere oldu
        return BetManager.Instance.IsAllActivePlayersBetsEqual() && BetManager.Instance.CurrentHighestBetAmount > 0;
    }
}
