using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locations : MonoBehaviour
{
    public class Edge {
        public GameObject g1, g2;

        public Edge(string g1Name, string g2Name) {
            this.g1 = GameObject.Find(g1Name);
            this.g2 = GameObject.Find(g2Name);
        }
    }

    public List<GameObject> navigationNodes = new List<GameObject>();
    public List<GameObject> locationNodes = new List<GameObject>();
    public List<Edge> connectedNodes = new List<Edge>();
    private Dictionary<GameObject, List<(GameObject node, float distance)>> adjacency;

    private void Start()
    {
        GameObject[] locations = GameObject.FindGameObjectsWithTag("Navigate");
        locationNodes.AddRange(locations);
        navigationNodes.AddRange(locations);
        navigationNodes.AddRange(GameObject.FindGameObjectsWithTag("NavigateOnly"));

        AddEdges();
        BuildAdjacency();
    }

    public void CreatePath(string src, string dst)
    {
        GameObject startNode = GameObject.Find(src);
        GameObject endNode = GameObject.Find(dst);

        List<GameObject> path = FindShortestPath(startNode, endNode);

        if (path != null)
        {
            CreateBlueStripPolyline(path, 30);
        }
        else
        {
            Debug.Log("No path found.");
        }
    }

    private void AddEdges()
    {
        connectedNodes.Add(new Edge("OSTL", "PPSL"));
        connectedNodes.Add(new Edge("PPSL", "GirlsCommonRoom"));
        connectedNodes.Add(new Edge("GirlsCommonRoom", "EWingDirector1"));
        connectedNodes.Add(new Edge("EWingDirector1", "EWingBalcony"));
        connectedNodes.Add(new Edge("EWingDirector1", "PoolTable"));
    }

    private void OnDrawGizmos() {
        foreach(Edge edge in connectedNodes) {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(
                edge.g1.transform.position + new Vector3(0, 10, 0), 
                edge.g2.transform.position + new Vector3(0, 10, 0)
            );

            Gizmos.color = edge.g1.tag == "Navigate" ? Color.yellow : Color.green;
            Gizmos.DrawSphere(edge.g1.transform.position + new Vector3(0, 10, 0), 5);
            Gizmos.color = edge.g2.tag == "Navigate" ? Color.yellow : Color.green;
            Gizmos.DrawSphere(edge.g2.transform.position + new Vector3(0, 10, 0), 5);
        }
    }

    private void BuildAdjacency() {
        adjacency = new Dictionary<GameObject, List<(GameObject node, float distance)>>();

        foreach (Edge edge in connectedNodes) {
            GameObject g1 = edge.g1;
            GameObject g2 = edge.g2;

            if (g1 == null || g2 == null) {
                Debug.LogWarning("Edge has null GameObject(s). Skipping.");
                continue;
            }

            float dist = Vector3.Distance(g1.transform.position, g2.transform.position);

            if (!adjacency.ContainsKey(g1)) {
                adjacency[g1] = new List<(GameObject, float)>();
            }
            adjacency[g1].Add((g2, dist));

            if (!adjacency.ContainsKey(g2)) {
                adjacency[g2] = new List<(GameObject, float)>();
            }
            adjacency[g2].Add((g1, dist));
        }
    }
    
    public List<GameObject> FindShortestPath(GameObject start, GameObject end) {
        if (adjacency == null) {
            Debug.LogError("Adjacency list not built. Call BuildAdjacency first.");
            return null;
        }

        if (!adjacency.ContainsKey(start) || !adjacency.ContainsKey(end)) {
            Debug.LogError("Start or end node not present in the graph.");
            return null;
        }

        Dictionary<GameObject, float> distances = new Dictionary<GameObject, float>();
        Dictionary<GameObject, GameObject> previous = new Dictionary<GameObject, GameObject>();
        List<GameObject> nodes = new List<GameObject>(adjacency.Keys);

        foreach (GameObject node in nodes) {
            distances[node] = Mathf.Infinity;
            previous[node] = null;
        }
        distances[start] = 0f;

        List<GameObject> queue = new List<GameObject>(nodes);

        while (queue.Count > 0) {
            queue.Sort((a, b) => distances[a].CompareTo(distances[b]));
            GameObject current = queue[0];
            queue.RemoveAt(0);

            if (current == end) break;

            if (distances[current] == Mathf.Infinity) break;

            foreach (var neighborPair in adjacency[current]) {
                GameObject neighbor = neighborPair.node;
                float alt = distances[current] + neighborPair.distance;

                if (alt < distances[neighbor]) {
                    distances[neighbor] = alt;
                    previous[neighbor] = current;
                }
            }
        }

        List<GameObject> path = new List<GameObject>();
        GameObject temp = end;

        while (temp != null) {
            path.Add(temp);
            temp = previous[temp];
        }

        path.Reverse();

        if (path.Count == 0 || path[0] != start) {
            return null;
        }

        return path;
    }

    public void CreateBlueStripPolyline(List<GameObject> points, float width)
    {
        if (points == null || points.Count < 2)
        {
            Debug.LogError("Need at least two points to create a strip.");
            return;
        }

        float halfWidth = width / 2f;

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        for (int i = 0; i < points.Count; i++)
        {
            Vector3 dir;
            if (i == 0)
            {
                dir = (points[i + 1].transform.position - points[i].transform.position).normalized;
            }
            else if (i == points.Count - 1)
            {
                dir = (points[i].transform.position - points[i - 1].transform.position).normalized;
            }
            else
            {
                Vector3 dirPrev = (points[i].transform.position - points[i - 1].transform.position).normalized;
                Vector3 dirNext = (points[i + 1].transform.position - points[i].transform.position).normalized;
                dir = (dirPrev + dirNext).normalized;
                if (dir == Vector3.zero)
                    dir = dirPrev;
            }

            Vector3 perp = Vector3.Cross(dir, Vector3.up).normalized;
            vertices.Add(points[i].transform.position - perp * halfWidth + new Vector3(0, 10, 0));
            vertices.Add(points[i].transform.position + perp * halfWidth + new Vector3(0, 10, 0));

            float uCoord = (float)i / (points.Count - 1);
            uvs.Add(new Vector2(uCoord, 0));
            uvs.Add(new Vector2(uCoord, 1));
        }

        for (int i = 0; i < points.Count - 1; i++)
        {
            int index = i * 2;
            triangles.Add(index);
            triangles.Add(index + 1);
            triangles.Add(index + 3);

            triangles.Add(index);
            triangles.Add(index + 3);
            triangles.Add(index + 2);
        }

        Mesh mesh = new Mesh();
        mesh.name = "BlueStripPolylineMesh";
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();

        GameObject stripObject = new GameObject("BlueStripPolyline");
        stripObject.transform.SetParent(transform, false);

        MeshFilter mf = stripObject.AddComponent<MeshFilter>();
        MeshRenderer mr = stripObject.AddComponent<MeshRenderer>();

        mf.mesh = mesh;

        Material blueMat = new Material(Shader.Find("Standard"));
        blueMat.color = Color.blue;
        mr.material = blueMat;
    }
}
