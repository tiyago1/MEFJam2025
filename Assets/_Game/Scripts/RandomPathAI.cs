using UnityEngine;
using Pathfinding;

public class RandomPathAI : MonoBehaviour
{
    public int searchLength = 500;
    public int spread = 1000;
    private IAstarAI ai;

    void Start()
    {
        ai = GetComponent<IAstarAI>();
    }

    void Update()
    {
        if (!ai.pathPending && (ai.reachedEndOfPath || !ai.hasPath))
        {
            ai.canSearch = false;
            RandomPath path = RandomPath.Construct(transform.position, searchLength);
            path.spread = spread;
            ai.SetPath(path);
        }
    }
}