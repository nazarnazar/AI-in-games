using UnityEngine;

public enum PitchSide
{
    Left,
    Right
}

public class Goal : MonoBehaviour
{
    [SerializeField] private Puck _ball;
    [SerializeField] private PitchSide _side;

    private Vector2 _leftPost;
    private Vector2 _rightPost;
    private Vector2 _facing;
    private Vector2 _center;
    private System.Action _goalScored;

	private void Start()
    {
        Bounds goalBounds = GetComponent<SpriteRenderer>().bounds;
        switch (_side)
        {
            case PitchSide.Left:
                _leftPost = new Vector2(goalBounds.center.x, goalBounds.min.y);
                _rightPost = new Vector2(goalBounds.center.x, goalBounds.max.y);
                _facing = new Vector2(1.0f, 0.0f);
                break;
            case PitchSide.Right:
                _leftPost = new Vector2(goalBounds.center.x, goalBounds.max.y);
                _rightPost = new Vector2(goalBounds.center.x, goalBounds.min.y);
                _facing = new Vector2(-1.0f, 0.0f);
                break;
        }
        _center = (_leftPost + _rightPost) / 2.0f;
    }

    public void SetGoalScoredAction(System.Action scored)
    {
        _goalScored = scored;
    }

    public Vector2 GetPosition()
    {
        return _center;
    }

    public Vector2 Facing()
    {
        return  _facing;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.name == "Puck" && _goalScored != null)
        {
            _goalScored();
        }
    }
}