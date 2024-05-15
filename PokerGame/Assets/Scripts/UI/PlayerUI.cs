using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private PlayerManager _player;

    [Header("")]
    [SerializeField] private GameObject _dealerIcon;
    [SerializeField] private GameObject _bigBlindIcon;
    [SerializeField] private GameObject _smallBlindIcon;

    [Header("Player Action Icons")]
    [SerializeField] private GameObject _checkIcon;
    [SerializeField] private GameObject _foldIcon;
    [SerializeField] private GameObject _callIcon;
    [SerializeField] private GameObject _betOrRaiseIcon;

    [Header("Money TMPs")]
    [SerializeField] private TextMeshPro _playerTotalStackText;
    [SerializeField] private TextMeshPro _betRaiseCallMoneyText;

    [Header("")]
    [SerializeField] private GameObject _passivePlayerTint;

    private void OnEnable()
    {
        HideIcons();
        HideBlindAndDealerIcons();
        HideBetMoneyText();
    }

    private void Start()
    {
        DealerManager.Instance.OnDealerChanged += DealerManager_OnDealerChanged;
        DealerManager.Instance.OnSmallBlindChanged += DealerManager_OnSmallBlindChanged;
        DealerManager.Instance.OnBigBlindChanged += DealerManager_OnBigBlindChanged;
        BetManager.Instance.OnBetUpdated += BetManager_OnBetUpdated;
        _player.OnPlayerActiveChanged += Player_OnPlayerActiveChanged;
        _player.OnPlayerActionChanged += Player_OnPlayerActionChanged;
    }

    private void BetManager_OnBetUpdated(PlayerManager player, int betAmount)
    {
        if (_player == player)
        {
            _betRaiseCallMoneyText.gameObject.SetActive(true);
            _betRaiseCallMoneyText.text = betAmount.ToString() + " $";

            if (player.IsPlayerAllIn)
            {
                _betRaiseCallMoneyText.text = betAmount.ToString() + " $ ALL IN ";
            }
        }
        else if (player == null)
        {
            _betRaiseCallMoneyText.gameObject.SetActive(false);
        }
    }

    private void Player_OnPlayerActionChanged(PlayerAction playerAction)
    {
        switch (playerAction)
        {
            case PlayerAction.Fold:
                HideIcons();
                _foldIcon.SetActive(true);
                _betRaiseCallMoneyText.gameObject.SetActive(false);
                break;
            case PlayerAction.Check:
                HideIcons();
                _checkIcon.SetActive(true);
                break;
            case PlayerAction.Bet:
                HideIcons();
                _betOrRaiseIcon.SetActive(true);
                _betRaiseCallMoneyText.gameObject.SetActive(true);
                _betRaiseCallMoneyText.text = _player.BetAmount.ToString() + " $";
                break;
            case PlayerAction.Raise:
                HideIcons();
                _betOrRaiseIcon.SetActive(true);
                _betRaiseCallMoneyText.gameObject.SetActive(true);
                _betRaiseCallMoneyText.text = _player.BetAmount.ToString() + " $";
                break;
            case PlayerAction.Call:
                HideIcons();
                _callIcon.SetActive(true);
                _betRaiseCallMoneyText.gameObject.SetActive(true);
                _betRaiseCallMoneyText.text = _player.BetAmount.ToString() + " $";
                break;
            default:
                break;
        }
    }

    private void Player_OnPlayerActiveChanged(bool isPlayerActive)
    {
        _passivePlayerTint.SetActive(!isPlayerActive);
    }

    private void DealerManager_OnDealerChanged(PlayerManager player)
    {
        if (_player.IsPlayerDealer && _player == player)
        {
            _dealerIcon.SetActive(true);
        }
        else
        {
            _dealerIcon.SetActive(false);
        }
    }

    private void DealerManager_OnSmallBlindChanged(PlayerManager smallBlindPlayer)
    {
        if (_player == smallBlindPlayer)
        {
            _smallBlindIcon.SetActive(true);
        }
        else
        {
            _smallBlindIcon.SetActive(false);
        }
    }
    private void DealerManager_OnBigBlindChanged(PlayerManager bigBlindPlayer)
    {
        if (_player == bigBlindPlayer)
        {
            _bigBlindIcon.SetActive(true);
        }
        else
        {
            _bigBlindIcon.SetActive(false);
        }
    }

    private void HideIcons()
    {
        _checkIcon.SetActive(false);
        _foldIcon.SetActive(false);
        _callIcon.SetActive(false);
        _betOrRaiseIcon.SetActive(false);
        _passivePlayerTint.SetActive(false);
    }

    private void HideBlindAndDealerIcons()
    {
        _dealerIcon.SetActive(false);
        _bigBlindIcon.SetActive(false);
        _smallBlindIcon.SetActive(false);
    }

    private void HideBetMoneyText()
    {
        _betRaiseCallMoneyText.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if (DealerManager.Instance != null)
        {
            DealerManager.Instance.OnDealerChanged -= DealerManager_OnDealerChanged;
            DealerManager.Instance.OnSmallBlindChanged -= DealerManager_OnSmallBlindChanged;
            DealerManager.Instance.OnBigBlindChanged -= DealerManager_OnBigBlindChanged;
        }
        if (BetManager.Instance != null)
        {
            BetManager.Instance.OnBetUpdated -= BetManager_OnBetUpdated;
        }
        _player.OnPlayerActiveChanged -= Player_OnPlayerActiveChanged;
        _player.OnPlayerActionChanged -= Player_OnPlayerActionChanged;
    }
}