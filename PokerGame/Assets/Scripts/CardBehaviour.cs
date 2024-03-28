using UnityEngine;

public class CardBehaviour : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private SpriteRenderer _backSpriteRenderer;

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
                _spriteRenderer.sprite = CardSO.CardSprite;
            }
            else
            {
                //facedown sprite eklenecek
                _spriteRenderer.sprite = _backSpriteRenderer.sprite;
            }
        }
        else if (CardSO != null && CardSO.CardParent == null)
        {
            _spriteRenderer.sprite = CardSO.CardSprite;
        }
    }

    public void SetCardTransform(Transform transform)
    {
        this.transform.SetPositionAndRotation(transform.position, transform.rotation);
        this.transform.localScale = transform.localScale;
    }
}
