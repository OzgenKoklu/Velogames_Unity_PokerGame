using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject _pauseMenuPanel;
    [SerializeField] private Button _pauseMenuButton;

    [SerializeField] private Button _continueGameButton;
    [SerializeField] private Button _newRoundButton;
    [SerializeField] private Button _mainMenuButton;


    private void Awake()
    {
        _pauseMenuButton.onClick.AddListener(ShowPauseMenuPanel);
        _continueGameButton.onClick.AddListener(HidePauseMenuPanel);
        _newRoundButton.onClick.AddListener(InitiateNewRound);
        _mainMenuButton.onClick.AddListener(ReturnToMainMenu);
        _pauseMenuPanel.SetActive(false);
    }
    private void ShowPauseMenuPanel()
    {      
        bool isGamePaused = true;

        GameManager.Instance.SetTimeScale(isGamePaused);

        _pauseMenuPanel.SetActive(true);
    }

    private void HidePauseMenuPanel()
    {
        bool isGamePaused = false;

        GameManager.Instance.SetTimeScale(isGamePaused);

        _pauseMenuPanel.SetActive(false);
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

    private void OnDisable()
    {
        _pauseMenuButton.onClick.RemoveAllListeners();
        _continueGameButton.onClick.RemoveAllListeners();
        _newRoundButton.onClick.RemoveAllListeners();
        _mainMenuButton.onClick.RemoveAllListeners();
    }

}
