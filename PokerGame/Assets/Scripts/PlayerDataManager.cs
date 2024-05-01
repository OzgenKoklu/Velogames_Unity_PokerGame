using System.IO;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    [SerializeField] private PlayerManager MainPlayer;
    [SerializeField] private PlayerData _mainPlayerData;

    public const string FilePathForJson = "Assets/Resources/playerData.json";

    private void Start()
    {
        MainPlayer.PlayerData = LoadPlayerData();

        UpdateLocalFields();
        //_mainPlayerData = MainPlayer.PlayerData;

        GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
        GameManager.Instance.OnTournamentStarted += GameManager_OnTournamentStarted;
        GameManager.Instance.OnMainPlayerWinsTheTournament += GameManager_OnMainPlayerWinsTheTournament;
        BetManager.Instance.OnMainPlayerWin += BetManager_OnMainPlayerWin;
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

