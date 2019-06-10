using UnityEngine;
using TMPro;
using System.Collections;

public class Player : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _sr;
    [SerializeField] private TextMeshPro _number;
    [SerializeField] private int _homeRegionIndex;
    [SerializeField] private int _attackRegionIndex;
    [SerializeField] private Sprite _idleSprite;
    [SerializeField] private Sprite[] _movingSprites;
    [SerializeField] private float _movingSpeed;

    public MovingAgent MovingAgent { get; private set; }
    public Region HomeRegion { get; set; }
    public Region AttackRegion { get; set; }
    public Team Team { get; set; }
    public SupportSpot SupportSpot { get; set; }

    private Coroutine _movingRoutine;

    public void InitPlayer()
    {
        _number.text = Random.Range(1, 100).ToString();
        MovingAgent = GetComponent<MovingAgent>();
    }

    public int GetPlayerNumber()
    {
        return int.Parse(_number.text);
    }

    public int GetHomeRegionIndex()
    {
        return _homeRegionIndex;
    }

    protected void SetHomeRegionIndex(int newHomeRegionIndex)
    {
        _homeRegionIndex = newHomeRegionIndex;
        Team.UpdatePlayerHomeRegion(this);
    }

    public int GetAttackRegionIndex()
    {
        return _attackRegionIndex;
    }

    protected void SetAttackRegionIndex(int newAttackRegionIndex)
    {
        _attackRegionIndex = newAttackRegionIndex;
        Team.UpdatePlayerAttackRegion(this);
    }

    public bool IsAtTarget()
    {
        if (MovingAgent.TargetReached())
        {
            MovingAgent.Stop();
            TrackPuck();
            return true;
        }

        return false;
    }

    public bool CanKickThePuck()
    {
        if (Team.InControl())
            return Vector2.Distance(GetPosition(), GetPuck().GetPosition()) < MatchData.Instance().AttackPuckKickRadius;
        else
            return Vector2.Distance(GetPosition(), GetPuck().GetPosition()) < MatchData.Instance().DefencePuckKickRadius;
    }

    public Puck GetPuck()
    {
        return Team.GetPuck();
    }

    public Vector2 GetPosition()
    {
        return MovingAgent.GetAgentPosition();
    }

    public void SetMaxSpeed(float speed)
    {
        MovingAgent.MaxVelocity = speed;
    }

    public bool IsInControl()
    {
        return this == Team.GetControllingPlayer();
    }

    public bool IsClosestToPuck()
    {
        return this == Team.GetClosestToBallPlayer();
    }

    public Vector2 GetCurrentVelocity()
    {
        return MovingAgent.GetAgentVelocity();
    }

    public void TrackPuck()
    {
        Vector2 direction = Team.GetPuck().GetPosition() - MovingAgent.GetAgentPosition();
        float lookAngle = Mathf.Atan2(direction.y, direction.x);
        MovingAgent.SetAgentRotation(lookAngle * Mathf.Rad2Deg);
    }

    public bool IsAheadOfAttacker()
    {
        if (Team.GetControllingPlayer() == null)
        {
            return false;
        }
        float sqrDistanceToGoal = (Team.GetOpponentTeam().GetHomeGoal().GetPosition() - MovingAgent.GetAgentPosition()).sqrMagnitude;
        float sqrAttackerDistanceToGoal = (Team.GetOpponentTeam().GetHomeGoal().GetPosition() - Team.GetControllingPlayer().GetPosition()).sqrMagnitude;
        return (sqrDistanceToGoal < sqrAttackerDistanceToGoal);
    }

    public bool IsInHotRegion()
    {
        float xDistanceToGoal = Mathf.Abs(Team.GetOpponentTeam().GetHomeGoal().GetPosition().x - MovingAgent.GetAgentPosition().x);
        float xThirdPitchPart = Team.GetRink().GetRinkLength() / 3.0f;
        return (xDistanceToGoal < xThirdPitchPart);
    }

    public bool IsReadyForNextKick()
    {
        return true;
    }

    public Vector2 Heading()
    {
        return new Vector2(Mathf.Cos(MovingAgent.GetAgentRotation() * Mathf.Deg2Rad), Mathf.Sin(MovingAgent.GetAgentRotation() * Mathf.Deg2Rad));
    }

    public bool IsThreatened()
    {
        Player[] opponents = Team.GetOpponentTeam().GetPlayers();
        for (int i = 0; i < opponents.Length; i++)
        {
            if (Vector2.Distance(GetPosition(), Team.GetOpponentTeam().GetHomeGoal().GetPosition()) >
                Vector2.Distance(opponents[i].GetPosition(), Team.GetOpponentTeam().GetHomeGoal().GetPosition()) &&
                Vector2.Distance(GetPosition(), opponents[i].GetPosition()) < MatchData.Instance().ThreatenedRadius)
            {
                return true;
            }
        }
        return false;
    }

    public void ChangeColor(Color newColor, bool debugReason = true)
    {
        if (_sr != null)
        {
            if (debugReason && MatchData.Instance().IsDebugMode && transform.parent.name == "Team A" ||
                !debugReason)
            {
                _sr.color = newColor;
            }
        }
    }

    public void StartMovingAnimation()
    {
        if (_movingRoutine != null)
        {
            return;
        }
        _movingRoutine = StartCoroutine(Moving());
    }

    public void StopMovingAnimation()
    {
        if (_movingRoutine != null)
        {
            StopCoroutine(_movingRoutine);
            _movingRoutine = null;
        }
    }

    private IEnumerator Moving()
    {
        while (true)
        {
            for (int i = 0; i < _movingSprites.Length; i++)
            {
                _sr.sprite = _movingSprites[i];
                yield return new WaitForSeconds(1.0f / _movingSpeed);
            }
        }
    }
}