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

    //BettingPanel
    [SerializeField] private GameObject _bettingPanel;
    [SerializeField] private Button _bettingPanelCloseButton;
    [SerializeField] private Button _bettingPanelBetButton;
    [SerializeField] private Slider _bettingPanelSlider;
    [SerializeField] private TextMeshProUGUI _bettingPanelBetAmountText;
    [SerializeField] private int _smallestBetAmount;
    [SerializeField] private int _biggestBetAmount;
    [SerializeField] private int _selectedBetAmount;

    private void Awake()
    {
        Instance = this;
    }

    public void SetActionButtonsForPlayer()
    {
        ShowButtons();
        ShowBettingWindow();

        PlayerManager player = GameManager.Instance.MainPlayer;

        GetBetAmounts(player);
        _bettingPanelBetAmountText.text = $"${_smallestBetAmount:N0}";
        _selectedBetAmount = _smallestBetAmount;

        _foldButton.onClick.AddListener(player.FoldAction);
        _raiseOrBetButton.onClick.AddListener(ShowBettingWindow);
        _bettingPanelCloseButton.onClick.AddListener(HideBettingWindow);
        _bettingPanelBetButton.onClick.AddListener(OnBetButtonClick);
        _bettingPanelSlider.onValueChanged.AddListener(Slider_OnValueChanged);

        //hem burada, hem de SetActionbuttons'ý cagirdigimiz yerde ayný karþýlaþtýrmayý yapmamýz mantiksiz
        //bu yüzden daha iyi bir sekilde halledilmeli, ayni sekilde callbetAmountu da tek yerde hesaplasak daha iyi
        if (player.TotalBetInThisRound < BetManager.Instance.CurrentHighestBetAmount)
        {
            _callOrCheckButton.onClick.AddListener(GameManager.Instance.MainPlayer.CallAction);

            var callBetAmount = BetManager.Instance.CurrentHighestBetAmount - player.BetAmount;
            _callOrCheckButtonText.text = "CALL" + "(" + callBetAmount.ToString() + ")";
            _raiseOrBetButtonText.text = "RAISE";
        }
        else  //player is big blind, biggest bet == current bet 
        {
            _callOrCheckButton.onClick.AddListener(GameManager.Instance.MainPlayer.CheckAction);

            _callOrCheckButtonText.text = "CHECK";
            _raiseOrBetButtonText.text = "BET";
        }

        HideBettingWindow();
    }

    private void OnBetButtonClick()
    {
        PlayerManager player = GameManager.Instance.MainPlayer;
        player.BetAction(_selectedBetAmount);
    }

    private void GetBetAmounts(PlayerManager player)
    {
        _smallestBetAmount = BetManager.Instance.CurrentHighestBetAmount * 2;
        _biggestBetAmount = player.TotalStackAmount;
    }
    private void ShowBettingWindow()
    {
        _bettingPanel.SetActive(true);
    }

    private void HideBettingWindow()
    {
        _bettingPanel.SetActive(false);
    }

    private void Slider_OnValueChanged(float sliderValue)
    {
        Debug.Log("Slider Value: " + sliderValue);

        int betAmount = GetBetAmountValue(sliderValue);

        _bettingPanelBetAmountText.text = $"${betAmount:N0}";
        _selectedBetAmount = betAmount;

        if (sliderValue == 1)
        {
            _bettingPanelBetAmountText.text = $"ALL IN (${betAmount:N0})";
        }

    }

    private int GetBetAmountValue(float normalizedSliderValue)
    {
        // Calculate the range of the bet amounts
        int betRange = _biggestBetAmount - _smallestBetAmount;

        // Calculate the bet amount based on the normalized slider value
        int betAmount = _smallestBetAmount + Mathf.RoundToInt(normalizedSliderValue * betRange);

        return betAmount;
    }

    private void ShowButtons()
    {
        _actionButtons.SetActive(true);
    }

    public void ResetFunctionsAndHideButtons()
    {
        RemoveAllListenersFromButtons();

        _bettingPanelSlider.value = 0f;

        HideButtons();

        HideBettingWindow();
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
        _bettingPanelCloseButton.onClick.RemoveAllListeners();
        _bettingPanelBetButton.onClick.RemoveAllListeners();
        _bettingPanelSlider.onValueChanged.RemoveAllListeners();     
    }

    private void OnDestroy()
    {
        RemoveAllListenersFromButtons();
        Instance = null;
    }
}
