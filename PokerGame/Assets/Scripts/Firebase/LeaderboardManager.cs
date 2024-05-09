using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
    public event Action<List<LeaderboardEntry>> OnGetLeaderboardEntriesComplete;

    [SerializeField] private int _leaderboardLimit = 10;

    private void Start()
    {
        GetAllLeaderboardEntries();
    }

    // Method to fetch all leaderboard entries
    private void GetAllLeaderboardEntries()
    {
        List<LeaderboardEntry> entries = new List<LeaderboardEntry>();

        string leaderboardPath = "leaderboards/pokerStats"; // Replace with your leaderboard path
        DatabaseReference leaderboardRef = FirebaseDatabase.DefaultInstance.GetReference(leaderboardPath);

        leaderboardRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error fetching leaderboard data: " + task.Exception.Message);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                IEnumerable<DataSnapshot> children = snapshot.Children;

                for (int i = 0; i < _leaderboardLimit; i++)
                {
                    if (i >= children.Count())
                    {
                        break;
                    }

                    DataSnapshot child = children.ElementAt(i);

                    // Deserialize the leaderboard entry data
                    LeaderboardEntry entry = new LeaderboardEntry
                    {
                        PlayerName = child.Child("playerName").Value.ToString(),
                        HandWinRatio = float.Parse(child.Child("handWinRatio").Value.ToString()),
                        ShowdownWinRatio = float.Parse(child.Child("showdownWinRatio").Value.ToString()),
                        AllInWinRatio = float.Parse(child.Child("allInWinRatio").Value.ToString())
                    };

                    entries.Add(entry);
                }

                entries.Sort((a, b) => b.HandWinRatio.CompareTo(a.HandWinRatio));

                OnGetLeaderboardEntriesComplete?.Invoke(entries);
            }
        });
    }

    // Leaderboard Button Click Event
    public void GetLeaderboard()
    {
        GetAllLeaderboardEntries();
    }
}
