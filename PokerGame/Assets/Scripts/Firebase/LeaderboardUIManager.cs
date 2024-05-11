using System;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardUIManager : MonoBehaviour
{
    public event Action<LeaderboardEntry, LeaderboardEntryObject> OnLeaderboardEntryCreated;
    public event Action<bool> OnLeaderboardUIManagerStateChanged;

    [SerializeField] private LeaderboardManager _leaderboardManager;
    [SerializeField] private LeaderboardEntryObject _leaderboardEntryObject;
    [SerializeField] private RectTransform _leaderboardEntryParent;
    [SerializeField] private float _leaderboardEntryHeight = 75f;

    void OnEnable()
    {
        _leaderboardManager.OnGetLeaderboardEntriesComplete += LeaderboardManager_OnGetLeaderboardEntriesComplete;
        ClearLeaderboardItems();
    }

    private void LeaderboardManager_OnGetLeaderboardEntriesComplete(List<LeaderboardEntry> leaderboardEntries)
    {
        Debug.Log("Handling leaderboard entries");

        _leaderboardEntryParent.sizeDelta = new Vector2(_leaderboardEntryParent.sizeDelta.x, leaderboardEntries.Count * _leaderboardEntryHeight);

        foreach (LeaderboardEntry leaderboardEntry in leaderboardEntries)
        {
            var leaderboardEntryObject = Instantiate(_leaderboardEntryObject, _leaderboardEntryParent);
            leaderboardEntryObject.gameObject.SetActive(true);
            OnLeaderboardEntryCreated?.Invoke(leaderboardEntry, leaderboardEntryObject);
        }
    }

    private void ClearLeaderboardItems()
    {
        if (_leaderboardEntryParent.childCount > 2)
        {
            for (int i = 0; i < _leaderboardEntryParent.childCount; i++)
            {
                if (i == 0 || i == 1)
                {
                    continue;
                }
                Destroy(_leaderboardEntryParent.GetChild(i).gameObject);
            }
        }
    }

    private void OnDisable()
    {
        _leaderboardManager.OnGetLeaderboardEntriesComplete -= LeaderboardManager_OnGetLeaderboardEntriesComplete;
        OnLeaderboardUIManagerStateChanged?.Invoke(false);
    }
}
