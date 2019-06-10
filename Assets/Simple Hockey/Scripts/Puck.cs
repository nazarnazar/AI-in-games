using UnityEngine;
using UnityEngine.SceneManagement;

public class Puck : MonoBehaviour
{
    [SerializeField] private float _kickTime;

    private Player _lastPlayerKickedPuck;
    private Rigidbody2D _rb;
    private float _debugKickForce;
    private MovingAgent _movingAgent;
    private float _kickTimer;

	private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _movingAgent = GetComponent<MovingAgent>();
        _debugKickForce = MatchData.Instance().DebugKickForce;
    }
	
	private void Update()
    {
        if (MatchData.Instance().IsDebugMode)
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                Kick(new Vector2(0.0f, 1.0f), _debugKickForce, null);
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                Kick(new Vector2(1.0f, 0.0f), _debugKickForce, null);
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                Kick(new Vector2(-1.0f, 0.0f), _debugKickForce, null);
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                Kick(new Vector2(0.0f, -1.0f), _debugKickForce, null);
            }
            if (Input.GetKey(KeyCode.Space))
            {
                Trap();
            }
        }

        _kickTimer -= Time.deltaTime;
	}

    public bool CanKick(Player p)
    {
        return  !(_kickTimer > 0.0f && p != null && _lastPlayerKickedPuck != null && p.Team != _lastPlayerKickedPuck.Team);
    }

    public bool Kick(Vector2 direction, float force, Player p)
    {
        if (!CanKick(p))
        {
            return false;
        }

        if (p != null && _lastPlayerKickedPuck != null && p.Team != _lastPlayerKickedPuck.Team)
        {
            _kickTimer = _kickTime;
        }

        _lastPlayerKickedPuck = p;
        _rb.AddForce(direction.normalized * force);
        return true;
    }

    public void Trap()
    {
        _rb.velocity = Vector2.zero;
    }

    public Vector2 GetPosition()
    {
        return _rb.position;
    }

    public Player GetLastPlayerKickedPuck()
    {
        return _lastPlayerKickedPuck;
    }

    public MovingAgent GetPuckMovingAgent()
    {
        return _movingAgent;
    }

    public void PlaceAtPosition(Vector2 pos)
    {
        _rb.velocity = Vector2.zero;
        transform.position = pos;
    }
}