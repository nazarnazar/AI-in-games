using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovingStyle
{
    Force,
    Velocity
}

public class MovingAgent : MonoBehaviour
{
    public float MaxForce;
    public float MinForce;
    public float MaxVelocity;
    public float BrakingForce;
    public float MinTargetDistance;
    public Deceleration ArriveDeceleration;
    public MovingAgent Evader;
    public MovingAgent Pursuer;
    public GameObject Wall;
    public SteeringType[] Steerings;
    public float MinColliderSize;
    public float ColliderSizeKoef;
    public MovingStyle MovingStyle;
    public Vector2[] Waypoints;
    public Vector2 PursuitOffset;
    public bool GroupLeader;
    public float NeighborhoodRadius;

    private SteeringBehaviors _steeringBehavior;
    private BoxCollider2D _boxCollider;
    private CircleCollider2D _circleCollider;
    private Rigidbody2D _rigidBody;
    private SteeringType[] _currentSteeringTypes;
    private GameObject _currentObstacle;
    private bool _isNeighbor;
    private GameObject[] _agents;

    private Vector2 _force;
    private Vector2 _target;

	private void Awake()
    {
        if (GetComponent<BoxCollider2D>() != null)
        {
            _boxCollider = GetComponent<BoxCollider2D>();
        }
        else if (GetComponent<CircleCollider2D>() != null)
        {
            _circleCollider = GetComponent<CircleCollider2D>();
        }
        _rigidBody = GetComponent<Rigidbody2D>();
        _steeringBehavior = new SteeringBehaviors(this);

        int steeringsLength = Steerings.Length;
        _currentSteeringTypes = new SteeringType[steeringsLength];
        for (int i = 0; i < steeringsLength; i++)
        {
            _currentSteeringTypes[i] = Steerings[i];
        }
        if (!HasSteeringType(SteeringType.ObstacleAvoidance))   // TODO: Fix this approach
        {
            /*
            if (gameObject.name != "Puck")
            {
                _circleCollider.enabled = false;
            }
            */
        }
        if (GroupLeader)
        {
            TagNeighbors();
        }
	}

	private void Update()
    {
        /*
        if (Input.GetMouseButton(0))
        {
            _target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        */

        for (int i = 0; i < _currentSteeringTypes.Length; i++)
        {
            switch (_currentSteeringTypes[i])
            {
                case SteeringType.Unknown:
                    _force = Vector2.zero;
                    break;         
                case SteeringType.Seek:
                    _force = _steeringBehavior.Seek(_target);
                    break;
                case SteeringType.Flee:
                    _force = _steeringBehavior.Flee(_target);
                    break;
                case SteeringType.Arrive:
                    _force = _steeringBehavior.Arrive(_target, ArriveDeceleration);
                    break;
                case SteeringType.Wander:
                    _force = _steeringBehavior.Wander();
                    break;
                case SteeringType.Pursuit:
                    _force = _steeringBehavior.Pursuit(Evader);
                    break;
                case SteeringType.Evade:
                    _force = _steeringBehavior.Evade(Pursuer);
                    break;
                case SteeringType.ObstacleAvoidance:
                    _force = _steeringBehavior.ObstacleAvoidance(_currentObstacle);
                    break;
                case SteeringType.Interpose:
                    _force = _steeringBehavior.Interpose(Evader, Pursuer);
                    break;
                case SteeringType.Hide:
                    _force = _steeringBehavior.Hide(Pursuer, Wall);
                    break;
                case SteeringType.FollowPath:
                    _force = _steeringBehavior.FollowPath(Waypoints);
                    break;
                case SteeringType.OffsetPursuit:
                    _force = _steeringBehavior.OffsetPursuit(Evader, PursuitOffset);
                    break;
                case SteeringType.Separation:
                    _force = _steeringBehavior.Separation(GetAgents());
                    break;
                case SteeringType.Alignment:
                    _force = _steeringBehavior.Alignment(GetAgents());
                    break;
                case SteeringType.Cohension:
                    _force = _steeringBehavior.Cohension(GetAgents());
                    break;
                case SteeringType.Follow:
                    _force = _steeringBehavior.Seek(Evader.GetAgentPosition());
                    break;
            }
            ApplyForce(_force);
        }

        //CalculateBoxColliderSize();
        //ZeroOverlap();
    }

