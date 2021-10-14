using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Npc))]
public class NpcShooting : CharacterShooting
{
    public static LayerMask characterLayerMask = 8;
    public static WaitForSeconds ShortDelay = new WaitForSeconds(0.1f);

    #region Instance Variables
    private Transform npcTransform;
    private Npc npcController;
    private Transform target;

    [SerializeField] private float reloadTimeMultiplier = 2f;
    [SerializeField] private float range = 10f;
    [SerializeField] private Gun gun;
    [Range(0f,3f)]
    [SerializeField] private float shootOffset = 1f;

    private IEnumerator reloadingCoroutine;
    private IEnumerator followPlayerAfterDelayCoroutine;

    private WaitForSeconds ReloadSeconds;
    #endregion

    #region MonoBehaviour
    private void Awake()
    {
        npcTransform = transform;
        npcController = GetComponent<Npc>();
        reloadingCoroutine = ReloadingCoroutine();
        InitializeData();
    }
    private void Start()
    {
        if (PlayerMovement.Instance != null) target = PlayerMovement.Instance.transform;
    }
    private void OnEnable()
    {
        FollowPlayer();
    }
    private void Update()
    {
        if (target == null) return;

        if (shootingState == ShootingState.Idle)
        {
            if (HasAmmo())
            {
                if (TargetInRange(target.position))
                {
                    if (NothingBetweenTarget())
                    {
                        //TurnWeaponHolderToPlayer();
                        ShootOneTime();
                    }
                }
            }
            else ReloadGun();
        }
    }
    #endregion

    #region Methods
    private void TurnWeaponHolderToPlayer()
    {
        Vector3 lookDirection = (target.position - npcTransform.position);
        weaponHolder.rotation = Quaternion.LookRotation(lookDirection);
    }
    private bool NothingBetweenTarget() => !Physics.Linecast(currentGun.FirePointPosition, target.position, characterLayerMask);
    private bool HasAmmo() => currentGun.AmmoLeft > 0;
    public void InitializeData()
    {
        if (weaponHolder == null) Debug.LogWarning($"no weapon holder assigned => {name}");
        if(gun != null) InitializeGuns(gun, 1);
        ReloadSeconds = new WaitForSeconds(currentGun.ReloadTime * reloadTimeMultiplier);
    }
    public void FollowPlayer()
    {
        followPlayerAfterDelayCoroutine = FollowPlayerAfterDelay();
        StartCoroutine(followPlayerAfterDelayCoroutine);
    }
    private IEnumerator FollowPlayerAfterDelay()
    {
        yield return ShortDelay;
        if (target != null) npcController.StartFollowingTarget(target, range);
        else npcController.StartRoaming();
    }

    private bool TargetInRange(Vector3 targetPosition)
    {
        return Vector3.Distance(npcTransform.position, targetPosition) <= range;
    }

    private IEnumerator ReloadingCoroutine()
    {
        yield return ReloadSeconds;
        currentGun.SetGutCurrenAmmoToMax();
        shootingState = ShootingState.Idle;
    }
    public override void ReloadGun()
    {
        shootingState = ShootingState.Reloading;
        reloadingCoroutine = ReloadingCoroutine();
        StartCoroutine(reloadingCoroutine);
    }

    public override void ShootOneTime()
    {
        shootingState = ShootingState.Firing;
        currentGun.ShootAtTarget(target, shootOffset);
        shootingState = ShootingState.Idle;
    }
    #endregion
}
