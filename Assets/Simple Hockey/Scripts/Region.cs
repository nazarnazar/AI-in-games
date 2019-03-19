using UnityEngine;

public class Region
{
    public Vector2 TopLeft { get; private set; }
    public Vector2 BottomRight { get; private set; }
    public Vector2 Center { get; private set; }

    public Region(SpriteRenderer renderer)
    {
        TopLeft = new Vector2(renderer.bounds.min.x, renderer.bounds.max.y);
        BottomRight = new Vector2(renderer.bounds.max.x, renderer.bounds.min.y);
        Center = new Vector2(renderer.bounds.center.x, renderer.bounds.center.y);
    }

    public Region(Vector2 topLeft, float width, float height)
    {
        TopLeft = topLeft;
        BottomRight = new Vector2(topLeft.x + width, topLeft.y - height);
        Center = (TopLeft + BottomRight) / 2.0f;
    }

    public Region(Vector2 topLeft, Vector2 bottomRight)
    {
        TopLeft = topLeft;
        BottomRight = bottomRight;
        Center = (TopLeft + BottomRight) / 2.0f;
    }

    public float GetWidth()
    {
        return Mathf.Abs(TopLeft.x - BottomRight.x);
    }

    public float GetHeight()
    {
        return Mathf.Abs(TopLeft.y - BottomRight.y);
    }
}