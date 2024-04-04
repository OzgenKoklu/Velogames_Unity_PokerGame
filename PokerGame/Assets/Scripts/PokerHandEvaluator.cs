using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

    public static Dictionary<int, CardRank> PlayerHandsBestCardRank = new Dictionary<int, CardRank>();
    

    public static PokerHandEvaluator Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }


    public HandRank EvaluateHand(List<CardSO> hand, out CardRank playerHandBestCardForBestRank)
    {
        playerHandBestCardForBestRank = CardRank.Two;
        if (IsRoyalFlush(hand))
        {
            playerHandBestCardForBestRank = CardRank.Ace;//playerHandBestCardHand esgeçilebilir, sadece bir çeþit Royal Flush var.
            return HandRank.RoyalFlush;
        }     
        //Buralara eklenmeli ve temizlenmeli. 
        if (IsStraightFlush(hand))return HandRank.StraightFlush;
        if (IsFourOfAKind(hand)) return HandRank.FourOfAKind;
        if (IsFullHouse(hand)) return HandRank.FullHouse;
        if (IsFlush(hand)) return HandRank.Flush;
        if (IsStraight(hand)) return HandRank.Straight;
        if (IsThreeOfAKind(hand, out  playerHandBestCardForBestRank))
        {
            Debug.Log("Three of a kind Hand Rank: " + playerHandBestCardForBestRank.ToString());
            return HandRank.ThreeOfAKind;
        }
        if (IsTwoPair(hand, out  playerHandBestCardForBestRank))
        {
            Debug.Log("Two Pair Hand Rank: " + playerHandBestCardForBestRank.ToString());
            return HandRank.TwoPair;
        }
        if (IsOnePair(hand, out playerHandBestCardForBestRank))
        {
            Debug.Log("One Pair Hand Rank: " + playerHandBestCardForBestRank.ToString());
            return HandRank.OnePair;
        }
        //If hand has just a high card, we should determine the best card at hand:
        var rankGroups = hand.GroupBy(card => card.Value); //neden bunu kodda zibilyon kere yaptýrýyorum da sadece evaluation'ýn baþýnda yapmýyorum. mantýken burda yapsam ve bu þekilde fonksiyonlara yedirsem daha iyi
        playerHandBestCardForBestRank = rankGroups.First().Key;
        Debug.Log("High Card Hand Rank: " + playerHandBestCardForBestRank.ToString());
        return HandRank.HighCard;
    }

    public int EvaluateAndFindWinner(List<List<CardSO>> playerHands)
    {
        int winnerIndex = -1;
        HandRank bestRank = HandRank.HighCard;
        List<int> potentialWinners = new List<int>(); // To track indices of players with the highest rank

        // Step 1: Evaluate each hand and determine the highest rank
        for (int i = 0; i < playerHands.Count; i++)
        {           
            HandRank currentRank = EvaluateHand(playerHands[i], out CardRank playerHandBestCardRank);
            PlayerHandsBestCardRank[i] = playerHandBestCardRank;
            Debug.Log($"Player {i + 1}: {currentRank} with a rank of {PlayerHandsBestCardRank[i]}");

            if (currentRank > bestRank)
            {
                bestRank = currentRank;
                potentialWinners.Clear(); // Clear previous records, as a new best rank has been found
                potentialWinners.Add(i); // Add this player as a potential winner
            }
            else if (currentRank == bestRank)
            {
                potentialWinners.Add(i); // Add this player also as a potential winner
            }
        }

        

        // Step 2: Filter by the highest rank and proceed to tiebreakers if necessary
        if (potentialWinners.Count == 1)
        {
            // Only one player has the highest rank, no tiebreaker needed
            winnerIndex = potentialWinners[0];
        }
        else if (potentialWinners.Count > 1)
        {
            // Multiple players have the highest rank, proceed to tiebreaker
            Debug.Log("Multiple players with the highest rank. Proceeding to tiebreakers.");
            winnerIndex = TiebreakAmongTopHands(potentialWinners, playerHands);
        }

        Debug.Log($"Winner determined: Player {winnerIndex + 1}");
        return winnerIndex;
    }

    private int TiebreakAmongTopHands(List<int> potentialWinners, List<List<CardSO>> playerHands)
    {
        int currentBestIndex = potentialWinners[0]; // Start with the first potential winner
        List<CardSO> currentBestHand = playerHands[currentBestIndex];

        for (int i = 1; i < potentialWinners.Count; i++)
        {
            int currentIndex = potentialWinners[i];
            List<CardSO> currentHand = playerHands[currentIndex];

            // Compare the current best hand with the next contender
            int comparisonResult = CompareHandsForTiebreaker(currentBestHand, currentHand);

            if (comparisonResult > 0)
            {
                // currentBestHand is still the best, no change needed
                Debug.Log($"Player {currentBestIndex + 1}'s hand remains the leading hand.");
            }
            else if (comparisonResult < 0)
            {
                // currentHand takes the lead as the best hand
                Debug.Log($"Player {currentIndex + 1}'s hand takes the lead over Player {currentBestIndex + 1}.");
                currentBestIndex = currentIndex;
                currentBestHand = currentHand; // Update the current best hand
            }
            else
            {
                // If there's a tie, additional logic can be applied here
                Debug.Log($"Player {currentBestIndex + 1} and Player {currentIndex + 1} are still tied.");
                // For simplicity, this example doesn't break further ties.
            }
        }

        return currentBestIndex; // Returns the index of the player with the best hand among the potential winners
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

    private bool IsThreeOfAKind(List<CardSO> hand, out CardRank pairRank)
    {
        pairRank = CardRank.Two; //Initializes as the least possible value
        var rankGroups = hand.GroupBy(card => card.Value);

        if(rankGroups.Any(group => group.Count() == 3)){
            pairRank = rankGroups.First(group => group.Count() == 3).Key;
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool IsTwoPair(List<CardSO> hand, out CardRank pairRank)
    {
        pairRank = CardRank.Two; //Initializes as the least possible value
        var rankGroups = hand.GroupBy(card => card.Value);

        if(rankGroups.Count(group => group.Count() == 2) == 2)
        {
            pairRank = rankGroups.First(group => group.Count() == 2).Key; // Get the rank of the pair
            return true;
        }else
        {
            return false;
        }
    }

    private bool IsOnePair(List<CardSO> hand, out CardRank pairRank)
    {
        pairRank = CardRank.Two; //Initializes as the least possible value
        var rankGroups = hand.GroupBy(card => card.Value);
        if (rankGroups.Any(group => group.Count() == 2))
        {
            // Found a group with 2 cards (the pair)
            pairRank = rankGroups.First(group => group.Count() == 2).Key; // Get the rank of the pair
            return true;
        }
        else
        {
            return false;
        }

    }
}
