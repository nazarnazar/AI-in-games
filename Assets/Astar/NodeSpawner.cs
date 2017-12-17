using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeSpawner : MonoBehaviour
{
    public GameObject node;

    private List<GameObject> nodes = new List<GameObject>();
    private int w;
    private int h;

    void Start()
    {
        Vector2 screenWorldSize = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        float width = screenWorldSize.x * 2f;
        float height = screenWorldSize.y * 2f;
        float nodeSize = node.GetComponent<SpriteRenderer>().bounds.size.x;
        int hPieces = Mathf.CeilToInt(height / nodeSize);
        int wPieces = Mathf.CeilToInt(width / nodeSize);
        w = wPieces;
        h = hPieces;
        for (int i = 0; i < hPieces; i++)
            for (int j = 0; j < wPieces; j++)
            {
                GameObject tempNode = Instantiate(node);
                tempNode.transform.position = new Vector3((-width / 2f + nodeSize / 2f) + (j * nodeSize), (height / 2f - nodeSize / 2f) - (i * nodeSize), 0f);
                if (i == 4 && j == 4)
                {
                    Destroy(tempNode.GetComponent<Collider2D>());
                    tempNode.GetComponent<SpriteRenderer>().color = Color.white;
                }
                else if (i == hPieces - 6 && j == wPieces - 6)
                {
                    Destroy(tempNode.GetComponent<Collider2D>());
                    tempNode.GetComponent<SpriteRenderer>().color = Color.black;
                }
                nodes.Add(tempNode);
            }
    }

    public List<GameObject> GetNodes()
    {
        return nodes;
    }

    public int GetWidth()
    {
        return w;
    }

    public int GetHeight()
    {
        return h;
    }
}
