using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiPlayerBehaviour : MonoBehaviour
{
    [SerializeField] private PokerPlayerHand _pokerPlayerHand;
    [SerializeField] private PlayerManager _playerManager;

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


}
