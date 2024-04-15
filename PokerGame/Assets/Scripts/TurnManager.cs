using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    public float maxThinkTime = 10f; // Maximum time for the AI's thinking

    [SerializeField] private TextMeshProUGUI _playerMoveInfoText;
    public event Action<PlayerManager> OnPlayerTurn;
    private List<PlayerAction> _aiPlayerActionsForTurn;

    public static TurnManager Instance { get; private set; }
    public PlayerManager CurrentPlayer { get; private set; }
    private int _currentPlayerIndex;
    private PlayerAction _previousPlayerAction = PlayerAction.Fold;
    public enum PlayerAction { Fold, Check, Bet, Raise, Call}

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
            SetFirstPlayer();

            Debug.Log(CurrentPlayer.name + "'s turn!");
            return;
        }

        //// !!!!
        /// Big Blind son hamleyi yapacak kisidir. Kontrol etmeyi unutma!!! Daha sonra Flop state'ine gecilmelidir...

        if (state == GameManager.GameState.PlayerTurn)
        {
            OnPlayerTurn?.Invoke(CurrentPlayer);

            if (CurrentPlayer != GameManager.Instance.MainPlayer)
            {
                StartCoroutine(AiBotMoveWithRandomWait());

                //check if player is last in this turn or not, if last, change state to flop, else, change player turn

            }
            else if (CurrentPlayer == GameManager.Instance.MainPlayer)
            {
                //if player is our player, set on UI objects for player input.

                StartCoroutine(TenSecondTimer());

                //Burada previous Action'ı bizim playerın actionına eşitlememiz lazım
                // _previousPlayerAction = CurrentPlayer.PlayerAction;
                Debug.Log("Our Turn, index: " + _currentPlayerIndex + " Select your Move: ");
                //check if player is last in this turn or not, if last, change state to flop, else, change player turn
            }
            return;
        }

        if (state == GameManager.GameState.PostFlop)
        {
            List<List<CardSO>> allPlayerCards = PokerDeckManager.Instance.GetAllPlayerHands();

            //evaluate etmeli ama hemen winner seçmemeli. Evaluation skorlarina göre de player AI'lari bet fold check yapmali.
            List<int> playerHandRankList = PokerHandEvaluator.Instance.EvaluateHandStrengths(allPlayerCards);

            PostFlopAiPlayerActionGeneration(playerHandRankList);

            //sonraki asama icin setFirstPlayer tarzi bir fonksiyon kullanmali, ve aldigimiz actionlar orada Ai botlar için kullanilmali.
            SetFirstPlayer();

            return;
        }

        //showdown'a katilmadan önce post floptan alinan oyuncu listeleri güncellenmeli, ona göre oyundan çikmamiş en yüksek el winning hand seçilmeli.
        if (state == GameManager.GameState.Showdown)
        {
            //cok bloat var, playerHandRankList muhtemelen gameManager'da tutulmali ve setter/getter'lari olmali.

            List<List<CardSO>> allPlayerCards = PokerDeckManager.Instance.GetAllPlayerHands();
            List<int> playerHandRankList = PokerHandEvaluator.Instance.EvaluateHandStrengths(allPlayerCards);
            //turdaki player aksiyonlari sonrasi burasi olmalý. Sadece ShowDown'da hesap etmeli.
            PokerHandEvaluator.WinningHandResults winningHandResult = PokerHandEvaluator.Instance.SelectTheWinnerForTheShowdown(playerHandRankList);
            HandleWinningHandResult(winningHandResult);
        }
    }
    IEnumerator AiBotMoveWithRandomWait()
    {
        // Generate a random wait time between 0 to 10 seconds
        float waitTime = UnityEngine.Random.Range(2f, maxThinkTime);
        Debug.Log("Player's wait time: " + waitTime);
        float startTime = Time.time;
        Debug.Log("Start time: " + startTime);

        Slider timebar = CurrentPlayer.GetTimerSlider();
        // Initialize the timebar
        timebar.maxValue = maxThinkTime;
        timebar.value = maxThinkTime;

        while (Time.time - startTime < waitTime)
        {
            // Update the timebar
            timebar.value = maxThinkTime - (Time.time - startTime);
            yield return null; // Wait until next frame
        }

        // AI bot makes a move after the random wait time
        ExecuteAIMove();
    }

    IEnumerator TenSecondTimer()
    {
        float startTime = Time.time;
        Debug.Log("Start time: " + startTime);

        Slider timebar = CurrentPlayer.GetTimerSlider();
        // Initialize the timebar
        timebar.maxValue = maxThinkTime;
        timebar.value = maxThinkTime;

        while (Time.time - startTime < maxThinkTime)
        {
            // Update the timebar
            timebar.value = maxThinkTime - (Time.time - startTime);
            yield return null; // Wait until next frame
        }
    }

    private void ExecuteAIMove()
    {
        Debug.Log("Previous Action: " + _previousPlayerAction);
        CurrentPlayer.PlayerAction = CurrentPlayer.PlayerHand.AiBotActionPreFlop(_previousPlayerAction);
        _previousPlayerAction = CurrentPlayer.PlayerAction; //dont forget to reset it to fold or Null after each betting round ends.
        _playerMoveInfoText.text = CurrentPlayer.name + " Made the move: " + CurrentPlayer.PlayerAction;
        
        ChangePlayerTurn();
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
}
