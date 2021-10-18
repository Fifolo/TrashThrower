using UnityEngine;

public abstract class GunData : ScriptableObject
{
    [Range(1f, 100f)]
    public int maxAmmo = 30;
    [Range(0.1f, 2f)]
    public float shootingRate = 0.5f;
    [Min(1f)]
    public float damage = 10f;
    [Range(1f, 5f)]
    public float reloadTime = 1f;
    [Range(0f,1f)]
    public float critChance = 0.3f;
    [Range(1f,10f)]
    public float critMultiplier = 1.5f;
    public AudioClip fireClip;
    public AudioClip reloadingClip;
    public GameObject muzzleFlash;
    public GameObject steam;
}
