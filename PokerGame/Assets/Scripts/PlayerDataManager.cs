using System.IO;
using UnityEditor.Build;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    [SerializeField] private PlayerManager MainPlayer;
    [SerializeField] private PlayerData _mainPlayerData;

    public const string FilePathForJson = "Assets/Resources/playerData.json";

    private void OnEnable()
    {
        GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
    }

    private void Start()
    {
        MainPlayer.PlayerData = LoadPlayerData();
    }

    /*
    public int Id { get; set; }
    public string Name { get; set; }

    //PLAYER STATS
    public int CurrentBalance { get; set; }
    public int TournamentsWon { get; set; }
    public int TournamentsAttended { get; set; }
    public float PercentageOfHandsWon { get; set; }
    public float PercentageOfAllInsWon { get; set; }
    public float ShowDownsWon { get; set; }
    public float ShowDownsAttended { get; set; }
*/
    int showDownsAttended;
    int showDownsWon;
    int allInShowdownsAttended;
    int allInShowdownsWon;
    int allHandsAttended;
    int allHandsWon;

    private void GameManager_OnGameStateChanged(GameManager.GameState state)
    {
        if(state == GameManager.GameState.Showdown)
        {
            if (MainPlayer.IsPlayerActive)
            {
                allHandsAttended++;
                showDownsAttended++;
            }
            if(MainPlayer.IsPlayerAllIn)
            {
                allHandsAttended++;
                allInShowdownsAttended++;
            }
        }else if(state == GameManager.GameState.EveryoneFolded)
        {
            if(MainPlayer.IsPlayerActive)
            {
                allHandsAttended++;
                allHandsWon++;
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

