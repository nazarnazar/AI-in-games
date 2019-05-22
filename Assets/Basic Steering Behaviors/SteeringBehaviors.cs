using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SteeringType
{
    Unknown = 0,
    Seek = 1,
    Flee = 2,
    Arrive = 3,
    Wander = 4,
    Pursuit = 5,
    Evade = 6,
    ObstacleAvoidance = 7,
    WallAvoidance = 8,
    Interpose = 9,
    Hide = 10,
    FollowPath = 11,
    OffsetPursuit = 12,
    Separation = 13,
    Alignment = 14,
    Cohension = 15,
    Follow = 16
}

public enum Deceleration
{
    Fast = 1,
    Normal = 2,
    Slow = 3,
}

public class SteeringBehaviors
{
    private MovingAgent _movingAgent;
    private Vector2 _wanderTarget;
    private int _currentWaypointIndex;

    public SteeringBehaviors(MovingAgent movingAgent)
    {
        _movingAgent = movingAgent;
        _wanderTarget = new Vector2(1.0f, 1.0f);
    }

    public Vector2 Seek(Vector2 targetPos)
    {
        Vector2 desiredVelocity = targetPos - _movingAgent.GetAgentPosition();
        desiredVelocity.Normalize();
        desiredVelocity *= _movingAgent.MaxForce;
        return (desiredVelocity - _movingAgent.GetAgentVelocity());
    }

    public Vector2 Flee(Vector2 targetPos)
    {
        const float panicDistanceSqr = 1.0f * 1.0f;

        Vector2 desiredVelocity = _movingAgent.GetAgentPosition() - targetPos;
        if (desiredVelocity.sqrMagnitude > panicDistanceSqr)
        {
            return Vector2.zero;
        }
        desiredVelocity.Normalize();
        desiredVelocity *= _movingAgent.MaxForce;
        return (desiredVelocity - _movingAgent.GetAgentVelocity());
    }

    public Vector2 Arrive(Vector2 targetPos, Deceleration deceleration)
    {
        const float decelerationTweaker = 1.0f;
        Vector2 desiredVelocity = targetPos - _movingAgent.GetAgentPosition();
        float distance = desiredVelocity.magnitude;
        float decelerationKoef = Mathf.Clamp((distance / ((float)deceleration * decelerationTweaker)), 0.0f, 1.0f);
        desiredVelocity.Normalize();
        desiredVelocity *= decelerationKoef * _movingAgent.MaxForce;
        return (desiredVelocity - _movingAgent.GetAgentVelocity());
    }

    public Vector2 Wander()
    {
        const float wanderRadius = 2.0f;
        const float wanderDistance = 2.0f;
        const float wanderJitter = 0.2f;
        _wanderTarget += new Vector2(Random.Range(-1.0f, 1.0f) * wanderJitter, Random.Range(-1.0f, 1.0f) * wanderJitter);
        _wanderTarget.Normalize();
        Vector2 target = _wanderTarget + ((Vector2)_movingAgent.transform.right * wanderDistance);
        Vector2 desiredVelocity = target - _movingAgent.GetAgentPosition();
        desiredVelocity.Normalize();
        desiredVelocity *= _movingAgent.MaxForce;
        return desiredVelocity;
    }

    public Vector2 Pursuit(MovingAgent evader)
    {
        Vector2 toEvader = evader.GetAgentPosition() - _movingAgent.GetAgentPosition();
        float lookAheadTime = toEvader.magnitude / (_movingAgent.MaxForce + evader.MaxForce);
        return Seek(evader.GetAgentPosition() + evader.GetAgentVelocity() * lookAheadTime);
    }

    public Vector2 Evade(MovingAgent pursuer)
    {
        Vector2 toPursuer = pursuer.GetAgentPosition() - _movingAgent.GetAgentPosition();
        float lookAheadTime = toPursuer.magnitude / (_movingAgent.MaxForce + pursuer.MaxForce);
        return Flee(pursuer.GetAgentPosition() + pursuer.GetAgentVelocity() * lookAheadTime);
    }

