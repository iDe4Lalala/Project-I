using UnityEngine;

public class ViewRifleAnimationManager : MonoBehaviour
{
    [SerializeField] private PlayerManager _playerManager;
    [SerializeField] private Animator _reloadAnimator;

    public void PlayReloadAnimation()
    {
        _reloadAnimator.SetTrigger("Reload");
    }

    public void FinishedReload()
    {
        _playerManager.FinishedReload();
    }
}
