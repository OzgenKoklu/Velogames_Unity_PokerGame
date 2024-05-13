using System.Collections.Generic;
using UnityEngine;

public class CommunityCards : MonoBehaviour, ICardParent
{
    public static CommunityCards Instance { get; private set; }
    public List<CardSO> CommunityCardsList;
    private int _communityCardsLimit = 5;
    private int _cardPositionXLeftBoundry = -3;
    private float _cardSpacing = 1.5f;

    public bool? IsPlayerUs => null;

    private void Awake()
    {
        CommunityCardsList = new List<CardSO>();
        Instance = this;
    }
    public void ClearCards()
    {
        CommunityCardsList?.Clear();
    }

    public Transform GetCardFollowTransform()
    {
        float offset = (CommunityCardsList.Count - 1) * _cardSpacing + _cardPositionXLeftBoundry;
        float positionX = transform.position.x + offset;

        Transform cardTransform = new GameObject().transform;
        cardTransform.position = new Vector3(positionX, transform.position.y, transform.position.z);
        cardTransform.SetParent(transform); // Optional: Set parent to keep the hierarchy organized

        return cardTransform;
    }

    public List<CardSO> GetCardList()
    {
        return CommunityCardsList;
    }

    public void AddCard(CardSO newCard)
    {
        if (CommunityCardsList.Count <= _communityCardsLimit)
        {
            CommunityCardsList.Add(newCard); // Add the card to the hand        
            newCard.CardParent = this;
        }
    }
}
