using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using System;

public class AiController : MonoBehaviour, IPunObservable
{
    GameObject[] goalLocations;

    NavMeshAgent agent;
    const float detectionRadius = 30;
    const float fleeRadius = 10;

    public bool isMiniboss = false;
    public bool forTutorial;

    const float normalSpeed = 2f;
    const float minibossSpeed = 4f;
    const float normalAngularSpeed = 120f;
    private bool isFleeing;

    const float fleeingSpeed = 20f;
    const float fleeingAngularSpeed = 500f;

    /* Serialisation stuff. */
    private Vector3 lastSteeringTarget;
    private bool lastIsFleeing;
    private float nextForcedSerialise;

    void ResetAgent()
    {
        SetFleeing(false);

        int index = UnityEngine.Random.Range(0, goalLocations.Length);
        agent.SetDestination(goalLocations[index].transform.position);
    }

    public void DetectNewObstacle(Vector3 position){
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogError("DetectNewObstacle should not be called on client.");
            return;
        }

        /* FIXME: Have seen this occasionally leading to errors.
         * if this is expected, move the checks inside the last if, and remove the warnings. */
        if (!agent.isOnNavMesh)
        {
            Debug.LogWarning("DetectNewObstacle called on agent not on navmesh.");
            return;
        }
        else if (!agent.isActiveAndEnabled)
        {
            Debug.LogWarning("DetectNewObstacle called on agent which is not active.");
            return;
        }

        if (Vector3.Distance(position, this.transform.position) < detectionRadius)
        {
            Vector3 fleeDirection = (this.transform.position - position).normalized;
            Vector3 newgoal = this.transform.position + fleeDirection * fleeRadius;

            NavMeshPath path = new NavMeshPath();
            agent.CalculatePath(newgoal, path);

            if (path.status != NavMeshPathStatus.PathInvalid)
            {
                agent.SetDestination(path.corners[path.corners.Length - 1]);
                SetFleeing(true);
            }
            else
            {
                NavMeshHit hit;
                NavMeshPath newPath = new NavMeshPath();
                float newRadius = Mathf.Infinity;

                if(NavMesh.SamplePosition(newgoal, out hit, newRadius, NavMesh.AllAreas)){
                    agent.CalculatePath(hit.position, newPath);
                    agent.SetDestination(newPath.corners[newPath.corners.Length - 1]);
                    SetFleeing(true);
                }
            }
        }
    }

    void SetFleeing(bool fleeing)
    {
        isFleeing = fleeing;
        if (isFleeing)
        {
            agent.speed = fleeingSpeed;
            agent.angularSpeed = fleeingAngularSpeed;
        }
        else
        {
            agent.speed = isMiniboss ? minibossSpeed : normalSpeed;
            agent.angularSpeed = normalAngularSpeed;
        }
    }

    void Start()
    {
        // Access the agents NavMesh
        agent = GetComponent<NavMeshAgent>();

        if (PhotonNetwork.IsMasterClient)
        {
            goalLocations =
                GameObject.FindGameObjectsWithTag(forTutorial
                    ? "tut_goal"
                    : "goal");
            ResetAgent();
        }
    }

    private void Update()
    {
        if (!agent.isActiveAndEnabled || !agent.isOnNavMesh)
            return;

        if (PhotonNetwork.IsMasterClient)
        {
            if (agent.remainingDistance < 2)
            {
                ResetAgent();
            }
        }
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            /* Obviously only send when changed (PUN does support this) */
            if (agent.steeringTarget != lastSteeringTarget ||
                isFleeing != lastIsFleeing ||
                Time.time > nextForcedSerialise)
            {
                stream.SendNext(agent.steeringTarget);
                stream.SendNext(isFleeing);
                stream.SendNext(agent.nextPosition);
                stream.SendNext(agent.velocity);

                lastSteeringTarget = agent.steeringTarget;
                lastIsFleeing = isFleeing;
                /* Avoid updating everyone at once. */
                nextForcedSerialise = Time.time + UnityEngine.Random.Range(6f, 10f);
            }
        }
        else
        {
            try
            {
                float d = (float)(PhotonNetwork.Time - info.SentServerTime);

                agent.SetDestination((Vector3) stream.ReceiveNext());

                SetFleeing((bool)stream.ReceiveNext());

                Vector3 pos = (Vector3) stream.ReceiveNext();
                Vector3 vel = (Vector3) stream.ReceiveNext();
                agent.velocity = vel;
                agent.nextPosition = pos + (vel * d);
            }
            catch
            {
                Debug.LogError($"Error deserialising agent. ({gameObject.name})" +
                               " Ensure agents don't have PhotonTransformViews, etc, " +
                               "or set PhotonView observable search to manual to disable our agent serialisation.");
            }
        }
    }
}
