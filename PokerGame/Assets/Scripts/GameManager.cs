using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public event Action OnGameStarted;
    public event Action OnPreFlop;

    public enum GameState
    {
        PreFlop,        // Before the flop (first three community cards) is dealt
        Flop,           // The flop is dealt
        PostFlop,       // Betting round after the flop
        Turn,           // The turn (fourth community card) is dealt
        PostTurn,       // Betting round after the turn
        River,          // The river (fifth community card) is dealt
        PostRiver,      // Final betting round after the river
        Showdown,       // Players reveal their hands to determine the winner
        PotDistribution,// The pot is distributed to the winner(s)
        NewRound,       // Setup for a new round of poker
        PlayerTurn,     // A player's turn to act
        GameOver        // The game is over
    }

    private GameState _currentState;
    private bool _isGameStarted = false;

    [SerializeField] private CommunityCards _communityCards;
    [SerializeField] private PokerPlayerHand _playerHand;
    [SerializeField] private PokerPlayerHand _aiPlayerOneHand;
    [SerializeField] private PokerPlayerHand _aiPlayerTwoHand;
    [SerializeField] private PokerPlayerHand _aiPlayerThreeHand;
    [SerializeField] private PokerPlayerHand _aiPlayerFourHand;
    private List<PokerPlayerHand> _playerHands;

    public List<PlayerManager> Players => _players;
    [SerializeField] private List<PlayerManager> _players;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializePlayers();
        StartGame();
        DrawInitialCommunityCards();
        PokerHandEvaluator.Instance.EvaluateAndFindWinner(GetAllPlayerHands());
    }

    public void StartGame()
    {
        _isGameStarted = true;
        OnGameStarted?.Invoke();
        Debug.Log("Ongamestarted");
    }

    public bool IsGameStarted()
    {
        return _isGameStarted;
    }

    public void SetState(GameState newState)
    {
        _currentState = newState;
    }

    public GameState GetState()
    {
        return _currentState;
    }

    public void StartPreFlop()
    {
        _currentState = GameState.PreFlop;
        DealCardsToPlayers();

        DealerManager.Instance.GetBigBlind().BetAmount = 5;
        DealerManager.Instance.GetSmallBlind().BetAmount = 10;

        BetManager.SetBet(DealerManager.Instance.GetDealerPlayer(), betAmount: 10);
        TurnManager.Instance.SetPlayerTurn();
    }

    private void DrawInitialCommunityCards()
    {
        CardSO card1 = DrawCardFromDeck();
        CardSO card2 = DrawCardFromDeck();
        CardSO card3 = DrawCardFromDeck();

        if (card1 != null)
        {
            _communityCards.AddCard(card1);
            CardVisualsManager.Instance.SpawnCardObject(card1, card1.CardParent);

        }
        if (card2 != null)
        {
            _communityCards.AddCard(card2);
            CardVisualsManager.Instance.SpawnCardObject(card2, card2.CardParent);
        }
        if (card3 != null)
        {
            _communityCards.AddCard(card3);
            CardVisualsManager.Instance.SpawnCardObject(card3, card3.CardParent);
        }
    }

    private void DealCardsToPlayers()
    {
        //Draws 2 cards for each player. 
        foreach (PokerPlayerHand hand in _playerHands)
        {
            CardSO card1 = DrawCardFromDeck(); // Draw the first card
            CardSO card2 = DrawCardFromDeck(); // Draw the second card

            if (card1 != null)
                hand.AddCard(card1); // Add the first card to the player's hand
            CardVisualsManager.Instance.SpawnCardObject(card1, card1.CardParent);

            if (card2 != null)
                hand.AddCard(card2); // Add the second card to the player's hand
            CardVisualsManager.Instance.SpawnCardObject(card2, card2.CardParent);
        }
    }

    private CardSO DrawCardFromDeck()
    {
        return PokerDeckManager.Instance.DrawCard();
    }
    private void InitializePlayers()
    {
        _playerHands = new List<PokerPlayerHand>
        {
            _playerHand,
            _aiPlayerOneHand,
            _aiPlayerTwoHand,
            _aiPlayerThreeHand,
            _aiPlayerFourHand
        };
    }

    public List<List<CardSO>> GetAllPlayerHands()
    {
        List<List<CardSO>> allHands = new List<List<CardSO>>();
        List<CardSO> communityCards = _communityCards.GetCardList();

        // Add each player's combined hand to the list
        allHands.Add(CombineHandWithCommunity(_playerHand.GetCardList(), communityCards));
        allHands.Add(CombineHandWithCommunity(_aiPlayerOneHand.GetCardList(), communityCards));
        allHands.Add(CombineHandWithCommunity(_aiPlayerTwoHand.GetCardList(), communityCards));
        allHands.Add(CombineHandWithCommunity(_aiPlayerThreeHand.GetCardList(), communityCards));
        allHands.Add(CombineHandWithCommunity(_aiPlayerFourHand.GetCardList(), communityCards));

        return allHands;
    }

    private List<CardSO> CombineHandWithCommunity(List<CardSO> playerHand, List<CardSO> communityCards)
    {
        List<CardSO> combinedHand = new List<CardSO>(playerHand);
        combinedHand.AddRange(communityCards);
        return combinedHand;
    }
}
