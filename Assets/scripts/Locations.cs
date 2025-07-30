using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Locations : MonoBehaviour
{
    public class Edge
    {
        public GameObject g1, g2;

        public Edge(string g1Name, string g2Name)
        {
            this.g1 = GameObject.Find(g1Name);
            this.g2 = GameObject.Find(g2Name);
        }
    }

    public List<GameObject> navigationNodes = new List<GameObject>();
    public List<GameObject> locationNodes = new List<GameObject>();
    public List<Edge> connectedNodes = new List<Edge>();
    public List<GameObject> currentPath;

    private Dictionary<GameObject, List<(GameObject node, float distance)>> adjacency;
    private GameObject stripObject;
    [TextArea(10, 20)]
    public string connectionsData;
    private void Start()
    {
        GameObject[] locations = GameObject.FindGameObjectsWithTag("Navigate");
        locationNodes.AddRange(locations);
        navigationNodes.AddRange(locations);
        navigationNodes.AddRange(GameObject.FindGameObjectsWithTag("NavigateOnly"));
        stripObject = new GameObject("BlueStripPolyline");

        ParseConnectionsFromString();
        BuildAdjacency();
    }

    private void Update()
    {
        RawImage arrowRawImage = GameObject.Find("NavArrowRenderedImage").GetComponent<RawImage>();
        if (currentPath != null && currentPath.Count > 0)
        {
            arrowRawImage.enabled = true;

            GameObject player = GameObject.Find("pla");
            float curDist = Vector3.Distance(currentPath[0].transform.position, player.transform.position);
            bool toRemoveFirst = false;
            if (curDist < 60)
            {
                toRemoveFirst = true;
            }

            if (currentPath.Count >= 2)
            {
                // basically, if the first node in the path would make u go the opposite way, we will remove it, beacuse it means you crossed it.
                Vector3 playerPos = player.transform.position;
                Vector3 node0Pos = currentPath[0].transform.position;
                Vector3 node1Pos = currentPath[1].transform.position;
                Vector3 directionToNode0 = Vector3.Normalize(node0Pos - playerPos);
                Vector3 directionToNode1 = Vector3.Normalize(node1Pos - playerPos);
                if ((Vector3.Dot(directionToNode0, directionToNode1) <= 0))
                {
                    toRemoveFirst = true;
                }
            }

            if (toRemoveFirst)
            {
                currentPath.RemoveAt(0);
            }

            List<GameObject> pathToDraw = new List<GameObject>(currentPath);
            pathToDraw.Insert(0, player);
            CreateBlueStripPolyline(pathToDraw, 20);
        }
        else if (stripObject != null)
        {
            arrowRawImage.enabled = false;
            Destroy(stripObject);
        }
    }

    public void TeleportPlayerTo(string src)
    {
        GameObject player = GameObject.Find("pla");
        GameObject toTpTo = GameObject.Find(src);
        if (toTpTo != null)
        {
            Vector3 tpPos = toTpTo.transform.position;
            CharacterController chari = player.GetComponent<CharacterController>();
            chari.enabled = false;
            player.transform.position = new Vector3(tpPos.x, 43.1F, tpPos.z);
            chari.enabled = true;
        }
    }

    public float GetArrowRot()
    {
        if (currentPath != null && currentPath.Count > 0)
        {
            GameObject player = GameObject.Find("pla");
            Vector3 playerLook = Quaternion.AngleAxis(player.transform.eulerAngles.y, Vector3.up) * new Vector3(0, 0, -1);
            Vector3 deltaDir = player.transform.position - currentPath[0].transform.position;
            float lookAngle = Mathf.Atan2(playerLook.x, playerLook.z), deltaAngle = Mathf.Atan2(deltaDir.x, deltaDir.z);
            return (deltaAngle - lookAngle) * Mathf.Rad2Deg;
        }

        return 0F;
    }

    public void CreatePath(string src, string dst)
    {
        GameObject startNode = GameObject.Find(src);
        GameObject player = GameObject.Find("pla");

        if (startNode == null)
        {
            float dist = float.MaxValue;

            foreach (GameObject g in navigationNodes)
            {
                float newDist = Vector3.Distance(g.transform.position, player.transform.position);
                if (newDist < dist)
                {
                    startNode = g;
                    dist = newDist;
                }
            }
        }
        GameObject endNode = GameObject.Find(dst);

        currentPath = FindShortestPath(startNode, endNode);

        if (currentPath == null)
        {
            Debug.Log("No path found.");
        }
    }

    private void ParseConnectionsFromString()
    {
        if (string.IsNullOrEmpty(connectionsData))
        {
            Debug.LogWarning("Connections data string is empty!");
            return;
        }

        string[] connections = connectionsData.Split(';');
        foreach (string connection in connections)
        {
            string[] nodes = connection.Split(',');
            if (nodes.Length == 2)
            {
                string node1 = nodes[0].Trim();
                string node2 = nodes[1].Trim();

                if (!string.IsNullOrEmpty(node1) && !string.IsNullOrEmpty(node2))
                {
                    connectedNodes.Add(new Edge(node1, node2));
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        foreach (Edge edge in connectedNodes)
        {
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

    // Auto genned functions below

    private void BuildAdjacency()
    {
        adjacency = new Dictionary<GameObject, List<(GameObject node, float distance)>>();

        foreach (Edge edge in connectedNodes)
        {
            GameObject g1 = edge.g1;
            GameObject g2 = edge.g2;

            if (g1 == null || g2 == null)
            {
                Debug.LogWarning("Edge has null GameObject(s). Skipping.");
                continue;
            }

            float dist = Vector3.Distance(g1.transform.position, g2.transform.position);

            // add to adj list 1
            if (!adjacency.ContainsKey(g1))
            {
                adjacency[g1] = new List<(GameObject, float)>();
            }
            if (!adjacency[g1].Contains((g2, dist)))
            {
                adjacency[g1].Add((g2, dist));
            }

            // add to adj list 2
            if (!adjacency.ContainsKey(g2))
            {
                adjacency[g2] = new List<(GameObject, float)>();
            }
            if (!adjacency[g2].Contains((g1, dist)))
            {
                adjacency[g2].Add((g1, dist));
            }
        }
    }

    public List<GameObject> FindShortestPath(GameObject start, GameObject end)
    {
        if (adjacency == null)
        {
            Debug.LogError("Adjacency list not built. Call BuildAdjacency first.");
            return null;
        }

        if (!adjacency.ContainsKey(start) || !adjacency.ContainsKey(end))
        {
            Debug.LogError("Start or end node not present in the graph.");
            return null;
        }

        Dictionary<GameObject, float> distances = new Dictionary<GameObject, float>();
        Dictionary<GameObject, GameObject> previous = new Dictionary<GameObject, GameObject>();
        List<GameObject> nodes = new List<GameObject>(adjacency.Keys);

        foreach (GameObject node in nodes)
        {
            distances[node] = Mathf.Infinity;
            previous[node] = null;
        }
        distances[start] = 0f;

        List<GameObject> queue = new List<GameObject>(nodes);

        while (queue.Count > 0)
        {
            queue.Sort((a, b) => distances[a].CompareTo(distances[b]));
            GameObject current = queue[0];
            queue.RemoveAt(0);

            if (current == end) break;

            if (distances[current] == Mathf.Infinity) break;

            foreach (var neighborPair in adjacency[current])
            {
                GameObject neighbor = neighborPair.node;
                float alt = distances[current] + neighborPair.distance;

                if (alt < distances[neighbor])
                {
                    distances[neighbor] = alt;
                    previous[neighbor] = current;
                }
            }
        }

        List<GameObject> path = new List<GameObject>();
        GameObject temp = end;

        while (temp != null)
        {
            path.Add(temp);
            temp = previous[temp];
        }

        path.Reverse();

        if (path.Count == 0 || path[0] != start)
        {
            return null;
        }

        return path;
    }

    public void CreateBlueStripPolyline(List<GameObject> points, float width)
    {
        if (points == null || points.Count < 2)
        {
            return;
        }

        float halfWidth = width / 2f;

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        for (int i = 0; i < points.Count; i++)
        {
            Vector3 p0 = new Vector3(), p2 = new Vector3();
            if (i != 0)
            {
                p0 = points[i - 1].transform.position;
                p0 = new Vector3(p0.x, 0, p0.z);
            }
            if (i != points.Count - 1)
            {
                p2 = points[i + 1].transform.position;
                p2 = new Vector3(p2.x, 0, p2.z);
            }

            Vector3 p1 = points[i].transform.position;
            p1 = new Vector3(p1.x, 0, p1.z);

            Vector3 dir;
            if (i == 0)
            {
                dir = (p2 - p1).normalized;
            }
            else if (i == points.Count - 1)
            {
                dir = (p1 - p0).normalized;
            }
            else
            {
                Vector3 dirPrev = (p1 - p0).normalized;
                Vector3 dirNext = (p2 - p1).normalized;
                dir = (dirPrev + dirNext).normalized;
                if (dir == Vector3.zero)
                    dir = dirPrev;
            }

            Vector3 perp = Vector3.Cross(dir, Vector3.up).normalized;
            vertices.Add(p1 - perp * halfWidth + new Vector3(0, 10, 0));
            vertices.Add(p1 + perp * halfWidth + new Vector3(0, 10, 0));

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

        Destroy(stripObject);
        stripObject = new GameObject("BlueStripPolyline");
        stripObject.transform.SetParent(transform, false);

        MeshFilter mf = stripObject.AddComponent<MeshFilter>();
        MeshRenderer mr = stripObject.AddComponent<MeshRenderer>();

        mf.mesh = mesh;

        Material blueMat = new Material(Shader.Find("Standard"));
        blueMat.color = Color.blue;
        mr.material = blueMat;
    }
}
