using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[System.Serializable]
[BurstCompile]
public struct SubChunkData
{
    public int[] triangles;
    public Vector2[] uvs;       // Base texture map uv coordinates
    public Vector2[] uvs2;      // Light map uvs - blends in shader
}

// Generate mesh data which is used by reference. Vertices will always be the same (Shared).
[BurstCompile]
public class RawSubChunkData : MonoBehaviour
{
    private Main main = null;

    public enum FaceTypes { Top = 0, Front = 1, Back = 2, Right = 3, Left = 4, Bottom = 5 }

    public enum BlockType { Air, Stone, Water };

    public Vector3[] vertices;
    public Vector2[] uvs;
    public Vector2[] uvs2;

    public Vector2 position = Vector3.zero;

    public float blockMapWidth = 16.0f;   // Grid - 16x16 - Gets used to divide into fraction within this class - MUST be float

    public float widthOfBlockOnMap = 0.00390625f;   // 1/256

    // Job containers =================================

    // Job only called once
    public bool jobActive = false;
    JobHandle jobHandle = default;

    GenerateSubChunkMeshDataJob meshJob;
    private NativeArray<Vector3> nativeBlockVertices;
    private NativeArray<Vector2> nativeBlockUvs;
    private NativeArray<Vector2> nativeBlockUvs2;
    private NativeArray<Vector3> nativeVertices;
    private NativeArray<Vector2> nativeUvs;
    private NativeArray<Vector2> nativeUvs2;

    [BurstCompile]
    private void OnDestroy()
    {
        // Free memory

        main = null;

        vertices = null;
        uvs = null;
    }

    [BurstCompile]
    private void Awake()
    {
        main = GameObject.Find("Main").GetComponent<Main>();
    }

