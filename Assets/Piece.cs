using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    [SerializeField]
    public int[] tris;

    Triangle[] triangles;
    
    void Start()
    {
        triangles = GetComponentsInChildren<Triangle>();

        SetTriangles();
    }

    void SetTriangles()
    {
        foreach (int i in tris)
        {
            triangles[i][0] = true;
        }
    }

    void OnValidate()
    {
        if (triangles == null || triangles.Length == 0)
            return;

        foreach (var triangle in triangles)
        {
            triangle[0] = false;
        }

        SetTriangles();
    }
}
