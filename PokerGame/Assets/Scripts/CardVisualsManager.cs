
using System.Collections.Generic;
using UnityEngine;
using static pheval.Rank;

public class CardVisualsManager : MonoBehaviour
{
    [SerializeField] private Transform _cardPrefab;
    private List<CardBehaviour> allActiveCards = new List<CardBehaviour>();

    public static CardVisualsManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        PlayerManager.OnPlayerFolded += PlayerManager_OnPlayerFolded;
    }

    private void PlayerManager_OnPlayerFolded(PlayerManager foldedPlayer)
    {
        List<CardSO> foldedCardSOList = foldedPlayer.PlayerHand.GetCardList();
        RemovePlayerCardsFromActiveCards(foldedCardSOList);
    }

    public CardBehaviour SpawnCardObject(CardSO cardSO, ICardParent cardParent)
    {
        Transform cardObjectTransform = Instantiate(_cardPrefab);
        CardBehaviour cardObject = cardObjectTransform.GetComponent<CardBehaviour>();
        cardObject.SetCardScriptableObject(cardSO);
        cardObject.SetCardTransform(cardParent.GetCardFollowTransform());
        allActiveCards.Add(cardObject);
        return cardObject;
    }

    public void HighlightHand(List<CardSO> WinningCardList, string winningHandCode)
    {
        foreach (CardSO cardSO in WinningCardList)
        {
            char firstKey = cardSO.CardCode[0]; //getting first key
            string rankToCheck = firstKey.ToString().ToUpper();//for string operations
            if (winningHandCode.Contains(rankToCheck))
            {
                int index = winningHandCode.IndexOf(rankToCheck);

                if (index != -1) // Make sure the rank was found
                {
                    //winningHandCode = winningHandCode.Remove(index, firstKey);

                    CardBehaviour cardBehaviour = cardSO.CardBehavior;
                    cardBehaviour.SetCardAsSelected();
                    Debug.Log(firstKey + "from cardlist card > " + cardSO + "  will be highlighted");
                }

            }

        }
    }

    public void RemovePlayerCardsFromActiveCards(List<CardSO> cardSOList)
    {
        foreach (var cardSO in cardSOList)
        {
            CardBehaviour cardToRemove = cardSO.CardBehavior;
            allActiveCards.Remove(cardToRemove);
        }
    }

    public void GetToShowdownPosition()
    {
        foreach (CardBehaviour cardBehaviour in allActiveCards)
        {
            cardBehaviour.GetToShowDownPosition();
        }
    }

    public void FlipAllCards()
    {
        foreach (CardBehaviour cardBehaviour in allActiveCards)
        {
            cardBehaviour.FlipCard();
        }
    }

}
