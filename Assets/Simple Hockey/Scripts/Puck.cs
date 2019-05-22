using UnityEngine;

public class Puck : MonoBehaviour
{
    private Player _lastPlayerKickedPuck;
    private Rigidbody2D _rb;
    private float _debugKickForce;
    private MovingAgent _movingAgent;

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
	}

    public void Kick(Vector2 direction, float force, Player p)
    {
        _lastPlayerKickedPuck = p;
        _rb.AddForce(direction.normalized * force);
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