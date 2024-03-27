using UnityEngine;

public class PokerPlayerHand : MonoBehaviour
{
  
        public CardSO[] HoleCards = new CardSO[2]; // Array to store the player's hole cards

    public void AddCardToHand(CardSO newCard)
    {
        for (int i = 0; i < HoleCards.Length; i++)
        {
            if (HoleCards[i] == null) // Check if there is space in the hand
            {
                HoleCards[i] = newCard; // Add the card to the hand
                return; // Exit the method
            }
        }
        Debug.LogWarning("Attempted to add a card to a full hand.");
    }

}
