using UnityEngine;

public abstract class GunData : ScriptableObject
{
    public int maxAmmo = 30;
    public float shootingRate = 0.5f;
    public float damage = 10f;
    public float reloadTime = 1f;
}
