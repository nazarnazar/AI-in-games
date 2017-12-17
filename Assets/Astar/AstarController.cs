using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstarController : MonoBehaviour
{
    public NodeSpawner nodeSpawner;

    private List<GameObject> nodes;
    private int width;
    private int height;
    private int startIndex;
    private int finishIndex;
    private float[] G;

    public void BeginAStar()
    {
        StartCoroutine(AStartInProcess());
    }

    IEnumerator AStartInProcess()
    {
        nodes = nodeSpawner.GetNodes();
        width = nodeSpawner.GetWidth();
        height = nodeSpawner.GetHeight();
        Debug.Log(width);

        startIndex = GetStartNode(nodes);
        finishIndex = GetFinishNode(nodes);

        if (startIndex == -1 || finishIndex == -1)
        {
            Debug.Log("No start or finish!");
            yield break;
        }

        int size = nodes.Count;
        int[,] AdjMatrix = new int[size, size];
        for (int i = 0; i < height; i++)
            for (int j = 0; j < width; j++)
            {
                if (i != 0)
                    AdjMatrix[i * width + j, (i - 1) * width + j] = 1;
                if (i != height - 1)
                    AdjMatrix[i * width + j, (i + 1) * width + j] = 1;
                if (j != 0)
                    AdjMatrix[i * width + j, i * width + j - 1] = 1;
                if (j != width - 1)
                    AdjMatrix[i * width + j, i * width + j + 1] = 1;
            }

        List<int> closed = new List<int>();
        List<int> open = new List<int>();
        open.Add(startIndex);

        int[] from = new int[size];

        G = new float[size];
        float[] F = new float[size];
        for (int i = 0; i < size; i++)
            G[i] = float.MaxValue;
        for (int i = 0; i < size; i++)
            F[i] = float.MaxValue;

        G[startIndex] = 0f;
        F[startIndex] = G[startIndex] + FuncH(startIndex, finishIndex);

        while (open.Count > 0)
        {
            int curr = MinF(open);
            if (curr == finishIndex)
            {
                Debug.Log("Success");
                PaintFrom(from);
                yield break;
            }
            open.Remove(curr);
            closed.Add(curr);
            List<int> neighbours = new List<int>();
            for (int j = 0; j < size; j++)
                if (AdjMatrix[curr, j] != 0 && nodes[j].GetComponent<SpriteRenderer>().color != Color.blue && !closed.Contains(j))
                    neighbours.Add(j);
            for (int j = 0; j < neighbours.Count; j++)
            {
                if (!open.Contains(neighbours[j]))
                    open.Add(neighbours[j]);
                float tempG = G[curr] + AdjMatrix[curr, neighbours[j]];
                if (tempG < G[neighbours[j]])
                {
                    from[neighbours[j]] = curr;
                    G[neighbours[j]] = tempG;
                    F[neighbours[j]] = G[neighbours[j]] + FuncH(neighbours[j], finishIndex);
                }
            }
            Paint(open, closed);
            yield return null;
        }
        Debug.Log("Failure");
    }

    void Paint(List<int> open, List<int> closed)
    {
        for (int i = 0; i < open.Count; i++)
            nodes[open[i]].GetComponent<SpriteRenderer>().color = Color.grey;
        for (int i = 0; i < closed.Count; i++)
            nodes[closed[i]].GetComponent<SpriteRenderer>().color = Color.red;
        nodes[startIndex].GetComponent<SpriteRenderer>().color = Color.white;
    }

    void PaintFrom(int[] from)
    {
        for (int i = finishIndex; from[i] != startIndex; i = from[i])
            nodes[from[i]].GetComponent<SpriteRenderer>().color = Color.yellow;
        nodes[startIndex].GetComponent<SpriteRenderer>().color = Color.white;
        nodes[finishIndex].GetComponent<SpriteRenderer>().color = Color.black;
    }

    int MinF(List<int> open)
    {
        float minValue = G[open[0]] + FuncH(open[0], finishIndex);
        int minIndex = open[0];
        for (int i = 0; i < open.Count; i++)
        {
            if (G[open[i]] + FuncH(open[i], finishIndex) < minValue)
            {
                minValue = G[open[i]] + FuncH(open[i], finishIndex);
                minIndex = open[i];
            }
        }
        return minIndex;
    }

    float FuncH(int a, int b)
    {
        return Vector3.Distance(nodes[a].transform.position, nodes[b].transform.position);
    }

    int GetStartNode(List<GameObject> nodes)
    {
        for (int i = 0; i < nodes.Count; i++)
            if (nodes[i].GetComponent<SpriteRenderer>().color == Color.white)
                return i;
        return -1;
    }

    int GetFinishNode(List<GameObject> nodes)
    {
        for (int i = 0; i < nodes.Count; i++)
            if (nodes[i].GetComponent<SpriteRenderer>().color == Color.black)
                return i;
        return -1;
    }

    public void Restart()
    {
        Application.LoadLevel(Application.loadedLevel);
    }
}
