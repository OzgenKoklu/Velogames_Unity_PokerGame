using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class GameManager : MonoBehaviour
{
    [SerializeField] private CommunityCards _communityCards;
    [SerializeField] private PokerPlayerHand _playerHand;
    [SerializeField] private PokerPlayerHand _aiPlayerOneHand;
    [SerializeField] private PokerPlayerHand _aiPlayerTwoHand;
    [SerializeField] private PokerPlayerHand _aiPlayerThreeHand;
    [SerializeField] private PokerPlayerHand _aiPlayerFourHand;
    [SerializeField] private CardVisualsManager _cardVisualManager;
    private List<PokerPlayerHand> _playerHands;

    private void Start()
    {     
        InitializePlayers();
        DrawInitialCommunityCards();
        DealCardsToPlayers();
    }

    private void DrawInitialCommunityCards()
    {
        CardSO card1 = DrawCardFromDeck();
        CardSO card2 = DrawCardFromDeck();
        CardSO card3 = DrawCardFromDeck();

        if (card1 != null)
        {
            _communityCards.AddCard(card1);
           _cardVisualManager.SpawnCardObject(card1, _communityCards.GetCardParent());

        }
        if (card2 != null)
        {
            _communityCards.AddCard(card2);
            _cardVisualManager.SpawnCardObject(card2, _communityCards.GetCardParent());
        }
        if (card3 != null)
        {
            _communityCards.AddCard(card3);
            _cardVisualManager.SpawnCardObject(card3, _communityCards.GetCardParent());
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
            _cardVisualManager.SpawnCardObject(card1, hand.GetCardParent());
                
            if (card2 != null)
                hand.AddCard(card2); // Add the second card to the player's hand
            _cardVisualManager.SpawnCardObject(card2, hand.GetCardParent());
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
