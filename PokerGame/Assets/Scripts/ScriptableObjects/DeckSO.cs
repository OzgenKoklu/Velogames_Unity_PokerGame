using System.Collections.Generic;
using UnityEngine;
public class DeckSO : ScriptableObject
{
   [SerializeField] public List<CardSO> Cardlist = new List<CardSO>(); 
}