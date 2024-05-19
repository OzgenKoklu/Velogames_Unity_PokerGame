using System;
using UnityEngine;


public class AiPlayerBehaviour : MonoBehaviour
{
    [SerializeField] private PlayerManager _playerManager;
    [SerializeField] private PokerPlayerHand _pokerPlayerHand;

    public enum HandStrength { Amazing, Strong, Medium, WeakPlus, Weak }

    // *** POSTFLOP DECISIONS ***
    //5-7 card Decision making (post flop), maybe it should take bluffs into account, like if the earlier player bets, the player will become likely to fold or something. 
    public PlayerAction DecidePostFlop(HandStrength handStrength)
    {
        PlayerAction playerAction;

        int highestBetAmount = BetManager.Instance.CurrentHighestBetAmount;

        int ourCurrentBetAmount = _playerManager.TotalBetInThisRound;

        float zeroBetFactor = highestBetAmount == 0 ? 0.5f : 1; // If highest bet is zero, weak hands will call more often, since they wont lose much.

        float possibilityFactor = CalculatePossibilityFactor();

        float conservativeFactor = CalculateConservativeFactor(handStrength);

        float combinedFactor = possibilityFactor * conservativeFactor * zeroBetFactor;

        Debug.Log("Combined factor of player: " + _playerManager.PlayerName + " " + combinedFactor + " HandStrength " + handStrength);

        Debug.Log("Current Highest bet amount: " + highestBetAmount + "Our current bet amount: " + ourCurrentBetAmount);

        if (ourCurrentBetAmount < highestBetAmount)
        {
            if (handStrength == HandStrength.Amazing)
            {
                playerAction = UnityEngine.Random.Range(0, 5) < 1 * combinedFactor ? PlayerAction.Call : PlayerAction.Raise; //%20 Call %80 Raise (conservative Factor & possibilityFactor = 1)

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
            else if (handStrength == HandStrength.Strong)
            {
                playerAction = UnityEngine.Random.Range(0, 5) < 2 * combinedFactor ? PlayerAction.Call : PlayerAction.Raise; //%40 Call %60 Raise (conservative Factor & possibilityFactor = 1)

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
            else if (handStrength == HandStrength.Medium)
            {
                playerAction = UnityEngine.Random.Range(0, 4) < 3 * combinedFactor ? PlayerAction.Call : PlayerAction.Raise; //%75 Call %25 Raise (conservative Factor & possibilityFactor = 1)

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
            else if (handStrength == HandStrength.WeakPlus)
            {
                playerAction = UnityEngine.Random.Range(0, 3) < 1 * combinedFactor ? PlayerAction.Fold : PlayerAction.Call;

                if (playerAction == PlayerAction.Call)
                {
                    CallAction();
                }

                return playerAction;
            }
            else
            {
                playerAction = UnityEngine.Random.Range(0, 3) < 2 * combinedFactor ? PlayerAction.Fold : PlayerAction.Call;

                if (playerAction == PlayerAction.Call)
                {
                    CallAction();
                }

                return playerAction;
            }
        }
        else if (ourCurrentBetAmount == highestBetAmount) // It can also be zero 
        {
            // Adjusted logic for response to aggression
            switch (handStrength)
            {
                case HandStrength.Amazing:
                    playerAction = UnityEngine.Random.Range(0, 4) < 1 * combinedFactor ? PlayerAction.Check : PlayerAction.Bet;

                    if (playerAction == PlayerAction.Bet)
                    {
                        BetAction(handStrength);
                    }

                    return playerAction;
                case HandStrength.Strong:
                    playerAction = UnityEngine.Random.Range(0, 3) < 1 * combinedFactor ? PlayerAction.Check : PlayerAction.Bet;

                    if (playerAction == PlayerAction.Bet)
                    {
                        BetAction(handStrength);
                    }
                    return playerAction;
                case HandStrength.Medium:
                    playerAction = UnityEngine.Random.Range(0, 3) < 2 * combinedFactor ? PlayerAction.Check : PlayerAction.Bet;

                    if (playerAction == PlayerAction.Bet)
                    {
                        BetAction(handStrength);
                    }

                    return playerAction;
                case HandStrength.WeakPlus:
                    playerAction = UnityEngine.Random.Range(0, 5) < 3 * combinedFactor ? PlayerAction.Fold : PlayerAction.Check;

                    return playerAction;
                case HandStrength.Weak:
                    playerAction = UnityEngine.Random.Range(0, 10) < 5 * combinedFactor ? PlayerAction.Fold : PlayerAction.Check;
                    return playerAction;
            }
        }
        // it cant be > highest bet anyway. (unluess bug happens)

        return PlayerAction.Fold; // Default action
    }

    // *** PREFLOP DECISIONS ***
    // *************************
    // 2 card Decision making (pre flop), maybe it should take bluffs into account, like if the earlier player bets, the player will become likely to fold or something.
    // for prefold stage only, (for 2 cards) Weak hands still have more chance to check at this stage, no WeakPlus or Amazing hands.
    public PlayerAction DecidePreFlop(HandStrength handStrength)
    {
        PlayerAction playerAction;

        int highestBetAmount = BetManager.Instance.CurrentHighestBetAmount;

        int ourCurrentBetAmount = _playerManager.TotalBetInThisRound;

        ///!!! IMPORTANT !!! Conservative options are on the left for game-mechanics purposes. The conservative factor will increase the likelyhood of >
        /// selecting the conservative option. To avoid constant raises in betting rounds, AI players tendancy toward making brave moves lessens  over time
        /// as their bets get larger and larger.
        /// 
        float conservativeFactor = CalculateConservativeFactor(handStrength);

        //Debug.Log("After Clamp - ConservativeFactor: " + " for " + _playerManager.name + ": " + conservativeFactor);

        Debug.Log("conservative factor of player:(preflop) " + _playerManager.PlayerName + " " + conservativeFactor + " HandStrength " + handStrength);

        Debug.Log("Current Highest bet amount: " + highestBetAmount + "Our current bet amount: " + ourCurrentBetAmount);

        if (ourCurrentBetAmount < highestBetAmount)
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
        else if (ourCurrentBetAmount == highestBetAmount)
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
                playerAction = UnityEngine.Random.Range(0, 20) < 19 * conservativeFactor ? PlayerAction.Check : PlayerAction.Bet; // 95% Check, 5% Bet (for conservative factor = 1)

                if (playerAction == PlayerAction.Bet)
                {
                    BetAction(handStrength);
                }

                return playerAction;
            }
            else
            {
                playerAction = UnityEngine.Random.Range(0, 20) < 1 * conservativeFactor ? PlayerAction.Fold : PlayerAction.Check; // 95% Check, 5% Fold (for conservative factor = 1)

                //no logic needed for either check or fold. 

                return playerAction;
            }
        }
        return PlayerAction.Fold; // Default fallback action
    }

    private void CallAction()
    {
        int callBetAmount = BetManager.Instance.CurrentHighestBetAmount - _playerManager.TotalBetInThisRound;
        int maxCallAmount = _playerManager.TotalStackAmount;

        BetManager.Instance.SetBet(_playerManager, Mathf.Min(callBetAmount, maxCallAmount));

        _playerManager.IsPlayerAllIn = callBetAmount >= maxCallAmount;
    }

    private void BetAction(HandStrength handStrength)
    {
        RaiseAction(handStrength);
    }

    private void RaiseAction(HandStrength handStrength)
    {
        int raiseBetAmount = CalculateRaiseAmount(handStrength);

        BetManager.Instance.SetBet(_playerManager, raiseBetAmount);

        _playerManager.IsPlayerAllIn = _playerManager.BetAmount >= _playerManager.TotalStackAmount;
        BetManager.Instance.CurrentHighestBetAmount = _playerManager.TotalBetInThisRound + raiseBetAmount;
    }

    public int CalculateRaiseAmount(HandStrength handStrength)
    {
        int currentHighestBet = BetManager.Instance.CurrentHighestBetAmount;
        int minimumRaiseAmount = currentHighestBet != 0 ? currentHighestBet * 2 - _playerManager.TotalBetInThisRound : BetManager.Instance.GetMinimumRaiseAmount();
        int raiseMultiplier = GetRandomMultiplier(handStrength);

        int raiseAmount = minimumRaiseAmount * raiseMultiplier;

        raiseAmount = Mathf.Clamp(raiseAmount, minimumRaiseAmount, _playerManager.TotalStackAmount);
        return raiseAmount;
    }

    private float GetHandStrengthAdjustment(HandStrength handStrength)
    {
        return handStrength == HandStrength.Amazing ? 0.3f :
               handStrength == HandStrength.Strong ? 0.5f :
               handStrength == HandStrength.Medium ? 0.7f :
               handStrength == HandStrength.WeakPlus ? 1.0f : 1.5f;
    }

    public int GetRandomMultiplier(HandStrength handStrength)
    {
        float exponent = GetHandStrengthAdjustment(handStrength);

        float skewedRandom = Mathf.Pow(UnityEngine.Random.value, exponent);
        return (int)(skewedRandom * 4) + 1; // Scaling to get a value from 1 to 4
    }

    private float CalculateConservativeFactor(HandStrength handStrength)
    {
        int currentBet = _playerManager.TotalBetInThisRound;
        int currentStackAmount = _playerManager.TotalStackAmount;
        int highestBet = BetManager.Instance.CurrentHighestBetAmount;

        float betProximity = Mathf.Abs((float)(currentBet - highestBet) / highestBet);
        float stackRisk = (float)highestBet / currentStackAmount;
        //Debug.Log("Before Clamp - ConservativeFactor: " + " for " + _playerManager.name + ": " + conservativeFactor);
        return Mathf.Clamp(betProximity + stackRisk, 1, 4);
    }

    //this is for making players taking braver actions when the amount of community cards available are less. There are still 2 cards remaining for flop
    private float CalculatePossibilityFactor()
    {
        int communityCardsCount = CommunityCards.Instance.GetCardList().Count;

        return communityCardsCount switch
        {
            3 => 1.0f,
            4 => 1.2f,
            5 => 1.4f,
            _ => 1.0f
        };
    }
}