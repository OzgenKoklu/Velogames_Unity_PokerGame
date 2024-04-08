using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    public static TimerManager Instance { get; private set; }
    public bool IsRunning { get; private set; }
    public float TimeLeft { get => _timeLeft; }
    private float _timeLeft;

    [SerializeField] private float _startTime = 30f;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (IsRunning)
        {
            _timeLeft -= Time.deltaTime;
            if (_timeLeft <= 0)
            {
                StopTimer();
            }
        }
    }

    public void StartTimer()
    {
        _timeLeft = _startTime;
        IsRunning = true;
    }

    public void StopTimer()
    {
        IsRunning = false;
    }
}
