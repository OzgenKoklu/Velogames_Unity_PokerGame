using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject _pauseMenuPanel;
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private Button _pauseMenuButton;

    [SerializeField] private Button _continueGameButton;
    [SerializeField] private Button _newRoundButton;
    [SerializeField] private Button _mainMenuButton;

    //For GameOver Panel
    [SerializeField] private Button _gameOverContinueWatchingGameButton;
    [SerializeField] private Button _gameOverNewRoundButton;
    [SerializeField] private Button _gameOverMainMenuButton;
    [SerializeField] private PlayerManager _mainPlayer;

    private void Awake()
    {
        _pauseMenuButton.onClick.AddListener(ShowPauseMenuPanel);
        _continueGameButton.onClick.AddListener(HidePauseMenuPanel);
        _newRoundButton.onClick.AddListener(InitiateNewRound);
        _mainMenuButton.onClick.AddListener(ReturnToMainMenu);

        _gameOverContinueWatchingGameButton.onClick.AddListener(HideGameOverPanel);
        _gameOverNewRoundButton.onClick.AddListener(InitiateNewRound);
        _gameOverMainMenuButton.onClick.AddListener(ReturnToMainMenu);

        _pauseMenuPanel.SetActive(false);
        _gameOverPanel.SetActive(false);
    }

    private void Start()
    {
        _mainPlayer.OnPlayerBusted += MainPlayer_OnPlayerBusted;
    }

    private void MainPlayer_OnPlayerBusted()
    {
        ShowGameOverPanel();
    }

    private void ShowPauseMenuPanel()
    {
        bool isGamePaused = true;
        GameManager.Instance.SetTimeScale(isGamePaused);
        _pauseMenuPanel.SetActive(true);
    }

    private void ShowGameOverPanel()
    {
        bool isGamePaused = true;
        GameManager.Instance.SetTimeScale(isGamePaused);
        _gameOverPanel.SetActive(true);
    }

    private void HidePauseMenuPanel()
    {
        bool isGamePaused = false;
        GameManager.Instance.SetTimeScale(isGamePaused);
        _pauseMenuPanel.SetActive(false);
    }

    private void HideGameOverPanel()
    {
        bool isGamePaused = false;
        GameManager.Instance.SetTimeScale(isGamePaused);
        _gameOverPanel.SetActive(false);
    }

    private void InitiateNewRound()
    {
        if (GameSceneManager.Instance == null) return;
        HidePauseMenuPanel();
        GameSceneManager.Instance.LoadGameScene();
    }

    private void ReturnToMainMenu()
    {
        if (GameSceneManager.Instance == null) return;
        HidePauseMenuPanel();
        GameSceneManager.Instance.ReturnToMainMenu();
    }

    private void OnDestroy()
    {
        _pauseMenuButton.onClick.RemoveAllListeners();
        _continueGameButton.onClick.RemoveAllListeners();
        _newRoundButton.onClick.RemoveAllListeners();
        _mainMenuButton.onClick.RemoveAllListeners();
    }

}