
using UnityEngine;

public class CardVisualsManager : MonoBehaviour
{
    [SerializeField] private Transform _cardPrefab;

    public static CardVisualsManager Instance { get; private set; }   
    private void Awake()
    {
        Instance = this;   
    }
    public CardBehaviour SpawnCardObject(CardSO cardSO, ICardParent cardParent)
    {
        Transform cardObjectTransform = Instantiate(_cardPrefab);
        CardBehaviour cardObject = cardObjectTransform.GetComponent<CardBehaviour>();
        cardObject.SetCardScriptableObject(cardSO);
        cardObject.SetCardTransform(cardParent.GetCardFollowTransform());
        return cardObject;
    }

}
