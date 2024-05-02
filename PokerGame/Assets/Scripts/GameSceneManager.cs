using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance { get; private set; }
    public event Action<Scene> OnSceneLoadCompleted;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene " + scene.name + " has loaded!");

        if (SceneManager.GetActiveScene().buildIndex == 1)  // Assuming GameManager is in scene 1
        {
            if (GameManager.Instance != null) // Check if GameManager exists
            {
                OnSceneLoadCompleted?.Invoke(scene);
            }
        }
    }


    private void Start()
    {
        PlayerDataManager.Instance.OnSuccessfulPlayerDataHandling += Instance_OnSuccessfulPlayerDataHandling;
    }

    private void Instance_OnSuccessfulPlayerDataHandling()
    {
        SceneManager.LoadScene(1); 
        PlayerDataManager.Instance.OnSuccessfulPlayerDataHandling -= Instance_OnSuccessfulPlayerDataHandling;
    }
}
