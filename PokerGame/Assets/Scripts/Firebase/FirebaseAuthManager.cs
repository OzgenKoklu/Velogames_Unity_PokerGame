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

    public DependencyStatus DependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser user;

    public TMP_InputField registerEmail;
    public TMP_InputField registerPassword;

    public TMP_InputField loginEmail;
    public TMP_InputField loginPassword;

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
        auth = FirebaseAuth.DefaultInstance;

        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    private void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId);
            }
        }
    }

    public void RegisterButton()
    {
        StartCoroutine(RegisterCoroutine());
    }

    private IEnumerator RegisterCoroutine()
    {
        string message = "";

        var authTask = auth.CreateUserWithEmailAndPasswordAsync(registerEmail.text, registerPassword.text);
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
        }

        OnRegisterResultMessageChanged?.Invoke(message);
    }

    public void LoginButton()
    {
        StartCoroutine(LoginCoroutine());
    }

    private IEnumerator LoginCoroutine()
    {
        string message = "";

        var authTask = auth.SignInWithEmailAndPasswordAsync(loginEmail.text, loginPassword.text);
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
        OnLoginResultMessageChanged?.Invoke(message);
    }

    public void LogoutButton()
    {
        auth.SignOut();
    }

    public void DeleteUserButton()
    {
        if (user != null)
        {
            user.DeleteAsync().ContinueWithOnMainThread(task =>
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

    private void OnDestroy()
    {
        auth.StateChanged -= AuthStateChanged;
        auth = null;
    }
}
