using UnityEngine;
using UnityEngine.Rendering;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip _cardDealingSound;
    [SerializeField] private AudioClip _smallAmountOfChipsClip;
    [SerializeField] private AudioClip _mediumAmountOfChipsClip;
    [SerializeField] private AudioClip _bigAmountOfChipsClip;
    [SerializeField] private AudioClip _foldSoundClip;
    [SerializeField] private AudioClip _checkSoundClip;

    private void Start()
    {
        PokerDeckManager.Instance.OnAnyCardDealt += PokerDeckManager_OnAnyCardDealt;
        PlayerManager.OnPlayersPokerMove += PlayerManager_OnPlayersPokerMove;
    }

    private void PlayerManager_OnPlayersPokerMove(PlayerAction playerAction, int betAmount)
    {
        if (playerAction == PlayerAction.Fold)
        {
            PlaySound(_foldSoundClip);
        }
        else if (playerAction == PlayerAction.Check)
        {
            PlaySound(_checkSoundClip);
        }
        else // Bet Raise Or Call -> Chips will flow
        {
            if (betAmount > 500)
            {
                PlaySound(_bigAmountOfChipsClip);
            }
            else if (betAmount > 200)
            {
                PlaySound(_mediumAmountOfChipsClip);
            }
            else
            {
                PlaySound(_smallAmountOfChipsClip);
            }
        }
    }

    private void PokerDeckManager_OnAnyCardDealt()
    {
        PlaySound(_cardDealingSound);
    }

    private void PlaySound(AudioClip audioclip, float volume = 1f)
    {
        AudioSource.PlayClipAtPoint(audioclip, Vector3.zero, volume);
    }
}
