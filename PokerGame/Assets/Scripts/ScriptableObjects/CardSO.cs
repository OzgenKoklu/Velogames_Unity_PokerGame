using UnityEngine;

public enum CardSuit { Clubs, Diamonds, Hearts, Spades }
public enum CardRank { Two = 2, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King, Ace }

public class CardSO : ScriptableObject
{
    [SerializeField] private CardSuit _suit;
    [SerializeField] private CardRank _value;
    [SerializeField] private Sprite _cardSprite;
    [SerializeField] private string _cardCode;

    public CardSuit Suit { get => _suit; }
    public CardRank Value { get => _value; }
    public Sprite CardSprite { get => _cardSprite; }
    public string CardCode { get => _cardCode; }
    public CardBehaviour CardBehavior { get; set; }
    public ICardParent CardParent { get; set; }
}