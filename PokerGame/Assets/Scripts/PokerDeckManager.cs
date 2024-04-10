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
    private List<PokerPlayerHand> _playerHands;

    
    private void Awake()
    {
        Instance = this;
        LoadDeck();
        ShuffleDeck();
    }

    private void Start()
    {
        GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
    }

    private void GameManager_OnGameStateChanged(GameManager.GameState obj)
    {
        if(obj == GameManager.GameState.NewRound)
        {
            OnGameStart();
        }
    }

    private void OnGameStart()
    {
        InitializePlayers();
        DrawInitialCommunityCards();
        DealCardsToPlayers();

        //aslýnda direkt tur baþýnda deðil bi tuþa basýp kýyas iþlemi baþlayýnca olmalý, hatta bu winning hand sadece 7 kard açýlýnca filan olmalý.
        List<List<CardSO>> allPlayerCards = GetAllPlayerHands();  

        //evaluate etmeli ama hemen winner seçmemeli. Evaluation skorlarýna göre de player AI'larý bet fold check yapmalý.
        List<int> playerHandRankList = PokerHandEvaluator.Instance.EvaluateHandStrengths(allPlayerCards);


        //turdaki player aksiyonlarý sonrasý burasý olmalý.
        PokerHandEvaluator.WinningHandResults winningHandResult = PokerHandEvaluator.Instance.SelectTheWinner(playerHandRankList);
        HandleWinningHandResult(winningHandResult);
    }

    //bu fonksiyonun deck'Le bi ilgisi yok o yüzden aslýnda game manager'a taþýnmasý mantýklý olabilir. buradan yapýlacak þeylerin oradan yapýlmasý doðru olabilir.
    private void HandleWinningHandResult(PokerHandEvaluator.WinningHandResults winningHandResult)
    {
        Debug.Log("Winning hand type: " + winningHandResult.WinningHandType + "- Player Index(0,1,2,3,4), 0 is the player. : " + winningHandResult.WinningHandIndex + " - Winning Hand(5Cards) Ranks: " + winningHandResult.WinningCardCodes);
        string winningHandType = winningHandResult.WinningHandType;
        int winningHandPlayerIndex = winningHandResult.WinningHandIndex;

        //Show ile UI'da winning hand gösterecek bi mesaj. ShowWinningHand and player Name(indexten çýkartýlýr) 

        string winningHandCardCodes = winningHandResult.WinningCardCodes;
        List<CardSO> WinningCardList = winningHandResult.WinningCardList;
        CardVisualsManager.Instance.HighlightHand(WinningCardList, winningHandCardCodes);
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

        // Add each player's combined hand to the list
        allHands.Add(_playerHand.GetCardListWithCommunityCardsAdded());
        allHands.Add(_aiPlayerOneHand.GetCardListWithCommunityCardsAdded());
        allHands.Add(_aiPlayerTwoHand.GetCardListWithCommunityCardsAdded());
        allHands.Add(_aiPlayerThreeHand.GetCardListWithCommunityCardsAdded());
        allHands.Add(_aiPlayerFourHand.GetCardListWithCommunityCardsAdded());

        return allHands;
    }

    public List<CardSO> CombineHandWithCommunityCards(List<CardSO> playerHand)
    {
        List<CardSO> communityCards = _communityCards.GetCardList();
        List<CardSO> combinedHand = new List<CardSO>(playerHand);
        combinedHand.AddRange(communityCards);
        return combinedHand;
    }
}
