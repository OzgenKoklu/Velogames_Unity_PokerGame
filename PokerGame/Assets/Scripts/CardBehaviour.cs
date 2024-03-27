using UnityEngine;

public class CardBehaviour : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    public CardSO CardSO;
    public bool IsFaceDown;

    public void SetCardScriptableObject(CardSO cardSO)
    {
        CardSO = cardSO;
        UpdateCardVisual();
    }

    public void FlipCard()
    {
        IsFaceDown = !IsFaceDown;
        UpdateCardVisual ();
    }

    private void UpdateCardVisual()
    {
        if (CardSO != null)
        {
            if (!IsFaceDown)
            {
                _spriteRenderer.sprite = CardSO.CardSprite;
            }
            else
            {
                //facedown sprite eklenecek
                _spriteRenderer.sprite = null;
            }
        }
    }
}
