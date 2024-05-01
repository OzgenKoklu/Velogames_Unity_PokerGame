using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using TMPro;
using UnityEngine;

public class BetManager : MonoBehaviour
{
    public static BetManager Instance { get; private set; }

    public event Action<PlayerManager, int> OnBetUpdated;

    public int CurrentHighestBetAmount
    {
        get => _currentHighestBetAmount;
        set => _currentHighestBetAmount = value;
    }
    private int _currentHighestBetAmount;

    public int TempPot
    {
        get => _tempPot;
        set => _tempPot = value;
    }
    [SerializeField] private int _tempPot = 0;

    public List<Pot> PotsList;
    [SerializeField] private List<Pot> _potsList;

    public Dictionary<PlayerManager, int> potContributionDictionary;

    public int BaseRaiseBetAmount
    {
        get => _baseRaiseAmount;
        set => _baseRaiseAmount = value;
    }
    [SerializeField] private int _baseRaiseAmount = 10;


    [SerializeField] public List<Pot> showdownPots;

    //GECICI BURDA
    [SerializeField] private TextMeshProUGUI _winningResultText;
    private Coroutine _runningCoroutine;
    private float _resultsTimer = 5f;

    private void Awake()
    {
        Instance = this;
        _tempPot = 0;
        showdownPots = new List<Pot>();
        potContributionDictionary = new Dictionary<PlayerManager, int>();
    }

    private void Start()
    {
        GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
        DealerManager.Instance.OnDealerChanged += DealerManager_OnDealerChanged;
    }
    private void GameManager_OnGameStateChanged(GameManager.GameState state)
    {      

        if (state == GameManager.GameState.Showdown)
        {
            showdownPots = DevideIntoPots();

            OnBetUpdated?.Invoke(null, -1); // for resetting the bet amount

            int potCount = showdownPots.Count;

            Debug.Log("Number of pots for the showdown: " + potCount);

            CardVisualsManager.Instance.FlipAllCards();
            CardVisualsManager.Instance.GetToShowdownPosition();
            string winningResult = "";
            for (int i = 0; i < potCount; i++)
            {
                
                var eligiblePlayersForThisPot = showdownPots[i]._eligiblePlayerList;
                PokerHandEvaluator.WinningHandResults winningHandResult = PokerHandEvaluator.Instance.SelectTheWinnerForTheShowdown(eligiblePlayersForThisPot);
                Debug.Log("Winning result for the pot index " + i + " is " + winningHandResult.WinnerList[0]);

                winningResult = winningHandResult.WinnerList[0].PlayerName + " wins the main pot. Amount: " + showdownPots[i].MoneyInPot; 
                HandleWinningHandResult(winningHandResult, showdownPots[i]);
            }
            //GameManager.Instance.ConcludeBettingRound();

            
            _runningCoroutine = StartCoroutine(ResultsCoroutine(winningResult));
        }
        else if (state == GameManager.GameState.EveryoneFolded)
        {
            showdownPots = DevideIntoPots();

            List<PlayerManager> player = GameManager.Instance.ActivePlayers;
            Debug.Log("Before Money transfer, total chips : " + player[0].TotalStackAmount);
            string winningResultText = player[0].PlayerName + " wins the blinds. Amount:" + showdownPots[0].MoneyInPot;
            player[0].TotalStackAmount += showdownPots[0].MoneyInPot;
            Debug.Log("After Money transfer, total chips : " + player[0].TotalStackAmount);
            showdownPots[0].MoneyInPot = 0;
            //GameManager.Instance.ConcludeBettingRound();

            
            _runningCoroutine = StartCoroutine(ResultsCoroutine(winningResultText));
        }
    }

    IEnumerator ResultsCoroutine(string winningResult)
    {
        float startTime = Time.time;

        _winningResultText.text = winningResult;

        while (Time.time - startTime < _resultsTimer)
        { 
            yield return null; 
        }

        _winningResultText.text = "";

        GameManager.Instance.StartGameRound();
    }

