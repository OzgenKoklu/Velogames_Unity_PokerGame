using System.Collections.Generic;
using UnityEngine;

public class PokerPlayerHand : MonoBehaviour, ICardParent
{
    public List<CardSO> HoleCardsList; // Array to store the player's hole cards
    private int _playerHandCardLimit = 2;
    public bool? IsPlayerUs { get => _isPlayerUs; }
    [SerializeField] private bool _isPlayerUs;
    [SerializeField] private Transform _handFollowTransform;

    [Header("Card Transform Values:")]
    [SerializeField] private float initialXOffset = -0.6f;
    [SerializeField] private float xOffsetMultiplier = 1.2f;
    [SerializeField] private float initialRotationAngle = 15f;
    [SerializeField] private float rotationAngleMultiplier = -30f;
    [SerializeField] private float initialYPosition = -4f;
    [SerializeField] private float aiRotationAngle = 20f;
    [SerializeField] private float aiScaleMultiplier = 0.6f;

    private void Awake()
    {
        HoleCardsList = new List<CardSO>();
    }

    public void AddCard(CardSO newCard)
    {
        if (HoleCardsList.Count < _playerHandCardLimit)
        {
            HoleCardsList.Add(newCard); // Add the card to the hand
            newCard.CardParent = this;
        }
    }

    public void ClearCards()
    {
        HoleCardsList?.Clear();
    }

    public Transform GetCardFollowTransform()
    {
        Transform cardTransform = _handFollowTransform;

        if (_isPlayerUs)
        {
            float newXPosition = initialXOffset + (xOffsetMultiplier * (HoleCardsList.Count - 1));
            float newRotationAngle = initialRotationAngle + (rotationAngleMultiplier * (HoleCardsList.Count - 1));
            cardTransform.SetPositionAndRotation(new Vector2(newXPosition, initialYPosition), Quaternion.AngleAxis(newRotationAngle, Vector3.forward));
        }
        else // if player is AI or Online
        {
            float newRotationAngle = aiRotationAngle * (HoleCardsList.Count - 1);
            cardTransform.rotation = Quaternion.AngleAxis(newRotationAngle, Vector3.forward);
            cardTransform.localScale = Vector3.one * aiScaleMultiplier;
        }

        return cardTransform;
    }

    public List<CardSO> GetCardList()
    {
        return HoleCardsList;
    }
}