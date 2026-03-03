using UnityEngine;

public class EnemyAnimationContoller : MonoBehaviour
{
    private EnemyComponents _enemyComponents;

    public void Initialize(EnemyComponents enemyComponents)
    {
        _enemyComponents = enemyComponents;
    }

    public void UpdateAnimatorSpeed(bool isMoving)
    {
        if(_enemyComponents == null) return;
        _enemyComponents.Animator.SetBool("IsMoving", isMoving);
    }
}