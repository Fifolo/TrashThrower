using UnityEngine;

public class PlayerShooting : CharacterShooting
{
    [SerializeField] protected Transform fpsCam;
    private void Update()
    {
        UpdateGunRotation();
    }

    private void UpdateGunRotation()
    {
        if (Physics.Raycast(fpsCam.position, fpsCam.forward, out RaycastHit hit))
        {
            Vector3 direction = (hit.point - currentGun.transform.position).normalized;
            weaponHolder.rotation = Quaternion.LookRotation(direction);
        }
    }
}
