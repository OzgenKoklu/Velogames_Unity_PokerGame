using TMPro;
using UnityEngine;

public class ProfilePageUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _playerName;
    [SerializeField] private TMP_Text _currentBalance;
    [SerializeField] private TMP_Text _tournamentsAttended;
    [SerializeField] private TMP_Text _tournamentsWon;
    [SerializeField] private TMP_Text _allHandsAttended;
    [SerializeField] private TMP_Text _allHandsWon;
    [SerializeField] private TMP_Text _allInShowdownsAttended;
    [SerializeField] private TMP_Text _allInShowdownsWon;
    [SerializeField] private TMP_Text _showDownsAttended;
    [SerializeField] private TMP_Text _showDownsWon;

    private void OnEnable()
    {
        UpdateProfilePage();
    }

    private void UpdateProfilePage()
    {
        var playerData = PlayerDataManager.Instance.GetPlayerData();

        if (playerData == null)
        {
            Debug.Log("Player data is null");
            return;
        }

        _playerName.text = playerData.Name;
        _currentBalance.text = playerData.CurrentBalance.ToString();
        _tournamentsAttended.text = playerData.TournamentsAttended.ToString();
        _tournamentsWon.text = playerData.TournamentsWon.ToString();
        _allHandsAttended.text = playerData.AllHandsAttended.ToString();
        _allHandsWon.text = playerData.AllHandsWon.ToString();
        _allInShowdownsAttended.text = playerData.AllInShowdownsAttended.ToString();
        _allInShowdownsWon.text = playerData.AllInShowdownsWon.ToString();
        _showDownsAttended.text = playerData.ShowDownsAttended.ToString();
        _showDownsWon.text = playerData.ShowDownsWon.ToString();
    }

    private void ClearAllPlayerDataFields()
    {
        _playerName.text = "";
        _currentBalance.text = "";
        _tournamentsAttended.text = "";
        _tournamentsWon.text = "";
        _allHandsAttended.text = "";
        _allHandsWon.text = "";
        _allInShowdownsAttended.text = "";
        _allInShowdownsWon.text = "";
        _showDownsAttended.text = "";
        _showDownsWon.text = "";
    }

    private void OnDisable()
    {
        ClearAllPlayerDataFields();
    }
}
