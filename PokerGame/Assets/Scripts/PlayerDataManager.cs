using Firebase.Auth;
using System.IO;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance { get; private set; }
    [SerializeField] private string FirebaseUserId; // Store Firebase user ID here

    public event Action OnSuccessfulPlayerDataHandling;

    public const string FilePathForJson = "Assets/Resources/playerData.json";
    public const string FirebaseDataPath = "playerData";

    [SerializeField] private PlayerManager MainPlayer;
    [SerializeField] private PlayerData _playerData;
    private DatabaseReference _databaseReference;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
        //_playerData = LoadPlayerData();

    }
    private void Start()
    {
        FirebaseAuthManager.Instance.OnLoginSuccessful += Firebase_OnLoginSuccessful;
        GameSceneManager.Instance.OnSceneLoadCompleted += GameSceneManager_OnSceneLoadCompleted;
    }

    private void GameSceneManager_OnSceneLoadCompleted(Scene scene)
    {
        if (scene.buildIndex != 1) return;
        MainPlayer = GameManager.Instance.MainPlayer;
        PlayerData playerData = MainPlayer.PlayerData;
        playerData = _playerData;

        UpdateLocalFields();

        GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
        GameManager.Instance.OnTournamentStarted += GameManager_OnTournamentStarted;
        GameManager.Instance.OnMainPlayerWinsTheTournament += GameManager_OnMainPlayerWinsTheTournament;
        GameManager.Instance.OnTournamentEnded += GameManager_OnTournamentEnded;
        BetManager.Instance.OnMainPlayerWin += BetManager_OnMainPlayerWin;


        //for test
        UploadPlayerDataToFirebaseForLeaderboard();
    }

    private void GameManager_OnTournamentEnded()
    {
        //Take Player Data _playerData. 
        //Make some adjustments on it like getting win ratios etc.
        //upload it to firebase leaderboards. 
        //for now lets keep it simple, take HandWinRatio = (float) allHandsWon/allHandsAttended. 
        //Upload it to firebase leaderboards with all the other necessary things.
        UploadPlayerDataToFirebaseForLeaderboard();
    }

    private void UploadPlayerDataToFirebaseForLeaderboard()
    {
        // Calculate win ratios (optional, adjust calculations as needed)
        float handWinRatio = _allHandsWon > 0 ? (float)_allHandsWon / _allHandsAttended : 0f;
        float showdownWinRatio = _showDownsWon > 0 ? (float)_showDownsWon / _showDownsAttended : 0f;
        float allInWinRatio = _allInShowdownsWon > 0 ? (float)_allInShowdownsWon / _allInShowdownsAttended : 0f;
        // tournaments won vs attended da olur. 
        // Prepare leaderboard entry data
        Dictionary<string, object> leaderboardEntry = new Dictionary<string, object>()
        {
            { "playerId", FirebaseUserId }, // Replace with appropriate player ID field if different
            { "playerName", _playerData.Name }, // Optional: Player name
            { "handWinRatio", handWinRatio },
            { "showdownWinRatio", showdownWinRatio },
            { "allInWinRatio", allInWinRatio },
         };

        // Upload data to Firebase Realtime Database leaderboard path
        string leaderboardPath = "leaderboards/pokerStats"; // Replace with your leaderboard path
        DatabaseReference leaderboardRef = FirebaseDatabase.DefaultInstance.GetReference(leaderboardPath);

        // Here, you can choose between two approaches:

        // Option 1: Push data under a unique identifier (consider using ServerValue.Timestamp)
        leaderboardRef.Push().SetValueAsync(leaderboardEntry)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Error uploading data to leaderboard: " + task.Exception.Message);
                }
                else
                {
                    Debug.Log("Player data uploaded to leaderboard successfully!");
                }
            });

        // Option 2: Update data under player ID (assuming unique player IDs)
        // leaderboardRef.Child(FirebaseUserId).SetValueAsync(leaderboardEntry)
        //     .ContinueWithOnMainThread(task =>
        //     {
        //       // ... (handle success/failure)
        //     });

        // Choose the approach that best suits your leaderboard structure and data management needs.
    }

    private void Firebase_OnLoginSuccessful()
    {
        _databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user != null)
        {
            FirebaseUserId = user.UserId;
            DownloadPlayerDataFromFirebase();
        }
        else
        {
            Debug.LogWarning("Firebase user not logged in. Waiting for login...");
        }

        DownloadPlayerDataFromFirebase();
    }

    private async void DownloadPlayerDataFromFirebase()
    {
        DatabaseReference playerDataRef = _databaseReference.Child(FirebaseDataPath).Child(FirebaseUserId);

        try
        {
            DataSnapshot snapshot = await playerDataRef.GetValueAsync();

            if (snapshot.Exists)
            {
                string downloadedJson = snapshot.GetValue(true).ToString();  // Use GetValue(true)
                _playerData = JsonUtility.FromJson<PlayerData>(downloadedJson);
                Debug.Log("Downloaded player data from Firebase.");
                OnSuccessfulPlayerDataHandling?.Invoke();
            }
            else
            {
                Debug.Log("No player data found for user. Creating new one locally and on Firebase.");
                _playerData = GenerateNewPlayerData();
                UploadPlayerDataToFirebase();
                OnSuccessfulPlayerDataHandling?.Invoke();
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error downloading player data: " + e.Message);
        }
    }

    private PlayerData GenerateNewPlayerData()
    {
        PlayerData newPlayerData = new PlayerData();
        int fourLetterRandomValue = UnityEngine.Random.Range(1000, 10000);
        newPlayerData.Name = "Player" + fourLetterRandomValue.ToString();
        newPlayerData.Id = fourLetterRandomValue.ToString();

        return newPlayerData;
    }

    private async void UploadPlayerDataToFirebase()
    {
        string jsonData = JsonUtility.ToJson(_playerData);

        DatabaseReference playerDataRef = _databaseReference.Child(FirebaseDataPath).Child(FirebaseUserId);

        try
        {
            await playerDataRef.SetValueAsync(jsonData);
            Debug.Log("Player data uploaded to Firebase successfully!");
        }
        catch (Exception e)
        {
            Debug.LogError("Error uploading player data: " + e.Message + "\nInner Exception: " + e.InnerException.Message);
        }

    }

    int _tournamentsAttended;
    int _tournamentsWon;

    int _showDownsAttended;
    int _showDownsWon;

    int _allInShowdownsAttended;
    int _allInShowdownsWon;

    int _allHandsAttended;
    int _allHandsWon;


    private void BetManager_OnMainPlayerWin()
    {
        if (MainPlayer.IsPlayerAllIn)
        {
            _allInShowdownsWon++;
        }
        _allHandsWon++;
        _showDownsWon++;
        Debug.Log("Main player won.");

    }
    private void GameManager_OnTournamentStarted()
    {
        _tournamentsAttended++;
    }

    private void GameManager_OnMainPlayerWinsTheTournament()
    {
        _tournamentsWon++;
    }

    private void GameManager_OnGameStateChanged(GameManager.GameState state)
    {
        Debug.Log("Player Data Manager State Changed -sub");
        if (state == GameManager.GameState.Showdown)
        {
            if (MainPlayer.IsPlayerActive)
            {
                _allHandsAttended++;
                _showDownsAttended++;
            }
            if (MainPlayer.IsPlayerAllIn)
            {
                _allInShowdownsAttended++;
            }

            SavePlayerData();
        }
        else if (state == GameManager.GameState.EveryoneFolded)
        {
            if (MainPlayer.IsPlayerActive)
            {
                _allHandsAttended++;
                _allHandsWon++;
            }

            SavePlayerData();
        }
    }

    public void SavePlayerData()
    {
        //if (_mainPlayerData == null) return; 
        UpdatePlayerData();
        PlayerData playerData = MainPlayer.PlayerData;
        string jsonData = JsonUtility.ToJson(playerData);

        try
        {
            UploadPlayerDataToFirebase();
            File.WriteAllText(FilePathForJson, jsonData);
            Debug.Log("Player data saved successfully!");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error saving player data: " + e.Message);
        }
    }

    private void UpdatePlayerData()
    {

        _playerData.TournamentsWon = _tournamentsWon;
        _playerData.TournamentsAttended = _tournamentsAttended;
        _playerData.AllHandsWon = _allHandsWon;
        _playerData.AllHandsAttended = _allHandsAttended;
        _playerData.AllInShowdownsWon = _allInShowdownsWon;
        _playerData.AllInShowdownsAttended = _allInShowdownsAttended;
        _playerData.ShowDownsWon = _showDownsWon;
        _playerData.ShowDownsAttended = _showDownsAttended;
        /*
        PlayerData playerData = MainPlayer.PlayerData;
        playerData.TournamentsWon = _tournamentsWon;
        playerData.TournamentsAttended = _tournamentsAttended;
        playerData.AllHandsWon = _allHandsWon;
        playerData.AllHandsAttended = _allHandsAttended;
        playerData.AllInShowdownsWon = _allInShowdownsWon;
        playerData.AllInShowdownsAttended = _allInShowdownsAttended;
        playerData.ShowDownsWon = _showDownsWon;
        playerData.ShowDownsAttended = _showDownsAttended;*/
    }

    private void UpdateLocalFields()
    {
        PlayerData playerData = _playerData;
        _tournamentsWon = playerData.TournamentsWon;
        _tournamentsAttended = playerData.TournamentsAttended;
        _allHandsWon = playerData.AllHandsWon;
        _allHandsAttended = playerData.AllHandsAttended;
        _allInShowdownsWon = playerData.AllInShowdownsWon;
        _allInShowdownsAttended = playerData.AllInShowdownsAttended;
        _showDownsWon = playerData.ShowDownsWon;
        _showDownsAttended = playerData.ShowDownsAttended;
    }



    public PlayerData LoadPlayerData()
    {
        PlayerData playerData = new PlayerData();
        if (!File.Exists(FilePathForJson))
        {
            Debug.Log("Player data file not found. Creating a new one.");
            return playerData;
        }

        string jsonData;
        try
        {
            jsonData = File.ReadAllText(FilePathForJson);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error loading player data: " + e.Message);
            return null;
        }

        playerData = JsonUtility.FromJson<PlayerData>(jsonData);

        Debug.Log("Player data loaded successfully!");
        return playerData;

    }
}

