using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDisplayer : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    // [SerializeField] private CardSO _cardSO;
    [SerializeField] private GameObject _cardPrefab;
    [SerializeField] private DeckSO _deckSO;

    private void Awake()
    {
        int i = 0;
        foreach (var cardSO in _deckSO.Cardlist)
        {
            var card = Instantiate(_cardPrefab);
            card.GetComponent<SpriteRenderer>().sprite = cardSO.CardSprite;
            card.transform.position = new Vector2(i, 0);
            i++;
        }
    }
}
