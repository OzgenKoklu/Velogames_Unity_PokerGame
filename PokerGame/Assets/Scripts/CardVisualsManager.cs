
using UnityEngine;

public class CardVisualsManager : MonoBehaviour
{
    [SerializeField] private Transform _cardPrefab;

    public CardBehaviour SpawnCardObject(CardSO cardSO, ICardParent cardParent)
    {
        Transform cardObjectTransform = Instantiate(_cardPrefab);
        CardBehaviour cardObject = cardObjectTransform.GetComponent<CardBehaviour>();
        cardObject.SetCardScriptableObject(cardSO);
        cardObject.SetCardPostion(cardParent.GetCardFollowTransform().position);
        return cardObject;
    }

}
