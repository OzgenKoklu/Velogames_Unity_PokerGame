using UnityEngine;

public class CardBehaviour : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _cardFaceSpriteRenderer;
    [SerializeField] private SpriteRenderer _cardBackSpriteRenderer;

    public CardSO CardSO;

    public void SetCardScriptableObject(CardSO cardSO)
    {
        CardSO = cardSO;
        UpdateCardVisual();
    }

    private void UpdateCardVisual()
    {
        if (CardSO != null && CardSO.CardParent != null)
        {
            if (CardSO.CardParent.IsPlayerUs == true)
            {
                _cardFaceSpriteRenderer.sprite = CardSO.CardSprite;
            }
            else
            {
                //facedown sprite eklenecek
                _cardFaceSpriteRenderer.sprite = _cardBackSpriteRenderer.sprite;
            }
        }
        else if (CardSO != null && CardSO.CardParent == null)
        {
            _cardFaceSpriteRenderer.sprite = CardSO.CardSprite;
        }
    }

    public void SetCardTransform(Transform transform)
    {
        this.transform.SetPositionAndRotation(transform.position, transform.rotation);
        this.transform.localScale = transform.localScale;
    }
}
