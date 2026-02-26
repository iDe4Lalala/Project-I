using UnityEngine;

public class PlayerAnimationContoller : MonoBehaviour
{
    private PlayerComponents _playerComponents;
    
    public void Initialize(PlayerComponents playerComponents)
    {
        _playerComponents = playerComponents;
    }

    public void SetMove(bool isMoving)
    {
        _playerComponents.Animator.SetBool("IsMoving", isMoving);
    }

    public void SetJump()
    {
        _playerComponents.Animator.SetTrigger("Jump");

        if (!_playerComponents.Animator.GetBool("IsGround")) return;
        _playerComponents.Animator.SetBool("IsGround", false);
    }

    public void SetIsGround(bool isGround)
    {
        if (_playerComponents.Animator.GetBool("IsGround") == isGround) return;
        _playerComponents.Animator.SetBool("IsGround", isGround);
    }
}
