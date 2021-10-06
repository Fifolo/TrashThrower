using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class InGameUI : Singleton<InGameUI>
{
    #region Editor Exposed Fields
    [SerializeField] private TextMeshProUGUI plasticText;
    [SerializeField] private TextMeshProUGUI paperText;
    [SerializeField] private TextMeshProUGUI metalText;
    [SerializeField] private TextMeshProUGUI glassText;
    [SerializeField] private TextMeshProUGUI differentText;
    [SerializeField] private TextMeshProUGUI totalBagWeightText;
    [SerializeField] private TextMeshProUGUI totalTrashAmountText;
    [SerializeField] private TextMeshProUGUI scoreText;
    #endregion

    #region Instance Variables
    public List<TrashCan> trashCans;
    private TrashPicker trashPicker;
    public int TotalTrashAmount { get; private set; }
    public float TotalBagWeight { get; private set; }
    public int Score { get; private set; } 
    #endregion

    #region Mono Behaviour
    protected override void Awake()
    {
        base.Awake();
        trashCans = new List<TrashCan>();
        TotalTrashAmount = 0;
        TotalBagWeight = 0;
        Score = 0;
    }
    private void Start()
    {
        SubscribeToEvents();
        InitializeCans();
        InitializeTrashPicker(GameObject.Find("Player").GetComponentInChildren<TrashPicker>());
    }

    private void SubscribeToEvents()
    {
        NpcController.OnNpcDeath += NpcController_OnNpcDeath;
    }

    private void NpcController_OnNpcDeath(NpcData.NPC_Type npc_Type)
    {
        switch (npc_Type)
        {
            case NpcData.NPC_Type.Bad:
                Score += 1;
                break;
            case NpcData.NPC_Type.Good:
                Score -= 1;
                break;
        }
        scoreText.text = Score.ToString();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        UnsubsrcribeToEvents();
    }
    #endregion

    #region Methods
    private void InitializeTrashPicker(TrashPicker newTrashPicker)
    {
        trashPicker = newTrashPicker;
        trashPicker.OnThrashPickUp += TrashPicker_OnThrashAction;
        trashPicker.OnThrashRemove += TrashPicker_OnThrashAction;
        SetBagWeight(trashPicker.CurrentBagWeight);
        ResetTrashAmountInBag();
    }
    private void ResetTrashAmountInBag()
    {
        differentText.text = trashPicker.GetAmountFromBag(Trash.TrashType.Different).ToString();
        plasticText.text = trashPicker.GetAmountFromBag(Trash.TrashType.Plastic).ToString();
        paperText.text = trashPicker.GetAmountFromBag(Trash.TrashType.Paper).ToString();
        metalText.text = trashPicker.GetAmountFromBag(Trash.TrashType.Metal).ToString();
        glassText.text = trashPicker.GetAmountFromBag(Trash.TrashType.Glass).ToString();
    }
    //in case new "changing trash picker mechanic" is added
    public void ChangeTrashPicker(TrashPicker newTrashPicker)
    {
        if (newTrashPicker != trashPicker)
        {
            trashPicker.OnThrashPickUp -= TrashPicker_OnThrashAction;
            trashPicker.OnThrashRemove -= TrashPicker_OnThrashAction;
            InitializeTrashPicker(newTrashPicker);
        }
    }
    private void TrashPicker_OnThrashAction(Trash.TrashType trashType)
    {
        //not optimized??
        switch (trashType)
        {
            case Trash.TrashType.Different:
                differentText.text = trashPicker.GetAmountFromBag(trashType).ToString();
                break;
            case Trash.TrashType.Plastic:
                plasticText.text = trashPicker.GetAmountFromBag(trashType).ToString();
                break;
            case Trash.TrashType.Paper:
                paperText.text = trashPicker.GetAmountFromBag(trashType).ToString();
                break;
            case Trash.TrashType.Metal:
                metalText.text = trashPicker.GetAmountFromBag(trashType).ToString();
                break;
            case Trash.TrashType.Glass:
                glassText.text = trashPicker.GetAmountFromBag(trashType).ToString();
                break;
        }

        SetBagWeight(trashPicker.CurrentBagWeight);
    }


    private void InitializeCans()
    {
        TrashCan[] foundCans = FindObjectsOfType<TrashCan>(true);
        foreach (TrashCan trashCan in foundCans)
        {
            AddNewCanToList(trashCan);
        }
    }
    public void AddNewCanToList(TrashCan newCan)
    {
        //check if not already in the list
        if (!trashCans.Contains(newCan))
        {
            trashCans.Add(newCan);
            newCan.OnTrashPutInCan += OnTrashPutInCan;

            // if there is any trash, update UI
            if (newCan.TrashInCanAmount != 0)
            {
                TotalTrashAmount += newCan.TrashInCanAmount;
                SetTotalTrashAmount(TotalTrashAmount);
            }
        }
    }

    private void OnTrashPutInCan()
    {
        TotalTrashAmount += 1;
        SetTotalTrashAmount(TotalTrashAmount);

    }
    private void UnsubsrcribeToEvents()
    {
        foreach (TrashCan can in trashCans)
        {
            can.OnTrashPutInCan -= OnTrashPutInCan;
        }

        NpcController.OnNpcDeath -= NpcController_OnNpcDeath;

        if (trashPicker != null)
        {
            trashPicker.OnThrashPickUp -= TrashPicker_OnThrashAction;
            trashPicker.OnThrashRemove -= TrashPicker_OnThrashAction;
        }
    }

    private void SetBagWeight(float amount) => totalBagWeightText.text = amount.ToString();
    private void SetTotalTrashAmount(int amount) => totalTrashAmountText.text = amount.ToString();
    private void SetTrashTypeAmount(Trash.TrashType trashType, int amount)
    {
        switch (trashType)
        {
            case Trash.TrashType.Different:
                differentText.text = amount.ToString();
                break;
            case Trash.TrashType.Glass:
                glassText.text = amount.ToString();
                break;
            case Trash.TrashType.Plastic:
                plasticText.text = amount.ToString();
                break;
            case Trash.TrashType.Paper:
                paperText.text = amount.ToString();
                break;
            case Trash.TrashType.Metal:
                metalText.text = amount.ToString();
                break;
        }
    }
    #endregion
}
