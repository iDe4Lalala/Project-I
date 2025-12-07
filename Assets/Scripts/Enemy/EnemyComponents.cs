using UnityEngine;
using UnityEngine.AI;

public class EnemyComponents : MonoBehaviour
{
    /// <summary>
    ///  敵のコンポーネント
    /// </summary>
    
    [field: SerializeField] public EnemyManager EnemyManager { get; private set; }
    [field: SerializeField] public CapsuleCollider CapsuleCollider { get; private set; }
    [field: SerializeField] public Animator Animator { get; private set; }
    [field: SerializeField] public NavMeshAgent NavMeshAgent { get; private set; }
    [field: SerializeField] public GameObject EnemyHand { get; private set; }
    [field: SerializeField] public Camera Camera { get; private set; }
    [field: SerializeField] public HumanDataBase HumanDataBase { get; private set; }
    public RifleManager RifleManager { get; private set; }

    public void SetRifleManager(GameObject rifle)
    {
        RifleManager = rifle.GetComponent<RifleManager>();
        RifleManager.GetOwnerInfo(HumanType.Enemy, gameObject);
    }
}
