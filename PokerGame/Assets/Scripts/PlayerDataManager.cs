using Firebase.Auth;
using System.IO;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System;

public class PlayerDataManager : MonoBehaviour
{
    [SerializeField] private string FirebaseUserId; // Store Firebase user ID here

    public const string FilePathForJson = "Assets/Resources/playerData.json";
    public const string FirebaseDataPath = "playerData";

    //[SerializeField] private PlayerManager MainPlayer;
    [SerializeField] private PlayerData _playerData;
    private DatabaseReference _databaseReference;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        _playerData = LoadPlayerData();

    }
    private void Start()
    {
        FirebaseAuthManager.Instance.OnLoginSuccessful += Firebase_OnLoginSuccessful;
    }

    /*
    MainPlayer.PlayerData = LoadPlayerData();

    UpdateLocalFields();
    //_mainPlayerData = MainPlayer.PlayerData;

    GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
    GameManager.Instance.OnTournamentStarted += GameManager_OnTournamentStarted;
    GameManager.Instance.OnMainPlayerWinsTheTournament += GameManager_OnMainPlayerWinsTheTournament;
    BetManager.Instance.OnMainPlayerWin += BetManager_OnMainPlayerWin;
    */

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
            }
            else
            {
                Debug.Log("No player data found for user. Creating new one locally and on Firebase.");
                _playerData = GenerateNewPlayerData();
                UploadPlayerDataToFirebase();
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error downloading player data: " + e.Message);
        }
        /*
        playerDataRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error downloading player data: " + task.Exception.Message);
            }
            else if (task.Result.Exists)
            {
                string downloadedJson = task.Result.GetRawValue().ToString();
                _playerData = JsonUtility.FromJson<PlayerData>(downloadedJson);
                Debug.Log("Downloaded player data from Firebase.");
            }
            else
            {
                Debug.Log("No player data found for user. Creating new one locally and on Firebase.");
                _playerData = GenerateNewPlayerData();
                UploadPlayerDataToFirebase();
            }
        });*/
    }

    private PlayerData GenerateNewPlayerData()
    {
        PlayerData newPlayerData = new PlayerData();
        newPlayerData.Name = FirebaseUserId + "ID";
        newPlayerData.Id = FirebaseUserId;

        // Set all other data to zero (or your desired initial values)
        //newPlayerData.TournamentsAttended = 0;
        //newPlayerData.TournamentsWon = 0;
        // ... other data fields ...

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

        /* try
         {
             DatabaseReference playerDataRef = _databaseReference.Child(FirebaseDataPath).Child(FirebaseUserId);

             playerDataRef.SetValueAsync(jsonData)
                 .ContinueWithOnMainThread(task =>
                 {
                     if (task.IsFaulted)
                     {
                         Debug.LogError("Error uploading player data: " + task.Exception.Message);
                     }
                     else
                     {
                         Debug.Log("Player data uploaded to Firebase successfully!");
                     }
                 });


         }catch(Exception e)
         {
             Debug.LogError("Error uploading player data: " + e.Message + "\nInner Exception: " + e.InnerException.Message);
         }*/
    }



    int _tournamentsAttended;
    int _tournamentsWon;

    int _showDownsAttended;
    int _showDownsWon;

    int _allInShowdownsAttended;
    int _allInShowdownsWon;

    int _allHandsAttended;
    int _allHandsWon;

    /*
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
        PlayerData playerData = MainPlayer.PlayerData;
        playerData.TournamentsWon = _tournamentsWon;
        playerData.TournamentsAttended = _tournamentsAttended;
        playerData.AllHandsWon = _allHandsWon;
        playerData.AllHandsAttended = _allHandsAttended;
        playerData.AllInShowdownsWon = _allInShowdownsWon;
        playerData.AllInShowdownsAttended = _allInShowdownsAttended;
        playerData.ShowDownsWon = _showDownsWon;
        playerData.ShowDownsAttended = _showDownsAttended;
    }

    private void UpdateLocalFields()
    {
        PlayerData playerData = MainPlayer.PlayerData;
        _tournamentsWon = playerData.TournamentsWon;
        _tournamentsAttended = playerData.TournamentsAttended;
        _allHandsWon = playerData.AllHandsWon;
        _allHandsAttended = playerData.AllHandsAttended;
        _allInShowdownsWon = playerData.AllInShowdownsWon;
        _allInShowdownsAttended = playerData.AllInShowdownsAttended;
        _showDownsWon = playerData.ShowDownsWon;
        _showDownsAttended = playerData.ShowDownsAttended;
    }


    */
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

