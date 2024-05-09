using System;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardUIManager : MonoBehaviour
{
    public event Action<LeaderboardEntry, LeaderboardEntryObject> OnLeaderboardEntryCreated;

    [SerializeField] private LeaderboardManager _leaderboardManager;
    [SerializeField] private LeaderboardEntryObject _leaderboardEntryObject;
    [SerializeField] private RectTransform _leaderboardEntryParent;
    [SerializeField] private float _leaderboardEntryHeight = 75f;

    void Start()
    {
        _leaderboardManager.OnGetLeaderboardEntriesComplete += LeaderboardManager_OnGetLeaderboardEntriesComplete;
    }

    private void LeaderboardManager_OnGetLeaderboardEntriesComplete(List<LeaderboardEntry> leaderboardEntries)
    {
        _leaderboardEntryParent.sizeDelta = new Vector2(_leaderboardEntryParent.sizeDelta.x, leaderboardEntries.Count * _leaderboardEntryHeight);

        foreach (LeaderboardEntry leaderboardEntry in leaderboardEntries)
        {
            var leaderboardEntryObject = Instantiate(_leaderboardEntryObject, _leaderboardEntryParent);
            leaderboardEntryObject.gameObject.SetActive(true);
            OnLeaderboardEntryCreated?.Invoke(leaderboardEntry, leaderboardEntryObject);
        }
    }

    private void OnDestroy()
    {
        _leaderboardManager.OnGetLeaderboardEntriesComplete -= LeaderboardManager_OnGetLeaderboardEntriesComplete;
    }
}
