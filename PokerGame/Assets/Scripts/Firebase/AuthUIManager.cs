using TMPro;
using UnityEngine;

public class AuthUIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _registerResultMessage;
    [SerializeField] private TMP_Text _loginResultMessage;

    private void Start()
    {
        FirebaseAuthManager.Instance.OnRegisterResultMessageChanged += FirebaseAuthManager_OnRegisterResultMessageChanged;
        FirebaseAuthManager.Instance.OnLoginResultMessageChanged += FirebaseAuthManager_OnLoginResultMessageChanged;
    }

    private void FirebaseAuthManager_OnLoginResultMessageChanged(string message)
    {
        _loginResultMessage.text = message;
    }

    private void FirebaseAuthManager_OnRegisterResultMessageChanged(string message)
    {
        _registerResultMessage.text = message;
    }
}