
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static pheval.Rank;

public class CardVisualsManager : MonoBehaviour
{
    [SerializeField] private Transform _cardPrefab;
    private List<CardBehaviour> _allActiveCards = new List<CardBehaviour>();
    private List<CardBehaviour> _allSpawnedCards = new List<CardBehaviour>();

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
        _allSpawnedCards.Add(cardObject);
        _allActiveCards.Add(cardObject);
        return cardObject;
    }

    public bool IsThereAnySpawnedCards()
    {
        if (_allSpawnedCards.Count != 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void DestroyAllActiveCards()
    {
        // Loop through each CardBehaviour in the list
        foreach (CardBehaviour activeCard in _allSpawnedCards)
        {
            // Check if the activeCard has a valid attached GameObject
            if (activeCard != null && activeCard.gameObject != null)
            {
                // Destroy the attached GameObject using Destroy()
                Destroy(activeCard.gameObject);
            }
        }

        // Clear the allActiveCards list (optional)
        _allSpawnedCards.Clear();
        _allActiveCards.Clear();
    }

    public void HighlightHand(List<CardSO> WinningCardList, string winningHandCode, bool isTie)
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
                    if (isTie)
                    {
                        if (cardSO.CardParent is CommunityCards)
                        {
                            winningHandCode = winningHandCode.Remove(index, 1);
                        }
                        else
                        {
                            //!!!Implement further logic if needed
                            //First cards in the WinningCardList is CommunityCards by nature. 
                            //After getting through them and removing them from winningHandCode string
                            //the tie hands should be highlighted
                            //This might need an extra list to work
                            //e.g.: Winning hand is AA421 
                            //AA2 are community cards. 4 1 remains in the winningHandCode
                            //The max amount of community cards in 5-char code > 5 Char
                            //Least >> 3 char. 
                            //maybe we dont need to implement anything extra, Im just confused at the moment.
                        }
                    }
                    else // single winner, just remove the winning hand code.
                    {
                        winningHandCode = winningHandCode.Remove(index, 1);
                    }

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
            _allActiveCards.Remove(cardToRemove);
        }
    }

    //Triggering these two in showdown but it might happen earlier if all the active players go "All in" 
    public void GetToShowdownPosition()
    {
        foreach (CardBehaviour cardBehaviour in _allActiveCards)
        {
            cardBehaviour.GetToShowDownPosition();
        }
    }

    //Triggering these two in showdown but it might happen earlier if all the active players go "All in" 
    public void FlipAllCards()
    {
        foreach (CardBehaviour cardBehaviour in _allActiveCards)
        {
            cardBehaviour.FlipCard();
        }
    }

    private void OnDestroy()
    {
        PlayerManager.OnPlayerFolded -= PlayerManager_OnPlayerFolded;
    }
}
