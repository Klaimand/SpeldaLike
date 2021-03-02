using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class KLD_PlaneGenerator : SerializedMonoBehaviour
{

    [SerializeField] Vector2Int planeSquares = new Vector2Int(30, 30);
    [SerializeField] Vector2 squareSize = new Vector2(1f, 1f);

    [SerializeField] Material material;

    [SerializeField] MeshFilter meshNormalsToDraw;

    private void Update()
    {
        for (int i = 0; i < meshNormalsToDraw.mesh.normals.GetLength(0); i++)
        {
            Debug.DrawRay(meshNormalsToDraw.mesh.vertices[i], meshNormalsToDraw.mesh.normals[i]);
        }
    }

    [Button]
    public void CreatePlane()
    {

        GameObject curGO = new GameObject("newPolyplane");
        MeshFilter meshFilter = curGO.AddComponent<MeshFilter>();
        curGO.AddComponent<MeshRenderer>().material = material;

        Mesh mesh;
        mesh = new Mesh();
        meshFilter.mesh = mesh;
        mesh.Clear();

        mesh.vertices = GenerateVertices();
        mesh.triangles = GenerateTriangles();

        mesh.RecalculateNormals();
        //mesh.Optimize();
    }

    [Button]
    public void CountVertice(MeshFilter _meshFilter)
    {
        print(_meshFilter.gameObject.name + "has a mesh that has " + _meshFilter.mesh.vertices.GetLength(0) + " vertices");
    }

    Vector3[] GenerateVertices()
    {
        Vector3[] verticesInst = new Vector3[planeSquares.x * planeSquares.y * 6];

        for (int y = 0; y < planeSquares.y; y++)
        {
            for (int x = 0; x < planeSquares.x; x++)
            {
                int cornerIndex = ((y * planeSquares.x) + x) * 6;
                Vector3 cornerPosition = new Vector3(x * squareSize.x, 0f, y * squareSize.y);
                if (x == 0 && y == 0)
                {
                    //cornerPosition = new Vector3(cornerPosition.x, 1f, cornerPosition.z);
                }

                //verticesInst[cornerIndex] = cornerPosition;
                //verticesInst[cornerIndex + 1] = cornerPosition + Vector3.forward * squareSize.y;
                //verticesInst[cornerIndex + 2] = cornerPosition + Vector3.forward * squareSize.y + Vector3.right * squareSize.x;
                //verticesInst[cornerIndex + 3] = cornerPosition + Vector3.right * squareSize.x;
                //verticesInst[cornerIndex + 4] = cornerPosition;
                //verticesInst[cornerIndex + 5] = cornerPosition + Vector3.forward * squareSize.y + Vector3.right * squareSize.x;

                //verticesInst[cornerIndex] = cornerPosition + Vector3.up * Random.value;
                //verticesInst[cornerIndex + 1] = cornerPosition + Vector3.forward * squareSize.y + Vector3.up * Random.value;
                //verticesInst[cornerIndex + 2] = cornerPosition + Vector3.forward * squareSize.y + Vector3.right * squareSize.x + Vector3.up * Random.value;
                //verticesInst[cornerIndex + 3] = cornerPosition + Vector3.right * squareSize.x + Vector3.up * Random.value;
                //verticesInst[cornerIndex + 4] = cornerPosition + Vector3.up * Random.value;
                //verticesInst[cornerIndex + 5] = cornerPosition + Vector3.forward * squareSize.y + Vector3.right * squareSize.x + Vector3.up * Random.value;

                verticesInst[cornerIndex] = cornerPosition;
                verticesInst[cornerIndex + 1] = cornerPosition + Vector3.forward * squareSize.y;
                verticesInst[cornerIndex + 2] = cornerPosition + Vector3.forward * squareSize.y + Vector3.right * squareSize.x;
                verticesInst[cornerIndex + 3] = cornerPosition + Vector3.right * squareSize.x;
                verticesInst[cornerIndex + 4] = cornerPosition;
                verticesInst[cornerIndex + 5] = cornerPosition + Vector3.forward * squareSize.y + Vector3.right * squareSize.x;

                verticesInst[cornerIndex] = verticesInst[cornerIndex] + Vector3.up * Mathf.PerlinNoise(verticesInst[cornerIndex].x / (float)planeSquares.x, verticesInst[cornerIndex].z / (float)planeSquares.y);
                verticesInst[cornerIndex + 1] = verticesInst[cornerIndex + 1] + Vector3.up * Mathf.PerlinNoise(verticesInst[cornerIndex + 1].x / (float)planeSquares.x, verticesInst[cornerIndex + 1].z / (float)planeSquares.y);
                verticesInst[cornerIndex + 2] = verticesInst[cornerIndex + 2] + Vector3.up * Mathf.PerlinNoise(verticesInst[cornerIndex + 2].x / (float)planeSquares.x, verticesInst[cornerIndex + 2].z / (float)planeSquares.y);
                verticesInst[cornerIndex + 3] = verticesInst[cornerIndex + 3] + Vector3.up * Mathf.PerlinNoise(verticesInst[cornerIndex + 3].x / (float)planeSquares.x, verticesInst[cornerIndex + 3].z / (float)planeSquares.y);
                verticesInst[cornerIndex + 4] = verticesInst[cornerIndex + 4] + Vector3.up * Mathf.PerlinNoise(verticesInst[cornerIndex + 4].x / (float)planeSquares.x, verticesInst[cornerIndex + 4].z / (float)planeSquares.y);
                verticesInst[cornerIndex + 5] = verticesInst[cornerIndex + 5] + Vector3.up * Mathf.PerlinNoise(verticesInst[cornerIndex + 5].x / (float)planeSquares.x, verticesInst[cornerIndex + 5].z / (float)planeSquares.y);

            }
        }
        print("generated " + verticesInst.GetLength(0) + " vertices");
        return verticesInst;
    }

    int[] GenerateTriangles()
    {
        int[] trianglesInst = new int[planeSquares.x * planeSquares.y * 6];

        for (int y = 0; y < planeSquares.y; y++)
        {
            for (int x = 0; x < planeSquares.x; x++)
            {
                int verticeCornerIndex = ((y * planeSquares.x) + x) * 6;

                int triangleIntIndex = ((y * planeSquares.x) + x) * 6;

                trianglesInst[triangleIntIndex] = verticeCornerIndex;
                trianglesInst[triangleIntIndex + 1] = verticeCornerIndex + 1;
                trianglesInst[triangleIntIndex + 2] = verticeCornerIndex + 2;

                trianglesInst[triangleIntIndex + 3] = verticeCornerIndex + 4;
                trianglesInst[triangleIntIndex + 4] = verticeCornerIndex + 5;
                trianglesInst[triangleIntIndex + 5] = verticeCornerIndex + 3;
            }

        }

        return trianglesInst;
    }

}
