using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
public class ShapeMap : MonoBehaviour
{
    public BlockEngine blockEngine = null;

    public float threshold = 0f;                                // Set higher to remove more of the sample found (can be used to remove "edges" of result)
    public float2 noiseFilter = new float2(0.04f, 0.04f);      // Make smaller to make the section cut out larger

    public Dictionary<Vector3, bool[]> subMaps = new Dictionary<Vector3, bool[]>(); // Maybe use <float> instead so you can modify later?

    //public int depth = 0;

    public float defaultMapSize = 512;                  // size of the height map (limit = 46,336 / √2,147,483,647 - 46,340.9500010519853390887900102 )
    public int COLUMN_BLOCK_HEIGHT = 256;        // Total vertical number of blocks - chunk
    public int SUB_CHUNKS_PER_CHUNK = 32;        // 16 sub chunks in a "chunk"
    public int SUB_CHUNK_SIZE = 16;              // Cube
    public int SUB_CHUNK_HEIGHT = 8;             // Can NOT be 16 (24 verts * 16 * 16 * 16 > 65536)

    // Task progression data for visual display -  % done, time left
    public LoadingProgressionData loadingProgressionData = new LoadingProgressionData(true);

    // ==================================================

    public float xy;
    public float xz;
    public float yz;
    public float yx;
    public float zx;
    public float zy;

    public float3 offset = new float3(100, 100, 100);
    public float3 secondOffset = new float3(1000, 1000, 1000);

    public List<bool> results = new List<bool>();

    public float tempResult = 0;

    // ==================================================

    private List<Vector3> positionsToRemove = new List<Vector3>();

    public Dictionary<Vector3, NoiseForShapeMapJob> jobs = new Dictionary<Vector3, NoiseForShapeMapJob>();
    public Dictionary<Vector3, JobHandle> jobHandles = new Dictionary<Vector3, JobHandle>();

    public Dictionary<Vector3, NativeArray<bool>> nativeShapeMaps = new Dictionary<Vector3, NativeArray<bool>>();

    // ==================================================

    [BurstCompile]
    private void OnDestroy()
    {
        // Free memory

        blockEngine = null;

        subMaps.Clear();
        subMaps = null;
        results.Clear();
        results = null;

        jobs.Clear();
        jobs = null;
        jobHandles.Clear();
        jobHandles = null;

        nativeShapeMaps.Clear();
        nativeShapeMaps = null;

        positionsToRemove.Clear();
        positionsToRemove = null;
    }

    [BurstCompile]
    public void CleanUp()
    {
        subMaps.Clear();
    }

    [BurstCompile]
    private void Awake()
    {
        blockEngine = GetComponent<BlockEngine>();
    }

    [BurstCompile]
    public void OnApplicationQuit()
    {
        // force all jobs to finish to avoid memory leak.

        foreach (Vector3 key in jobHandles.Keys)
        {
            jobHandles[key].Complete();

            jobs[key].nativeMap.Dispose();
        }

        jobs.Clear();
        jobHandles.Clear();
    }

    [BurstCompile]
    public void LateUpdate()
    {
        if (jobHandles.Count == 0) return;

        positionsToRemove.Clear();

        foreach (Vector3 key in jobHandles.Keys)
        {
            if (jobHandles[key].IsCompleted)
            {
                jobHandles[key].Complete();

                subMaps[key] = jobs[key].nativeMap.ToArray();

                jobs[key].nativeMap.Dispose();

                positionsToRemove.Add(key);
            }
        }

        for (int i = 0; i < positionsToRemove.Count; i++)
        {
            // Remove all field dictionary elements
            jobs.Remove(positionsToRemove[i]);

            jobHandles.Remove(positionsToRemove[i]);

            nativeShapeMaps.Remove(positionsToRemove[i]);
        }

        positionsToRemove.Clear();
    }

    // 3D noise generation for terrain shapes
    [BurstCompile]
    public struct NoiseForShapeMapJob : IJob
    {
        public NativeArray<bool> nativeMap;

        // Needed to know where to stop block map check if neighbour doesn't exist 
        // (avoid holes in terrain)
        public bool hasNorthSubChunkNeighbour;
        public bool hasSouthSubChunkNeighbour;
        public bool hasEastSubChunkNeighbour;
        public bool hasWestSubChunkNeighbour;
        public bool hasTopSubChunkNeighbour;
        public bool hasBottomSubChunkNeighbour;     // true working our way down. false when is the lowest generated sub chunk in the chunk

        public int size;
        public int height;

        public Vector3 position;

        public float3 offset;
        public float3 secondOffset;

        public float threshold;
        public float2 noiseFilter;

        private float tempResult;

