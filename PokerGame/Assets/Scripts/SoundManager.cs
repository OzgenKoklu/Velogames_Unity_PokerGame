using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip _cardDealingSound;

    private void Start()
    {
        PokerDeckManager.Instance.OnAnyCardDealt += PokerDeckManager_OnAnyCardDealt;
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
