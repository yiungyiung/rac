using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Barrier : MonoBehaviour
{
    [Header("Control points")]
    
    public Point[] points;
    [Header("Barrier settings")]
    [Tooltip("Height of barrier  (Default 1)")]
    public float height = 1;
    [Tooltip("Number of subdivisions between two points. Higher value equals smoother mesh but more vertex count.")]
    [Min(1)]
    public int subdivisions = 5;
    [Tooltip("Speed of arrow  (Default 1)")]
    public float arrowSpeed = 1;
    [Tooltip("Arrow texture tiling (Default 1)")]
    public float tiling = 1;
    [Tooltip("Barrier material")]
    public Material mat;

    float[] vertDistanceFromStart;
    float offset = 0f;
    
    [Serializable]
    public struct Point
    {
        public Transform pos;
        public float weight;
    }

    void Start()
    {
        var mf = gameObject.AddComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        mf.mesh = mesh;
        var ren = gameObject.AddComponent<MeshRenderer>();
        ren.material = mat;
       gameObject.AddComponent<MeshCollider>();
    }

    private void OnDrawGizmos()
    {
        foreach (var p in points)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(p.pos.position, 0.1f);
            Gizmos.color = Color.white;
            Gizmos.DrawLine(p.pos.position, p.pos.position + p.pos.rotation * Vector3.forward * p.weight);
            Gizmos.DrawLine(p.pos.position, p.pos.position + p.pos.rotation * -Vector3.forward * p.weight);
            Gizmos.color =Color.red;
            Gizmos.DrawWireSphere(p.pos.position + p.pos.rotation * Vector3.forward * p.weight, 0.1f);
        }
    }

    //Returns all vertices of the barrier
    Vector3[] GetVerts()
    {
        Vector3[] verts = new Vector3[2 * ((points.Length - 1) * subdivisions + 1)];
        vertDistanceFromStart = new float[verts.Length / 2];
        int index = 0;
        for (int i = 0; i < points.Length - 1; i++)
        {
            Vector3 p1 = points[i].pos.position + points[i].pos.rotation * Vector3.forward * points[i].weight;
            Vector3 p2 = points[i + 1].pos.position + points[i+1].pos.rotation * -Vector3.forward * points[i + 1].weight;


            for (int j = 0; j < subdivisions; j++)
            {
                float t = j / (float)subdivisions;
                //B(t) = (1 - t)3P0 + 3(1 - t)2tP1 + 3(1 - t)t2P2 + t3P3 , 0 < t < 1
                Vector3 B = Mathf.Pow(1 - t, 3) * points[i].pos.position + 3 * Mathf.Pow(1 - t, 2) * t * p1 + 3 * (1 - t) * t * t * p2 + t * t * t * points[i + 1].pos.position;
                
                verts[index] = B;
                if(index > 0)
                    vertDistanceFromStart[index] = vertDistanceFromStart[index-1] + Vector3.Distance(verts[index], verts[index-1]);
                index++;
            }
        }
       
        verts[index] = points[points.Length-1].pos.position;
        vertDistanceFromStart[index] = vertDistanceFromStart[index - 1] + Vector3.Distance(verts[index], verts[index - 1]);
        index++;
        // now place copy
        for (; index < verts.Length; ++index)
        {
            verts[index] = verts[index - verts.Length / 2] + Vector3.up * height;
        }

        foreach (var v in verts)
        {
            Debug.Log(v);
        }
        return verts;
    }

    // Update is called once per frame
    void Update()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        MeshCollider col = GetComponent<MeshCollider>();
        var verts = GetVerts();

       

        Vector2[] uvs = new Vector2[verts.Length];
        //generate triangles
        int[] tris = new int[6 * (verts.Length / 2 - 1)];
        for (int i = 0; i < verts.Length / 2 - 1; i++)
        {
            tris[6 * i + 0] = i;
            tris[6 * i + 1] = i + 1;
            tris[6 * i + 2] = i + verts.Length / 2;
            tris[6 * i + 3] = i + 1;
            tris[6 * i + 4] = i + verts.Length / 2 + 1;
            tris[6 * i + 5] = i + verts.Length / 2;
           
        }
        mesh.triangles = tris;

        //set UVs and adjust vertices for position
        for (int i = 0; i < verts.Length; i++)
        {
            uvs[i] = new Vector2(offset + vertDistanceFromStart[i%(verts.Length/2)] * tiling / height,i < verts.Length / 2 ? 0 : 1);
            verts[i] = transform.InverseTransformPoint(verts[i]);
        }
        mesh.vertices = verts;
        mesh.uv = uvs;

        //arrow motion
        offset += Time.deltaTime * -arrowSpeed;
        offset %= vertDistanceFromStart[points.Length - 1];

        //update normals
        mesh.RecalculateTangents();
        mesh.RecalculateNormals();
        
        //update mesh collider mesh
        col.sharedMesh = mesh;
    }
}
