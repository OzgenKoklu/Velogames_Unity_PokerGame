using UnityEngine;

public class CardBehaviour : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _cardFaceSpriteRenderer;
    [SerializeField] private SpriteRenderer _cardBackSpriteRenderer;
    [SerializeField] private GameObject _selectedVisualGameObject;

    public CardSO CardSO;

    public void SetCardScriptableObject(CardSO cardSO)
    {
        CardSO = cardSO;
        cardSO.CardBehavior = this;
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
            else if(CardSO.CardParent is CommunityCards)
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

    public void SetCardAsSelected()
    {
        // _selectedVisualGameObject?.SetActive(true);
        Debug.Log("Card Selected: " + CardSO.CardCode);
    }
}