    private void FixedUpdate()
    {
        if (HasSteeringType(SteeringType.WallAvoidance))
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right);
            if (hit.collider != null)
            {
                ApplyForce(_steeringBehavior.WallAvoidance(hit));
            }
            Vector2 tempVector = transform.right;
            float angle = Mathf.PI / 4.0f;
            tempVector.Set(tempVector.x * Mathf.Cos(angle) - tempVector.y * Mathf.Sin(angle),
                tempVector.x * Mathf.Sin(angle) + tempVector.y * Mathf.Cos(angle));
            hit = Physics2D.Raycast(transform.position, tempVector);
            if (hit.collider != null)
            {
                ApplyForce(_steeringBehavior.WallAvoidance(hit));
            }
            tempVector = transform.right;
            tempVector.Set(tempVector.x * Mathf.Cos(-angle) - tempVector.y * Mathf.Sin(-angle),
                tempVector.x * Mathf.Sin(-angle) + tempVector.y * Mathf.Cos(-angle));
            hit = Physics2D.Raycast(transform.position, tempVector);
            if (hit.collider != null)
            {
                ApplyForce(_steeringBehavior.WallAvoidance(hit));
            }
        }
    }
        
    private void ApplyForce(Vector2 force)
    {
        if (force.sqrMagnitude < MinForce)
        {
            return;
        }
        if (_rigidBody.velocity.sqrMagnitude > MaxVelocity)
        {
            _rigidBody.AddForce(-_rigidBody.velocity * BrakingForce * Time.deltaTime);
        }
        else
        {
            _rigidBody.AddForce(force * Time.deltaTime);
        }
        if (MovingStyle == MovingStyle.Force)
        {
            _rigidBody.MoveRotation(Mathf.Atan2(force.y, force.x) * (180.0f / Mathf.PI));
        }
        else
        {
            _rigidBody.MoveRotation(Mathf.Atan2(GetAgentVelocity().y, GetAgentVelocity().x) * (180.0f / Mathf.PI));
        }
    }

    public void AddSteeringType(SteeringType st)
    {
        SteeringType[] sTypes = new SteeringType[_currentSteeringTypes.Length + 1];
        for (int i = 0; i < _currentSteeringTypes.Length; i++)
        {
            if (st == _currentSteeringTypes[i])
            {
                return;
            }
            sTypes[i] = _currentSteeringTypes[i];
        }
        sTypes[_currentSteeringTypes.Length] = st;
        _currentSteeringTypes = sTypes;

        _currentSteeringTypes = new SteeringType[] { st };  // TODO: Fix this bug
    }

    public void RemoveSteeringType(SteeringType st)
    {
        if (_currentSteeringTypes.Length == 0)
        {
            return;
        }

        int toRemove = 0;
        for (int i = 0; i < _currentSteeringTypes.Length; i++)
        {
            if (st == _currentSteeringTypes[i])
            {
                toRemove++;
            }
        }
        SteeringType[] sTypes = new SteeringType[_currentSteeringTypes.Length - toRemove];
        for (int i = 0, j = 0; i < _currentSteeringTypes.Length - 1; i++, j++)
        {
            while (st == _currentSteeringTypes[j])
            {
                j++;
            }
            sTypes[i] = _currentSteeringTypes[j];
        }
        _currentSteeringTypes = sTypes;

        _currentSteeringTypes = new SteeringType[] { };  // TODO: Fix this bug
    }

    public bool HasSteeringType(SteeringType st)
    {
        for (int i = 0; i < _currentSteeringTypes.Length; i++)
        {
            if (st == _currentSteeringTypes[i])
            {
                return true;
            }
        }
        return false;
    }

    public void SetTarget(Vector2 target)
    {
        _target = target;
    }

    public Vector2 GetTarget()
    {
        return _target;
    }

    public bool TargetReached()
    {
        return ((_target - GetAgentPosition()).magnitude < MinTargetDistance);
    }

    public void SetAgentPosition(Vector2 position)
    {
        _rigidBody.position = position;
    }

    public Vector2 GetAgentPosition()
    {
        return _rigidBody.position;
    }

    public void SetAgentVelocity(Vector2 vel)
    {
        _rigidBody.velocity = vel;
    }

    public void Stop()
    {
        _rigidBody.angularVelocity = 0.0f;
        _rigidBody.velocity = Vector2.zero;
    }

    public Vector2 GetAgentVelocity()
    {
        return _rigidBody.velocity;
    }
     
    public void SetAgentRotation(float angle)
    {
        _rigidBody.rotation = angle;
    }

    public float GetAgentRotation()
    {
        return _rigidBody.rotation;
    }

    public bool IsTaggedAsNeighbor()
    {
        return _isNeighbor;
    }

    public void CalculateBoxColliderSize()
    {
        float size = MinColliderSize + GetAgentVelocity().sqrMagnitude * ColliderSizeKoef;
        if (_boxCollider != null)
        {
            _boxCollider.size = new Vector2(size, _boxCollider.size.y);
            _boxCollider.offset = new Vector2(size / 2.0f, 0.0f);
        }
    }

    public void TagAsNeighbor()
    {
        _isNeighbor = true;
    }

    public void Untag()
    {
        _isNeighbor = false;
    }

    private GameObject[] GetAgents()
    {
        if (_agents == null)
        {
            _agents = GameObject.FindGameObjectsWithTag("Agent");
        }
        return _agents;
    }

    private void TagNeighbors()
    {
        GameObject[] agents = GetAgents();
        int agentsLength = agents.Length;
        MovingAgent tempMovingAgent;
        for (int i = 0; i < agentsLength; i++)
        {
            tempMovingAgent = agents[i].GetComponent<MovingAgent>();
            tempMovingAgent.Untag();
            if (tempMovingAgent != this &&
                (tempMovingAgent.GetAgentPosition() - GetAgentPosition()).sqrMagnitude < (NeighborhoodRadius * NeighborhoodRadius))
            {
                tempMovingAgent.TagAsNeighbor();
            }
        }
        TagAsNeighbor();
    }

    private void ZeroOverlap()
    {
        const float agentsMinDist = 1.0f;
        GameObject[] agents = GetAgents();
        MovingAgent tempAgent;
        for (int i = 0; i < agents.Length; i++)
        {
            tempAgent = agents[i].GetComponent<MovingAgent>();
            if (tempAgent != this)
            {
                Vector2 toAgent = GetAgentPosition() - tempAgent.GetAgentPosition();
                float dist = toAgent.magnitude;
                float overlap = agentsMinDist - dist;
                if (overlap > 0.0f)
                {
                    SetAgentPosition(GetAgentPosition() + (toAgent / dist) * overlap);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        _currentObstacle = other.gameObject;
        // AddSteeringType(SteeringType.ObstacleAvoidance); // TODO: Fix that
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        _currentObstacle = null;
        // RemoveSteeringType(SteeringType.ObstacleAvoidance);
    }
}