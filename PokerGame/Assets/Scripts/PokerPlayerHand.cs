using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PokerPlayerHand : MonoBehaviour, ICardParent
{
    public List<CardSO> HoleCardsList; // Array to store the player's hole cards
    private int _playerHandCardLimit = 2;
    public bool? IsPlayerUs { get => _isPlayerUs; }
    [SerializeField] private bool _isPlayerUs;
    [SerializeField] private Transform _handFollowTransform;
    [SerializeField] private AiPlayerBehaviour _aiPlayerBehavior;

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

    public List<CardSO> GetCardListWithCommunityCardsAdded()
    {
        List<CardSO> cardListWithCommunityCardsAdded = PokerDeckManager.Instance.CombineHandWithCommunityCards(HoleCardsList);
        return cardListWithCommunityCardsAdded;
    }

    public bool IsPlayerAiBot()
    {
        return _aiPlayerBehavior != null;
    }

    public TurnManager.PlayerAction AiBotActionPreFlop()
    {
        TurnManager.PlayerAction playerAction = _aiPlayerBehavior.DecidePreFlop();

       // Debug.Log("Pre flop - Player with hole cards" + HoleCardsList[0] + "&" + HoleCardsList[1] +  "made the decision to:  " + playerAction);
        return playerAction;
    }

    public TurnManager.PlayerAction AiBotActionPostFlop(int playerHandRank)
    {
        AiPlayerBehaviour.HandStrength handStrength= _aiPlayerBehavior.HandStrenghtCalculator(playerHandRank);
        TurnManager.PlayerAction playerAction =_aiPlayerBehavior.DecidePostFlop(handStrength); ;
        Debug.Log("Post flop - Player with hand rank: " + playerHandRank + " made the decision to:  " + playerAction);
        return playerAction;
    }
}