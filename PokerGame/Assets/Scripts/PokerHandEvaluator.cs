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
        // Additional information might be needed for tiebreakers, depending on your implementation

        Debug.Log("Evaluating hands for all players...");

        for (int i = 0; i < playerHands.Count; i++)
        {
            HandRank currentRank = EvaluateHand(playerHands[i]);

            Debug.Log($"Player {i + 1} has a hand of rank: {currentRank}");
            if (currentRank > bestRank)
            {
                bestRank = currentRank;
                winnerIndex = i;
                Debug.Log($"New best hand found! Player {i + 1} takes the lead with a {currentRank}.");
            }
            // Implement tiebreaker logic here if currentRank == bestRank
        }

        if (winnerIndex != -1)
        {
            Debug.Log($"Winner determined: Player {winnerIndex + 1} with a {bestRank}.");
        }
        else
        {
            Debug.Log("No winner could be determined.");
        }

        return winnerIndex; // Returns the index of the winning player, or -1 if not determined
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
