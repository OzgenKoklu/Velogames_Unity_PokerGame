using pheval;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static PokerHandEvaluator;

public class PokerDeckManager : MonoBehaviour
{
    public static PokerDeckManager Instance { get; private set; }

    private List<CardSO> _pokerDeck = new();
    private List<CardSO> _discardPile = new();

    //deckSO is to initialize the card deck list
    [SerializeField] private DeckSO _deckSO;

    [SerializeField] private CommunityCards _communityCards;
    [SerializeField] private PokerPlayerHand _playerHand;
    [SerializeField] private PokerPlayerHand _aiPlayerOneHand;
    [SerializeField] private PokerPlayerHand _aiPlayerTwoHand;
    [SerializeField] private PokerPlayerHand _aiPlayerThreeHand;
    [SerializeField] private PokerPlayerHand _aiPlayerFourHand;

    [SerializeField] private GameObject _communityCardsGameObject;
    private List<PokerPlayerHand> _playerHands;


    private void Awake()
    {
        Instance = this;
        LoadDeck();
        InitializePlayers();
        ShuffleDeck();
    }

    private void Start()
    {
        GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
    }

    private void GameManager_OnGameStateChanged(GameManager.GameState obj)
    {
        if (obj == GameManager.GameState.NewRound)
        {
            DealCardsToPlayers();
            //bu yaklaþým sýkýntýlý çünkü diðer scriptlerdeki "newRound'da olmasý gereken iþlemlerin tamamlandýðýna emin olmadan state'i ilerletiyor. 
            //Preflop'a geçiren aþama BetManager'dayken buradaki kart daðýtým iþlemleri olmadan ilerliyordu. Belki courotine baþlatýlmalý ve böylece paralel olarak ilerlemeli bazý iþlemler.
            return;
        }

        if (obj == GameManager.GameState.PreFlop)
        {


            return;
        }

        if (obj == GameManager.GameState.Flop)
        {
            DrawInitialCommunityCards(); //normally should draw in flop. Lets draw and hide for now. 

            //!!! Bu state change'in burda iþi yok belki game manager'ýn takip ettiði bir event triggerlanmalý ve o tetiklemeli.
            //bu yaklaþým sýkýntýlý çünkü turn manager'daki scriptlerdeki "newRound'da olmasý gereken iþlemlerin tamamlandýðýna emin olmadan state'i ilerletiyor. 
            //Turn manager'da kalsa muhtemelen 3 community card çekilmeden playerTurn'e geçirirdi.

            GameManager.Instance.SetGameState(GameManager.GameState.PostFlop);
            GameManager.Instance.SetGameState(GameManager.GameState.PlayerTurn);

            return;
        }

        if (obj == GameManager.GameState.Turn)
        {
            //draw 4th card for community cards
            DrawOneMoreCommunityCard();

            GameManager.Instance.SetGameState(GameManager.GameState.PostTurn);
            GameManager.Instance.SetGameState(GameManager.GameState.PlayerTurn);

            return;
        }

        if (obj == GameManager.GameState.River)
        {
            //draw 5th card for community cards
            DrawOneMoreCommunityCard();

            GameManager.Instance.SetGameState(GameManager.GameState.PostRiver);
            GameManager.Instance.SetGameState(GameManager.GameState.PlayerTurn);
            return;
        }

        if (obj == GameManager.GameState.GameOver)
        {
            //reset the deck
            ResetDeck();
            return;
        }
    }

    // Load all cards from DeckSO into the deck List
    void LoadDeck()
    {
        foreach (CardSO card in _deckSO.Cardlist)
        {
            _pokerDeck.Add(card);
        }
    }

    // Shuffle the deck with random index'
    void ShuffleDeck()
    {
        for (int i = 0; i < _pokerDeck.Count; i++)
        {
            CardSO temp = _pokerDeck[i];
            int randomIndex = Random.Range(0, _pokerDeck.Count);
            _pokerDeck[i] = _pokerDeck[randomIndex];
            _pokerDeck[randomIndex] = temp;
        }
    }

    // Draw a card from the deck
    public CardSO DrawCard()
    {
        if (_pokerDeck.Count > 0)
        {
            CardSO cardToDraw = _pokerDeck[0];
            _pokerDeck.RemoveAt(0); // Remove the card from the deck
            _discardPile.Add(cardToDraw); // add it to a discard pile
            return cardToDraw;
        }
        else
        {
            Debug.LogError("No more cards in the deck.");
            return null; // Return null if there are no cards left
        }
    }

    public void ResetDeck()
    {
        _pokerDeck.AddRange(_discardPile); // Add all cards from discard pile back to the deck
        _discardPile.Clear(); // Clear the discard pile
        ShuffleDeck(); // Shuffle the deck
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
    private void DrawOneMoreCommunityCard()
    {
        CardSO card = DrawCardFromDeck();
        if (card != null)
        {
            _communityCards.AddCard(card);
            CardVisualsManager.Instance.SpawnCardObject(card, card.CardParent);
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
        return DrawCard();
    }
    private void InitializePlayers()
    {
        _playerHands = new List<PokerPlayerHand>
        {
            _aiPlayerOneHand,
            _aiPlayerTwoHand,
            _playerHand, // Index 2 = player
            _aiPlayerThreeHand,
            _aiPlayerFourHand
        };
    }

    public List<PokerPlayerHand> GetPlayerHands()
    {
        return _playerHands;
    }

    public List<List<CardSO>> GetAllPlayerHands()
    {
        List<List<CardSO>> allHands = new List<List<CardSO>>();

        // Add each player's combined hand to the list

        allHands.Add(_aiPlayerOneHand.GetCardListWithCommunityCardsAdded());
        allHands.Add(_aiPlayerTwoHand.GetCardListWithCommunityCardsAdded());
        allHands.Add(_playerHand.GetCardListWithCommunityCardsAdded()); // index 2
        allHands.Add(_aiPlayerThreeHand.GetCardListWithCommunityCardsAdded());
        allHands.Add(_aiPlayerFourHand.GetCardListWithCommunityCardsAdded());

        return allHands;
    }

    public List<CardSO> CombineHandWithCommunityCards(List<CardSO> playerHand)
    {
        List<CardSO> communityCards = _communityCards.GetCardList();
        List<CardSO> combinedHand = new List<CardSO>(communityCards);
        combinedHand.AddRange(playerHand); 
        //first cards are always community cards,
        //this is important in winning player selection algoritm. (for tie situations)
        return combinedHand;
    }
}
