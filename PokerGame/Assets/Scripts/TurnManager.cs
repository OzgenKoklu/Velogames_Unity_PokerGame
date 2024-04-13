using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using static TurnManager;

public class TurnManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _playerMoveInfoText;
    [SerializeField] private TextMeshProUGUI _playerTimerText;
    public event Action<PlayerManager> OnPlayerTurn;
    private List<PlayerAction> _aiPlayerActionsForTurn;

    public static TurnManager Instance { get; private set; }
    public PlayerManager CurrentPlayer { get; private set; }
    private int _currentPlayerIndex;
    public enum PlayerAction { Fold, Check, Bet, Raise }

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
        if (state == GameManager.GameState.PreFlop)
        {
            //keeping ai player actions in a list
            _aiPlayerActionsForTurn = PreFlopAiPlayerActionGeneration();
          
            SetFirstPlayer();

            //PlayerAksiyonlarý ile oyun loopunu devam ettireceðiz

            Debug.Log(CurrentPlayer.name + "'s turn!");
            return;
        }

        if (state == GameManager.GameState.PlayerTurn)
        {
            OnPlayerTurn?.Invoke(CurrentPlayer);
          
            if(_currentPlayerIndex != 2)
            {
                //player is AI
                PlayerAction playerAction = _aiPlayerActionsForTurn[_currentPlayerIndex];

                Debug.Log("Ai turn, index: " + _currentPlayerIndex + " Move: " + playerAction);

                //logic to write things on UI and also virtually elongating the process (like timer on screen etc)

                //check if player is last in this turn or not, if last, change state to flop, else, change player turn
                ChangePlayerTurn();
            }else if(_currentPlayerIndex == 2)
            {
                //if player is our player, set on UI objects for player input.
                Debug.Log("Our Tern, index: " + _currentPlayerIndex + " Select your Move: ");
                //check if player is last in this turn or not, if last, change state to flop, else, change player turn
               
            }
            return;
        }

        if (state == GameManager.GameState.PostFlop)
        {
            List<List<CardSO>> allPlayerCards = PokerDeckManager.Instance.GetAllPlayerHands();

            //evaluate etmeli ama hemen winner seçmemeli. Evaluation skorlarýna göre de player AI'larý bet fold check yapmalý.
            List<int> playerHandRankList = PokerHandEvaluator.Instance.EvaluateHandStrengths(allPlayerCards);

            PostFlopAiPlayerActionGeneration(playerHandRankList);

            //sonraki aþama içni setFirstPlayer tarzý bir fonksiyon kullanmalý, ve aldýðýmýz actionlar orada Ai botlar için kullanýlmalý.
            SetFirstPlayer();

            return;
        }

        //showdown'a katýlmadan önce post floptan alýnan oyuncu listeleri güncellenmeli, ona göre oyundan çýkmamýþ en yüksek el winning hand seçilmeli.
        if (state == GameManager.GameState.Showdown)
        {
            //cok bloat var, playerHandRankList muhtemelen gameManager'da tutulmalý ve setter/getter'larý olmalý.

            List<List<CardSO>> allPlayerCards = PokerDeckManager.Instance.GetAllPlayerHands();
            List<int> playerHandRankList = PokerHandEvaluator.Instance.EvaluateHandStrengths(allPlayerCards);
            //turdaki player aksiyonlarý sonrasý burasý olmalý. Sadece ShowDown'da hesap etmeli.
            PokerHandEvaluator.WinningHandResults winningHandResult = PokerHandEvaluator.Instance.SelectTheWinnerForTheShowdown(playerHandRankList);
            HandleWinningHandResult(winningHandResult);
        }


    }

    private void SetFirstPlayer()
    {
        var players = GameManager.Instance.Players;

        _currentPlayerIndex = DealerManager.Instance.GetFirstPlayerIndexAfterBigBlind();
        CurrentPlayer = players[_currentPlayerIndex];
        CurrentPlayer.IsPlayerTurn = true;
        GameManager.Instance.SetGameState(GameManager.GameState.PlayerTurn);
    
        return;
    }

    public void ChangePlayerTurn()
    {
        var players = GameManager.Instance.Players;

        if (_currentPlayerIndex + 1 >= players.Count)
        {
            CurrentPlayer.IsPlayerTurn = false;
            _currentPlayerIndex = 0;
            CurrentPlayer = players[_currentPlayerIndex];
            CurrentPlayer.IsPlayerTurn = true;
            GameManager.Instance.SetGameState(GameManager.GameState.PlayerTurn);

            Debug.Log(CurrentPlayer.name + "'s turn!");
            return;
        }

        CurrentPlayer.IsPlayerTurn = false;
        _currentPlayerIndex++;
        CurrentPlayer = players[_currentPlayerIndex];
        CurrentPlayer.IsPlayerTurn = true;
        GameManager.Instance.SetGameState(GameManager.GameState.PlayerTurn);

        Debug.Log(CurrentPlayer.name + "'s turn!");

        // Wait for player to make a move for a while
        // ...
        // ...
    }

    private List<PlayerAction> PreFlopAiPlayerActionGeneration()
    {
        List<PokerPlayerHand> playerHands = PokerDeckManager.Instance.GetPlayerHands();
        List<PlayerAction> playerActions = new List<PlayerAction>();
        //preflop Ai behaviour check 
        foreach (var playerHand in playerHands)
        {
            int i = playerHands.IndexOf(playerHand);

            if (playerHands[i].IsPlayerAiBot())
            {
                // Adds the AI bot's pre-flop action to the list
                playerActions.Add(playerHand.AiBotActionPreFlop());
            }
            else
            {
                // If player is human, assume default action or handle differently
                playerActions.Add(PlayerAction.Fold);
            }
        }

        return playerActions;
    }

    private void PostFlopAiPlayerActionGeneration(List<int> playerHandRankList)
    {
        List<PokerPlayerHand> playerHands = PokerDeckManager.Instance.GetPlayerHands();

        foreach (var playerHandRank in playerHandRankList)
        {
            int i = playerHandRankList.IndexOf(playerHandRank);

            if (playerHands[i].IsPlayerAiBot())
            {
                //returns AiPlayerBehaviour.PlayerAction enum
                playerHands[i].AiBotActionPostFlop(playerHandRank);
            }
        }

    }

    //bu fonksiyonun deck'Le bi ilgisi yok o yüzden aslýnda game manager'a taþýnmasý mantýklý olabilir. buradan yapýlacak þeylerin oradan yapýlmasý doðru olabilir.
    private void HandleWinningHandResult(PokerHandEvaluator.WinningHandResults winningHandResult)
    {
        Debug.Log("Winning hand type: " + winningHandResult.WinningHandType + "- Player Index(0,1,2,3,4), 0 is the player. : " + winningHandResult.WinningHandIndex + " - Winning Hand(5Cards) Ranks: " + winningHandResult.WinningCardCodes);
        string winningHandType = winningHandResult.WinningHandType;
        int winningHandPlayerIndex = winningHandResult.WinningHandIndex;

        //Show ile UI'da winning hand gösterecek bi mesaj. ShowWinningHand and player Name(indexten çýkartýlýr) 

        string winningHandCardCodes = winningHandResult.WinningCardCodes;
        List<CardSO> WinningCardList = winningHandResult.WinningCardList;
        CardVisualsManager.Instance.HighlightHand(WinningCardList, winningHandCardCodes);
    }
}
