using System.Collections.Generic;
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

    public int CurrentPot
    {
        get => _currentPot;
        set => _currentPot = value;
    }
    [SerializeField] private int _currentPot = 0;

    public Pot MainPot
    {
        get => _mainPot;
        set => _mainPot = value;
    }
    [SerializeField] private Pot _mainPot;

    public List<Pot> SidePots
    {
        get => _sidePots;
        set => _sidePots = value;
    }
    private List<Pot> _sidePots;

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
        _currentPot = 0;
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
            GameManager.Instance.SetGameState(GameManager.GameState.PreFlop);
        }
    }

    public void SetBet(PlayerManager player, int betAmount)
    {
        player.BetAmount += betAmount;
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
            if (player.BetAmount != CurrentHighestBetAmount) return false;
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
        Debug.Log("collecting bets.");
        int betAmount = player.BetAmount;
        _currentPot += betAmount;
        player.TotalStackAmount -= player.BetAmount;
        player.TotalBetInThisRound += betAmount;
        player.BetAmount = 0;

        if (player.IsPlayerAllIn)
        {
            //if (!IsThereAnySidePots())
            {

            }
        }
    }

   
}

public class Pot 
{
    private int _potContributionLimit; //represent individual contribution limit to the pot, set by the all in player
    private int _potCurrency; //total amount of money in the pot, including the folded players money
    private List<PlayerManager> _eligiblePlayerList; //All Players who are not folded and also paid at least the amount of the _potContributionLimit

    public void SetPotLimit(int potLimit)
    {
        _potContributionLimit = potLimit;
    }
    public int GetPotLimit()
    {
        return _potContributionLimit;
    }

    public void CollectBetsToPot(int betAmount, out int remainder)
    {
        if(betAmount > _potContributionLimit)
        {
            _potCurrency += _potContributionLimit;
            remainder = betAmount - _potContributionLimit;
        }
        else
        {
            _potCurrency += betAmount;
            remainder = 0;
        }
    }

    public List<PlayerManager> PlayersEligibleForThisPot()
    {
        return _eligiblePlayerList;
    }

    public void AddEligiblePlayer(PlayerManager player)
    {
        _eligiblePlayerList.Add(player);
    }

    public bool IsPlayerEligibleForThisPot(PlayerManager player)
    {
        if (player.IsFolded) return false;
        if(player.TotalBetInThisRound >= _potContributionLimit) return true;      
        return false;
    }
}
