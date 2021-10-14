using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastGun : Gun
{
    [SerializeField] private RaycastGunData raycastGunData;
    protected Animator gunAnimator;
    private Transform originTransform;
    private static LayerMask excludeLayers;
    public override int MaxAmmo => raycastGunData.maxAmmo;

    public override float ReloadTime => raycastGunData.reloadTime;

    public override float ShootingRate => raycastGunData.reloadTime;

    protected virtual void SetRaycastOrigin()
    {
        Transform cameraTrasform = transform.GetComponentInParent<Camera>().transform;
        if (cameraTrasform != null) originTransform = cameraTrasform;
    }
    protected override void Awake()
    {
        excludeLayers = LayerMask.GetMask("Ground", "Character");

        base.Awake();
        gunAnimator = GetComponentInChildren<Animator>();
    }
    private void Start()
    {
        //doing this in awake causes error!!
        SetRaycastOrigin();
    }
    public override bool Shoot()
    {
        if (CanShot())
        {
            CastRaycast();
            return true;
        }
        return false;
    }
    private void CastRaycast()
    {
        if (!(originTransform == null))
        {
            if (Physics.Raycast(originTransform.position, originTransform.forward, out RaycastHit hit, raycastGunData.raycastRange, excludeLayers))
            {
                if (hit.transform.TryGetComponent(out IDamagable damagable)) damagable.TakeDamage(raycastGunData.damage);
            }
            Instantiate(raycastGunData.steam, firePoint.position, firePoint.rotation);
            currentAmmo--;
            audioSource.PlayOneShot(raycastGunData.fireClip);
            gunAnimator.SetTrigger("Fire");
            lastBulletSpawnTime = Time.time;
        }
        else Debug.Log("Cant shoot cuz origin transform is null");
    }
    private bool CanShot() => currentAmmo > 0 && (Time.time > (lastBulletSpawnTime + raycastGunData.shootingRate));
    public override void PlayAnimationSound() => audioSource.PlayOneShot(raycastGunData.reloadingClip);

    public override bool ShootAtTarget(Transform target, float randomOffset)
    {
        Debug.Log("not yet implemented");
        return false;
    }
}