    public Vector2 ObstacleAvoidance(GameObject obstacle)
    {
        float agentColliderSize = _movingAgent.GetComponent<BoxCollider2D>().size.x;
        Vector2 distance = (Vector2)obstacle.GetComponent<Transform>().position - _movingAgent.GetAgentPosition();
        float multiplier = 1.0f + (agentColliderSize - distance.x) / agentColliderSize;
        const float breakingKoef = 0.1f;
        float obstacleSize = obstacle.GetComponent<BoxCollider2D>().size.x / 2.0f;
        Vector2 steeringForce = new Vector2((obstacleSize - distance.x) * breakingKoef, (obstacleSize - distance.y) * multiplier);
        steeringForce.Normalize();
        steeringForce *= _movingAgent.MaxForce;
        return steeringForce;
    }

    public Vector2 WallAvoidance(RaycastHit2D hit)
    {
        Vector2 steeringForce = hit.normal;
        steeringForce.Normalize();
        steeringForce *= _movingAgent.MaxForce;
        return steeringForce;
    }

    public Vector2 Interpose(MovingAgent agentOne, MovingAgent agentTwo)
    {
        Vector2 middlePoint = (agentOne.GetAgentPosition() + agentTwo.GetAgentPosition()) / 2.0f;
        float timeToReachMiddlePoint = ((_movingAgent.GetAgentPosition() - middlePoint).magnitude / _movingAgent.MaxForce);
        Vector2 aheadPosOne = agentOne.GetAgentPosition() + agentOne.GetAgentVelocity() * timeToReachMiddlePoint;
        Vector2 aheadPosTwo = agentTwo.GetAgentPosition() + agentTwo.GetAgentVelocity() * timeToReachMiddlePoint;
        middlePoint = (aheadPosOne + aheadPosTwo) / 2.0f;
        return Arrive(new Vector2(_movingAgent.GetAgentPosition().x, middlePoint.y), Deceleration.Normal);
    }

    public Vector2 GetHidingPosition(MovingAgent pursuer, GameObject obstacle)
    {
        const float hideDistance = 2.0f;
        float obRadius = obstacle.GetComponent<BoxCollider2D>().size.x;
        Vector2 distance = (Vector2)obstacle.transform.position - pursuer.GetAgentPosition();
        Vector2 hidingPos = (Vector2)obstacle.transform.position + distance.normalized * (hideDistance + obRadius);
        return hidingPos;
    }

    public Vector2 Hide(MovingAgent pursuer, GameObject obstacle)
    {
        return Arrive(GetHidingPosition(pursuer, obstacle), Deceleration.Normal);
    }

    public Vector2 FollowPath(Vector2[] waypoints)
    {
        const float maxVelocity = 4.0f;
        const float brakeDist = 2.0f;
        const float minWaypointDistance = 0.5f;
        if ((waypoints[_currentWaypointIndex] - _movingAgent.GetAgentPosition()).magnitude < minWaypointDistance)
        {
            _currentWaypointIndex++;
            if (_currentWaypointIndex >= waypoints.Length)
            {
                _currentWaypointIndex = 0;
            }
        }
        int nextWaypointIndex = _currentWaypointIndex + 1;
        if (nextWaypointIndex >= waypoints.Length)
        {
            nextWaypointIndex = 0;
        }
        float cornerKoef = Cornering(waypoints[_currentWaypointIndex], waypoints[nextWaypointIndex]);
        float toWaypoint = (waypoints[_currentWaypointIndex] - _movingAgent.GetAgentPosition()).magnitude;
        float distKoef = Mathf.Clamp01(toWaypoint / brakeDist);
        float velocityKoef = Mathf.Lerp(cornerKoef, 1.0f, distKoef);
        float desiredVelocity = maxVelocity * velocityKoef;
        if (desiredVelocity > _movingAgent.GetAgentVelocity().magnitude)
        {
            return Seek(waypoints[_currentWaypointIndex]);
        }
        else
        {
            return Braking(waypoints[_currentWaypointIndex]);
        }
    }

