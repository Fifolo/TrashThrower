using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BadNpcData", menuName = "NpcData/BadNpcData")]
public class BadNpcData : NpcData
{
    [HideInInspector]
    public bool dropsTrash = false;
    [HideInInspector]
    public float dropRate = 2f;
    [HideInInspector]
    public float initialWaitTime = 2f;
    [HideInInspector]
    public float dropChance = 2f;


    //public bool shootsAtPlayer = false;
    //public bool followsPlayer = false;

    public override void InitializeNpc(GameObject npc)
    {
        npc_Type = NPC_Type.Bad;

        if (dropsTrash)
        {
            TrashDropping trashDropping = npc.AddComponent<TrashDropping>();
            trashDropping.InitializeData(this);
            trashDropping.StartDroppingTrash();
        }
        //if (shootsAtPlayer) Debug.LogWarning("shooting at player not yet implemented");
        //if (followsPlayer) Debug.LogWarning("following player not yet implemented");
    }
}
