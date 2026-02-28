using UnityEngine;
using UnityEngine.AI;

public class EnemyComponents : MonoBehaviour
{
    [field: SerializeField] public EnemyManager EnemyManager { get; private set; }
    [field: SerializeField] public CapsuleCollider CapsuleCollider { get; private set; }
    [field: SerializeField] public Animator Animator { get; private set; }
    [field: SerializeField] public NavMeshAgent NavMeshAgent { get; private set; }
    [field: SerializeField] public GameObject WeaponSocket { get; private set; }
    [field: SerializeField] public Camera Camera { get; private set; }
    [field: SerializeField] public HumanDataBase HumanDataBase { get; private set; }
    [field: SerializeField] public AudioSource FootstepAudioSource { get; private set; }
    [field: SerializeField] public AudioClip FootstepAudioClip { get; private set; }
    public RifleManager RifleManager { get; private set; }
}
