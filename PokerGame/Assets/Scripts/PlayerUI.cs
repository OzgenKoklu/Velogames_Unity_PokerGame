using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private PlayerManager _player;
    [SerializeField] private GameObject _dealerIcon;
    [SerializeField] private GameObject _passivePlayerTint;
    [SerializeField] private TextMeshPro _playerTotalStackText;

    private void Start()
    {
        DealerManager.Instance.OnDealerChanged += OnDealerChanged;
        //TurnManager.Instance.OnPlayerTurn += TurnManager_OnPlayerTurn;
        _player.OnPlayerActiveChanged += Player_OnPlayerActiveChanged;
        //_player.OnPlayerActionChanged += Player_OnPlayerActionChanged;
        _dealerIcon.SetActive(false);
        _passivePlayerTint.SetActive(false);
    }

    //private void Player_OnPlayerActionChanged(PlayerAction playerAction)
    //{
    //    throw new NotImplementedException();
    //}

    private void Player_OnPlayerActiveChanged(bool isPlayerActive)
    {
        _passivePlayerTint.SetActive(!isPlayerActive);
    }

    private void OnDisable()
    {
        DealerManager.Instance.OnDealerChanged -= OnDealerChanged;
        //TurnManager.Instance.OnPlayerTurn -= TurnManager_OnPlayerTurn;
    }

    //private void TurnManager_OnPlayerTurn(PlayerManager player)
    //{
    //    throw new NotImplementedException();
    //}

    private void OnDealerChanged(PlayerManager player)
    {
        if (_player.IsPlayerDealer && player == _player)
        {
            _dealerIcon.SetActive(true);
        }
        else
        {
            _dealerIcon.SetActive(false);
        }
    }
}
