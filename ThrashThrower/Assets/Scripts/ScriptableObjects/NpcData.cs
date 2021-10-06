using UnityEngine;

public abstract class NpcData : ScriptableObject
{
    public float roamingRange = 10f;
    public float runningSpeed = 8f;
    //public float walkingSpeed = 5f;

    //Amount of time that npc waits after reaching destination, before setting new path
    public static readonly float NEW_PATH_CD = 1f;
    //Amount of time that npc controller waits, before trying to look for new valid path
    public static readonly float CAN_REACH_POSITION_CD = 0.2f;
    //check if npc reached position cd
    public static readonly float DISTANCE_CHECK_CD = 0.4f;

    public abstract void InitializeNpc(GameObject npc);
    protected NPC_Type npc_Type;
    public NPC_Type Npc_Type { get { return npc_Type; } }
    public enum NPC_Type
    {
        Good,
        Bad
    }
}
