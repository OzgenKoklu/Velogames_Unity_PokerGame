using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;

public class BetManager : MonoBehaviour
{
    public static BetManager Instance { get; private set; }

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

    private void Awake()
    {
        Instance = this;
        _tempPot = 0;
    }

    private void Start()
    {
        GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
        DealerManager.Instance.OnDealerChanged += DealerManager_OnDealerChanged;
    }
    private void GameManager_OnGameStateChanged(GameManager.GameState state)
    {
        _currentState = state;
    }

    private void DealerManager_OnDealerChanged(PlayerManager dealerPlayer)
    {
        if (_currentState == GameManager.GameState.NewRound)
        {
            SetBet(DealerManager.Instance.GetSmallBlind(), _baseRaiseAmount / 2);
            SetBet(DealerManager.Instance.GetBigBlind(), _baseRaiseAmount);
            _currentHighestBetAmount = DealerManager.Instance.GetBigBlind().BetAmount;
            potContributionDictionary = new Dictionary<PlayerManager, int>();
            GameManager.Instance.SetGameState(GameManager.GameState.PreFlop);
            
           
        }
    }

    public void SetBet(PlayerManager player, int betAmount)
    {
        player.BetAmount = betAmount;
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

    public void DevideIntoPots()
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

            if (player.IsFolded)
            { //no pots are defined by folded players

            }
            else if (player.IsPlayerAllIn)
            {
                int indexOfValue = potDefiningBetValues.IndexOf(playerBetAmount);
                if (indexOfValue == -1) //If same value exists do not need an extra pot
                {
                    potDefiningBetValues.Add(playerBetAmount);
                    Debug.Log("Pot defining value added. " + playerBetAmount);
                }
            }
            int playerBet = playerContribution.Value;

        }

        List<Pot> pots = new List<Pot>();

        potDefiningBetValues.Sort();

        foreach(var intiger in potDefiningBetValues)
        {
            Debug.Log("int: " + intiger);
        }




        // Loop through players in contribution dictionary
       
    }
}

public class Pot
{
    public int potLimit = 0;
    public int MoneyInPot = 0;
  
    public List<PlayerManager> _eligiblePlayerList;


    public void AddEligiblePlayer(PlayerManager player)
    {
        if(player != null && !_eligiblePlayerList.Contains(player))
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
