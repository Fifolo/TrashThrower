using UnityEngine;
public class MachineGunBullet : Bullet
{
    protected override void ReturnToPool()
    {
        if (ObjectPool<MachineGunBullet>.Instance != null) ObjectPool<MachineGunBullet>.Instance.ReturnObject(this);

        else
        {
            Debug.Log("ObjectPool<MachineGunBullet>.Instance is null");
            Destroy(gameObject);
        }
    }
}
