using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using TMPro;
using System;
using System.Collections;

public class FirebaseAuthManager : MonoBehaviour
{
    public static FirebaseAuthManager Instance;

    public event Action<string> OnRegisterResultMessageChanged;
    public event Action<string> OnLoginResultMessageChanged;
    public event Action<string> OnLogoutResultMessageChanged;
    public event Action<bool> OnAuthStateChanged;

    public event Action OnLoginSuccessful;

    [SerializeField] private DependencyStatus DependencyStatus;
    [SerializeField] private FirebaseAuth _auth;
    [SerializeField] private FirebaseUser _user;

    [SerializeField] private TMP_InputField _registerEmail;
    [SerializeField] private TMP_InputField _registerPassword;

    [SerializeField] private TMP_InputField _loginEmail;
    [SerializeField] private TMP_InputField _loginPassword;

    private void Awake()
    {
        Instance = this;

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
        Debug.Log("Setting up Firebase Auth");
        _auth = FirebaseAuth.DefaultInstance;

        _auth.StateChanged += AuthStateChanged;

        AuthStateChanged(this, null);
    }

    private void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (_auth.CurrentUser != _user)
        {
            bool signedIn = _user != _auth.CurrentUser && _auth.CurrentUser != null;
            if (!signedIn && _user != null)
            {
             //   Debug.Log("Signed out " + _user.UserId);
            }
            _user = _auth.CurrentUser;
            if (signedIn)
            {
             //   Debug.Log("Signed in " + _user.UserId);
            }

            OnAuthStateChanged?.Invoke(signedIn);
        }
    }

    public void RegisterButton()
    {
        StartCoroutine(RegisterCoroutine());
    }

    private IEnumerator RegisterCoroutine()
    {
        string message = "";

        var authTask = _auth.CreateUserWithEmailAndPasswordAsync(_registerEmail.text, _registerPassword.text);
        yield return new WaitUntil(() => authTask.IsCompleted);

        if (authTask.IsCanceled)
        {
            Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
            yield break;
        }
        if (authTask.IsFaulted)
        {
            Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + authTask.Exception);

            FirebaseException firebaseException = authTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseException.ErrorCode;

            message = "Register Failed!";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WeakPassword:
                    message = "Weak Password";
                    break;
                case AuthError.EmailAlreadyInUse:
                    message = "Email Already In Use";
                    break;
            }
        }
        else
        {
            // Firebase user has been created.
            Firebase.Auth.AuthResult result = authTask.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                result.User.DisplayName, result.User.UserId);
            message = "User created successfully.";

            OnSignUpSuccess("Email"); //apperantly the args can be Email, Facebook, Google etc
        }

        OnRegisterResultMessageChanged?.Invoke(message);
    }

    // After successfully signing up a user
    void OnSignUpSuccess(string signUpMethod)
    {
        try
        {
            // Log the sign-up event with Firebase Analytics
            Firebase.Analytics.FirebaseAnalytics.LogEvent(
                Firebase.Analytics.FirebaseAnalytics.EventSignUp,
                new Firebase.Analytics.Parameter[] {
            new Firebase.Analytics.Parameter(
                Firebase.Analytics.FirebaseAnalytics.ParameterMethod, signUpMethod)
                }
            );
            Debug.Log("On Sign Up Success, analytics triggered");
        }
        catch (Exception e)
        {
            Debug.LogError("Error: " + e.Message);
        }
    }

    // After successfully logging in a user
    public void OnLoginSuccessfulFireBaseAnalytics(string signInMethod)
    {
        try
        {
            Firebase.Analytics.FirebaseAnalytics.LogEvent(
         Firebase.Analytics.FirebaseAnalytics.EventLogin,
                new Firebase.Analytics.Parameter[] {
                    new Firebase.Analytics.Parameter(
                 Firebase.Analytics.FirebaseAnalytics.ParameterMethod, signInMethod),
        }
        );
        }
        catch (Exception e)
        {
            Debug.LogError("Error: " + e.Message);
        }
    }

    public void LoginButton()
    {
        StartCoroutine(LoginCoroutine());
    }

    private IEnumerator LoginCoroutine()
    {
        string message = "";

        var authTask = _auth.SignInWithEmailAndPasswordAsync(_loginEmail.text, _loginPassword.text);
        yield return new WaitUntil(() => authTask.IsCompleted);

        if (authTask.IsCanceled)
        {
            Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
            message = "Login canceled.";
            OnLoginResultMessageChanged?.Invoke(message);
            yield break;
        }
        if (authTask.IsFaulted)
        {
            Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + authTask.Exception);

            FirebaseException firebaseException = authTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseException.ErrorCode;

            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WrongPassword:
                    message = "Wrong Password";
                    break;
                case AuthError.InvalidEmail:
                    message = "Invalid Email";
                    break;
                default:
                    message = "Login Failed";
                    break;
            }

            OnLoginResultMessageChanged?.Invoke(message);
            yield break;
        }

        Firebase.Auth.AuthResult result = authTask.Result;
        Debug.LogFormat("User signed in successfully: {0} ({1})", result.User.DisplayName, result.User.UserId);
        message = "Login successful.";
        OnLoginSuccessfulFireBaseAnalytics("Email");
        OnLoginSuccessful?.Invoke();
        OnLoginResultMessageChanged?.Invoke(message);
    }

    public void LogoutButton()
    {
        _auth.SignOut();
        OnLogoutResultMessageChanged?.Invoke("Logout successful.");
    }

    public void DeleteUserButton()
    {
        if (_user != null)
        {
            _user.DeleteAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("DeleteAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("DeleteAsync encountered an error: " + task.Exception);
                    return;
                }

                Debug.Log("User deleted successfully.");
            });
        }
    }

    public bool GetLoginStatus()
    {
        bool signedIn = _user != _auth.CurrentUser && _auth.CurrentUser != null;
        return signedIn;
    }

    private void OnDestroy()
    {
        _auth.StateChanged -= AuthStateChanged;
        _auth = null;
    }
}