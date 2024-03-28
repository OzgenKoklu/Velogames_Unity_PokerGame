using UnityEngine;

public enum CardSuit { Clubs, Diamonds, Hearts, Spades }
public enum CardValue { Two = 2, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King, Ace }

public class CardSO : ScriptableObject
{
    [SerializeField] private CardSuit _suit;
    [SerializeField] private CardValue _value;
    [SerializeField] private Sprite _cardSprite;

    public CardSuit Suit { get => _suit; }
    public CardValue Value { get => _value; }
    public Sprite CardSprite { get => _cardSprite; }
    public ICardParent CardParent { get; set; }
}
