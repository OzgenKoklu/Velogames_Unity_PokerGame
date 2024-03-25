using UnityEngine;

public enum CardSuit { Clubs, Diamonds, Hearts, Spades }
public enum CardValue { Two = 2, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King, Ace }

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class CardSO : ScriptableObject
{
    [SerializeField] private CardSuit _suit;
    [SerializeField] private CardValue _value;
    [SerializeField] private Rect _spriteSheetRect;

}
