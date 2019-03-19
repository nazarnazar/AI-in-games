using UnityEngine;

public class SupportSpot : MonoBehaviour
{
    private float _score;

    public Vector2 Position
    {
        get
        {
            return transform.position;
        }
        set
        {
            transform.position = value;
        }
    }
    public float Score
    {
        get
        {
            return _score;
        }
        set
        {
            _score = value;
            transform.localScale = Vector2.one * value * 0.5f;
        }
    }
}