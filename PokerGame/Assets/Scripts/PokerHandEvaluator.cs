using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using pheval;
using static pheval.Rank;
using UnityEngine.UIElements;

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


    public int EvaluateHandRank(List<CardSO> hand)
    {
        string cardCodes = ConcatenateCardCodes(hand);

        int cardCount = hand.Count();

        var rank = -1;

        switch (cardCount)
        {
            case 5:
                rank = pheval.Eval.Eval5String(cardCodes);
                break;
            case 6:
                rank = pheval.Eval.Eval6String(cardCodes);
                break;
            case 7:
                rank = pheval.Eval.Eval7String(cardCodes);
                break;

            default:
                Debug.Log("Unexpected Card Count.");
                break;
        }

        return rank;
    }
    public string ConcatenateCardCodes(List<CardSO> hand)
    {
        // Using LINQ to select the _cardCode from each CardSO in the hand
        var cardCodes = hand.Select(card => card.CardCode);

        // Joining all the card codes into a single string
        string concatenatedCodes = string.Join("", cardCodes);

        return concatenatedCodes;
    }


    public WinningHandResults EvaluateAndFindWinner(List<List<CardSO>> playerHands)
    {
        int winnerIndex = -1;
        int bestRank = 7462; //not a magical number, just the weakest possible hand rank

        // Step 1: Evaluate each hand and determine the highest rank
        for (int i = 0; i < playerHands.Count; i++)
        {
            int currentRank = EvaluateHandRank(playerHands[i]);

            var category1 = pheval.Rank.GetCategory(currentRank);

            Debug.Log($"Player {i}: Rank(smaller is better): {currentRank} with a category of {category1}");

            if (currentRank < bestRank)
            {
                bestRank = currentRank;
                winnerIndex = i;
            }
            else if (currentRank == bestRank)
            {
                //tie situation
            }
        }

        string winningHandDescriptionCode = pheval.Rank.DescribeRankShort(bestRank);
        string winningHandType = pheval.Rank.DescribeRank(bestRank);
        // Debug.Log($"Winner determined: Player {winnerIndex + 1}, Cards: {winningHandDescriptionCode}, hand type:{winningHandType} ");

        WinningHandResults winningHandResults = new WinningHandResults()
        {
            WinningHandIndex = winnerIndex,
            WinningCardCodes = winningHandDescriptionCode,
            WinningHandType = winningHandType
        };

        return winningHandResults;
    }

    public struct WinningHandResults
    {
        public int WinningHandIndex;
        public string WinningCardCodes;
        public string WinningHandType;
    }

}
