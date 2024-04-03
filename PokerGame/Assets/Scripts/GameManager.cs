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

    private List<PokerPlayerHand> _playerHands;

    private void Start()
    {
        InitializePlayers();
        DrawInitialCommunityCards();
        DealCardsToPlayers();
        PokerHandEvaluator.Instance.EvaluateAndFindWinner(GetAllPlayerHands());
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
