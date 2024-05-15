using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button _loginButton;
    [SerializeField] private Button _logoutButton;
    [SerializeField] private Button _registerButton;
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _profileButton;
    [SerializeField] private TMP_Text _resultMessage;

    private void Start()
    {
        _resultMessage.text = "";

        FirebaseAuthManager.Instance.OnAuthStateChanged += FirebaseAuthManager_OnAuthStateChanged;
        FirebaseAuthManager.Instance.OnLogoutResultMessageChanged += SetResultMessage;
    }

    private void FirebaseAuthManager_OnAuthStateChanged(bool isSignedIn)
    {
        SetUIObjectsVisibilty(isSignedIn);
    }

    private void SetUIObjectsVisibilty(bool isSignedIn)
    {
        _playButton.gameObject.SetActive(isSignedIn);
        _profileButton.gameObject.SetActive(isSignedIn);
        _logoutButton.gameObject.SetActive(isSignedIn);
        _resultMessage.gameObject.SetActive(isSignedIn);

        _loginButton.gameObject.SetActive(!isSignedIn);
        _registerButton.gameObject.SetActive(!isSignedIn);
    }

    public void SetResultMessage(string message)
    {
        _resultMessage.text = message;
    }

    private void OnDestroy()
    {
        FirebaseAuthManager.Instance.OnAuthStateChanged -= FirebaseAuthManager_OnAuthStateChanged;
        FirebaseAuthManager.Instance.OnLogoutResultMessageChanged -= SetResultMessage;
    }
}
