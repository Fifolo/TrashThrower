using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(AudioSource))]
public abstract class Gun : MonoBehaviour
{
    [SerializeField] protected Transform firePoint;

    #region Instance Variables
    public Vector3 FirePointPosition { get { return firePoint.position; } }
    public abstract int MaxAmmo { get; }
    public abstract float ReloadTime { get; }
    public abstract float ShootingRate { get; }
    public int AmmoLeft { get { return currentAmmo; } }

    protected int currentAmmo = 0;
    protected float lastBulletSpawnTime = 0;

    protected Transform gunTransform;
    protected AudioSource audioSource;
    #endregion

    protected virtual void Awake()
    {
        gunTransform = transform;
        audioSource = GetComponent<AudioSource>();

        if (firePoint == null)
        {
            firePoint = gunTransform.GetComponentInChildren<Transform>();
            if (firePoint == null)
            {
                Debug.LogError($"no fire point assigned for weapon {name} ");
            }
        }

        SetGutCurrenAmmoToMax();
    }
    public abstract bool Shoot();
    public abstract bool ShootAtTarget(Transform target, float randomOffset);
    public virtual void SetGutCurrenAmmoToMax() => currentAmmo = MaxAmmo;
    public abstract void PlayAnimationSound();
}
