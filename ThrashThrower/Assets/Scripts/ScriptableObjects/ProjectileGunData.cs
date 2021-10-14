using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileGunData", menuName = "GunData/ProjectileGunData")]
public class ProjectileGunData : GunData
{
    [Range(5f, 100f)]
    public float projectileSpeed = 10f;
}
