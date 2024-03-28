using System.Collections.Generic;
using UnityEngine;

public interface ICardParent
{
    public bool? IsPlayerUs { get; }

    public Transform GetCardFollowTransform();

    public void AddCard(CardSO newCard);

    public List<CardSO> GetCardList();

    public void ClearCards();
}
