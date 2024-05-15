using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Analytics;
using Firebase.Auth;
using Firebase.Extensions;
using System;

public class FirebaseAnalyticsManager : MonoBehaviour
{
    public static FirebaseAnalyticsManager Instance { get; private set; }

    public FirebaseAnalytics Analytics;
    public DependencyStatus DependencyStatus;
    private bool _isInitialized = false;
    private DateTime _appOpenTime;

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this.gameObject); // Persist this object across scenes
    }

    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            DependencyStatus = task.Result;
            if (DependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + DependencyStatus);
            }
        });
    }

    private void InitializeFirebase()
    {
        try
        {
            FirebaseApp app = FirebaseApp.DefaultInstance;

            // Set the user's sign up method property
            FirebaseAnalytics.SetUserProperty("sign_up_method", "Unity");

            // Log an app open event
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventAppOpen);

            // Set user ID
            if (FirebaseAuth.DefaultInstance.CurrentUser != null)
            {
                FirebaseAnalytics.SetUserId(FirebaseAuth.DefaultInstance.CurrentUser.UserId);
            }
            Debug.Log("Initialization of the firebase analytics complete.");
            _isInitialized = true;
            LogAppOpenEvent();
        }
        catch (Exception e)
        {
            Debug.LogError("Firebase initialization failed: " + e.Message);
        }
    }

    // Example method to log a custom event
    public void LogCustomEvent(string eventName, Dictionary<string, object> parameters = null)
    {
        try
        {
            if (!_isInitialized)
            {
                Debug.LogError("Firebase Analytics is not initialized yet.");
                return;
            }

            if (parameters == null)
            {
                FirebaseAnalytics.LogEvent(eventName);
            }
            else
            {
                // Convert dictionary parameters to Firebase Analytics parameters
                Firebase.Analytics.Parameter[] eventParameters = new Firebase.Analytics.Parameter[parameters.Count];
                int i = 0;
                foreach (var parameter in parameters)
                {
                    eventParameters[i] = new Firebase.Analytics.Parameter(parameter.Key, parameter.Value.ToString());
                    i++;
                }

                // Log the event with parameters
                FirebaseAnalytics.LogEvent(eventName, eventParameters);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to log custom event '" + eventName + "': " + e.Message);
        }
    }

    // Example method to set user properties
    public void SetUserProperties(Dictionary<string, object> properties)
    {
        try
        {
            if (!_isInitialized)
            {
                Debug.LogError("Firebase Analytics is not initialized yet.");
                return;
            }

            foreach (var property in properties)
            {
                FirebaseAnalytics.SetUserProperty(property.Key, property.Value.ToString());
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to set user properties: " + e.Message);
        }
    }

    void OnApplicationQuit()
    {
        // Calculate session duration and log the event when the application quits
        TimeSpan sessionDuration = DateTime.Now - _appOpenTime;
        LogSessionDurationEvent(sessionDuration.TotalMinutes);
    }

    public void TestLogSessionDuration()
    {
        Debug.Log("Testing Analytics - Logout Duration");
        TimeSpan sessionDuration = DateTime.Now - _appOpenTime;
        LogSessionDurationEvent(sessionDuration.TotalMinutes);
    }

    //aditional methods
    private void LogAppOpenEvent()
    {
        FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventAppOpen);
        _appOpenTime = DateTime.Now;
    }
    private void LogSessionDurationEvent(double durationInMinutes)
    {
        // Log the session duration as a custom event
        Firebase.Analytics.Parameter[] eventParameters = new Firebase.Analytics.Parameter[1];
        eventParameters[0] = new Firebase.Analytics.Parameter("session_duration", durationInMinutes);
        FirebaseAnalytics.LogEvent("session_duration", eventParameters);
    }
}