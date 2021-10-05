using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NpcController : MonoBehaviour, IDamagable
{
    #region Editor Exposed Fields
    [SerializeField] private NpcData npcData;
    #endregion

    #region Instance Variables
    public NpcData.NPC_Type npc_Type { get { return npcData.Npc_Type; } }
    private NavMeshAgent agent;
    private Transform npcTransform;
    private NPC_State currentState = NPC_State.Idle;
    public enum NPC_State
    {
        Idle,
        Roaming,
        FightingPlayer
    }
    #endregion

    #region MonoBehaviour
    private void Awake()
    {
        npcTransform = transform;
        agent = npcTransform.GetComponent<NavMeshAgent>();

        CheckNpcData();
        currentState = NPC_State.Roaming;
        StartCoroutine(Roam());
    }
    private void CheckNpcData()
    {
        if (npcData == null)
        {
            Debug.LogWarning($"Npc: {name}, npc data not attached");
        }
        else agent.speed = npcData.runningSpeed;
    }
    private IEnumerator Roam()
    {
        //Debug.Log("Entering roaming state");
        while (currentState == NPC_State.Roaming)
        {
            //get valid destination
            //Debug.Log("Searching for valid destination...");
            Vector3 nextDestination = GetRandomPosition();
            while (!CanReachPositionWithCalculatePath(nextDestination))
            {
                yield return new WaitForSeconds(NpcData.CAN_REACH_POSITION_CD);
                nextDestination = GetRandomPosition();
            }
            //Debug.Log($"Valid destination found {nextDestination}");
            //set agent destination
            agent.SetDestination(nextDestination);

            //wait until agent reaches the target

            while (!AgentReachedDestination(nextDestination))
            {
                yield return new WaitForSeconds(NpcData.DISTANCE_CHECK_CD);
            }

            //wait for some time before going to new path
            yield return new WaitForSeconds(npcData.newPathCd);
            //Debug.Log($"{timeBetweenNewPath} seconds passed, returning to main loop");
        }
    }
    #endregion

    #region Methods
    private bool AgentReachedDestination(Vector3 destination)
    {
        //disable y (height) calculations
        //Debug.Log("AgentReachedDestination called");
        Vector2 agentPosition = new Vector2(npcTransform.position.x, npcTransform.position.z);
        Vector2 dest = new Vector2(destination.x, destination.z);
        //Debug.Log($"Vector2.Distance(agentPosition, dest) = {Vector2.Distance(agentPosition, dest)}");
        return Vector2.Distance(agentPosition, dest) < agent.stoppingDistance;
    }
    private Vector3 GetRandomPosition()
    {
        Vector3 randomPosition = new Vector3(
            npcTransform.position.x + Random.Range(-npcData.roamingRange, npcData.roamingRange),
            0,
            npcTransform.position.z + Random.Range(-npcData.roamingRange, npcData.roamingRange));
        return randomPosition;
    }
    //less expensive but for some reason it sometimes allows wrong position to pass
    private bool CanReachPositionWithRaycast(Vector3 position)
    {
        if (NavMesh.Raycast(agent.transform.position, position, out NavMeshHit hit, NavMesh.AllAreas))
        {
            return true;
        }
        return false;
    }
    //more expensive but accurate
    private bool CanReachPositionWithCalculatePath(Vector3 position)
    {
        NavMeshPath path = new NavMeshPath();
        if (agent.CalculatePath(position, path))
        {
            if (path.status == NavMeshPathStatus.PathComplete) return true;
        }
        return false;
    }

    public void TakeDamage(float damage)
    {
        Debug.Log($"{name}, just took {damage} damage");
    }
    #endregion
}
