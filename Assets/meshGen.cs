using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class meshGen : MonoBehaviour
{
    int[] tris;
    Vector3[] verts;
    Vector2[] uvs;
    void Start()
    {
        MeshRenderer obj;
        Mesh mesh = new Mesh();
        obj = GetComponent<MeshRenderer>();
        MeshFilter mshflt = GetComponent<MeshFilter>();
        verts = new Vector3[8]{
            new Vector3(0,0,0), // 0
            new Vector3(1,0,0), // 1
            new Vector3(1,1,0), // 2
            new Vector3(0,1,0), // 3
            new Vector3(0,0,1), // 4
            new Vector3(1,0,1), // 5
            new Vector3(1,1,1), // 6
            new Vector3(0,1,1)  // 7
        };
        mesh.vertices = verts;  
        tris = new int[36] {
            0,3,1,
            1,3,2,

            4,7,5,
            5,7,6,

            0,4,1,
            1,4,6,
            
            1,5,2,
            2,5,6,

            4,7,3,
            0,7,2,

            1,2,6,
            5,2,6
        };
        mesh.triangles = tris;
        mesh.RecalculateNormals();
        uvs = new Vector2[8]{
            new Vector2(0,0),
            new Vector2(1,0),
            new Vector2(1,1),
            new Vector2(1,0),
            new Vector2(0,0),
            new Vector2(1,0),
            new Vector2(1,1),
            new Vector2(1,0)
        };
        mesh.uv = uvs;
        mshflt.mesh = mesh;
    }
}
