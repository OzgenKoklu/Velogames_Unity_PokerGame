using pheval;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiPlayerBehaviour : MonoBehaviour
{
    [SerializeField] private PokerPlayerHand _pokerPlayerHand;

    /*Ranks from pheval library for hand strenght reference
     public static Category GetCategory(int rank)
       {
           if (rank > 6185) return Category.HighCard;        // 1277 high card
           if (rank > 3325) return Category.OnePair;         // 2860 one pair
           if (rank > 2467) return Category.TwoPair;         //  858 two pair
           if (rank > 1609) return Category.ThreeOfAKind;  //  858 three-kind
           if (rank > 1599) return Category.Straight;         //   10 straights
           if (rank > 322) return Category.Flush;            // 1277 flushes
           if (rank > 166) return Category.FullHouse;       //  156 full house
           if (rank > 10) return Category.FourOfAKind;   //  156 four-kind
           return Category.StraightFlush;                    //   10 straight-flushes
    }*/
    public enum HandStrength { Amazing, Strong, Medium, WeakPlus, Weak }
    public enum PlayerAction { Fold, Check, Bet, Raise }

    //5-7 card Decision making (post flop)
    public PlayerAction DecidePostFlop(HandStrength handStrength)
    {
        if (handStrength == HandStrength.Amazing)
        {
            return UnityEngine.Random.Range(0, 4) <= 2 ? PlayerAction.Check : PlayerAction.Bet;
        }
        else if (handStrength == HandStrength.Strong)
        {
            return UnityEngine.Random.Range(0, 5) <= 2 ? PlayerAction.Check : PlayerAction.Bet;
        }
        else if (handStrength == HandStrength.Medium)
        {
            return UnityEngine.Random.Range(0, 4) < 1 ? PlayerAction.Check : PlayerAction.Bet;
        }
        else if (handStrength == HandStrength.WeakPlus)
        {
            return UnityEngine.Random.Range(0, 3) <= 1 ? PlayerAction.Check : PlayerAction.Fold;
        }
        else 
        {
            return UnityEngine.Random.Range(0, 3) <= 1 ? PlayerAction.Check : PlayerAction.Fold;
        }
    }



    // for prefold stage only, (for 2 cards) 
    public PlayerAction DecidePreFlop()
    {
        var handStrength = TwoCardHandEvaluator(_pokerPlayerHand.GetCardList());

        if (handStrength == HandStrength.Strong)
        {
            return PlayerAction.Bet; // Check or raise maybe?
        }
        else if (handStrength == HandStrength.Medium)
        {
            // Randomly decide to check or bet with medium strength hands
            return UnityEngine.Random.Range(0, 3) <= 1 ? PlayerAction.Check : PlayerAction.Bet;
        }
        else // Weak hand
        {
            // Occasionally bluff with a weak hand
            return UnityEngine.Random.Range(0, 10) <= 4 ? PlayerAction.Bet : PlayerAction.Fold;
        }
    }

    public HandStrength HandStrenghtCalculator(int handRank)
    {
        if (handRank < 322) //full house or better
        {
            return HandStrength.Amazing;
        }
        else if (handRank < 2467) // three of a kind or bettter 
        {
            return HandStrength.Strong;
        }
        else if (handRank < 6185) //two pair hand, still rare somewhat
        {
            return HandStrength.Medium;
        }
        else if (handRank < 3325) //One Pair Hand
        {
            return HandStrength.WeakPlus;
        }
        else // high card Hand
        {
            return HandStrength.Weak;
        }
    }
    private HandStrength TwoCardHandEvaluator(List<CardSO> holeCards)
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