    private void DealerManager_OnDealerChanged(PlayerManager dealerPlayer)
    {
        
            PlayerManager smallBlind = DealerManager.Instance.GetSmallBlind();
            SetBet(smallBlind, _baseRaiseAmount / 2);
            CollectBets(DealerManager.Instance.GetSmallBlind()); // rotus cekelim

            PlayerManager bigBlind = DealerManager.Instance.GetBigBlind();
            SetBet(bigBlind, _baseRaiseAmount);
            CollectBets(bigBlind);//rotus cekelim
          
            _currentHighestBetAmount = _baseRaiseAmount;
            
            GameManager.Instance.SetGameState(GameManager.GameState.PreFlop);        
    }

    public void SetBet(PlayerManager player, int betAmount)
    {
        player.BetAmount = betAmount;
        OnBetUpdated?.Invoke(player, betAmount);
        //Debug.Log(player.name + " bet " + betAmount);
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
        DealerManager.Instance.OnDealerChanged -= DealerManager_OnDealerChanged;
    }

    public bool AreAllActivePlayersBetsEqual()
    {
        var activePlayers = GameManager.Instance.ActivePlayers;

        if (activePlayers == null || activePlayers.Count == 0) { return false; }

        foreach (var player in activePlayers)
        {
            if (player.IsPlayerAllIn) continue;
            if (player.TotalBetInThisRound != CurrentHighestBetAmount) return false;
        }
        //everyone has the same bet amount and its the highest bet amount
        return true;
    }

    public void SetMinimumRaiseAmount(int raiseAmount)
    {
        _baseRaiseAmount += raiseAmount;
    }

    public int GetMinimumRaiseAmount()
    {
        return _baseRaiseAmount;
    }

    // DÜZELTÝLECEK!!!!!!!!!
    //bu fonksiyonun betle Le bi ilgisi yok o yüzden aslinda game manager'a tasinmasý mantikli olabilir. buradan yapilacak seylerin oradan yapilmasý dogru olabilir.
    private void HandleWinningHandResult(PokerHandEvaluator.WinningHandResults winningHandResult, Pot potInHand)
    {
        string winningHandType = winningHandResult.WinningHandType;
        string winningHandCardCodes = winningHandResult.WinningCardCodes;
        var winningPlayerList = winningHandResult.WinnerList;
        Debug.Log("Winning hand type: " + winningHandType + "- Winner List (playerManagerList) : " + winningPlayerList + " - Winning Hand(5Cards) Ranks: " + winningHandCardCodes);

        Debug.Log("Is there tie: " + winningHandResult.IsTie);
        //Show ile UI'da winning hand gösterecek bi mesaj. ShowWinningHand and player Name(indexten çikartilir) 

        //Main pot side pot ayarlamalari yapilacak. 
        int totalPotToSlipt = potInHand.MoneyInPot;
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


        potInHand.MoneyInPot = 0; // set pot to zero.
        List<CardSO> WinningCardList = winningHandResult.WinningCardList;
        bool isItATie = winningHandResult.IsTie;
        CardVisualsManager.Instance.HighlightHand(WinningCardList, winningHandCardCodes, isItATie);
    }

    public void CollectBets(PlayerManager player)
    {
        Debug.Log("Collecting Bets from player.");
        player.TotalBetInThisRound += player.BetAmount;
        player.TotalStackAmount -= player.BetAmount;
        player.BetAmount = 0;

        if (potContributionDictionary.TryGetValue(player, out int currentBet))
        {
            // Player exists, update total bet
            potContributionDictionary[player] = player.TotalBetInThisRound;
            Debug.Log("Total bet in dictionary: " + potContributionDictionary[player]);
        }
        else
        {
            // New player, add entry to dictionary
            potContributionDictionary.Add(player, player.TotalBetInThisRound);
            Debug.Log("Total bet in dictionary: " + potContributionDictionary[player]);
        }
    }


