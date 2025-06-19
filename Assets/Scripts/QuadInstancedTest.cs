using UnityEngine;
using System.Collections.Generic;

public class QuadInstancedTest : MonoBehaviour
{
    public Material material;

    Mesh quadMesh;
    List<Matrix4x4> matrices;

    void Start()
    {
        // Create a quad mesh
        quadMesh = CreateQuadMesh();

        // Create matrices for instancing
        matrices = new List<Matrix4x4>();
        float quadScale = 10f; // Make each quad 10x bigger!
        float spacing = 12f; // Space them out more to avoid overlap

        for (int i = 0; i < 100; i++)
        {
            // Position them in a grid
            Vector3 pos = new Vector3((i % 10) * spacing, 0, (i / 10) * spacing);

            // Scale up each quad
            Matrix4x4 matrix = Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one * quadScale);
            matrices.Add(matrix);
        }
    }

    void Update()
    {
        if (material != null && quadMesh != null)
        {
            // Draw 100 instances
            Graphics.DrawMeshInstanced(quadMesh, 0, material, matrices);
        }
    }

    Mesh CreateQuadMesh()
    {
        var mesh = new Mesh();

        Vector3[] vertices = {
            new Vector3(-0.5f, -0.5f, 0),
            new Vector3( 0.5f, -0.5f, 0),
            new Vector3(-0.5f,  0.5f, 0),
            new Vector3( 0.5f,  0.5f, 0)
        };

        int[] triangles = { 0, 2, 1, 2, 3, 1 };

        Vector2[] uvs = {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        return mesh;
    }
}