using System.Collections.Generic;
using UnityEngine;

public class PokerPlayerHand : MonoBehaviour, ICardParent
{

    public List<CardSO> HoleCardsList; // Array to store the player's hole cards
    private int _playerHandCardLimit = 2;
    [SerializeField] private bool _isPlayerUs;
    [SerializeField] private Transform _handFollowTransform;

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
        //this part needs better explanation and also I need to get rid of the "magic numbers"
        if(_isPlayerUs)
        {
            Transform cardTransform = _handFollowTransform;
            cardTransform.position = new Vector2(-0.6f + (1.2f * (HoleCardsList.Count - 1)), -4);
            cardTransform.rotation = Quaternion.AngleAxis(15f + (-30f * (HoleCardsList.Count - 1)), Vector3.forward);
            return cardTransform;
        }
        else // if player is Ai or Online
        {
            Transform cardTransform = _handFollowTransform;
            cardTransform.rotation = Quaternion.AngleAxis(20f *(HoleCardsList.Count-1), Vector3.forward)  ;
            cardTransform.localScale = Vector3.one *.6f;
            return cardTransform;
        }
        
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
