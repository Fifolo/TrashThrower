using UnityEngine;

public class PistolBullet : Bullet
{
    protected override void ReturnToPool()
    {
        if (ObjectPool<PistolBullet>.Instance != null) ObjectPool<PistolBullet>.Instance.ReturnObject(this);

        else
        {
            Debug.Log("ObjectPool<PistolBullet>.Instance is null");
            Destroy(gameObject);
        }
    }
}
