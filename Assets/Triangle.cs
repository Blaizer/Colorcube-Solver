using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Triangle : MonoBehaviour
{
    bool[] colors = new bool[3];

    MeshRenderer renderer;

    void Awake()
    {
        renderer = GetComponent<MeshRenderer>();
        CreateMesh();
        UpdateColor();
    }

    void CreateMesh()
    {
        var collider = GetComponent<PolygonCollider2D>();
        var mesh = gameObject.AddComponent<MeshFilter>().sharedMesh = new Mesh();
        mesh.vertices = collider.points.Select(p => (Vector3)p).ToArray();
        mesh.triangles = new int[] { 0, 1, 2 };
        mesh.UploadMeshData(true);
    }

    public bool this[int index]
    {
        get
        {
            return colors[index];
        }
        set
        {
            colors[index] = value;
            UpdateColor();
        }
    }

    void UpdateColor()
    {
        if (colors[0] == colors[1] && colors[1] == colors[2])
        {
            renderer.material.color = Color.white;
        }
        else if (colors[0])
        {
            if (colors[1])
                renderer.material.color = Triangles.Instance.secondayColors[0];
            else if (colors[2])
                renderer.material.color = Triangles.Instance.secondayColors[1];
            else
                renderer.material.color = Triangles.Instance.primaryColors[0];
        }
        else if (colors[1])
        {
            if (colors[2])
                renderer.material.color = Triangles.Instance.secondayColors[2];
            else
                renderer.material.color = Triangles.Instance.primaryColors[1];
        }
        else
        {
            renderer.material.color = Triangles.Instance.primaryColors[2];
        }
    }
}
