using UnityEngine;
using System.Collections;
public abstract class ProjectileGun<T> : Gun where T : Bullet
{
    [SerializeField] private ProjectileGunData projectileGunData;
    public override int MaxAmmo => projectileGunData.maxAmmo;
    public override float ReloadTime => projectileGunData.reloadTime;
    public override float ShootingRate => projectileGunData.shootingRate;

    public override bool Shoot()
    {
        if (CanShot())
        {
            SpawnBullet();
            return true;
        }
        return false;
    }

    private bool CanShot() => currentAmmo > 0 && Time.time > (lastBulletSpawnTime + projectileGunData.shootingRate);
    protected virtual void SpawnBullet()
    {
        Bullet bullet = GetBulletFromPool();
        if (bullet != null)
        {
            bullet.transform.position = firePoint.position;
            bullet.Damage = projectileGunData.damage;
            bullet.gameObject.SetActive(true);
            bullet.SetBulletMovement(firePoint.forward, projectileGunData.projectileSpeed);

            //Instantiate(projectileGunData.steam, firePoint.position, firePoint.rotation);
            audioSource.PlayOneShot(projectileGunData.fireClip);

            lastBulletSpawnTime = Time.time;
            currentAmmo--;
        }
        else
        {
            Debug.Log($"Object pool for {name} not set");
        }
    }
    protected virtual T GetBulletFromPool() => ObjectPool<T>.Instance.GetLastObject();
    public override void PlayAnimationSound() => audioSource.PlayOneShot(projectileGunData.reloadingClip);
    protected virtual void SpawnBulletWithTarget(Transform target, float offset)
    {
        Bullet bullet = GetBulletFromPool();
        if (bullet != null)
        {
            Vector3 targetPosition = target.position;
            targetPosition.x += Random.Range(-offset, offset);
            targetPosition.y += Random.Range(-offset, offset);
            targetPosition.z += Random.Range(-offset, offset);

            Vector3 direction = (targetPosition - firePoint.position).normalized;
            bullet.transform.position = firePoint.position;
            bullet.Damage = projectileGunData.damage;
            bullet.gameObject.SetActive(true);
            bullet.SetBulletMovement(direction, projectileGunData.projectileSpeed);

            //Instantiate(projectileGunData.steam, firePoint.position, firePoint.rotation);
            audioSource.PlayOneShot(projectileGunData.fireClip);

            lastBulletSpawnTime = Time.time;
            currentAmmo--;
        }
        else Debug.Log($"Object pool for {name} not set");
    }
    public override bool ShootAtTarget(Transform target, float randomOffset)
    {
        if (CanShot())
        {
            SpawnBulletWithTarget(target, randomOffset);
            return true;
        }
        return false;
    }
    #region Debugging
    /*
    public void ShootDebug()
    {
        SpawnBullet();
    }
    public void ReloadGunDebug()
    {
        SetGutCurrenAmmoToMax();
        audioSource.PlayOneShot(projectileGunData.reloadingClip);
    }
    IEnumerator shootingCoroutine;
    public void OnMouseDownDebug()
    {
        shootingCoroutine = ShootingCoroutine();
        StartCoroutine(shootingCoroutine);
    }
    public void OnMouseUpDebug()
    {
        StopCoroutine(shootingCoroutine);
    }
    private IEnumerator ShootingCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(projectileGunData.reloadTime);
            SpawnBullet();
        }
    }
    */
    #endregion
}
