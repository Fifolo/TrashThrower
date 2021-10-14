using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class InGameUI : Singleton<InGameUI>
{
    #region Editor Exposed Fields
    [SerializeField] private TextMeshProUGUI plasticText;
    [SerializeField] private TextMeshProUGUI paperText;
    [SerializeField] private TextMeshProUGUI metalText;
    [SerializeField] private TextMeshProUGUI glassText;
    [SerializeField] private TextMeshProUGUI differentText;
    [SerializeField] private TextMeshProUGUI totalBagWeightText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI currentAmmoText;
    [SerializeField] private TextMeshProUGUI maxAmmoText;
    [SerializeField] private Slider contaminationSlider;
    [SerializeField] private TextMeshProUGUI contaminationNumber;
    #endregion

    #region Instance Variables
    private TrashPicker currentTrashPicker;
    public int Score { get; private set; }
    #endregion

    #region Mono Behaviour
    protected override void Awake()
    {
        base.Awake();
        Score = 0;
    }
    private void OnEnable()
    {
        if (GameManager.Instance.currentState == GameManager.GameState.Playing)
        {
            InitializeUI();
            SubscribeToEventsOnPlay();
            InitializeUI();
        }
    }
    private void OnDisable()
    {
        UnsubscribeToEventsOnPlay();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        UnsubscribeToEventsOnPlay();
    }
    #endregion

    #region Methods

    private void InitializeUI()
    {
        Gun currentPlayerGun = FindObjectOfType<PlayerShooting>().CurrentGun;
        //Gun currentPlayerGun = GameObject.Find("Player 1").GetComponentInChildren<PlayerShooting>().CurrentGun;
        SetAmountText(currentAmmoText, currentPlayerGun.AmmoLeft);
        SetAmountText(maxAmmoText, currentPlayerGun.MaxAmmo);
    }
    private void SubscribeToEventsOnPlay()
    {
        InitializeTrashPicker(GameObject.Find("Player 1").GetComponentInChildren<TrashPicker>());

        Npc.OnNpcDeath += Npc_OnNpcDeath;
        PlayerShooting.OnWeaponSwap += PlayerShooting_OnWeaponSwap;
        PlayerShooting.OnPlayerReloadComplete += PlayerShooting_OnPlayerReloadComplete;
        PlayerShooting.OnPlayerShot += PlayerShooting_OnGunFire;
        Trash.OnCurrentContaminationChange += Trash_OnCurrentContaminationChange;
    }
    private void InitializeTrashPicker(TrashPicker newTrashPicker)
    {
        UnsubscribeToCurrentTrashPicker();

        currentTrashPicker = newTrashPicker;
        currentTrashPicker.OnThrashPickUp += TrashPicker_OnThrashPickupAndRemove;
        currentTrashPicker.OnThrashRemove += TrashPicker_OnThrashPickupAndRemove;
        SetBagWeight(currentTrashPicker.CurrentBagWeight);
        SetTrashTypesAmountInBag();
    }
    private void UnsubscribeToEventsOnPlay()
    {
        UnsubscribeToCurrentTrashPicker();

        Npc.OnNpcDeath -= Npc_OnNpcDeath;
        PlayerShooting.OnWeaponSwap -= PlayerShooting_OnWeaponSwap;
        PlayerShooting.OnPlayerReloadComplete -= PlayerShooting_OnPlayerReloadComplete;
        PlayerShooting.OnPlayerShot -= PlayerShooting_OnGunFire;
        Trash.OnCurrentContaminationChange -= Trash_OnCurrentContaminationChange;
    }

    private void UnsubscribeToCurrentTrashPicker()
    {
        if (currentTrashPicker != null)
        {
            currentTrashPicker.OnThrashPickUp -= TrashPicker_OnThrashPickupAndRemove;
            currentTrashPicker.OnThrashRemove -= TrashPicker_OnThrashPickupAndRemove;
        }
    }
    private void Trash_OnCurrentContaminationChange(float contamination)
    {
        if (contamination > 0)
        {
            float contaminationPercent = (contamination / Trash.MaxContamination) * 100;
            contaminationSlider.value = contaminationPercent;
            contaminationNumber.text = $"{(int)contaminationPercent}%";
        }
    }

    private void PlayerShooting_OnPlayerReloadComplete(Gun gun) => SetAmountText(currentAmmoText, gun.AmmoLeft);
    private void PlayerShooting_OnGunFire(Gun gun) => SetAmountText(currentAmmoText, gun.AmmoLeft);
    private void PlayerShooting_OnWeaponSwap(Gun gun)
    {
        SetAmountText(currentAmmoText, gun.AmmoLeft);
        SetAmountText(maxAmmoText, gun.MaxAmmo);
    }
    private void Npc_OnNpcDeath(int pointsOnDeath)
    {
        Score += pointsOnDeath;
        SetAmountText(scoreText, Score);
    }

    private void SetTrashTypesAmountInBag()
    {
        SetAmountText(differentText, currentTrashPicker.GetAmountFromBag(Trash.TrashType.Different));
        SetAmountText(plasticText, currentTrashPicker.GetAmountFromBag(Trash.TrashType.Plastic));
        SetAmountText(paperText, currentTrashPicker.GetAmountFromBag(Trash.TrashType.Paper));
        SetAmountText(metalText, currentTrashPicker.GetAmountFromBag(Trash.TrashType.Metal));
        SetAmountText(glassText, currentTrashPicker.GetAmountFromBag(Trash.TrashType.Glass));
    }

    private void SetAmountText(TextMeshProUGUI textField, int amount) => textField.text = amount.ToString();

    private void TrashPicker_OnThrashPickupAndRemove(Trash.TrashType trashType)
    {
        switch (trashType)
        {
            case Trash.TrashType.Different:
                SetAmountText(differentText, currentTrashPicker.GetAmountFromBag(trashType));
                break;
            case Trash.TrashType.Plastic:
                SetAmountText(plasticText, currentTrashPicker.GetAmountFromBag(trashType));
                break;
            case Trash.TrashType.Paper:
                SetAmountText(paperText, currentTrashPicker.GetAmountFromBag(trashType));
                break;
            case Trash.TrashType.Metal:
                SetAmountText(metalText, currentTrashPicker.GetAmountFromBag(trashType));
                break;
            case Trash.TrashType.Glass:
                SetAmountText(glassText, currentTrashPicker.GetAmountFromBag(trashType));
                break;
        }
        SetBagWeight(currentTrashPicker.CurrentBagWeight);
    }
    private void SetBagWeight(float amount) => totalBagWeightText.text = amount.ToString();

    /* not used for now
    //in case new "changing trash picker mechanic" is added
    public void ChangeTrashPicker(TrashPicker newTrashPicker)
    {
        if (newTrashPicker != currentTrashPicker)
        {
            currentTrashPicker.OnThrashPickUp -= TrashPicker_OnThrashAction;
            currentTrashPicker.OnThrashRemove -= TrashPicker_OnThrashAction;
            InitializeTrashPicker(newTrashPicker);
        }
    }
    */
    #endregion
}
