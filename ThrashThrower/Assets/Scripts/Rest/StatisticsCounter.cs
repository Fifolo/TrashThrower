using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatisticsCounter : Singleton<StatisticsCounter>
{
    public int Score { get; private set; }
    //npc's
    public int GoodNpcsKilled { get; private set; }
    public int BadNpcsKilled { get; private set; }

    //trash
    public int TrashPutInCans { get; private set; }

    //damage
    public float TotalDamageTaken { get; private set; }
    public float TotalDamageDealt { get; private set; }

    private void Start()
    {
        HealthController.OnPlayerDamageTaken += HealthController_OnPlayerDamageTaken;
        BadNpc.OnBadNpcDamageTaken += BadNpc_OnBadNpcDamageTaken;
        Npc.OnNpcDeath += Npc_OnNpcDeath;
        TrashCan.OnTrashPutInCan += TrashCan_OnTrashPutInCan;
    }

    private void Npc_OnNpcDeath(NpcData npcData)
    {
        //good npc kiiled
        if (npcData.pointsOnDeath < 0) GoodNpcsKilled += 1;

        //bad npc killed
        else if (npcData.pointsOnDeath > 0) BadNpcsKilled += 1;
    }

    private void TrashCan_OnTrashPutInCan(Trash trash)
    {
        TrashPutInCans += 1;
        Score += (int)trash.TrashContamintation;
    }
    private void BadNpc_OnBadNpcDamageTaken(float damageTakenByNpc) => TotalDamageDealt += damageTakenByNpc;
    private void HealthController_OnPlayerDamageTaken(float damageTakenByPlayer) => TotalDamageTaken += damageTakenByPlayer;
}
