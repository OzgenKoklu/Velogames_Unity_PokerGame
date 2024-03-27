using System.Collections.Generic;
using UnityEngine;

public class PokerDeckManager : MonoBehaviour
{
    //deckSO is to initialize the card deck list
    [SerializeField] private DeckSO _deckSO;
    private List<CardSO> _pokerDeck = new List<CardSO>();
    private List<CardSO> _discardPile = new List<CardSO>();
    public static PokerDeckManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
        LoadDeck();
        ShuffleDeck();
    }
    void Start()
    {
        
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
}
