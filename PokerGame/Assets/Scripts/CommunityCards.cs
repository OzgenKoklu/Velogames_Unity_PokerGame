using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommunityCards : MonoBehaviour, ICardParent
{
    public List<CardSO> CommunityCardsList;
    private int _communityCardsLimit = 5;
    private void Awake()
    {
        CommunityCardsList = new List<CardSO>();
    }
    public void ClearCards()
    {
        CommunityCardsList?.Clear();
    }

    public Transform GetCardFollowTransform()
    {
        return transform.transform;
    }

    public List<CardSO> GetCardList()
    {
       return CommunityCardsList;
    }

    public void AddCard(CardSO newCard)
    {
        if(CommunityCardsList.Count <= _communityCardsLimit)
        {
         CommunityCardsList.Add(newCard); // Add the card to the hand        
        }
    }

    public ICardParent GetCardParent()
    {
        return this as ICardParent;
    }

}
