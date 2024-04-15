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


    //5-7 card Decision making (post flop), maybe it should take bluffs into account, like if the earlier player bets, the player will become likely to fold or something. 
    public TurnManager.PlayerAction DecidePostFlop(HandStrength handStrength, TurnManager.PlayerAction? previousPlayersAction = null)
    {
        if (previousPlayersAction == null || previousPlayersAction == TurnManager.PlayerAction.Fold || previousPlayersAction == TurnManager.PlayerAction.Check)
        {
            if (handStrength == HandStrength.Amazing)
            {
                return UnityEngine.Random.Range(0, 5) <= 1 ? TurnManager.PlayerAction.Check : TurnManager.PlayerAction.Bet; //%20 check %80 bet
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
        else if (previousPlayersAction == TurnManager.PlayerAction.Bet || previousPlayersAction == TurnManager.PlayerAction.Raise)
        {
            // Adjusted logic for response to aggression
            switch (handStrength)
            {
                case HandStrength.Amazing:
                    return UnityEngine.Random.Range(0, 4) < 3 ? TurnManager.PlayerAction.Raise : TurnManager.PlayerAction.Call; // Mostly raise, occasionally call
                case HandStrength.Strong:
                    return UnityEngine.Random.Range(0, 3) == 0 ? TurnManager.PlayerAction.Fold : TurnManager.PlayerAction.Call; // Mostly call, rarely fold
                case HandStrength.Medium:
                    return UnityEngine.Random.Range(0, 3) == 0 ? TurnManager.PlayerAction.Call : TurnManager.PlayerAction.Fold; // Some chance to call, mostly fold
                case HandStrength.WeakPlus:
                    return UnityEngine.Random.Range(0, 5) < 1 ? TurnManager.PlayerAction.Call : TurnManager.PlayerAction.Fold; // Small chance to call, mostly fold
                case HandStrength.Weak:
                    return UnityEngine.Random.Range(0, 10) < 1 ? TurnManager.PlayerAction.Call : TurnManager.PlayerAction.Fold; // Very small chance to call, mostly fold
            }
        }
        else if (previousPlayersAction == TurnManager.PlayerAction.Call)
        {
            // Adjusted logic for response to a call
            switch (handStrength)
            {
                case HandStrength.Amazing:
                    return UnityEngine.Random.Range(0, 3) < 2 ? TurnManager.PlayerAction.Raise : TurnManager.PlayerAction.Bet; // Mostly raise, sometimes bet
                case HandStrength.Strong:
                    return UnityEngine.Random.Range(0, 4) < 3 ? TurnManager.PlayerAction.Bet : TurnManager.PlayerAction.Check; // Mostly bet, occasionally check
                case HandStrength.Medium:
                    return UnityEngine.Random.Range(0, 4) == 0 ? TurnManager.PlayerAction.Bet : TurnManager.PlayerAction.Check; // Some chance to bet, mostly check
                case HandStrength.WeakPlus:
                    return UnityEngine.Random.Range(0, 2) == 0 ? TurnManager.PlayerAction.Check : TurnManager.PlayerAction.Fold; // Equally likely to check or fold
                case HandStrength.Weak:
                    return TurnManager.PlayerAction.Check; // Default to check, cautious play
            }
        }

        return TurnManager.PlayerAction.Fold; // Default action
    }


    // for prefold stage only, (for 2 cards) Weak hands still have more chance to check at this stage, no WeakPlus or Amazing hands.
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
                return UnityEngine.Random.Range(0, 4) < 2 ? TurnManager.PlayerAction.Fold : TurnManager.PlayerAction.Check;// 50% Fold, 50% Check
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
                        return UnityEngine.Random.Range(0, 10) < 8 ? TurnManager.PlayerAction.Call : TurnManager.PlayerAction.Fold; // 80% Call, 20% Fold
                    }
                    else
                    {
                        return UnityEngine.Random.Range(0, 10) < 3 ? TurnManager.PlayerAction.Fold : TurnManager.PlayerAction.Call;  // 30%  Fold, 70% Call
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
                        return UnityEngine.Random.Range(0, 10) < 3 ? TurnManager.PlayerAction.Fold : TurnManager.PlayerAction.Check; // 30% Fold, 70% Check
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

}
