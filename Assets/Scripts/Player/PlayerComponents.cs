using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerComponents : MonoBehaviour
{
    /// <summary>
    ///  プレイヤーのコンポーネント
    /// </summary>
    [field: SerializeField] public PlayerManager PlayerManager { get; private set; }
    [field: SerializeField] public RifleManager RifleManager { get; private set; }
    [field: SerializeField] public CapsuleCollider CapsuleCollider { get; private set; }
    [field: SerializeField] public Rigidbody Rigidbody { get; private set; }
}
