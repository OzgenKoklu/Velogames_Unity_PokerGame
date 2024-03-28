using System.Collections.Generic;
using UnityEngine;

public class PokerPlayerHand : MonoBehaviour, ICardParent
{

    public List<CardSO> HoleCardsList; // Array to store the player's hole cards
    private int _playerHandCardLimit = 2;

    private void Awake()
    {
        HoleCardsList = new List<CardSO>();
    }
    public void AddCard(CardSO newCard)
    {
        if (HoleCardsList.Count <= _playerHandCardLimit)
        {
                HoleCardsList.Add(newCard); // Add the card to the hand
        }    
    }

    public void ClearCards()
    {
        HoleCardsList?.Clear();
    }

    public Transform GetCardFollowTransform()
    {
        //to set each card position.
        return transform.transform;
    }

    public List<CardSO> GetCardList()
    {
        return HoleCardsList;
    }

    public ICardParent GetCardParent()
    {
        return this as ICardParent;
    }
}
