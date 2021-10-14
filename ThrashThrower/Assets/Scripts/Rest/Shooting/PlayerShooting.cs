using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerShooting : CharacterShooting
{
    [SerializeField] protected Transform fpsCam;

    #region Events
    public delegate void PlayerWeaponAction(Gun gun);
    public static event PlayerWeaponAction OnWeaponSwap;
    public static event PlayerWeaponAction OnPlayerReloadComplete;
    public static event PlayerWeaponAction OnPlayerShot;
    #endregion

    #region Instance Variables
    private IEnumerator shootingCoroutine;
    private IEnumerator reloadingCoroutine;
    private WaitForSeconds ReloadSeconds;

    private PlayerData playerData;
    #endregion

    #region MonoBehaviour
    private void Awake()
    {
        reloadingCoroutine = ReloadingCoroutine();
        shootingCoroutine = ShootingCoroutine();
        if (TryGetComponent(out PlayerMovement playerMovement))
        {
            playerData = playerMovement.PlayerData;
            InitializeGuns(playerData.guns, playerData.guns.Length);
        }
        else Debug.Log($"{name} => cant set playerData, no playermovement attached");
        OnWeaponSwap += PlayerShooting_OnWeaponSwap;
    }
    #endregion

    #region Methods

    private void PlayerShooting_OnWeaponSwap(Gun gun)
    {
        if (this != null) StopAllCoroutines();
    }

    public override void ReloadGun()
    {
        if (currentGun.AmmoLeft < currentGun.MaxAmmo)
        {
            reloadingCoroutine = ReloadingCoroutine();
            StartCoroutine(reloadingCoroutine);
        }
    }
    private IEnumerator ReloadingCoroutine()
    {
        currentGun.PlayAnimationSound();
        shootingState = ShootingState.Reloading;
        yield return ReloadSeconds;
        currentGun.SetGutCurrenAmmoToMax();
        OnPlayerReloadComplete?.Invoke(currentGun);
        shootingState = ShootingState.Idle;
    }
    public void StartShooting()
    {
        if (shootingState == ShootingState.Idle)
        {
            shootingCoroutine = ShootingCoroutine();
            shootingState = ShootingState.Firing;
            StartCoroutine(shootingCoroutine);
        }
    }
    private IEnumerator ShootingCoroutine()
    {
        while (currentGun.AmmoLeft > 0)
        {
            if (currentGun.Shoot()) OnPlayerShot?.Invoke(currentGun);
            yield return null;
        }
    }

    public void StopShooting()
    {
        if (shootingState == ShootingState.Firing)
        {
            StopCoroutine(shootingCoroutine);
            shootingState = ShootingState.Idle;
        }
    }
    public override void ShootOneTime()
    {
        currentGun.Shoot();
        OnPlayerShot?.Invoke(currentGun);
    }
    protected override void SetGunAsActive(int newGunIndex)
    {
        if (shootingState == ShootingState.Reloading) return;

        base.SetGunAsActive(newGunIndex);
        OnWeaponSwap?.Invoke(currentGun);
        ReloadSeconds = new WaitForSeconds(currentGun.ReloadTime * playerData.reloadTimeMultiplier);
    }
    #endregion
}
