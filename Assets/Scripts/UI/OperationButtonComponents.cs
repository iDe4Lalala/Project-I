using UnityEngine;
using UnityEngine.UI;

public class OperationButtonComponents : MonoBehaviour
{
    [field: SerializeField] public FixedJoystick FixedJoystick { get; private set; }
    [field: SerializeField] public FloatingJoystick FloatingJoystick { get; private set; }
    [field: SerializeField] public Button JumpButton { get; private set; }
    [field: SerializeField] public Button ShootingButton { get; private set; }
}