        private int depth;   // can be across several chunks. 0 - 8 - 16 - 24 - 32

        private float xy;
        private float xz;
        private float yz;
        private float yx;
        private float zx;
        private float zy;

        private int tempY;

        private int iterator;

        private int northHeightIndex;
        private int southHeightIndex;
        private int eastHeightIndex;
        private int westHeightIndex;

        private bool bottomEdge;
        private bool northEdge;
        private bool southEdge;
        private bool eastEdge;
        private bool westEdge;

        // ---------------------------
        // Used to optimze 'new' word out of code
        public float2 tempFloat2;

        [BurstCompile]
        public void Execute()
        {
            iterator = 0;

            for (int y = 0; y < height; y++)
            {
                tempY = y + depth;

                for (int x = 0; x < size; x++)
                {
                    for (int z = 0; z < size; z++)
                    {
                        // Reset
                        northEdge = false;
                        southEdge = false;
                        eastEdge = false;
                        westEdge = false;
                        bottomEdge = false;
                        
                        if (z == 0)
                        {
                            southEdge = true;
                        }
                        else
                        {
                            if (z == size - 1)
                            {
                                northEdge = true;
                            }
                        }

                        // ============================================================
                        
                            if (x == 0)
                            {
                                westEdge = true;
                            }
                            else
                            {
                                if (x == size - 1)
                                {
                                    eastEdge = true;
                                }
                            }

                        // ============================================================

                        if (y == 0) bottomEdge = true;
                        //if (y == height - 1) topEdge = true;

                        if ((hasNorthSubChunkNeighbour == false
                            && northEdge)
                            || (hasSouthSubChunkNeighbour == false
                            && southEdge)
                            || (hasEastSubChunkNeighbour == false
                            && eastEdge)
                            || (hasWestSubChunkNeighbour == false
                            && westEdge)
                            || (hasBottomSubChunkNeighbour == false
                            && bottomEdge)) 
                        {
                            nativeMap[iterator] = false;

                            iterator++;
                        }
                        else
                        {
                            // 3D perlin noise.

                            tempFloat2.x = (position.x + x) * 0.04f;
                            tempFloat2.y = (position.y + tempY) * 0.04f;
                            xy = noise.snoise(tempFloat2);

                            tempFloat2.x = (position.x + x) * 0.04f;
                            tempFloat2.y = (position.z + z) * 0.04f;
                            xz = noise.snoise(tempFloat2);

                            tempFloat2.x = (position.y + tempY) * 0.04f;
                            tempFloat2.y = (position.z + z) * 0.04f;
                            yz = noise.snoise(tempFloat2);

                            tempFloat2.x = (position.y + tempY) * 0.04f;
                            tempFloat2.y = (position.x + x) * 0.04f;
                            yx = noise.snoise(tempFloat2);

                            tempFloat2.x = (position.z + z) * 0.04f;
                            tempFloat2.y = (position.x + x) * 0.04f;
                            zx = noise.snoise(tempFloat2);

                            tempFloat2.x = (position.z + z) * 0.04f;
                            tempFloat2.y = (position.y + tempY) * 0.04f;
                            zy = noise.snoise(tempFloat2);

                            tempResult = (xy + xz + yz + yx + zx + zy) / 6;

                            if (tempResult > threshold) { nativeMap[iterator] = true; }
                            else { nativeMap[iterator] = false; }

                            iterator++; 
                        }
                    }
                }
            }
        }
    }

    [BurstCompile]
    public void Generate(Vector3 position, bool hasNorthNeighbour, bool hasSouthNeighbour, bool hasEastNeighbour, bool hasWestNeighbour, bool hasBottomNeighbour)
    {
        subMaps.Add(position, null);

        nativeShapeMaps.Add(position, new NativeArray<bool>(SUB_CHUNK_SIZE * SUB_CHUNK_SIZE * SUB_CHUNK_HEIGHT, Allocator.Persistent));

        jobs.Add(position, new NoiseForShapeMapJob
        {
            position = position,
            nativeMap = nativeShapeMaps[position],
            hasNorthSubChunkNeighbour = hasNorthNeighbour,
            hasSouthSubChunkNeighbour = hasSouthNeighbour,
            hasEastSubChunkNeighbour = hasEastNeighbour,
            hasWestSubChunkNeighbour = hasWestNeighbour,
            hasBottomSubChunkNeighbour = hasBottomNeighbour,
            size = SUB_CHUNK_SIZE,
            height = SUB_CHUNK_HEIGHT,
            offset = Vector3.zero,
            secondOffset = secondOffset,
            noiseFilter = noiseFilter,
            threshold = threshold,
        });

        jobHandles.Add(position, jobs[position].Schedule(default));
    }
}
