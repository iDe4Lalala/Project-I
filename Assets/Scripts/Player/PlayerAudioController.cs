using UnityEngine;

public class PlayerAudioController : MonoBehaviour
{
    [SerializeField] private float _footSoundThreshold;
    private PlayerComponents _playerComponents;
    
    public void Initialize(PlayerComponents playerComponents)
    {
        _playerComponents = playerComponents;
    }

    public void UpdateFootstep(bool isGround, float moveMagnitude)
    {
        if (!isGround || moveMagnitude <= _footSoundThreshold)
        {
            _playerComponents.FootstepAudioSource.Stop();
            return;
        }

        if (_playerComponents.FootstepAudioSource.isPlaying) return;
        _playerComponents.FootstepAudioSource.PlayOneShot(_playerComponents.FootstepAudioClip);
    }
}