    [BurstCompile]
    public void OnApplicationQuit()
    {
        // force all jobs to finish to avoid memory leak.
        // Job only called once.

        if (jobActive)
        {
            jobHandle.Complete();

            nativeVertices.Dispose();
            nativeUvs.Dispose();
            nativeUvs2.Dispose();

            nativeBlockVertices.Dispose();
            nativeBlockUvs.Dispose();
            nativeBlockUvs2.Dispose();

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

                this.vertices = meshJob.nativeVertices.ToArray();
                this.uvs = meshJob.nativeUvs.ToArray();
                this.uvs2 = meshJob.nativeUvs2.ToArray();

                nativeVertices.Dispose();
                nativeUvs.Dispose();
                nativeUvs2.Dispose();

                nativeBlockVertices.Dispose();
                nativeBlockUvs.Dispose();
                nativeBlockUvs2.Dispose();

                jobActive = false;
            }
        }
    }

    // Raw Data
    [BurstCompile]
    public struct GenerateSubChunkMeshDataJob : IJob
    {
        // In - out - mesh data on sub chunk
        //public NativeArray<int> triangles;
        public NativeArray<Vector3> nativeVertices;
        public NativeArray<Vector2> nativeUvs;
        public NativeArray<Vector2> nativeUvs2;

        // Reuseable block - mesh data -
        // (Hardcoding these instead of using variable could yield faster results.)
        //public NativeArray<int> blockIndices;
        public NativeArray<Vector3> nativeBlockVertices;
        public NativeArray<Vector2> nativeBlockUvs;
        public NativeArray<Vector2> nativeBlockUvs2;

        public int size;
        public int height;

        [BurstCompile]
        public void Execute()
        {
            int iterator = 0;

            int tempIterator = 0;

            // Y > X > Z

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    for (int z = 0; z < size; z++)
                    {
                        tempIterator = iterator * nativeBlockVertices.Length;     // 0..14..28..42 or 0..24..48

                        for (int i = 0; i < nativeBlockVertices.Length; i++)       // 14/24 - number of vertices possible
                        {
                            nativeVertices[tempIterator + i] = new Vector3(x + nativeBlockVertices[i].x, y + nativeBlockVertices[i].y, z + nativeBlockVertices[i].z);
                            nativeUvs[tempIterator + i] = nativeBlockUvs[i];
                            nativeUvs2[tempIterator + i] = nativeBlockUvs2[i];
                        }

                        iterator++;
                    }
                }
            }
        }
    }

    // 16 x 16 x 8(Height). Increase object count (higher vertice/uv cost per "cube")
    [BurstCompile]
    public void GenerateSubChunkData()
    {
        //0, 2, 3, 0, 3, 1, - Front x
        //8, 4, 5, 8, 5, 9, - top
        //10, 6, 7, 10, 7, 11, - back
        //12, 13, 14, 12, 14, 15, - bottom
        //16, 17, 18, 16, 18, 19, - left
        //20, 21, 22, 20, 22, 23 - right

        nativeBlockVertices = new NativeArray<Vector3>(new Vector3[] {

            new Vector3(1f, 0f, 1f),       // front
            new Vector3(0f, 0f, 1f),
            new Vector3(1f, 1f, 1f),
            new Vector3(0f, 1f, 1f),
            
            new Vector3(1f, 1f, 0f),        // top
            new Vector3(0f, 1f, 0f), 
            new Vector3(1f, 0f, 0f), 
            new Vector3(0f, 0f, 0f),

            new Vector3(1f, 1f, 1f),        // back
            new Vector3(0f, 1f, 1f),
            new Vector3(1f, 1f, 0f), 
            new Vector3(0f, 1f, 0f), 
            
            new Vector3(1f, 0f, 0f),        // bottom
            new Vector3(1f, 0f, 1f), 
            new Vector3(0f, 0f, 1f), 
            new Vector3(0f, 0f, 0f), 
            
            new Vector3(0f, 0f, 1f),        // left
            new Vector3(0f, 1f, 1f), 
            new Vector3(0f, 1f, 0f),
            new Vector3(0f, 0f, 0f), 
            
            new Vector3(1f, 0f, 0f),        // right
            new Vector3(1f, 1f, 0f), 
            new Vector3(1f, 1f, 1f),
            new Vector3(1f, 0f, 1f)

        }, Allocator.TempJob);

        // Single cube
        nativeBlockUvs = new NativeArray<Vector2>(new Vector2[]{

            new Vector2(0.03125f, 0.03125f),          // front
            new Vector2(0.09375f, 0.03125f),
            new Vector2(0.03125f, 0.09375f),
            new Vector2(0.09375f, 0.09375f),

            new Vector2(0.03125f, 0.09375f),          // top?
            new Vector2(0.09375f, 0.09375f),
            new Vector2(0.03125f, 0.03125f),
            new Vector2(0.09375f, 0.03125f),

            new Vector2(0.03125f, 0.03125f),          // back?
            new Vector2(0.09375f, 0.03125f),
            new Vector2(0.03125f, 0.09375f),
            new Vector2(0.09375f, 0.09375f),

            new Vector2(0.03125f, 0.03125f),          // Bottom?
            new Vector2(0.03125f, 0.09375f),
            new Vector2(0.09375f, 0.09375f), 
            new Vector2(0.09375f, 0.03125f),

            new Vector2(0.03125f, 0.03125f),          // Left
            new Vector2(0.03125f, 0.09375f),
            new Vector2(0.09375f, 0.09375f),
            new Vector2(0.09375f, 0.03125f),

            new Vector2(0.03125f, 0.03125f),          // Right
            new Vector2(0.03125f, 0.09375f),
            new Vector2(0.09375f, 0.09375f),
            new Vector2(0.09375f, 0.03125f)

        }, Allocator.TempJob);

        // Single cube
        nativeBlockUvs2 = new NativeArray<Vector2>(new Vector2[]{

            new Vector2(0f, 0f),          // front
            new Vector2(1f, 0f),
            new Vector2(0f, 1f),
            new Vector2(1f, 1f),

            new Vector2(0f, 1f),          // top?
            new Vector2(1f, 1f),
            new Vector2(0f, 0f),
            new Vector2(1f, 0f),

            new Vector2(0f, 0f),          // back?
            new Vector2(1f, 0f),
            new Vector2(0f, 1f),
            new Vector2(1f, 1f),

            new Vector2(0f, 0f),          // Bottom?
            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(1f, 0f),

            new Vector2(0f, 0f),          // Left
            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(1f, 0f),

            new Vector2(0f, 0f),          // Right
            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(1f, 0f)

        }, Allocator.TempJob);

        nativeVertices = new NativeArray<Vector3>(24 * main.blockEngine.blockMap.SUB_CHUNK_SIZE * main.blockEngine.blockMap.SUB_CHUNK_SIZE * main.blockEngine.blockMap.SUB_CHUNK_HEIGHT, Allocator.TempJob);
        nativeUvs = new NativeArray<Vector2>(24 * main.blockEngine.blockMap.SUB_CHUNK_SIZE * main.blockEngine.blockMap.SUB_CHUNK_SIZE * main.blockEngine.blockMap.SUB_CHUNK_HEIGHT, Allocator.TempJob);
        nativeUvs2 = new NativeArray<Vector2>(24 * main.blockEngine.blockMap.SUB_CHUNK_SIZE * main.blockEngine.blockMap.SUB_CHUNK_SIZE * main.blockEngine.blockMap.SUB_CHUNK_HEIGHT, Allocator.TempJob);


        jobHandle = default;

        meshJob = new GenerateSubChunkMeshDataJob
        {
            // params
            //triangles = triangles,
            nativeVertices = nativeVertices,
            nativeUvs = nativeUvs,
            nativeUvs2 = nativeUvs2,
            //blockIndices = blockIndices,
            nativeBlockVertices = nativeBlockVertices,
            nativeBlockUvs = nativeBlockUvs,
            nativeBlockUvs2 = nativeBlockUvs2,
            size = main.blockEngine.blockMap.SUB_CHUNK_SIZE,
            height = main.blockEngine.blockMap.SUB_CHUNK_HEIGHT
        };

        jobHandle = meshJob.Schedule(jobHandle);

        jobActive = true;
    }

    //=============================================================================

    // 16 x 16 x 16. Cube Map format uvs. Terrible for texturing. Leaves gaps in objects
    // OLD
    [BurstCompile]
    public void GenerateSubChunkDataCubeMap()
    {
        nativeBlockVertices = new NativeArray<Vector3>(new Vector3[] {

            new Vector3(0, 1, 0),
            new Vector3(0, 0, 0),
            new Vector3(1, 1, 0),
            new Vector3(1, 0, 0),

            new Vector3(0, 0, 1),
            new Vector3(1, 0, 1),
            new Vector3(0, 1, 1),
            new Vector3(1, 1, 1),

            new Vector3(0, 1, 0),
            new Vector3(1, 1, 0),

            new Vector3(0, 1, 0),
            new Vector3(0, 1, 1),

            new Vector3(1, 1, 0),
            new Vector3(1, 1, 1),

        }, Allocator.TempJob);

        // Single cube
        nativeBlockUvs = new NativeArray<Vector2>(new Vector2[]{

            new Vector2(0, (1f/3f) * 2f),
            new Vector2(0.25f, (1f/3f) * 2f),
            new Vector2(0, (1f/3)),
            new Vector2(0.25f, (1f/3f)),

            new Vector2(0.5f, (1f/3f) * 2f),
            new Vector2(0.5f, (1f/3f)),
            new Vector2(0.75f, (1f/3f) * 2f),
            new Vector2(0.75f, (1f/3f)),

            new Vector2(1, (1f/3f) * 2f),
            new Vector2(1, (1f/3f)),

            new Vector2(0.25f, 1f),
            new Vector2(0.5f, 1f),

            new Vector2(0.25f, 0),
            new Vector2(0.5f, 0),

        }, Allocator.TempJob);

        nativeVertices = new NativeArray<Vector3>(14 * main.blockEngine.blockMap.SUB_CHUNK_SIZE * main.blockEngine.blockMap.SUB_CHUNK_SIZE * main.blockEngine.blockMap.SUB_CHUNK_HEIGHT, Allocator.TempJob);
        nativeUvs = new NativeArray<Vector2>(14 * main.blockEngine.blockMap.SUB_CHUNK_SIZE * main.blockEngine.blockMap.SUB_CHUNK_SIZE * main.blockEngine.blockMap.SUB_CHUNK_HEIGHT, Allocator.TempJob);

        jobHandle = default;

        meshJob = new GenerateSubChunkMeshDataJob
        {
            // params
            //triangles = triangles,
            nativeVertices = nativeVertices,
            nativeUvs = nativeUvs,
            //blockIndices = blockIndices,
            nativeBlockVertices = nativeBlockVertices,
            nativeBlockUvs = nativeBlockUvs,
            size = main.blockEngine.blockMap.SUB_CHUNK_SIZE,
            height = main.blockEngine.blockMap.SUB_CHUNK_HEIGHT
        };

        jobHandle = meshJob.Schedule(jobHandle);

        jobActive = true;
    }
}

