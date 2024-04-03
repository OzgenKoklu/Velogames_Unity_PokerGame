using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PokerHandEvaluator : MonoBehaviour
{
    public enum HandRank
    {
        HighCard,
        OnePair,
        TwoPair,
        ThreeOfAKind,
        Straight,
        Flush,
        FullHouse,
        FourOfAKind,
        StraightFlush,
        RoyalFlush
    }

    public static PokerHandEvaluator Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }


    public HandRank EvaluateHand(List<CardSO> hand)
    {
        if (IsRoyalFlush(hand)) return HandRank.RoyalFlush;
        if (IsStraightFlush(hand)) return HandRank.StraightFlush;
        if (IsFourOfAKind(hand)) return HandRank.FourOfAKind;
        if (IsFullHouse(hand)) return HandRank.FullHouse;
        if (IsFlush(hand)) return HandRank.Flush;
        if (IsStraight(hand)) return HandRank.Straight;
        if (IsThreeOfAKind(hand)) return HandRank.ThreeOfAKind;
        if (IsTwoPair(hand)) return HandRank.TwoPair;
        if (IsOnePair(hand)) return HandRank.OnePair;
        return HandRank.HighCard;
    }

    public int EvaluateAndFindWinner(List<List<CardSO>> playerHands)
    {
        int winnerIndex = -1;
        HandRank bestRank = HandRank.HighCard;
        List<CardSO> bestHandCards = null; // Holds the best hand cards for comparison in ties

        Debug.Log("Evaluating hands for all players...");

        for (int i = 0; i < playerHands.Count; i++)
        {
            List<CardSO> currentHand = playerHands[i];
            HandRank currentRank = EvaluateHand(currentHand);
            Debug.Log($"Player {i + 1} has a hand of rank: {currentRank}");

            if (currentRank > bestRank)
            {
                bestRank = currentRank;
                winnerIndex = i;
                bestHandCards = currentHand; // Update the best hand cards for tiebreaker comparisons
                Debug.Log($"New best hand found! Player {i + 1} takes the lead with a {currentRank}.");
            }
            else if (currentRank == bestRank)
            {
                // Tiebreaker comparison needed
                Debug.Log($"Tie detected for rank {currentRank}. Comparing hands for player {i + 1} and current best hand.");
                if (CompareHandsForTiebreaker(bestHandCards, currentHand) < 0)
                {
                    Debug.Log($"Player {i + 1} wins the tiebreaker against the previous best hand.");
                    winnerIndex = i;
                    bestHandCards = currentHand; // Update the best hand cards after winning the tie
                }
                else
                {
                    Debug.Log($"Player {i + 1} does not win the tiebreaker. Previous best hand remains leading.");
                }
            }
        }

        if (winnerIndex != -1)
        {
            Debug.Log($"Winner determined: Player {winnerIndex + 1} with a {bestRank}.");
        }
        else
        {
            Debug.Log("No winner could be determined.");
        }

        return winnerIndex;
    }

    private int CompareHandsForTiebreaker(List<CardSO> hand1, List<CardSO> hand2)
    {
        if(hand1 == null)
        {
            Debug.Log("Player 1 High Card is the best hand for now.");
            return -1; // Hand 2 wins by being the only hand at hand
        }
        // Logging the cards in each hand
        Debug.Log("Comparing hands for tiebreaker:");
        Debug.Log($"Hand 1 Cards: {string.Join(", ", hand1.Select(card => $"{card.Value} of {card.Suit}"))}");
        Debug.Log($"Hand 2 Cards: {string.Join(", ", hand2.Select(card => $"{card.Value} of {card.Suit}"))}");

        // Assume CardSO has a 'Value' property that can be compared directly
        // This loop assumes the hands are sorted in descending order of card value
        for (int i = 0; i < hand1.Count; i++)
        {
            if (i >= hand2.Count)
            {
                Debug.Log("Hand 1 has more cards than Hand 2, automatically wins the tiebreaker.");
                return 1; // Hand 1 wins by having more cards (in cases where hand size varies)
            }

            if (hand1[i].Value > hand2[i].Value)
            {
                Debug.Log($"Hand 1 wins tiebreaker with {hand1[i].Value} over {hand2[i].Value}.");
                return 1; // Hand 1 wins the tiebreaker
            }
            else if (hand1[i].Value < hand2[i].Value)
            {
                Debug.Log($"Hand 2 wins tiebreaker with {hand2[i].Value} over {hand1[i].Value}.");
                return -1; // Hand 2 wins the tiebreaker
            }
            // If the cards are equal, continue to the next card
        }

        if (hand1.Count < hand2.Count)
        {
            Debug.Log("Hand 2 has more cards than Hand 1, automatically wins the tiebreaker.");
            return -1; // Hand 2 wins by having more cards
        }

        // If all compared cards are equal and the hands are of the same size, the hands tie
        Debug.Log("Hands are equal after full comparison.");
        return 0;
    }

    private bool IsRoyalFlush(List<CardSO> hand)
    {
        return IsStraightFlush(hand) && hand.All(card => card.Value >= CardRank.Ten);
    }

    private bool IsStraightFlush(List<CardSO> hand)

    {
        return IsFlush(hand) && IsStraight(hand);
    }

    private bool IsFourOfAKind(List<CardSO> hand)
    {
        var rankGroups = hand.GroupBy(card => card.Value);
        return rankGroups.Any(group => group.Count() == 4);
    }

    private bool IsFullHouse(List<CardSO> hand)
    {
        var rankGroups = hand.GroupBy(card => card.Value);
        return rankGroups.Any(group => group.Count() == 3) && rankGroups.Any(group => group.Count() == 2);
    }

    private bool IsFlush(List<CardSO> hand)
    {
        return hand.GroupBy(card => card.Suit).Count() == 1;
    }

    private bool IsStraight(List<CardSO> hand)
    {
        var sortedRanks = hand.Select(card => (int)card.Value).OrderBy(rank => rank).ToList();
        if (sortedRanks.Last() == (int)CardRank.Ace && sortedRanks.First() == (int)CardRank.Two)
        {
            // Handle A-2-3-4-5 as a valid straight (wheel)
            sortedRanks.Remove(sortedRanks.Last());
            sortedRanks.Insert(0, 1);
        }
        for (int i = 1; i < sortedRanks.Count; i++)
        {
            if (sortedRanks[i] != sortedRanks[i - 1] + 1)
            {
                return false;
            }
        }
        return true;
    }

    private bool IsThreeOfAKind(List<CardSO> hand)
    {
        var rankGroups = hand.GroupBy(card => card.Value);
        return rankGroups.Any(group => group.Count() == 3);
    }

    private bool IsTwoPair(List<CardSO> hand)
    {
        var rankGroups = hand.GroupBy(card => card.Value);
        return rankGroups.Count(group => group.Count() == 2) == 2;
    }

    private bool IsOnePair(List<CardSO> hand)
    {
        var rankGroups = hand.GroupBy(card => card.Value);
        return rankGroups.Any(group => group.Count() == 2);
    }
}
