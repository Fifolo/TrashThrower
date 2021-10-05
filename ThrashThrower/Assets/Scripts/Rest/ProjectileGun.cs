using System.Collections;
using UnityEngine;

public class ProjectileGun : Gun
{
    [SerializeField] private ProjectileGunData projectileGunData;

    private float lastBulletSpawnTime = 0;
    protected override void Awake()
    {
        base.Awake();
        currentAmmo = projectileGunData.maxAmmo;
    }
    public override void FireOneTime()
    {
        if (currentGunState == GunState.Reloading) return;
        else if (currentAmmo > 0 && Time.time > (lastBulletSpawnTime + projectileGunData.shootingRate)) SpawnBullet();
    }
    protected override IEnumerator ReloadingCoroutine()
    {
        Debug.Log($"Starting to reload, current ammo = {currentAmmo}");

        currentGunState = GunState.Reloading;
        yield return new WaitForSeconds(projectileGunData.reloadTime);
        currentAmmo = projectileGunData.maxAmmo;
        currentGunState = GunState.Idle;

        Debug.Log($"Finished reloading, current ammo = {currentAmmo}");
    }

    protected virtual void SpawnBullet()
    {
        Bullet bullet = Instantiate(projectileGunData.projectilePrefab, firePoint.position, Quaternion.identity).GetComponent<Bullet>();
        bullet.Damage = projectileGunData.damage;
        bullet.SetBulletMovement(firePoint.forward, projectileGunData.projectileSpeed);

        lastBulletSpawnTime = Time.time;
        currentAmmo--;
    }

    protected override IEnumerator ShootingCoroutine()
    {
        if (currentAmmo > 0)
        {
            while (Time.time < (lastBulletSpawnTime + projectileGunData.shootingRate)) yield return null;
            currentGunState = GunState.Firing;
            while (currentAmmo > 0)
            {
                SpawnBullet();
                yield return new WaitForSeconds(projectileGunData.shootingRate);
            }
        }
    }
    private void OnDisable()
    {
        StopAllCoroutines();
        currentGunState = GunState.Idle;
    }

    public override void ReloadGun()
    {
        if(currentAmmo < projectileGunData.maxAmmo) StartCoroutine(reloadingCoroutine);
    }
}
