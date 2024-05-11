using UnityEngine;
using TMPro;
using System.Collections;

// Define a class to hold leaderboard entry data
public class LeaderboardEntry
{
    public string PlayerName { get; set; }
    public float HandWinRatio { get; set; }
    public float ShowdownWinRatio { get; set; }
    public float AllInWinRatio { get; set; }
}

public class LeaderboardEntryObject : MonoBehaviour
{
    [Header("-")]
    [SerializeField] private LeaderboardUIManager _leaderboardUIManager;

    [Header("Entry's Text Areas")]
    [SerializeField] private TMP_Text _playerName;
    [SerializeField] private TMP_Text _handWinRatio;
    [SerializeField] private TMP_Text _showdownWinRatio;
    [SerializeField] private TMP_Text _allInWinRatio;

    private void OnEnable()
    {
        _leaderboardUIManager.OnLeaderboardUIManagerStateChanged += LeaderboardUIManager_OnLeaderboardUIManagerStateChanged;
        _leaderboardUIManager.OnLeaderboardEntryCreated += LeaderboardUIManager_OnLeaderboardEntryCreated;
    }

    private void LeaderboardUIManager_OnLeaderboardUIManagerStateChanged(bool isActive)
    {
        if (!isActive)
        {
            _leaderboardUIManager.OnLeaderboardUIManagerStateChanged -= LeaderboardUIManager_OnLeaderboardUIManagerStateChanged;
            _leaderboardUIManager.OnLeaderboardEntryCreated -= LeaderboardUIManager_OnLeaderboardEntryCreated;
        }
    }

    private void LeaderboardUIManager_OnLeaderboardEntryCreated(LeaderboardEntry entry, LeaderboardEntryObject entryObject)
    {
        if (this == entryObject)
        {
            StartCoroutine(UpdateUIText(entry));
        }

        Debug.Log(entry.PlayerName + " " + entry.HandWinRatio + " " + entry.ShowdownWinRatio + " " + entry.AllInWinRatio);
    }

    private IEnumerator UpdateUIText(LeaderboardEntry entry)
    {
        yield return new WaitUntil(() => _playerName != null && _handWinRatio != null && _showdownWinRatio != null && _allInWinRatio != null);
        _playerName.text = entry.PlayerName;
        _handWinRatio.text = entry.HandWinRatio.ToString();
        _showdownWinRatio.text = entry.ShowdownWinRatio.ToString();
        _allInWinRatio.text = entry.AllInWinRatio.ToString();
    }

    private void OnDisable()
    {
        if (_leaderboardUIManager != null)
        {
            _leaderboardUIManager.OnLeaderboardUIManagerStateChanged -= LeaderboardUIManager_OnLeaderboardUIManagerStateChanged;
            Destroy(this);
        }
    }
}