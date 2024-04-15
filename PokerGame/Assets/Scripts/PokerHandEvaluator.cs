using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using pheval;
using static pheval.Rank;
using UnityEngine.UIElements;
using static AiPlayerBehaviour;

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


    public List<int> EvaluateHandStrengths(List<List<CardSO>> playerHands)
    {
        //using player ID -> handrank dictionary might be more secure to store these sort of data (for online game)
        //on the client side, one can easily access to index of other players in this current situation.
        List<int> handRanks = new List<int>();

        // Step 1: Evaluate each hand and write their rank in the list
        for (int i = 0; i < playerHands.Count; i++)
        {
            int handRank = EvaluateHandRank(playerHands[i]);
            handRanks.Add(handRank);            
        }
     
        return handRanks;
    }

    public WinningHandResults SelectTheWinnerForTheShowdown(List<int> playerRankList)
    {
        int bestHandIndex = -1;
        int bestRank = 7462; //not a magical number, just the weakest possible hand rank

        for (int i = 0; i < playerRankList.Count; i++)
        {
            if (playerRankList[i] < bestRank)
            {
                bestRank = playerRankList[i];
                bestHandIndex = i;
            }
            else if (playerRankList[i] == bestRank)
            {
                //tie situation
            }
        }

        string winningHandDescriptionCode = pheval.Rank.DescribeRankShort(bestRank);
        string winningHandType = pheval.Rank.DescribeRank(bestRank);

        WinningHandResults winningHandResults = new WinningHandResults()
        {
            WinningCardList = PokerDeckManager.Instance.GetAllPlayerHands()[bestHandIndex],
            WinningHandIndex = bestHandIndex,
            WinningCardCodes = winningHandDescriptionCode,
            WinningHandType = winningHandType
        };

        return winningHandResults;
    }

    public struct WinningHandResults
    {
        public List<CardSO> WinningCardList;
        public int WinningHandIndex;
        public string WinningCardCodes;
        public string WinningHandType;
    }

    public HandStrength TwoCardHandEvaluator(List<CardSO> holeCards)
    {
        bool isSuited = holeCards[0].Suit == holeCards[1].Suit;
        int card1Rank = (int)holeCards[0].Value; //2 = 2, Ace = 14
        int card2Rank = (int)holeCards[1].Value; //2 = 2, Ace = 14

        // High pairs
        if (card1Rank == card2Rank && card1Rank >= 10) // TT and above
        {
            return HandStrength.Strong;
        }

        // High suited connectors or AK
        if (isSuited && ((card1Rank >= 10 && card2Rank >= 10) || (card1Rank == 14 || card2Rank == 14)))
        {
            return HandStrength.Strong;
        }
        else if (!isSuited && ((card1Rank == 14 && card2Rank >= 10) || (card2Rank == 14 && card1Rank >= 10)))
        {
            return HandStrength.Strong;
        }

        // Medium pairs
        if (card1Rank == card2Rank && card1Rank >= 7 && card1Rank <= 9)
        {
            return HandStrength.Medium;
        }

        // Suited connectors
        if (isSuited && Mathf.Abs(card1Rank - card2Rank) == 1)
        {
            return HandStrength.Medium;
        }

        // High unsuited connectors
        if (!isSuited && ((card1Rank >= 10 && card2Rank >= 10) || (card1Rank == 14 || card2Rank == 14)))
        {
            return HandStrength.Medium;
        }

        return HandStrength.Weak;
    }
}
