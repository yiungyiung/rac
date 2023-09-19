using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.ProBuilder;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

public class exporter : MonoBehaviour
{

    public List<ProBuilderMesh> meshes;
    // Start is called before the first frame update
    void Start()
    {
        meshes = GetComponentsInChildren<ProBuilderMesh>().ToList<ProBuilderMesh>();
        var current = gameObject.AddComponent<ProBuilderMesh>();
        
        CombineMeshes.Combine(meshes);
       
    }

    // Update is called once per frame
    void Update()
    {
      
    }
}
