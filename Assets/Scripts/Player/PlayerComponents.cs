using UnityEngine;

public class PlayerComponents : MonoBehaviour
{
    [field: SerializeField] public PlayerManager PlayerManager { get; private set; }
    [field: SerializeField] public CapsuleCollider CapsuleCollider { get; private set; }
    [field: SerializeField] public Rigidbody Rigidbody { get; private set; }
    [field: SerializeField] public AspectRatioManager AspectRatioManager { get; private set; }
    [field: SerializeField] public Animator Animator { get; private set; }
    [field: SerializeField] public HumanDataBase HumanDataBase { get; private set; }
    [field: SerializeField] public GameObject RifleSocket { get; private set; }
    [field: SerializeField] public Camera Camera { get; private set; }
    [field: SerializeField] public AudioSource FootstepAudioSource { get; private set; }
    [field: SerializeField] public AudioClip FootstepAudioClip { get; private set; }
    public RifleManager RifleManager { get; private set; }

    public void SetRifleManager(GameObject rifle)
    {
        RifleManager = rifle.GetComponent<RifleManager>();
        RifleManager.GetOwnerInfo(HumanType.Player, gameObject);
    }
}
