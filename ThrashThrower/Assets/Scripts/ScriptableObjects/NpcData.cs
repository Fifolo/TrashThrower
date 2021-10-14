using UnityEngine;

public abstract class NpcData : ScriptableObject
{
    public float health = 100f;
    public float roamingRange = 10f;
    public float runningSpeed = 8f;
    public int pointsOnDeath = 0;
    //Amount of time that npc waits after reaching destination, before setting new path
    public float NEW_PATH_CD = 1f;
    //Amount of time that npc controller waits, before trying to look for new valid path
    public float CAN_REACH_POSITION_CD = 0.2f;
    //check if npc reached position cd
    public float DISTANCE_CHECK_CD = 0.4f;
    //how often should npc look for new path to target
    public float FOLLOW_CD = 1F;

    public abstract void InitializeNpc(GameObject npc);
    protected NpcType npcType;
    public NpcType NPCType { get { return npcType; } }
    public enum NpcType
    {
        Good,
        Bad
    }
}
