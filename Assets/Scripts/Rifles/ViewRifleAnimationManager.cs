using UnityEngine;

public class ViewRifleAnimationManager
{
    private Animator _reloadAnimator;

    public ViewRifleAnimationManager(Animator reloadAnimator)
    {
        _reloadAnimator = reloadAnimator;
    }

    public void SetReload()
    {
        _reloadAnimator.SetTrigger("Reload");
    }
}