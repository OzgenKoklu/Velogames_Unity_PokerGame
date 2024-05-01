using System.IO;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    [SerializeField] private PlayerManager MainPlayer;
    [SerializeField] private PlayerData _mainPlayerData;

    public const string FilePathForJson = "Assets/Resources/playerData.json";

    private void OnEnable()
    {

    }



    private void Start()
    {
        MainPlayer.PlayerData = LoadPlayerData();

        GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
        GameManager.Instance.OnTournamentStarted += GameManager_OnTournamentStarted;
        GameManager.Instance.OnMainPlayerWinsTheTournament += GameManager_OnMainPlayerWinsTheTournament;
        BetManager.Instance.OnMainPlayerWin += BetManager_OnMainPlayerWin;
    }

    /*
    public int Id { get; set; }
    public string Name { get; set; }

    //PLAYER STATS
    public int CurrentBalance { get; set; } (maybe login bonus - > TotalMoney - 1000 (Tournament feed), + - whatever comes out of the tournament.
    public int TournamentsWon { get; set; } --->Check 
    public int TournamentsAttended { get; set; }--->Check 
    public float PercentageOfHandsWon { get; set; }--->Check allhandWon/attended 
    public float PercentageOfAllInsWon { get; set; }--->Check  allInWon/allInAttended
    public float ShowDownsWon { get; set; } --->Check 
    public float ShowDownsAttended { get; set; }--->Check 
*/
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
        }
        else if (state == GameManager.GameState.EveryoneFolded)
        {
            if (MainPlayer.IsPlayerActive)
            {
                _allHandsAttended++;
                _allHandsWon++;
            }
        }
    }

    public void SavePlayerData()
    {
        string jsonData = JsonUtility.ToJson(MainPlayer.PlayerData);

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

    public PlayerData LoadPlayerData()
    {
        PlayerData playerData = new PlayerData() { Name = "DefaultName" };
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

