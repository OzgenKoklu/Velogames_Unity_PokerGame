using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
    public event Action<List<LeaderboardEntry>> OnGetLeaderboardEntriesComplete;

    [SerializeField] private int _leaderboardLimit = 10;

    // Method to fetch all leaderboard entries
    private IEnumerator GetAllLeaderboardEntries()
    {
        List<LeaderboardEntry> entries = new List<LeaderboardEntry>();

        string leaderboardPath = "leaderboards/pokerStats"; // Replace with your leaderboard path
        DatabaseReference leaderboardRef = FirebaseDatabase.DefaultInstance.GetReference(leaderboardPath);

        Task<DataSnapshot> DBTask = leaderboardRef.GetValueAsync();
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            int iterationCount = _leaderboardLimit;

            DataSnapshot snapshot = DBTask.Result;

            if (snapshot.ChildrenCount < _leaderboardLimit)
            {
                iterationCount = (int)snapshot.ChildrenCount;
            }

            for (int i = 0; i < iterationCount; i++)
            {
                DataSnapshot child = snapshot.Children.ElementAt(i);

                if (child == null)
                {
                    continue; // Skip to the next iteration if child is null
                }

                // Deserialize the leaderboard entry data
                LeaderboardEntry entry = new LeaderboardEntry
                {
                    PlayerName = child?.Child("playerName").Value?.ToString() ?? "",
                    HandWinRatio = float.TryParse(child.Child("handWinRatio").Value.ToString(), out float valueHandWinRatio) ? valueHandWinRatio : 0f,
                    ShowdownWinRatio = float.TryParse(child.Child("showdownWinRatio").Value.ToString(), out float valueShowdownWinRatio) ? valueShowdownWinRatio : 0f,
                    AllInWinRatio = float.TryParse(child.Child("allInWinRatio").Value.ToString(), out float valueAllInWinRatio) ? valueAllInWinRatio : 0f
                };

                entries.Add(entry);
            }

            entries.Sort((a, b) => b.HandWinRatio.CompareTo(a.HandWinRatio));

            OnGetLeaderboardEntriesComplete?.Invoke(entries);
        }
    }

    // Leaderboard Button Click Event
    public void GetLeaderboard()
    {
        StartCoroutine(GetAllLeaderboardEntries());
    }
}