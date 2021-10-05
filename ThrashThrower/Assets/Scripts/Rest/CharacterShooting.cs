using System.Collections.Generic;
using UnityEngine;

public class CharacterShooting : MonoBehaviour
{
    #region Editor Exposed Variables
    [SerializeField] protected Transform weaponHolder;
    [SerializeField] protected int maxWeapons = 2;
    [SerializeField] protected List<GameObject> starterGuns;
    #endregion

    #region Instance Variables
    protected Gun[] gunsInInventory;
    protected Gun currentGun;
    protected int currentGunIndex;
    #endregion

    #region MonoBehaviour
    private void Awake()
    {
        InitializeGuns();
        SetGunAsActive(0);
    }
    #endregion

    #region Methods

    private void InitializeGuns()
    {
        starterGuns.Capacity = maxWeapons;

        gunsInInventory = new Gun[maxWeapons];

        int i = 0;
        GameObject gunToAdd;
        foreach (GameObject gun in starterGuns)
        {
            gunToAdd = Instantiate(gun, weaponHolder);
            gunToAdd.SetActive(false);
            gunsInInventory[i] = gunToAdd.GetComponent<Gun>();
            i++;
        }
    }

    private void SetGunAsActive(int newGunIndex)
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

    private bool HasAnyGuns() => starterGuns.Count > 0;
    private bool CanAddGun() => starterGuns.Count < starterGuns.Capacity;
    public void SwapToNextWeapon() => SetGunAsActive(currentGunIndex + 1);
    public void FireOneTime() => currentGun.FireOneTime();
    public void StartFiring() => currentGun.StartShooting();
    public void StopFiring() => currentGun.StopShooting();
    public void ReloadGun() => currentGun.ReloadGun();
    #endregion
}
