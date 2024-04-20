using System;
using UnityEngine;
using static AiPlayerBehaviour;

public class AiPlayerBehaviour : MonoBehaviour
{
    [SerializeField] private PlayerManager _playerManager;
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

    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! Need to refactoring !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    // *** POSTFLOP DECISIONS ***
    // **************************
    //5-7 card Decision making (post flop), maybe it should take bluffs into account, like if the earlier player bets, the player will become likely to fold or something. 
    public PlayerAction DecidePostFlop(HandStrength handStrength, PlayerAction? previousPlayersAction = null)
    {
        if (previousPlayersAction == null || previousPlayersAction == PlayerAction.Fold || previousPlayersAction == PlayerAction.Check)
        {
            if (handStrength == HandStrength.Amazing)
            {
                return UnityEngine.Random.Range(0, 5) <= 1 ? PlayerAction.Check : PlayerAction.Bet; //%20 check %80 bet
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
        else if (previousPlayersAction == PlayerAction.Bet || previousPlayersAction == PlayerAction.Raise)
        {
            // Adjusted logic for response to aggression
            switch (handStrength)
            {
                case HandStrength.Amazing:
                    return UnityEngine.Random.Range(0, 4) < 3 ? PlayerAction.Raise : PlayerAction.Call; // Mostly raise, occasionally call
                case HandStrength.Strong:
                    return UnityEngine.Random.Range(0, 3) == 0 ? PlayerAction.Fold : PlayerAction.Call; // Mostly call, rarely fold
                case HandStrength.Medium:
                    return UnityEngine.Random.Range(0, 3) == 0 ? PlayerAction.Call : PlayerAction.Fold; // Some chance to call, mostly fold
                case HandStrength.WeakPlus:
                    return UnityEngine.Random.Range(0, 5) < 1 ? PlayerAction.Call : PlayerAction.Fold; // Small chance to call, mostly fold
                case HandStrength.Weak:
                    return UnityEngine.Random.Range(0, 10) < 1 ? PlayerAction.Call : PlayerAction.Fold; // Very small chance to call, mostly fold
            }
        }
        else if (previousPlayersAction == PlayerAction.Call)
        {
            // Adjusted logic for response to a call
            switch (handStrength)
            {
                case HandStrength.Amazing:
                    return UnityEngine.Random.Range(0, 3) < 2 ? PlayerAction.Raise : PlayerAction.Bet; // Mostly raise, sometimes bet
                case HandStrength.Strong:
                    return UnityEngine.Random.Range(0, 4) < 3 ? PlayerAction.Bet : PlayerAction.Check; // Mostly bet, occasionally check
                case HandStrength.Medium:
                    return UnityEngine.Random.Range(0, 4) == 0 ? PlayerAction.Bet : PlayerAction.Check; // Some chance to bet, mostly check
                case HandStrength.WeakPlus:
                    return UnityEngine.Random.Range(0, 2) == 0 ? PlayerAction.Check : PlayerAction.Fold; // Equally likely to check or fold
                case HandStrength.Weak:
                    return PlayerAction.Check; // Default to check, cautious play
            }
        }

        return PlayerAction.Fold; // Default action
    }

    // *** PREFLOP DECISIONS ***
    // *************************
    // 2 card Decision making (pre flop), maybe it should take bluffs into account, like if the earlier player bets, the player will become likely to fold or something.
    // for prefold stage only, (for 2 cards) Weak hands still have more chance to check at this stage, no WeakPlus or Amazing hands.
    public PlayerAction DecidePreFlop(HandStrength handStrength) 
    {
        PlayerAction playerAction;

        ///!!! IMPORTANT !!! Conservative options are on the left for game-mechanics purposes. The conservative factor will increase the likelyhood of >
        /// selecting the conservative option. To avoid constant raises in betting rounds, AI players tendancy toward making brave moves lessens  over time
        /// as their bets get larger and larger.
        /// 
        float conservativeFactor = CalculateConservativeFactor(handStrength);

        Debug.Log("After Clamp - ConservativeFactor: " + " for " + _playerManager.name +  ": " + conservativeFactor);

        if (_playerManager.BetAmount < BetManager.Instance.CurrentHighestBetAmount)
        {
            if (handStrength == HandStrength.Strong)
            {
                playerAction = UnityEngine.Random.Range(0, 4) < 2 * conservativeFactor ? PlayerAction.Call : PlayerAction.Raise; // 50% Call, 50% Raise (for conservative factor = 1)

                if (playerAction == PlayerAction.Call)
                {
                    CallAction();
                }
                else // Player raises. 
                {
                    RaiseAction(handStrength);                  
                }

                return playerAction;
            }
            else if (handStrength == HandStrength.Medium)
            {
                playerAction = UnityEngine.Random.Range(0, 10) < 9 * conservativeFactor ? PlayerAction.Call : PlayerAction.Raise; // 90% Call, 10% Raise (for conservative factor = 1)

                if (playerAction == PlayerAction.Call)
                {
                    CallAction();
                }
                else
                {
                    RaiseAction(handStrength);
                }
                return playerAction;
            }
            else
            {
                playerAction = UnityEngine.Random.Range(0, 10) < 3 * conservativeFactor ? PlayerAction.Fold : PlayerAction.Call; // 70% Call , 30% Fold (for conservative factor = 1)
                
                if (playerAction == PlayerAction.Call)
                {
                    CallAction();
                }

                // if player folds, no need to set any bet.
                return playerAction;
            }
        }
        else if (_playerManager.BetAmount == BetManager.Instance.CurrentHighestBetAmount)
        {
            if (handStrength == HandStrength.Strong)
            {
                playerAction = UnityEngine.Random.Range(0, 10) < 9 * conservativeFactor ? PlayerAction.Check : PlayerAction.Bet; // 90% Check, 10% Bet (for conservative factor = 1)

                if (playerAction == PlayerAction.Bet)
                {
                    BetAction(handStrength);
                }

                //no further logic needed for check
                return playerAction;
            }
            else if (handStrength == HandStrength.Medium)
            {
                playerAction =  UnityEngine.Random.Range(0, 20) < 19 * conservativeFactor? PlayerAction.Check : PlayerAction.Bet; // 95% Check, 5% Bet (for conservative factor = 1)

                if (playerAction == PlayerAction.Bet)
                {
                    BetAction(handStrength);
                }

                return playerAction;
            }
            else
            {
                playerAction =  UnityEngine.Random.Range(0, 20) < 1 * conservativeFactor? PlayerAction.Fold : PlayerAction.Check; // 95% Check, 5% Fold (for conservative factor = 1)

                //no logic needed for either check or fold. 

                return playerAction;
            }
        }
        return PlayerAction.Fold; // Default fallback action
    }

    private void CallAction()
    {
        var callBetAmount = BetManager.Instance.CurrentHighestBetAmount - _playerManager.BetAmount;
        BetManager.Instance.SetBet(_playerManager, callBetAmount);
    }

    private void BetAction(HandStrength handStrength)
    {
        RaiseAction(handStrength);
    }
    private void RaiseAction(HandStrength handStrength)
    {
        int raiseBetAmount = CalculateRaiseAmount(handStrength);

        // Set the raise amount in the bet manager, and also the highest bet. 
        BetManager.Instance.SetBet(_playerManager, raiseBetAmount);
        BetManager.Instance.CurrentHighestBetAmount += raiseBetAmount;
    }

    public int CalculateRaiseAmount(HandStrength handStrength)
    {
        // Calculate the minimum raise amount
        int minimumRaiseAmount = BetManager.Instance.CurrentHighestBetAmount * 2 - _playerManager.BetAmount;

        // Define a multiplier based on hand strength
        int raiseMultiplier = GetRandomMultiplier(handStrength);

        // Calculate the proposed raise amount
        int raiseAmount = minimumRaiseAmount * raiseMultiplier;

        // Ensure the raise does not exceed the player's total stack and adheres to the game's rules
        raiseAmount = Mathf.Max(raiseAmount, minimumRaiseAmount); // At least the minimum raise
        int totalStack = _playerManager.TotalStackAmount;
        raiseAmount = Mathf.Min(raiseAmount, totalStack); // Do not exceed the player's stack

        return raiseAmount;
    }

    public int GetRandomMultiplier(HandStrength handStrength)
    {
        float exponent;
        switch (handStrength)
        {
            case HandStrength.Amazing:
                exponent = 0.3f; // Strong skew towards higher values
                break;
            case HandStrength.Strong:
                exponent = 0.5f; // Moderate skew towards higher values
                break;
            case HandStrength.Medium:
                exponent = 0.7f; // Slight skew towards higher values
                break;
            case HandStrength.WeakPlus:
                exponent = 1.0f; // Uniform distribution
                break;
            case HandStrength.Weak:
                exponent = 1.5f; // Skew towards lower values
                break;
            default:
                exponent = 1.0f; // Uniform distribution for undefined cases
                break;
        }

        float skewedRandom = Mathf.Pow(UnityEngine.Random.value, exponent);
        return (int)(skewedRandom * 4) + 1; // Scaling to get a value from 1 to 4
    }

    private float CalculateConservativeFactor(HandStrength handStrength)
    {
        int currentBet = _playerManager.BetAmount;
        int currentStackAmount = _playerManager.TotalStackAmount;
        int highestBet = BetManager.Instance.CurrentHighestBetAmount;

        float betProximity = MathF.Abs((float)(currentBet - highestBet) / highestBet);
        float stackRisk = (float)(highestBet) / currentStackAmount;

        /*

        !!! Might use hand strenght or not for the post flop !!!. At this moment, I think its not needed since we already take hand strenght into acount 
        // In the DecidePreFlop Method. 
        float strengthAdjustment = 0f;

        switch (handStrength)
        {
            case HandStrength.Amazing:
                strengthAdjustment = 0f;
                break;
            case HandStrength.Strong:
                strengthAdjustment = 0.2f;
                break;
            case HandStrength.Medium:
                strengthAdjustment = 0.4f;
                break;
            case HandStrength.WeakPlus:
                strengthAdjustment = 0.6f;
                break;
            case HandStrength.Weak:
                strengthAdjustment = 0.8f;
                break;
            default:
                strengthAdjustment = 0.4f;
                break;
        }*/

        // Calculate the conservative factor, aiming for a result close to 1 for low risk and potentially as high as 4 for high risk
        float conservativeFactor =  betProximity + stackRisk; // + strengthAdjustment;
        Debug.Log("Before Clamp - ConservativeFactor: " + " for " + _playerManager.name + ": " + conservativeFactor);
        // Clamp the result to a reasonable range if needed
        return Mathf.Clamp(conservativeFactor, 1, 4);

    }
}
