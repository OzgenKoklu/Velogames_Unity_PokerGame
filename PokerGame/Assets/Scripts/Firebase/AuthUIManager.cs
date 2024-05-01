using System;
using TMPro;
using UnityEngine;

public class AuthUIManager : MonoBehaviour
{
    public TMP_Text registerResultMessage;
    public TMP_Text loginResultMessage;

    private void Start()
    {
        FirebaseAuthManager.Instance.OnRegisterResultMessageChanged += FirebaseAuthManager_OnRegisterResultMessageChanged;
        FirebaseAuthManager.Instance.OnLoginResultMessageChanged += FirebaseAuthManager_OnLoginResultMessageChanged;
    }

    private void FirebaseAuthManager_OnLoginResultMessageChanged(string message)
    {
        loginResultMessage.text = message;
    }

    private void FirebaseAuthManager_OnRegisterResultMessageChanged(string message)
    {
        registerResultMessage.text = message;
    }
}
