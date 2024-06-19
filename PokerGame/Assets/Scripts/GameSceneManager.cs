using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance { get; private set; }
    public event Action<Scene> OnSceneLoadCompleted;

    private readonly int _mainMenuSceneIndex = 0;
    private readonly int _mainGameSceneIndex = 1;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Application.platform != RuntimePlatform.Android)
            return;

        if (!Input.GetKey(KeyCode.Escape))
            return;

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        if (currentSceneIndex == _mainMenuSceneIndex)
        {
            QuitGame();
        }
        else if (currentSceneIndex == _mainGameSceneIndex)
        {
            if (GameManager.Instance.IsGamePaused)
            {
                PauseMenuUI.Instance.HidePauseMenuPanel();
            }
            else
            {
                SceneManager.LoadSceneAsync(_mainMenuSceneIndex);
            }
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene " + scene.name + " has loaded!");

        if (scene.buildIndex == _mainGameSceneIndex && GameManager.Instance != null)
        {
            OnSceneLoadCompleted?.Invoke(scene);
        }
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(_mainMenuSceneIndex);
    }

    public void LoadGameScene()
    {
        if (PlayerDataManager.Instance.IsPlayerDataHandlingSuccessful)
        {
            SceneManager.LoadScene(_mainGameSceneIndex);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}