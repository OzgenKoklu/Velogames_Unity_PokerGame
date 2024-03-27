using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<CardSO> CommunityCards;

    [SerializeField] private PokerPlayerHand _playerHand;
    [SerializeField] private PokerPlayerHand _aiPlayerOneHand;
    [SerializeField] private PokerPlayerHand _aiPlayerTwoHand;
    [SerializeField] private PokerPlayerHand _aiPlayerThreeHand;
    [SerializeField] private PokerPlayerHand _aiPlayerFourHand;
    private List<PokerPlayerHand> _playerHands;

    private void Start()
    {
        CommunityCards = new List<CardSO>();
        InitializePlayers();
        DrawInitialCommunityCards();
        DealCardsToPlayers();
    }

    private void DrawInitialCommunityCards()
    {
        //draw three community cards
        CommunityCards.Add(DrawCardFromDeck());
        CommunityCards.Add(DrawCardFromDeck());
        CommunityCards.Add(DrawCardFromDeck());
    }

    private void DealCardsToPlayers()
    {
        //Draws 2 cards for each player. 
        foreach (PokerPlayerHand hand in _playerHands)
        {
            CardSO card1 = DrawCardFromDeck(); // Draw the first card
            CardSO card2 = DrawCardFromDeck(); // Draw the second card

            if (card1 != null)
                hand.AddCardToHand(card1); // Add the first card to the player's hand
            if (card2 != null)
                hand.AddCardToHand(card2); // Add the second card to the player's hand
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
}
