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

    public int PotInThisSession
    {
        get => _potInThisSession;
        set => _potInThisSession = value;
    }
    [SerializeField] private int _potInThisSession = 0;

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
        _potInThisSession = 0;
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

    public void CollectBets()
    {
        Debug.Log("collecting bets.");
        var activePlayers = GameManager.Instance.ActivePlayers;

        //handle all in players here
        foreach (var player in activePlayers)
        {
            _potInThisSession += player.BetAmount;
            player.TotalStackAmount -= player.BetAmount;
            player.BetAmount = 0;
        }
    }
}