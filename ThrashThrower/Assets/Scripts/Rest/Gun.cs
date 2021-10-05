using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public abstract class Gun : MonoBehaviour
{
    [SerializeField] protected Transform firePoint;
    protected int currentAmmo = 0;
    protected Transform gunTransform;
    protected IEnumerator shootingCoroutine;
    protected IEnumerator reloadingCoroutine;
    protected GunState currentGunState;
    public enum GunState
    {
        Idle,
        Firing,
        Reloading
    }
    protected virtual void Awake()
    {
        gunTransform = transform;
        shootingCoroutine = ShootingCoroutine();
        reloadingCoroutine = ReloadingCoroutine();
        currentGunState = GunState.Idle;

        if (firePoint == null)
        {
            firePoint = gunTransform.GetComponentInChildren<Transform>();
            if (firePoint == null)
            {
                Debug.LogError($"no fire point assigned for weapon {name} ");
            }
        }
    }
    protected abstract IEnumerator ReloadingCoroutine();
    protected abstract IEnumerator ShootingCoroutine();
    public void StartShooting()
    {
        if (currentGunState == GunState.Reloading)
        {
            Debug.Log("Can't shoot now, reloading");
            return;
        }

        StartCoroutine(shootingCoroutine);
    }
    public void StopShooting()
    {
        StopCoroutine(shootingCoroutine);
        currentGunState = GunState.Idle;
    }
    public abstract void FireOneTime();
    public abstract void ReloadGun();
}
