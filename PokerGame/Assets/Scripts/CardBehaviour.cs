using UnityEngine;

public class CardBehaviour : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _cardFaceSpriteRenderer;
    [SerializeField] private SpriteRenderer _cardBackSpriteRenderer;
    [SerializeField] private GameObject _selectedVisualGameObject;
    private Vector3 _dealerCardPosition;
    private Transform _targetTransform;

    public float lerpDuration = 0.5f; // Duration of the lerp in seconds
    private float lerpTime = 0f;
    private bool isLerping = false;

    public CardSO CardSO;

    private void Start()
    {
        _dealerCardPosition = CardVisualsManager.Instance.GetDealerCardPosition();
    }

    public CardSO GetCardScriptableObject()
    {
        return CardSO;
    }

    public void StartLerping()
    {
        _dealerCardPosition = transform.position;
        lerpTime = 0f;
        isLerping = true;
    }

    private void Update()
    {
        if (isLerping)
        {
            LerpToTarget();
        }
    }

    private void LerpToTarget()
    {
        if (_targetTransform == null)
            return;

        lerpTime += Time.deltaTime;
        float t = lerpTime / lerpDuration;

        transform.position = Vector3.Lerp(_dealerCardPosition, _targetTransform.position, t);
        transform.rotation = Quaternion.Lerp(transform.rotation, _targetTransform.rotation, t);
        transform.localScale = Vector3.Lerp(transform.localScale, _targetTransform.localScale, t);

        if (t >= 1.0f)
        {
            isLerping = false;
        }
    }
    public bool IsLerpComplete()
    {
        return !isLerping;
    }

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
            else if (CardSO.CardParent is CommunityCards)
            {
                _cardFaceSpriteRenderer.sprite = CardSO.CardSprite;
            }
            else
            {
                _cardFaceSpriteRenderer.sprite = _cardBackSpriteRenderer.sprite;
            }
        }
        else if (CardSO != null && CardSO.CardParent == null)
        {
            _cardFaceSpriteRenderer.sprite = CardSO.CardSprite;
        }
    }

    public void FlipCard()
    {
        _cardFaceSpriteRenderer.sprite = CardSO.CardSprite;
    }

    public void GetToShowDownPosition()
    {
        if (CardSO.CardParent is CommunityCards) return;

        transform.SetPositionAndRotation(transform.position, Quaternion.identity);
    }

    public void SetCardTransform(Transform newTransform)
    {
        _targetTransform = newTransform;
        StartLerping();
    }

    /*
    public void SetCardTransform(Transform newTransform)
    {
        transform.SetPositionAndRotation(newTransform.position, newTransform.rotation);
        transform.localScale = newTransform.localScale;
    }*/

    public void SetCardAsSelected()
    {
        if (_selectedVisualGameObject != null)
        {
            _selectedVisualGameObject.SetActive(true);
        }
    }
}