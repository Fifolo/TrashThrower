using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public abstract class CharacterShooting : MonoBehaviour
{
    #region Editor Exposed Variables
    [SerializeField] protected Transform weaponHolder;
    #endregion

    #region Instance Variables
    protected Gun[] gunsInInventory;
    protected Gun currentGun;
    public Gun CurrentGun { get { return currentGun; } }
    protected int currentGunIndex;
    private WaitForSeconds ReloadSeconds;

    protected ShootingState shootingState = ShootingState.Idle;
    public enum ShootingState
    {
        Idle,
        Firing,
        Reloading
    }
    #endregion

    #region MonoBehaviour
    protected virtual void OnDisable()
    {
        shootingState = ShootingState.Idle;
        StopAllCoroutines();
    }
    #endregion

    #region Methods
    public virtual void InitializeGuns(Gun[] guns, int maxWeapons)
    {
        gunsInInventory = new Gun[maxWeapons];
        int lenght = maxWeapons;
        if (guns.Length < maxWeapons) lenght = guns.Length;
        GameObject gunToAdd;
        for (int i = 0; i < lenght; i++)
        {
            gunToAdd = Instantiate(guns[i].gameObject, weaponHolder);
            gunToAdd.SetActive(false);
            gunsInInventory[i] = gunToAdd.GetComponent<Gun>();
        }
        SetGunAsActive(0);
    }
    public virtual void InitializeGuns(Gun gun, int maxWeapons)
    {
        InitializeGuns(new Gun[] { gun }, maxWeapons);
    }

    protected virtual void SetGunAsActive(int newGunIndex)
    {
        if (newGunIndex > -1 && (newGunIndex <= gunsInInventory.Length))
        {
            if (newGunIndex == gunsInInventory.Length) newGunIndex = 0;

            if (gunsInInventory[newGunIndex] != null)
            {
                if (currentGun != null) currentGun.gameObject.SetActive(false);

                currentGun = gunsInInventory[newGunIndex];
                currentGun.gameObject.SetActive(true);
                currentGunIndex = newGunIndex;
            }
        }
    }
    public void SwapToNextWeapon() => SetGunAsActive(currentGunIndex + 1);
    public abstract void ShootOneTime();
    public abstract void ReloadGun();
    #endregion
}
