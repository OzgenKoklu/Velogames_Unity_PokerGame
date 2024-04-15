using UnityEngine;
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

        if (_playerManager.BetAmount < BetManager.Instance.CurrentHighestBetAmount)
        {
            if (handStrength == HandStrength.Strong)
            {
                playerAction = UnityEngine.Random.Range(0, 4) < 2 ? PlayerAction.Call : PlayerAction.Raise; // 50% Call, 50% Raise

                if (playerAction == PlayerAction.Call)
                {
                    var callBetAmount = BetManager.Instance.CurrentHighestBetAmount - _playerManager.BetAmount;
                    BetManager.Instance.SetBet(_playerManager, callBetAmount);
                }
                else
                {
                    //....
                    //...
                    //...
                }

                return playerAction;
            }
            else if (handStrength == HandStrength.Medium)
            {
                playerAction = UnityEngine.Random.Range(0, 10) < 9 ? PlayerAction.Call : PlayerAction.Raise; // 90% Call, 10% Raise

                if (playerAction == PlayerAction.Call)
                {
                    var callBetAmount = BetManager.Instance.CurrentHighestBetAmount - _playerManager.BetAmount;
                    BetManager.Instance.SetBet(_playerManager, callBetAmount);
                }
                else
                {
                    //....
                    //...
                    //...
                }
                return playerAction;
            }
            else
            {
                playerAction = UnityEngine.Random.Range(0, 10) < 7 ? PlayerAction.Call : PlayerAction.Fold; // 70% Call , 30% Fold
                if (playerAction == PlayerAction.Call)
                {
                    var callBetAmount = BetManager.Instance.CurrentHighestBetAmount - _playerManager.BetAmount;
                    BetManager.Instance.SetBet(_playerManager, callBetAmount);
                }
                else
                {
                    //....
                    //...
                    //...
                }
                return playerAction;
            }
        }
        else if (_playerManager.BetAmount == BetManager.Instance.CurrentHighestBetAmount)
        {
            if (handStrength == HandStrength.Strong)
            {
                return UnityEngine.Random.Range(0, 10) < 9 ? PlayerAction.Check : PlayerAction.Bet; // 90% Check, 10% Bet
            }
            else if (handStrength == HandStrength.Medium)
            {
                return UnityEngine.Random.Range(0, 20) < 19 ? PlayerAction.Check : PlayerAction.Bet; // 95% Check, 5% Bet
            }
            else
            {
                return UnityEngine.Random.Range(0, 20) < 19 ? PlayerAction.Check : PlayerAction.Fold; // 95% Check, 5% Fold
            }
        }
        return PlayerAction.Fold; // Default fallback action
    }
}
