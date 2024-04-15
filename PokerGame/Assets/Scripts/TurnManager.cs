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
    private PlayerAction? _previousPlayerAction = null;
    public enum PlayerAction { Fold, Check, Bet, Raise, Call }

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
            SetFirstPlayer();

            //burda set first player olacak sonra gene player turn olacak. 
            //ExecuteAIMovePostFlop() ile iş halledilebilir. belki player turn'ün içindeki AiBotMoveWithRandomWait'in içine condition konur post flop veya preflop olduğuna dair.


            return;
        }

        //showdown'a katilmadan önce post floptan alinan oyuncu listeleri güncellenmeli, ona göre oyundan çikmamiş en yüksek el winning hand seçilmeli.
        if (state == GameManager.GameState.Showdown)
        {

            // int handRank = PokerHandEvaluator.Instance.EvaluateHandRank(GetCardListWithCommunityCardsAdded()); şu şekil CardSO listeleri için handrank alınabiliyo.

            List<int> handRanksOfPlayersWhoAreStillinTheGame; //= PokerDeckManager.Instance.GetAllPlayerHands();



            // Aşağıdaki şekilde de bir liste içindeki kazanan el belirlenip winning hand result döndürülüyo.

            //PokerHandEvaluator.WinningHandResults winningHandResult = PokerHandEvaluator.Instance.SelectTheWinnerForTheShowdown(playerHandRankList);
            //HandleWinningHandResult(winningHandResult);
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
        CurrentPlayer.PlayerAction = CurrentPlayer.PlayerHand.AiBotActionPreFlop();
        _previousPlayerAction = CurrentPlayer.PlayerAction; //dont forget to reset it to fold or Null after each betting round ends.
        _playerMoveInfoText.text = CurrentPlayer.name + " Made the move: " + CurrentPlayer.PlayerAction;

        ChangePlayerTurn();
    }

    private void ExecuteAIMovePostFlop()
    {
        Debug.Log("Previous Action: " + _previousPlayerAction);
        CurrentPlayer.PlayerAction = CurrentPlayer.PlayerHand.AiBotActionPostFlop();
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
