using UnityEngine;
using System.Collections.Generic;

public class PuzzleManager : MonoBehaviour
{
    public NodePiece startNode;
    public NodePiece endNode;
    public float checkDistance = 1.15f;

    private int slotLayer;

    void Start()
    {
        slotLayer = LayerMask.GetMask("Slot");
        Debug.Log("Slot layer mask: " + slotLayer);

        // Refresh all PipePieces at start in case pipes are pre-placed
        PipePiece[] allPipes = FindObjectsOfType<PipePiece>();
        foreach (PipePiece pipe in allPipes)
            pipe.RefreshAnimator();

        // Log all slots and their layers
        PuzzlePiece[] all = FindObjectsOfType<PuzzlePiece>();
        Debug.Log("Total PuzzlePieces: " + all.Length);
        foreach (PuzzlePiece p in all)
            Debug.Log(p.gameObject.name + " layer: " + LayerMask.LayerToName(p.gameObject.layer));

        // Log start node neighbors
        Debug.Log("Start pos: " + startNode.transform.position);
        DebugNeighbors(startNode.transform.position);
    }

    void DebugNeighbors(Vector2 origin)
    {
        Vector2[] dirs = { Vector2.right, Vector2.left, Vector2.up, Vector2.down };
        string[] names = { "right", "left", "up", "down" };

        for (int i = 0; i < dirs.Length; i++)
        {
            Vector2 searchCenter = origin + dirs[i] * checkDistance;
            Collider2D[] hits = Physics2D.OverlapCircleAll(searchCenter, 0.4f, slotLayer);
            Debug.Log("Searching " + names[i] + " at " + searchCenter + " found: " + hits.Length + " colliders");
            foreach (Collider2D h in hits)
                Debug.Log("  -> " + h.gameObject.name + " layer: " + LayerMask.LayerToName(h.gameObject.layer));
        }
    }

    public void ActivateSystem()
    {
        List<object> connectedPath = GetConnectedPath();

        Debug.Log("Path length: " + connectedPath.Count);
        foreach (object p in connectedPath)
            Debug.Log("  " + GetPieceName(p));

        // Turn off all pipes
        PipePiece[] allPipes = FindObjectsOfType<PipePiece>();
        foreach (PipePiece pipe in allPipes)
            pipe.SetActive(false);

        startNode.SetActive(false);
        endNode.SetActive(false);

        // Light up connected path
        foreach (object piece in connectedPath)
        {
            if (piece is PuzzlePiece)
            {
                PipePiece pipe = ((PuzzlePiece)piece).GetComponent<PipePiece>();
                if (pipe != null)
                    pipe.SetActive(true);
            }
            else if (piece is NodePiece)
            {
                ((NodePiece)piece).SetActive(true);
            }
        }

        bool solved = connectedPath.Count > 0 &&
                      connectedPath[connectedPath.Count - 1] is NodePiece &&
                      (NodePiece)connectedPath[connectedPath.Count - 1] == endNode;

        if (solved)
        {
            Debug.Log("Puzzle Solved!");
            FindObjectOfType<PuzzleComplete>().CompletePuzzle();
        }
        else
        {
            Debug.Log("Path incomplete - last: " +
                (connectedPath.Count > 0 ? GetPieceName(connectedPath[connectedPath.Count - 1]) : "NONE"));
        }
    }

    string GetPieceName(object piece)
    {
        if (piece is PuzzlePiece) return ((PuzzlePiece)piece).gameObject.name;
        if (piece is NodePiece) return ((NodePiece)piece).gameObject.name;
        return "unknown";
    }

    List<object> GetConnectedPath()
    {
        List<object> path = new List<object>();
        object current = startNode;
        object previous = null;
        int safety = 0;

        while (current != null && safety < 50)
        {
            path.Add(current);
            if (current is NodePiece && (NodePiece)current == endNode)
                break;

            object next = GetNextPiece(current, previous);
            previous = current;
            current = next;
            safety++;
        }

        return path;
    }

    object GetNextPiece(object current, object previous)
    {
        Vector2 pos = GetPosition(current);

        if (GetRight(current))
        {
            object n = GetNeighborObject(pos, Vector2.right);
            if (n != null && GetLeft(n) && n != previous) return n;
        }
        if (GetLeft(current))
        {
            object n = GetNeighborObject(pos, Vector2.left);
            if (n != null && GetRight(n) && n != previous) return n;
        }
        if (GetUp(current))
        {
            object n = GetNeighborObject(pos, Vector2.up);
            if (n != null && GetDown(n) && n != previous) return n;
        }
        if (GetDown(current))
        {
            object n = GetNeighborObject(pos, Vector2.down);
            if (n != null && GetUp(n) && n != previous) return n;
        }

        return null;
    }

    Vector2 GetPosition(object piece)
    {
        if (piece is PuzzlePiece) return ((PuzzlePiece)piece).transform.position;
        if (piece is NodePiece) return ((NodePiece)piece).transform.position;
        return Vector2.zero;
    }

    bool GetUp(object piece)
    {
        if (piece is PuzzlePiece) return ((PuzzlePiece)piece).up;
        if (piece is NodePiece) return ((NodePiece)piece).up;
        return false;
    }

    bool GetDown(object piece)
    {
        if (piece is PuzzlePiece) return ((PuzzlePiece)piece).down;
        if (piece is NodePiece) return ((NodePiece)piece).down;
        return false;
    }

    bool GetLeft(object piece)
    {
        if (piece is PuzzlePiece) return ((PuzzlePiece)piece).left;
        if (piece is NodePiece) return ((NodePiece)piece).left;
        return false;
    }

    bool GetRight(object piece)
    {
        if (piece is PuzzlePiece) return ((PuzzlePiece)piece).right;
        if (piece is NodePiece) return ((NodePiece)piece).right;
        return false;
    }

    object GetNeighborObject(Vector2 origin, Vector2 dir)
    {
        // Cast further and use larger radius to handle uneven spacing
        float maxSearchDistance = 3f;
        float stepSize = 0.1f;
        float searchRadius = 0.35f;

        object closest = null;
        float closestDist = float.MaxValue;

        // Step along the direction looking for the first hit
        for (float dist = 0.3f; dist <= maxSearchDistance; dist += stepSize)
        {
            Vector2 searchCenter = origin + dir * dist;
            Collider2D[] hits = Physics2D.OverlapCircleAll(searchCenter, searchRadius, slotLayer);

            foreach (Collider2D hit in hits)
            {
                // Skip the origin piece itself
                float distToHit = Vector2.Distance(origin, hit.transform.position);
                if (distToHit < 0.3f) continue;

                // Make sure it's in the right direction
                Vector2 toHit = (Vector2)hit.transform.position - origin;
                float dot = Vector2.Dot(toHit.normalized, dir);
                if (dot < 0.7f) continue;

                if (distToHit < closestDist)
                {
                    PuzzlePiece pp = hit.GetComponent<PuzzlePiece>();
                    if (pp != null) { closest = pp; closestDist = distToHit; continue; }

                    NodePiece np = hit.GetComponent<NodePiece>();
                    if (np != null) { closest = np; closestDist = distToHit; }
                }
            }

            // Stop at first hit found
            if (closest != null) break;
        }

        if (closest != null)
            Debug.Log("Found neighbor: " + GetPieceName(closest) + " at dist: " + closestDist);
        else
            Debug.Log("No neighbor found from " + origin + " in dir " + dir);

        return closest;
    }
}