using UnityEngine;

public interface IReloadRuntime
{
    public void InitializeAnimator(Animator animator);
    public void UpdateReload(float deltaTime);
}
