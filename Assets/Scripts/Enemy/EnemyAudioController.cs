using UnityEngine;

public class EnemyAudioController : MonoBehaviour
{
    private EnemyComponents _enemyComponents;

    public void Initialize(EnemyComponents enemyComponents)
    {
        _enemyComponents = enemyComponents;
    }

    public void UpdateFootstep(bool isMoving)
    {
        if (_enemyComponents == null) return;
        if (!isMoving)
        {
            _enemyComponents.FootstepAudioSource.Stop();
            return;
        }

        if (_enemyComponents.FootstepAudioSource.isPlaying) return;
        _enemyComponents.FootstepAudioSource.PlayOneShot(_enemyComponents.FootstepAudioClip);
    }    
}