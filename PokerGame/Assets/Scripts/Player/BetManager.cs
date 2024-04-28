using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

    private GameManager.GameState _currentState;
    [SerializeField] public List<Pot> showdownPots;

    private void Awake()
    {
        Instance = this;
        _tempPot = 0;
        showdownPots = new List<Pot>();
    }

    private void Start()
    {
        GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
        DealerManager.Instance.OnDealerChanged += DealerManager_OnDealerChanged;
    }
    private void GameManager_OnGameStateChanged(GameManager.GameState state)
    {
        _currentState = state;

        if (state == GameManager.GameState.Showdown)
        {
            showdownPots = DevideIntoPots();

        }
    }

    private void DealerManager_OnDealerChanged(PlayerManager dealerPlayer)
    {
        if (_currentState == GameManager.GameState.NewRound)
        {
            SetBet(DealerManager.Instance.GetSmallBlind(), _baseRaiseAmount / 2);
            DealerManager.Instance.GetSmallBlind().TotalBetInThisRound = _baseRaiseAmount / 2;//rotus cekelim
            SetBet(DealerManager.Instance.GetBigBlind(), _baseRaiseAmount);
            DealerManager.Instance.GetBigBlind().TotalBetInThisRound = _baseRaiseAmount; //rotus cekelim
            _currentHighestBetAmount = DealerManager.Instance.GetBigBlind().BetAmount;
            potContributionDictionary = new Dictionary<PlayerManager, int>();
            GameManager.Instance.SetGameState(GameManager.GameState.PreFlop);


        }
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

        //Will take the current potContributionDictionary. 

        //There are 5 players in this dictionary. We will check the amount they put to the pot. We will also check wheter they are IsPlayerAllIn or IsPlayerFolded.

        //From there onwards we will create main pots and side pots if necessary. 


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

    public bool IsPlayerEligibleForThisPot(PlayerManager player, Pot pot)
    {
        if (player.IsFolded) return false;
        if (player.TotalBetInThisRound >= pot.potLimit) return true;
        return false;
    }
}
