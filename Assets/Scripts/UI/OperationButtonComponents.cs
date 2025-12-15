using UnityEngine;
using UnityEngine.UI;

public class OperationButtonComponents : MonoBehaviour
{
    [field: SerializeField] public FixedJoystick FixedJoystick { get; private set; }      // ジョイスティック(プレイヤー移動用)
    [field: SerializeField] public FloatingJoystick FloatingJoystick { get; private set; }    // ジョイスティック(プレイヤー視点操作用)
    [field: SerializeField] public Button JumpButton { get; private set; }    // ジャンプボタン
    [field: SerializeField] public Button ShootingButton { get; private set; }    // 射撃ボタン
}
