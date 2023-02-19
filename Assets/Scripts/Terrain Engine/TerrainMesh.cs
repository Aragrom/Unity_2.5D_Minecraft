using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
public class TerrainMesh : MonoBehaviour
{
    public TerrainEngine terrainEngine = null;

    // Terrain size - number of vertices per row (+ 1)
    // Terrain can be scaled using transform.
    public int size = 255;

    // Mesh data
    public int[] triangles;
    public Vector3[] vertices;
    public Vector2[] uvs;

    // Any numbers inside the low and high mask when used to modify the
    // heights of terrain force them to zero.
    // (Used to create the dip in the terrain where the chunks/subchunks exist)
    public int lowMask = 113;
    public int highMask = 143;

    public Mesh mesh = null;

    // Checked and emptied in LateUpdate()
    // Job containers ==========================================

    public bool jobActive = false;

    // Job only called once
    JobHandle jobHandle = default;
    private GenerateTerrainTrianglesDataJob meshJob;

    private NativeArray<int> trianglesJob;
    private NativeArray<Vector2> uvsJob;
    // =========================================================

    [BurstCompile]
    private void OnDestroy()
    {
        // Free memory

        terrainEngine = null;

        triangles = null;
        vertices = null;
        uvs = null;

        mesh = null;
    }

    [BurstCompile]
    public void OnApplicationQuit()
    {
        if (jobActive)
        {
            jobHandle.Complete();

            trianglesJob.Dispose();
            uvsJob.Dispose();

            jobActive = false;
        }
    }

    [BurstCompile]
    public void Awake()
    {
        terrainEngine = GetComponent<TerrainEngine>();
    }

    [BurstCompile]
    public void LateUpdate()
    {
        if (jobActive)
        {
            if (jobHandle.IsCompleted)
            {
                jobHandle.Complete();

                this.triangles = meshJob.triangles.ToArray();
                this.uvs = meshJob.uvs.ToArray();

                trianglesJob.Dispose();
                uvsJob.Dispose();

                jobActive = false;
            }
        }
    }

    // low mask > < high mask

    [BurstCompile]
    public void SetVertices(Vector3[] positions)
    {
        // Set main terrain

        mesh = terrainEngine.terrain.GetComponent<MeshFilter>().mesh;

        mesh.vertices = positions;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        mesh.RecalculateNormals();
        mesh.Optimize();

        // set terrain to default position
        terrainEngine.terrain.position = terrainEngine.terrainDefaultPosition;

        // Set terrain backfaces

        // set terrain backfaces move up so it can be moved to not be on the same spot as the old terrain (vertices are be set to exact position)
        /*terrainEngine.terrainBackface.position = terrainEngine.terrainDefaultPosition;

        mesh = terrainEngine.terrainBackface.GetComponent<MeshFilter>().mesh;

        mesh.vertices = positions;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        mesh.Optimize();

        // set terrain backfaces to default position
        terrainEngine.terrainBackface.position = terrainEngine.terrainDefaultPosition + (Vector3.down * 16);*/
    }

    // THIS NEEDS TO BE A JOB! <<<<<<<<<<<<<<<<<<<<
    // Generate mesh - logic test for job.
    [BurstCompile]
    public void CreateMesh(Vector3[] vertices, int[] triangles, Vector2[] uvs)
    {
        // Vertices =================

        /*vertices = new Vector3[(size + 1) * (size + 1)];

        int iterator = 0;

        for (int x = 0; x <= size; x++)
        {
            for (int z = 0; z <= size; z++)
            {
                vertices[iterator] = new Vector3(x, 0, z);
                iterator++;
            }
        }*/

        // Triangles ================

        /*this.triangles = new int[size * size * 6];

        int verticeCount = 0;
        int triangleCount = 0;

        for (int z = 0; z < size; z++)
        {
            for (int x = 0; x < size; x++)
            {
                this.triangles[triangleCount + 5] = verticeCount + 0;
                this.triangles[triangleCount + 4] = verticeCount + size + 1;
                this.triangles[triangleCount + 3] = verticeCount + 1;
                this.triangles[triangleCount + 2] = verticeCount + 1;
                this.triangles[triangleCount + 1] = verticeCount + size + 1;
                this.triangles[triangleCount + 0] = verticeCount + size + 2;

                verticeCount++;
                triangleCount += 6;
            }

            verticeCount++;
        }*/

        // Create main terrain mesh =================

        mesh = terrainEngine.terrain.GetComponent<MeshFilter>().mesh;

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        mesh.RecalculateNormals();
        mesh.Optimize();

        // Create terrain backface mesh =================

        /*mesh = terrainEngine.terrainBackface.GetComponent<MeshFilter>().mesh;

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        mesh.Optimize();
        //mesh.RecalculateNormals();*/
    }

    // Raw Data
    [BurstCompile]
    public struct GenerateTerrainTrianglesDataJob : IJob
    {        
        public NativeArray<int> triangles;
        public NativeArray<Vector2> uvs;

        public int size;

        [BurstCompile]
        public void Execute()
        {
            int verticeCount = 0;
            int triangleCount = 0;

            for (int z = 0; z < size; z++)
            {
                for (int x = 0; x < size; x++)
                {
                    triangles[triangleCount + 0] = verticeCount + 0;
                    triangles[triangleCount + 1] = verticeCount + size + 1;
                    triangles[triangleCount + 2] = verticeCount + 1;
                    triangles[triangleCount + 3] = verticeCount + 1;
                    triangles[triangleCount + 4] = verticeCount + size + 1;
                    triangles[triangleCount + 5] = verticeCount + size + 2;

                    verticeCount++;
                    triangleCount += 6;
                }

                verticeCount++;
            }

            int uvCount = 0;

            for (int z = 0; z <= size; z++)
            {
                for (int x = 0; x <= size; x++)
                {
                    uvs[uvCount] = new Vector2(x / (float)size, z / (float)size);

                    uvCount++;
                }
            }
        }
    }

    // Generate Complete Sub Chunks
    [BurstCompile]
    public void GenerateTerrainTriangleData()
    {
        trianglesJob = new NativeArray<int>(size * size * 6, Allocator.TempJob);
        uvsJob = new NativeArray<Vector2>((size + 1) * (size + 1), Allocator.TempJob);

        jobHandle = default;

        meshJob = new GenerateTerrainTrianglesDataJob
        {
            // params
            triangles = trianglesJob,
            uvs = uvsJob,
            size = size
        };

        jobHandle = meshJob.Schedule(jobHandle);

        jobActive = true;
    }
}
