using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokerDeckManager : MonoBehaviour
{
    public static PokerDeckManager Instance { get; private set; }

    public event Action OnCardDealingComplete;

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
    }

    private void Start()
    {
        GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
    }

    private void GameManager_OnGameStateChanged(GameManager.GameState state)
    {
        if (state == GameManager.GameState.NewRound)
        {
            ResetDeck(); //shuffles, adds the discard pile to the deck
            DealCardsToPlayers();
            //bu yaklaþým sýkýntýlý çünkü diðer scriptlerdeki "newRound'da olmasý gereken iþlemlerin tamamlandýðýna emin olmadan state'i ilerletiyor. 
            //Preflop'a geçiren aþama BetManager'dayken buradaki kart daðýtým iþlemleri olmadan ilerliyordu. Belki courotine baþlatýlmalý ve böylece paralel olarak ilerlemeli bazý iþlemler.
            return;
        }

        if (state == GameManager.GameState.Flop)
        {
            DrawInitialCommunityCards(); //normally should draw in flop. Lets draw and hide for now. 

            //!!! Bu state change'in burda iþi yok belki game manager'ýn takip ettiði bir event triggerlanmalý ve o tetiklemeli.
            //bu yaklaþým sýkýntýlý çünkü turn manager'daki scriptlerdeki "newRound'da olmasý gereken iþlemlerin tamamlandýðýna emin olmadan state'i ilerletiyor. 
            //Turn manager'da kalsa muhtemelen 3 community card çekilmeden playerTurn'e geçirirdi.

            GameManager.Instance.SetGameState(GameManager.GameState.PostFlop);
            GameManager.Instance.SetGameState(GameManager.GameState.PlayerTurn);

            return;
        }

        if (state == GameManager.GameState.Turn)
        {
            //draw 4th card for community cards
            DrawOneMoreCommunityCard();

            GameManager.Instance.SetGameState(GameManager.GameState.PostTurn);
            GameManager.Instance.SetGameState(GameManager.GameState.PlayerTurn);

            return;
        }

        if (state == GameManager.GameState.River)
        {
            //draw 5th card for community cards
            DrawOneMoreCommunityCard();

            GameManager.Instance.SetGameState(GameManager.GameState.PostRiver);
            GameManager.Instance.SetGameState(GameManager.GameState.PlayerTurn);
            return;
        }
    }

    // Load all cards from DeckSO into the deck List
    private void LoadDeck()
    {
        foreach (CardSO card in _deckSO.Cardlist)
        {
            _pokerDeck.Add(card);
        }
    }

    // Shuffle the deck with random index'
    private void ShuffleDeck()
    {
        for (int i = 0; i < _pokerDeck.Count; i++)
        {
            CardSO temp = _pokerDeck[i];
            int randomIndex = UnityEngine.Random.Range(0, _pokerDeck.Count);
            _pokerDeck[i] = _pokerDeck[randomIndex];
            _pokerDeck[randomIndex] = temp;
        }
    }

    // Draw a card from the deck
    private CardSO DrawCard()
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

    private void ResetDeck()
    {
        bool anySpawnedCards = CardVisualsManager.Instance.IsThereAnySpawnedCards();
        if (anySpawnedCards)
        {
            CardVisualsManager.Instance.DestroyAllActiveCards();
        }
        _communityCards.ClearCards();
        foreach (var playerHand in _playerHands)
        {
            if (playerHand.HoleCardsList.Count != 0)
            {
                playerHand.ClearCards();
            }
        }

        _pokerDeck.AddRange(_discardPile); // Add all cards from discard pile back to the deck
        _discardPile.Clear(); // Clear the discard pile
        ShuffleDeck(); // Shuffle the deck

    }

    private void DrawInitialCommunityCards()
    {
        StartCoroutine(DrawCommunityCardsCoroutine());
    }

    private IEnumerator DrawCommunityCardsCoroutine()
    {

        CardSO card1 = DrawCardFromDeck();
        if (card1 != null)
        {
            _communityCards.AddCard(card1);
            CardVisualsManager.Instance.SpawnCardObject(card1, card1.CardParent);
            yield return new WaitUntil(() => CardVisualsManager.Instance.IsCardLerpComplete(card1));
        }

        CardSO card2 = DrawCardFromDeck();
        if (card2 != null)
        {
            _communityCards.AddCard(card2);
            CardVisualsManager.Instance.SpawnCardObject(card2, card2.CardParent);
            yield return new WaitUntil(() => CardVisualsManager.Instance.IsCardLerpComplete(card2));
        }

        CardSO card3 = DrawCardFromDeck();
        if (card3 != null)
        {
            _communityCards.AddCard(card3);
            CardVisualsManager.Instance.SpawnCardObject(card3, card3.CardParent);
            yield return new WaitUntil(() => CardVisualsManager.Instance.IsCardLerpComplete(card3));
        }
        OnCardDealingComplete?.Invoke();

    }

    private void DrawOneMoreCommunityCard()
    {
        StartCoroutine(DrawOneMoreCommunityCardCoroutine());
    }

    private IEnumerator DrawOneMoreCommunityCardCoroutine()
    {
        CardSO card = DrawCardFromDeck();
        if (card != null)
        {
            _communityCards.AddCard(card);
            CardVisualsManager.Instance.SpawnCardObject(card, card.CardParent);
            yield return new WaitUntil(() => CardVisualsManager.Instance.IsCardLerpComplete(card));
        }
    }

    private void DealCardsToPlayers()
    {
        StartCoroutine(DealCardsToPlayersCoroutine());
    }

    private IEnumerator DealCardsToPlayersCoroutine()
    {
        foreach (PokerPlayerHand hand in _playerHands)
        {
            CardSO card1 = DrawCardFromDeck();
            if (card1 != null)
            {
                hand.AddCard(card1);
                CardVisualsManager.Instance.SpawnCardObject(card1, card1.CardParent);
                yield return new WaitUntil(() => CardVisualsManager.Instance.IsCardLerpComplete(card1));
            }

            CardSO card2 = DrawCardFromDeck();
            if (card2 != null)
            {
                hand.AddCard(card2);
                CardVisualsManager.Instance.SpawnCardObject(card2, card2.CardParent);
                yield return new WaitUntil(() => CardVisualsManager.Instance.IsCardLerpComplete(card2));
            }
        }
        OnCardDealingComplete?.Invoke();
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

    public List<CardSO> CombineHandWithCommunityCards(List<CardSO> playerHand)
    {
        List<CardSO> communityCards = _communityCards.GetCardList();
        List<CardSO> combinedHand = new List<CardSO>(communityCards);
        combinedHand.AddRange(playerHand);
        //first cards are always community cards,
        //this is important in winning player selection algoritm. (for tie situations)
        return combinedHand;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
        Instance = null;
    }
}
