using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private GameObject _actionButtons;
    [SerializeField] private Button _foldButton;
    [SerializeField] private Button _callOrCheckButton;
    [SerializeField] private Button _raiseOrBetButton;
    [SerializeField] private TextMeshProUGUI _callOrCheckButtonText;
    [SerializeField] private TextMeshProUGUI _raiseOrBetButtonText;

    [Header("Betting Panel")]
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
        // Show buttons and betting window on the UI
        ShowButtons();
        ShowBettingWindow();

        // Get player information
        PlayerManager player = GameManager.Instance.MainPlayer;
        int totalBet = player.TotalBetInThisRound;
        int currentHighestBet = BetManager.Instance.CurrentHighestBetAmount;
        int remainingStack = player.TotalStackAmount - totalBet;

        // Set the smallest bet amount
        GetBetAmounts(player);
        _bettingPanelBetAmountText.text = $"${_smallestBetAmount:N0}";
        _selectedBetAmount = _smallestBetAmount;

        // Set the fold button
        _foldButton.onClick.AddListener(player.FoldAction);

        // Set the call/check button based on current bet
        if (totalBet < currentHighestBet)
        {
            var callBetAmount = currentHighestBet - player.BetAmount;

            if (callBetAmount > player.TotalStackAmount)
            {
                _callOrCheckButtonText.text = "CALL ${" + (player.TotalStackAmount).ToString("N0") + "}";
            }
            else
            {
                _callOrCheckButtonText.text = "CALL ${" + (currentHighestBet - totalBet).ToString("N0") + "}";
            }
            _callOrCheckButton.onClick.AddListener(GameManager.Instance.MainPlayer.CallAction);
        }
        else
        {
            _callOrCheckButtonText.text = "CHECK";
            _callOrCheckButton.onClick.AddListener(GameManager.Instance.MainPlayer.CheckAction);
        }

        // Set the raise/bet button based on remaining stack and current bet
        if (remainingStack <= 0)
        {
            _raiseOrBetButtonText.text = "ALL IN";
            _raiseOrBetButton.onClick.AddListener(PlayerAllIn);
        }
        else
        {
            _raiseOrBetButtonText.text = totalBet < currentHighestBet ? "RAISE" : "BET";
            _raiseOrBetButton.onClick.AddListener(ShowBettingWindow); // Show window on first click
        }

        _bettingPanelBetButton.onClick.AddListener(OnBetButtonClick);
        _bettingPanelSlider.onValueChanged.AddListener(Slider_OnValueChanged);
        _bettingPanelCloseButton.onClick.AddListener(HideBettingWindow);

        // Hide betting window initially
        HideBettingWindow();
    }

    private void PlayerAllIn()
    {
        PlayerManager player = GameManager.Instance.MainPlayer;
        player.BetAction(player.TotalStackAmount);
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