    public List<Pot> DevideIntoPots()
    {
        // a function that will work in showdown or whenever necessary. 

        // Will take the current potContributionDictionary. 

        // From there onwards we will create main pots and side pots if necessary. 


        int playerCount = GameManager.Instance.Players.Count;

        List<int> potDefiningBetValues = new List<int>();

        //potDefiningBetValues = potContributionDictionary.Values.ToList();

        foreach (var playerContribution in potContributionDictionary)
        {
            PlayerManager player = playerContribution.Key;
            int playerBetAmount = playerContribution.Value;

            if (player.IsPlayerAllIn)
            {
                int indexOfValue = potDefiningBetValues.IndexOf(playerBetAmount);
                if (indexOfValue == -1) //If same value exists do not need an extra pot
                {
                    potDefiningBetValues.Add(playerBetAmount);
                    Debug.Log("Pot defining value added. " + playerBetAmount);
                }
            }
            //int playerBet = playerContribution.Value;

        }

        potDefiningBetValues.Sort();

        List<Pot> pots = new List<Pot>();
        Pot mainPot = new Pot();
        pots.Add(mainPot);

        Pot currentPot = mainPot;

        if (potDefiningBetValues.Count > 0)
        {
            foreach (var definingValue in potDefiningBetValues)
            {
                Debug.Log("Pot defining value: " + definingValue);
                currentPot.potLimit = definingValue;
                currentPot.isLastPot = false; //contribution amount is a 
                Pot sidePot = new();
                pots.Add(sidePot);
                currentPot = sidePot;
            }
        }


        //Decide eligible players for each bet.
        var activePlayers = GameManager.Instance.ActivePlayers;
        // Loop through players in contribution dictionary
        foreach (var pot in pots)
        {
            if (!pot.isLastPot)
            {
                foreach (var player in activePlayers)
                {
                    if (player.TotalBetInThisRound >= pot.potLimit)
                    {
                        pot.AddEligiblePlayer(player);
                        Debug.Log("Eligible player added to" + pot + " player. " + player.PlayerName);
                    }
                }
            }
            else
            {
                foreach (var player in activePlayers)
                {
                    if (player.TotalBetInThisRound == CurrentHighestBetAmount)
                    {
                        //add situation for when 2 of the biggest bets do not match (player with the most money goes all-in)
                        //should not accept their bet as is -> and pay remainder.
                        pot.AddEligiblePlayer(player);
                        Debug.Log("Eligible player added to" + pot + " player. " + player.PlayerName);
                    }
                }
            }
        }

        //get bets into the pots. 
        var allPlayers = GameManager.Instance.Players;
        foreach (var pot in pots)
        {
            if (!pot.isLastPot)
            {
                foreach (var player in allPlayers)
                {
                    if (player.TotalBetInThisRound >= pot.potLimit)
                    {
                        pot.MoneyInPot += pot.potLimit;
                        player.TotalBetInThisRound -= pot.potLimit;

                    }
                    else if (player.TotalBetInThisRound < pot.potLimit)
                    {
                        pot.MoneyInPot += player.TotalBetInThisRound;
                        player.TotalBetInThisRound = 0;
                    }
                }
            }
            else
            {
                foreach (var player in allPlayers)
                {
                    pot.MoneyInPot += player.TotalBetInThisRound;
                    player.TotalBetInThisRound = 0;
                }
            }

            Debug.Log("Total Money in pot: " + pot.MoneyInPot);
        }

        List<Pot> removeZeroMoneyPots = new List<Pot>(); //for situations that one player goes all in - other players do not bet any further
        foreach (var pot in pots)
        {
            if (pot.MoneyInPot == 0)
            {
                Debug.Log("pot removed:  " + pot);
                continue;
            }
            removeZeroMoneyPots.Add(pot);
        }
        pots = removeZeroMoneyPots;

        return pots;
    }

    public void ResetForTheNewRound()
    {
        CurrentHighestBetAmount = 0;
        showdownPots?.Clear();
        potContributionDictionary?.Clear();
        BaseRaiseBetAmount = 10;
    }

}

[Serializable]
public class Pot
{
    public int potLimit = int.MaxValue; //lets assume no one is all in, everyone is eligible for the main pot while there is no real pot limit set
    public int MoneyInPot = 0;
    public bool isLastPot = true; //contribution amount is a factor to be eligible if false

    public List<PlayerManager> _eligiblePlayerList;


    public void AddEligiblePlayer(PlayerManager player)
    {
        if (_eligiblePlayerList == null)
        {
            _eligiblePlayerList = new List<PlayerManager>();
        }
        if (player != null && !_eligiblePlayerList.Contains(player))
        {
            _eligiblePlayerList.Add(player);
        }
    }
 
}
