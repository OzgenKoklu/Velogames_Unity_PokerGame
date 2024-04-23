using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public static UiManager Instance { get; private set; }

    [SerializeField] private GameObject _actionButtons;
    [SerializeField] private Button _foldButton;
    [SerializeField] private Button _callOrCheckButton;
    [SerializeField] private Button _raiseOrBetButton;
    [SerializeField] private TextMeshProUGUI _callOrCheckButtonText;
    [SerializeField] private TextMeshProUGUI _raiseOrBetButtonText;

    private void Awake()
    {
        Instance = this;
    }

    public void SetActionButtonsForPlayer()
    {
        ShowButtons();

        PlayerManager player = GameManager.Instance.MainPlayer;

        //hem burada, hem de SetActionbuttons'ý cagirdigimiz yerde ayný karþýlaþtýrmayý yapmamýz mantiksiz
        //bu yüzden daha iyi bir sekilde halledilmeli, ayni sekilde callbetAmountu da tek yerde hesaplasak daha iyi
        if (player.BetAmount < BetManager.Instance.CurrentHighestBetAmount)
        {
            _foldButton.onClick.AddListener(GameManager.Instance.MainPlayer.FoldAction);
            _callOrCheckButton.onClick.AddListener(GameManager.Instance.MainPlayer.CallAction);
            _raiseOrBetButton.onClick.AddListener(GameManager.Instance.MainPlayer.RaiseAction);
            var callBetAmount = BetManager.Instance.CurrentHighestBetAmount - player.BetAmount;
            _callOrCheckButtonText.text = "CALL" + "(" + callBetAmount.ToString() + ")";
            _raiseOrBetButtonText.text = "RAISE";
        }
        else  //player is big blind, biggest bet == current bet 
        {
            _foldButton.onClick.AddListener(GameManager.Instance.MainPlayer.FoldAction);
            _callOrCheckButton.onClick.AddListener(GameManager.Instance.MainPlayer.CheckAction);
            _raiseOrBetButton.onClick.AddListener(GameManager.Instance.MainPlayer.BetAction);
            _callOrCheckButtonText.text = "CHECK";
            _raiseOrBetButtonText.text = "BET";
        }
    }

    private void ShowButtons()
    {
        _actionButtons.SetActive(true);
    }

    public void ResetFunctionsAndHideButtons()
    {
        RemoveAllListenersFromButtons();
    }

    private void HideButtons()
    {
        _actionButtons.SetActive(false);
    }

    private void RemoveAllListenersFromButtons()
    {
        _foldButton.onClick.RemoveAllListeners();
        _callOrCheckButton.onClick.RemoveAllListeners();
        _raiseOrBetButton.onClick.RemoveAllListeners();
        HideButtons();
    }
}