    private float Cornering(Vector2 waypointOne, Vector2 waypointTwo)
    {
        float a = (waypointOne - _movingAgent.GetAgentPosition()).magnitude;
        float b = (waypointTwo - waypointOne).magnitude;
        float c = (_movingAgent.GetAgentPosition() - waypointTwo).magnitude;
        float angleCos = (b * b + a * a - c * c) / (2.0f * b * a);
        float angle = Mathf.Acos(angleCos) * (180.0f / Mathf.PI);
        return angle / 180.0f;
    }

    private Vector2 Braking(Vector2 brakePoint)
    {
        const float brakingKoef = 1.0f;
        Vector2 brakeVector = _movingAgent.GetAgentPosition() - brakePoint;
        brakeVector.Normalize();
        brakeVector *= brakingKoef;
        return (brakeVector - _movingAgent.GetAgentVelocity());
    }

    public Vector2 OffsetPursuit(MovingAgent leader, Vector2 offset)
    {
        Vector2 worldOffsetPos = leader.GetAgentPosition() - leader.GetAgentVelocity().normalized - offset;
        Vector2 toLeader = worldOffsetPos - _movingAgent.GetAgentPosition();
        float lookAheadTime = toLeader.magnitude / (_movingAgent.MaxForce + leader.MaxForce);
        return Arrive(worldOffsetPos + leader.GetAgentVelocity() * lookAheadTime, Deceleration.Normal);
    }

    public Vector2 Separation(GameObject[] agents)
    {
        const float forceScaler = 10.0f;
        Vector2 steeringForce = Vector2.zero;
        MovingAgent tempAgent;
        int agentsLength = agents.Length;
        for (int i = 0; i < agentsLength; i++)
        {
            tempAgent = agents[i].GetComponent<MovingAgent>();
            if (_movingAgent != tempAgent && tempAgent.IsTaggedAsNeighbor())
            {
                Vector2 toNeighbor = _movingAgent.GetAgentPosition() - tempAgent.GetAgentPosition();
                steeringForce += toNeighbor.normalized / toNeighbor.magnitude;
            }
        }
        return (steeringForce * forceScaler);
    }

    public Vector2 Alignment(GameObject[] agents)
    {
        const float forceScaler = 1.0f;
        Vector2 steeringForce = Vector2.zero;
        MovingAgent tempAgent;
        float avarageRotation = 0.0f;
        int rotationsCount = 0;
        int agentsLength = agents.Length;
        for (int i = 0; i < agentsLength; i++)
        {
            tempAgent = agents[i].GetComponent<MovingAgent>();
            if (_movingAgent != tempAgent && tempAgent.IsTaggedAsNeighbor())
            {
                avarageRotation += tempAgent.GetAgentRotation() % 360.0f;
                rotationsCount++;
            }
        }
        if (rotationsCount > 0)
        {
            avarageRotation /= rotationsCount;
            steeringForce = new Vector2(Mathf.Cos(avarageRotation / (180.0f / Mathf.PI)),
                                        Mathf.Sin(avarageRotation / (180.0f / Mathf.PI)));
        }
        return (steeringForce.normalized / forceScaler);
    }

    public Vector2 Cohension(GameObject[] agents)
    {
        Vector2 steeringForce = Vector2.zero;
        MovingAgent tempAgent;
        Vector2 centerOfMass = Vector2.zero;
        int centersCount = 0;
        int agentsLength = agents.Length;
        for (int i = 0; i < agentsLength; i++)
        {
            tempAgent = agents[i].GetComponent<MovingAgent>();
            if (_movingAgent != tempAgent && tempAgent.IsTaggedAsNeighbor())
            {
                centerOfMass += tempAgent.GetAgentPosition();
                centersCount++;
            }
        }
        if (centersCount > 0)
        {
            centerOfMass /= centersCount;
            steeringForce = Arrive(centerOfMass, Deceleration.Slow);
        }
        return steeringForce;
    }
}