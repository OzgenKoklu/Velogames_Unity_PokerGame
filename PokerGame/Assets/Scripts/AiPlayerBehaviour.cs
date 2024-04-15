using pheval;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static AiPlayerBehaviour;

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
   

    //5-7 card Decision making (post flop), maybe it should take bluffs into account, like if the earlier player bets, the player will become likely to fold or something. 
    public TurnManager.PlayerAction DecidePostFlop(HandStrength handStrength)
    {
        if (handStrength == HandStrength.Amazing)
        {
            return UnityEngine.Random.Range(0, 4) <= 2 ? TurnManager.PlayerAction.Check : TurnManager.PlayerAction.Bet;
        }
        else if (handStrength == HandStrength.Strong)
        {
            return UnityEngine.Random.Range(0, 5) <= 2 ? TurnManager.PlayerAction.Check : TurnManager.PlayerAction.Bet;
        }
        else if (handStrength == HandStrength.Medium)
        {
            return UnityEngine.Random.Range(0, 4) < 1 ? TurnManager.PlayerAction.Check : TurnManager.PlayerAction.Bet;
        }
        else if (handStrength == HandStrength.WeakPlus)
        {
            return UnityEngine.Random.Range(0, 3) <= 1 ? TurnManager.PlayerAction.Check : TurnManager.PlayerAction.Fold;
        }
        else 
        {
            return UnityEngine.Random.Range(0, 3) <= 1 ? TurnManager.PlayerAction.Check : TurnManager.PlayerAction.Fold;
        }
    }

   

    // for prefold stage only, (for 2 cards) 
    public TurnManager.PlayerAction DecidePreFlop(HandStrength handStrength, TurnManager.PlayerAction? previousPlayersAction = null)
    { 
        //if previous player folds or the player is the first player, hence, previousPlayerAction is null.

        if (previousPlayersAction == null || previousPlayersAction == TurnManager.PlayerAction.Fold)
        {
            if (handStrength == HandStrength.Strong)
            {               
                return UnityEngine.Random.Range(0, 3) < 2 ? TurnManager.PlayerAction.Bet : TurnManager.PlayerAction.Check;// 66% Bet, 33% Check
            }
            else if (handStrength == HandStrength.Medium)
            {       
                return UnityEngine.Random.Range(0, 3) < 2 ? TurnManager.PlayerAction.Check : TurnManager.PlayerAction.Bet;// 66% Check, 33% Bet
            }
            else
            {
                return UnityEngine.Random.Range(0, 4) < 3 ? TurnManager.PlayerAction.Fold : TurnManager.PlayerAction.Check;// 75% Fold, 25% Check
            }
        }
        else
        {
            // Logic depending on the previous player's action if not first to act
            switch (previousPlayersAction.Value)
            {
                //takes the same action for raise or bet
                case TurnManager.PlayerAction.Bet:
                case TurnManager.PlayerAction.Raise:
                    if (handStrength == HandStrength.Strong)
                    {  
                        return UnityEngine.Random.Range(0, 10) < 7 ? TurnManager.PlayerAction.Raise : TurnManager.PlayerAction.Call;// 70% Raise, 30% Call
                    }
                    else if (handStrength == HandStrength.Medium)
                    {   
                        return UnityEngine.Random.Range(0, 10) < 7 ? TurnManager.PlayerAction.Call : TurnManager.PlayerAction.Fold; // 70% Call, 30% Fold
                    }
                    else
                    {                    
                        return UnityEngine.Random.Range(0, 10) < 8 ? TurnManager.PlayerAction.Fold : TurnManager.PlayerAction.Bet;  // 80%  Fold, 20% Bet (bluff)
                    }
                case TurnManager.PlayerAction.Check:
                    if (handStrength == HandStrength.Strong)
                    {      
                        return UnityEngine.Random.Range(0, 4) < 3 ? TurnManager.PlayerAction.Bet : TurnManager.PlayerAction.Check; // 75% chance to Bet, 25% chance to Check
                    }
                    else if (handStrength == HandStrength.Medium)
                    {
                       
                        return UnityEngine.Random.Range(0, 2) == 0 ? TurnManager.PlayerAction.Check : TurnManager.PlayerAction.Bet; // Equally likely to Check or Bet
                    }
                    else
                    {
                        return UnityEngine.Random.Range(0, 5) < 3 ? TurnManager.PlayerAction.Fold : TurnManager.PlayerAction.Check; // 60% Fold, 40% Check
                    }
                case TurnManager.PlayerAction.Call:
                    if (handStrength == HandStrength.Strong)
                    {                       
                        return UnityEngine.Random.Range(0, 4) < 3 ? TurnManager.PlayerAction.Raise : TurnManager.PlayerAction.Bet; // 75% Raise, 25% Bet
                    }
                    else if (handStrength == HandStrength.Medium)
                    {
                        return UnityEngine.Random.Range(0, 3) < 2 ? TurnManager.PlayerAction.Bet : TurnManager.PlayerAction.Check;// 66% Bet, 33% Check
                    }
                    else
                    {              
                        return UnityEngine.Random.Range(0, 10) < 7 ? TurnManager.PlayerAction.Check : TurnManager.PlayerAction.Fold;  // 70% Check, 30% Fold
                    }
            }
        }

        return TurnManager.PlayerAction.Fold; // Default fallback action
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
    

}
