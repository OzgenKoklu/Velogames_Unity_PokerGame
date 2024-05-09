using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenCloseButton : MonoBehaviour
{
    private enum ButtonActionType
    {
        Open,
        Close,
        OpenClose,
        Back,
        Quit,
        Pause
    }

    [SerializeField] private ButtonActionType _buttonAction;
    [SerializeField] private GameObject _buttonParentPanel;
    [SerializeField] private GameObject _openClosePanel;
    [SerializeField] private GameObject _backPanel;

    public void ButtonAction()
    {
        switch (_buttonAction)
        {
            case ButtonActionType.Open:
                _openClosePanel.gameObject.SetActive(true);
                break;
            case ButtonActionType.Close:
                _openClosePanel.gameObject.SetActive(false);
                break;
            case ButtonActionType.OpenClose:
                _openClosePanel.SetActive(!_openClosePanel.activeSelf);
                break;
            case ButtonActionType.Back:
                _backPanel.SetActive(true);
                break;
            case ButtonActionType.Quit:
                break;
            case ButtonActionType.Pause:
                break;
            default:
                break;
        }

        _buttonParentPanel.SetActive(false);
    }
}
