using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

[BurstCompile]
public class RawSurroundingCrossSectionData : MonoBehaviour
{
    private Main main = null;

    public enum State
    {
        None,           // No terrain exists here - sub chunk above the "height map"
        Default,        // Simple plane with 16 x 16 texture applied - for empty sub chunks below "height map"
        Mesh            // Where a mesh exists inside the map - Cross Section of terrain
    }

    public Vector3[] vertices;
    public Vector2[] uvs;

    public Vector2 position = Vector3.zero;

    // Checked and emptied in LateUpdate()
    // Job containers ==========================================

    public bool jobActive = false;

    // Job only called once
    JobHandle jobHandle = default;
    private GenerateSubChunkCrossSectionMeshDataJob job;

    private NativeArray<Vector3> nativeVertices;
    private NativeArray<Vector2> nativeUvs;

    // =========================================================

    [BurstCompile]
    private void OnDestroy()
    {
        // Free memory

        main = null;

        vertices = null;
        uvs = null;
    }

    [BurstCompile]
    public void Awake()
    {
        main = GameObject.Find("Main").GetComponent<Main>();
    }

    [BurstCompile]
    public void OnApplicationQuit()
    {
        // force all jobs to finish to avoid memory leak.
        // job only called once

        if (jobActive)
        {
            jobHandle.Complete();

            this.vertices = job.vertices.ToArray();
            this.uvs = job.uvs.ToArray();

            nativeVertices.Dispose();
            nativeUvs.Dispose();

            jobActive = false;
        }
    }

    [BurstCompile]
    public void LateUpdate()
    {
        if (jobActive)
        {
            if (jobHandle.IsCompleted)
            {
                jobHandle.Complete();

                this.vertices = job.vertices.ToArray();
                this.uvs = job.uvs.ToArray();

                nativeVertices.Dispose();
                nativeUvs.Dispose();

                jobActive = false;
            }
        }
    }

    // Raw Data
    [BurstCompile]
    public struct GenerateSubChunkCrossSectionMeshDataJob : IJob
    {
        // In - out - mesh data on sub chunk cross section
        // Dont need triangles as they are calculated at runtime
        public NativeArray<Vector3> vertices;
        public NativeArray<Vector2> uvs;

        public int size;
        public int height;

        [BurstCompile]
        public void Execute()
        {
            int iterator = 0;

            for (int y = 0; y <= size; y++)
            {
                for (int x = 0; x <= size; x++)
                {
                    vertices[iterator] = new Vector3(x, y, 0);
                    uvs[iterator] = new Vector2(x / size, y / size);
                    iterator++;
                }
            }
        }
    }

    // Generate Complete Sub Chunks
    [BurstCompile]
    public void GenerateSubChunkCrossChunkData()
    {
        nativeVertices = new NativeArray<Vector3>(main.blockEngine.blockMap.SUB_CHUNK_SIZE * main.blockEngine.blockMap.SUB_CHUNK_SIZE * main.blockEngine.blockMap.SUB_CHUNK_HEIGHT, Allocator.TempJob);
        nativeUvs = new NativeArray<Vector2>(main.blockEngine.blockMap.SUB_CHUNK_SIZE * main.blockEngine.blockMap.SUB_CHUNK_SIZE * main.blockEngine.blockMap.SUB_CHUNK_HEIGHT, Allocator.TempJob);

        jobHandle = default;

        job = new GenerateSubChunkCrossSectionMeshDataJob
        {
            // params
            vertices = nativeVertices,
            uvs = nativeUvs,
            size = main.blockEngine.blockMap.SUB_CHUNK_SIZE,
            height = main.blockEngine.blockMap.SUB_CHUNK_HEIGHT
        };

        jobHandle = job.Schedule(jobHandle);

        jobActive = true;

        // ===================================================================
    }
}
