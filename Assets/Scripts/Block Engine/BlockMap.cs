using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

// NOTE: Further research into making triangle arrays the same length so can benefit from entities instead of gameObjects.
// Having 16*16*7 (sub chunk size) < 1820 (max triangle length (65535/36 triangles per block))

[BurstCompile]
public class BlockMap : MonoBehaviour
{
    public BlockEngine blockEngine = null;

    public Vector3 subMapPosition = new Vector3(-256, 0, -256);
    private Vector3 savedSubMapPosition = Vector2.zero;               // used when incrementing the submap postion (x != 0 .. x == savedSubMapPosition.x)

    public bool isProcessingChunk = false;

    public int depth = 0;

    public float defaultMapSize = 512;                  // size of the height map
    public int COLUMN_BLOCK_HEIGHT = 1024;       // Total vertical number of blocks - chunk
    public int SUB_CHUNKS_PER_CHUNK = 64;        // 16 sub chunks in a "chunk"
    public int SUB_CHUNK_SIZE = 16;              // Cube
    public int SUB_CHUNK_HEIGHT = 8;            // 16 * 16 * 16 = 4096
    
    // All sub chunk data - triangles, faces and types.
    public Dictionary<Vector3, SubChunkData> subChunkData = new Dictionary<Vector3, SubChunkData>();

    public Dictionary<Vector2, List<Vector3>> subChunkPositionsByChunk = new Dictionary<Vector2, List<Vector3>>();

    // has the sub chunk data been generated for the chunk at this position (x, z)?
    public Dictionary<Vector2, bool> chunkData = new Dictionary<Vector2, bool>();   // false when begun, true when has completed chunk

    // Task progression data for visual display -  % done, time left
    public LoadingProgressionData loadingProgressionData = new LoadingProgressionData(true);

    // cached in class to stop generating a new list each late update frame.
    private List<Vector3> positionsToRemove = new List<Vector3>();
    private Vector3 tempIteratorPosition = Vector3.zero;
    private SubChunkData newSubChunkData = new SubChunkData();

    //private Vector3 positionOfChunkBeingProcessed = Vector3.zero;
    //private int counterForQuit = 0;     // need to know in the chunk being processed how many sub chunks are left to process. (Mainly so memory of native container is deallocated)

    // ================================================================================

    // All required globally in order to dispose safely

    public Dictionary<Vector3, JobHandle> jobHandles = new Dictionary<Vector3, JobHandle>();
    Dictionary<Vector3, BlockDataWithShapeMapForSubChunkJob[]> jobs = new Dictionary<Vector3, BlockDataWithShapeMapForSubChunkJob[]>();

    Dictionary<Vector3, NativeArray<float>> nativeHeightMaps = new Dictionary<Vector3, NativeArray<float>>();
    Dictionary<Vector3, NativeArray<float>> nativeNorthHeightMaps = new Dictionary<Vector3, NativeArray<float>>();
    Dictionary<Vector3, NativeArray<float>> nativeSouthHeightMaps = new Dictionary<Vector3, NativeArray<float>>();
    Dictionary<Vector3, NativeArray<float>> nativeEastHeightMaps = new Dictionary<Vector3, NativeArray<float>>();
    Dictionary<Vector3, NativeArray<float>> nativeWestHeightMaps = new Dictionary<Vector3, NativeArray<float>>();

    Dictionary<Vector3, NativeList<int>[]> nativeTriangles = new Dictionary<Vector3, NativeList<int>[]>();
    Dictionary<Vector3, NativeArray<Vector2>[]> nativeUvs = new Dictionary<Vector3, NativeArray<Vector2>[]>();
    //Dictionary<Vector3, NativeArray<bool>[]> nativeFaces = new Dictionary<Vector3, NativeArray<bool>[]>();
    //Dictionary<Vector3, NativeArray<int>[]> nativeTypes = new Dictionary<Vector3, NativeArray<int>[]>();

    Dictionary<Vector3, int> subChunkDepthLowest = new Dictionary<Vector3, int>();
    Dictionary<Vector3, int> subChunksRequired = new Dictionary<Vector3, int>();

    // ================================================================================
    // Added to remove 'new' data from code
    private Vector2 positionTemp = Vector2.zero;
    private Vector3 positionZeroForY = Vector3.zero;
    private Vector2 difference = Vector2.zero;

    private int highest = 0;
    private int lowest = 0;

    private int subChunkHighest = 0;
    private int subChunkLowest = 0;
    private int required = 0;

    private Vector3 tempSubChunkPosition = Vector3.zero;

    private Vector2 tempSubMapPositon = Vector2.zero;

    [BurstCompile]
    private void OnDestroy()
    {
        // Free memory

        blockEngine = null;
        subChunkData.Clear();
        subChunkData = null;
        subChunkPositionsByChunk.Clear();
        subChunkPositionsByChunk = null;

        chunkData.Clear();
        chunkData = null;
        jobHandles.Clear();
        jobHandles = null;

        jobs.Clear();
        jobs = null;
        nativeHeightMaps.Clear();
        nativeHeightMaps = null;
        nativeNorthHeightMaps.Clear();
        nativeNorthHeightMaps = null;
        nativeSouthHeightMaps.Clear();
        nativeSouthHeightMaps = null;
        nativeEastHeightMaps.Clear();
        nativeEastHeightMaps = null;
        nativeWestHeightMaps.Clear();
        nativeWestHeightMaps = null;

        nativeTriangles.Clear();
        nativeTriangles = null;
        nativeUvs.Clear();
        nativeUvs = null;
        //nativeFaces.Clear();
        //nativeFaces = null;
        //nativeTypes.Clear();
        //nativeTypes = null;

        subChunkDepthLowest.Clear();
        subChunkDepthLowest = null;
        subChunksRequired.Clear();
        subChunksRequired = null;

        StopAllCoroutines();
    }

    [BurstCompile]
    public void CleanUp()
    {
        chunkData.Clear();

        subChunkData.Clear();
        subChunkPositionsByChunk.Clear();
    }

    [BurstCompile]
    private void Awake()
    {
        blockEngine = GetComponent<BlockEngine>();

        //StartCoroutine(ControlledData());
    }

    [BurstCompile]
    public void OnApplicationQuit()
    {
        // About to manually remove the chunk being processed in the coroutine so stop it.
        StopAllCoroutines();

        // force all jobs to finish to avoid memory leak.
        foreach (Vector3 key in jobHandles.Keys)
        {            
            jobHandles[key].Complete();

            for (int i = 0; i < subChunksRequired[key]; i++)
            {
                nativeTriangles[key][i].Dispose();
                //nativeFaces[key][i].Dispose();
                //nativeTypes[key][i].Dispose();
                nativeUvs[key][i].Dispose();

                jobs[key][i].shapeMap.Dispose();
            }

            nativeHeightMaps[key].Dispose();
            nativeNorthHeightMaps[key].Dispose();
            nativeSouthHeightMaps[key].Dispose();
            nativeEastHeightMaps[key].Dispose();
            nativeWestHeightMaps[key].Dispose();            
        }

        jobHandles.Clear();

        // All other memory should be naturally freed once application closes
    }

    // was in LateUpdate
    [BurstCompile]
    private void LateUpdate()
    {
        if (jobHandles.Count == 0) return;

        foreach (Vector3 key in jobHandles.Keys)
        {
            if (jobHandles[key].IsCompleted)
            {
                jobHandles[key].Complete();

                tempIteratorPosition = new Vector3(key.x, (subChunkDepthLowest[key] * SUB_CHUNK_HEIGHT), key.z);

                for (int i = 0; i < subChunksRequired[key]; i++)
                {
                    newSubChunkData = new SubChunkData
                    {
                        triangles = jobs[key][i].nativeTriangles.ToArray(),
                        uvs = jobs[key][i].nativeUvs.ToArray()
                    };

                    subChunkPositionsByChunk[new Vector2(key.x, key.z)].Add(tempIteratorPosition);

                    subChunkData.Add(tempIteratorPosition, newSubChunkData);

                    nativeTriangles[key][i].Dispose();
                    nativeUvs[key][i].Dispose();

                    jobs[key][i].shapeMap.Dispose();

                    tempIteratorPosition.y += SUB_CHUNK_HEIGHT;
                }

                positionsToRemove.Add(key);

                nativeHeightMaps[key].Dispose();
                nativeNorthHeightMaps[key].Dispose();
                nativeSouthHeightMaps[key].Dispose();
                nativeEastHeightMaps[key].Dispose();
                nativeWestHeightMaps[key].Dispose();

                chunkData[new Vector2(key.x, key.z)] = true;
                
                // is doing to much and causing lag. break out loop <<<====================== note!
                break;
            }
        }

        for (int i = 0; i < positionsToRemove.Count; i++)
        {
            tempIteratorPosition = positionsToRemove[i];

            // Remove all field dictionary elements

            nativeHeightMaps.Remove(tempIteratorPosition);
            nativeNorthHeightMaps.Remove(tempIteratorPosition);
            nativeSouthHeightMaps.Remove(tempIteratorPosition);
            nativeEastHeightMaps.Remove(tempIteratorPosition);
            nativeWestHeightMaps.Remove(tempIteratorPosition);

            jobs.Remove(tempIteratorPosition);

            nativeTriangles.Remove(tempIteratorPosition);
            //nativeFaces.Remove(tempIteratorPosition);
            //nativeTypes.Remove(tempIteratorPosition);
            nativeUvs.Remove(tempIteratorPosition);

            jobHandles.Remove(tempIteratorPosition);

            subChunkDepthLowest.Remove(tempIteratorPosition);

            subChunksRequired.Remove(tempIteratorPosition);
        }

        positionsToRemove.Clear();
    }

    [BurstCompile]
    struct BlockDataWithShapeMapForSubChunkJob : IJob
    {
        public int depth;   // 0 - 8 - 16 - 24 - ... etc ... 240
        public int size;    // 16
        public int height;  // 8

        public NativeArray<float> nativeNorthHeightMap;       // reference for neighbour heights
        public NativeArray<float> nativeSouthHeightMap;       // reference for neighbour heights    
        public NativeArray<float> nativeEastHeightMap;        // reference for neighbour heights
        public NativeArray<float> nativeWestHeightMap;        // reference for neighbour heights
        public NativeArray<float> nativeHeightMap;            // reference for sub chunk heights

        /*public NativeArray<bool> northShapeMap;          // reference for neighbour shape
        public NativeArray<bool> southShapeMap;            // reference for neighbour shape    
        public NativeArray<bool> eastShapeMap;             // reference for neighbour shape
        public NativeArray<bool> westShapeMap;             // reference for neighbour shape*/
        public NativeArray<bool> shapeMap;                 // reference for sub chunk shape

        //public NativeArray<bool> nativeFaces;                  // Output
        //public NativeArray<int> nativeTypes;                   // Output
        public NativeList<int> nativeTriangles;                // Output
        public NativeArray<Vector2> nativeUvs;                  // Output

        //------------------------------

        int iterator;
        //int faceIterator = 0;
        int trianglesIterator;

        int northHeightIndex;
        int southHeightIndex;
        int eastHeightIndex;
        int westHeightIndex;

        // Checked so correct height map can be sampled when needed

        bool northEdgeDetected;
        bool southEdgeDetected;
        bool eastEdgeDetected;
        bool westEdgeDetected;
        bool topEdgeDetected;
        bool bottomEdgeDetected;

        public bool hasNorthNeighbour;
        public bool hasSouthNeighbour;
        public bool hasEastNeighbour;
        public bool hasWestNeighbour;
        public bool hasBottomNeighbour;
        public bool hasNorthEastNeighbour;
        public bool hasSouthEastNeighbour;
        public bool hasSouthWestNeighbour;
        public bool hasNorthWestNeighbour;

        //------------------------------

        public float3 subChunkPosition; // "sub chunk position" (0,0,0) - (8,0,0) etc

        public float threshold;
        //public float2 noiseFilter;

        public float tempResult;

        public float xy;
        public float xz;
        public float yz;
        public float yx;
        public float zx;
        public float zy;

        //------------------------------
        // used to optimize 'new' out of the code
        private int heightIndex;
        private float2 tempFloat2;

        private Vector2 tempVector2;

        [BurstCompile]
        public void Execute()
        {
            iterator = 0;
            //int faceIterator = 0;
            trianglesIterator = 0;

            northHeightIndex = 0;
            southHeightIndex = 0;
            eastHeightIndex = 0;
            westHeightIndex = 0;

            Vector2 grassDirtDifference = new Vector2(0.125f, 0.125f);
            Vector2 dirtDifference = new Vector2(0f, 0.125f);
            Vector2 stoneDifference = new Vector2(0.125f, 0f);
            Vector2 blackDifference = new Vector2(0, 0.875f);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    for (int z = 0; z < size; z++)
                    {
                        // Reset
                        northEdgeDetected = false;
                        southEdgeDetected = false;
                        eastEdgeDetected = false;
                        westEdgeDetected = false;
                        topEdgeDetected = false;
                        bottomEdgeDetected = false;

                        // index for height map. (x, z) - 2D
                        heightIndex = (x * size) + z;

                        // Are we at the "edge" of the height map sample? =========
                        // z = north and south
                        if (z != 0 && z != size - 1)
                        {
                            // z within boundary do normal sampling of height map
                            northHeightIndex = heightIndex + 1;
                            southHeightIndex = heightIndex - 1;
                        }
                        else
                        {
                            if (z == 0)
                            {
                                southHeightIndex = heightIndex + size - 1;

                                // Need to sample south height map with the south index
                                southEdgeDetected = true;

                                // Normal north
                                northHeightIndex = heightIndex + 1;
                            }
                            else
                            {
                                if (z == size - 1)
                                {
                                    // Going to need to sample north height map
                                    northHeightIndex = heightIndex - (size - 1);

                                    // Need to sample north height map with the north index.
                                    northEdgeDetected = true;

                                    // Normal south
                                    southHeightIndex = heightIndex - 1;
                                }
                            }
                        }

                        // ============================================================
                        // Are we at the "edge" of the height map sample? =============
                        // x = east and west
                        if (x != 0 && x != size - 1)
                        {
                            // x within boundary do normal sampling of height map
                            eastHeightIndex = heightIndex + size;
                            westHeightIndex = heightIndex - size;
                        }
                        else
                        {
                            if (x == 0)
                            {
                                // Going to need to sample west height map
                                westHeightIndex = heightIndex + (size * (size - 1)); // heightIndex + 240

                                // Need to sample west height map with the west index
                                westEdgeDetected = true;

                                // Normal east
                                eastHeightIndex = heightIndex + size;
                            }
                            else
                            {
                                if (x == size - 1)
                                {
                                    // Going to need to sample east height map
                                    eastHeightIndex = heightIndex - (size * (size - 1));

                                    // Need to sample east height map with the east index.
                                    eastEdgeDetected = true;

                                    // Normal west
                                    westHeightIndex = heightIndex - size;
                                }
                            }
                        }

                        // ============================================================

                        if (y == 0) bottomEdgeDetected = true;
                        if (y == height - 1) topEdgeDetected = true;

                        // is the block solid?

                        int blockHeight = y + depth;

                        // is the blockheight lower or equal to the height map
                        if ((int)(nativeHeightMap[heightIndex]) >= blockHeight
                            && shapeMap[iterator] == false)
                        {
                            //nativeTypes[iterator] = 1;

                            // Does not have a top but may have sides

                            //======================
                            //0, 2, 3, 0, 3, 1,         - Front x
                            //8, 4, 5, 8, 5, 9,         - top
                            //10, 6, 7, 10, 7, 11,      - back
                            //12, 13, 14, 12, 14, 15,   - bottom
                            //16, 17, 18, 16, 18, 19,   - left
                            //20, 21, 22, 20, 22, 23    - right
                            //======================

                            //-----------------------------------------------
                            // "Up" side - top

                            if (topEdgeDetected)
                            {
                                // if top edge need to use 3d perlin noise check to see if there is a block above. (selection is off of block map)

                                // 3D perlin noise
                                tempFloat2.x = (subChunkPosition.x + x) * 0.04f;
                                tempFloat2.y = (subChunkPosition.y + y + 1) * 0.04f;
                                xy = noise.snoise(tempFloat2);

                                tempFloat2.x = (subChunkPosition.x + x) * 0.04f;
                                tempFloat2.y = (subChunkPosition.z + z) * 0.04f;
                                xz = noise.snoise(tempFloat2);

                                tempFloat2.x = (subChunkPosition.y + y + 1) * 0.04f;
                                tempFloat2.y = (subChunkPosition.z + z) * 0.04f;
                                yz = noise.snoise(tempFloat2);

                                tempFloat2.x = (subChunkPosition.y + y + 1) * 0.04f;
                                tempFloat2.y = (subChunkPosition.x + x) * 0.04f;
                                yx = noise.snoise(tempFloat2);

                                tempFloat2.x = (subChunkPosition.z + z) * 0.04f;
                                tempFloat2.y = (subChunkPosition.x + x) * 0.04f;
                                zx = noise.snoise(tempFloat2);

                                tempFloat2.x = (subChunkPosition.z + z) * 0.04f;
                                tempFloat2.y = (subChunkPosition.y + y + 1) * 0.04f;
                                zy = noise.snoise(tempFloat2);

                                tempResult = (xy + xz + yz + yx + zx + zy) / 6;

                                if (tempResult > threshold)
                                {
                                    // has shape above = true - No block above

                                    if ((int)nativeHeightMap[heightIndex] == blockHeight)
                                    {
                                        // Grass top face

                                        tempVector2.x = nativeUvs[trianglesIterator + 4].x + 0.125f;
                                        tempVector2.y = 1 - nativeUvs[trianglesIterator + 4].y;
                                        nativeUvs[trianglesIterator + 4] = tempVector2;

                                        tempVector2.x = nativeUvs[trianglesIterator + 5].x + 0.125f;
                                        tempVector2.y = 1 - nativeUvs[trianglesIterator + 5].y;
                                        nativeUvs[trianglesIterator + 5] = tempVector2;

                                        tempVector2.x = nativeUvs[trianglesIterator + 8].x + 0.125f;
                                        tempVector2.y = 1 - nativeUvs[trianglesIterator + 8].y;
                                        nativeUvs[trianglesIterator + 8] = tempVector2;

                                        tempVector2.x = nativeUvs[trianglesIterator + 9].x + 0.125f;
                                        tempVector2.y = 1 - nativeUvs[trianglesIterator + 9].y;
                                        nativeUvs[trianglesIterator + 9] = tempVector2;
                                    }
                                    else
                                    {
                                        if (nativeHeightMap[heightIndex] <= blockHeight + 3)
                                        {
                                            // Grass top face
                                            tempVector2.x = nativeUvs[trianglesIterator + 4].x + 0.125f;
                                            tempVector2.y = 1 - nativeUvs[trianglesIterator + 4].y;
                                            nativeUvs[trianglesIterator + 4] = tempVector2;

                                            tempVector2.x = nativeUvs[trianglesIterator + 5].x + 0.125f;
                                            tempVector2.y = 1 - nativeUvs[trianglesIterator + 5].y;
                                            nativeUvs[trianglesIterator + 5] = tempVector2;

                                            tempVector2.x = nativeUvs[trianglesIterator + 8].x + 0.125f;
                                            tempVector2.y = 1 - nativeUvs[trianglesIterator + 8].y;
                                            nativeUvs[trianglesIterator + 8] = tempVector2;

                                            tempVector2.x = nativeUvs[trianglesIterator + 9].x + 0.125f;
                                            tempVector2.y = 1 - nativeUvs[trianglesIterator + 9].y;
                                            nativeUvs[trianglesIterator + 9] = tempVector2;
                                        }
                                        else
                                        {
                                            // Stone top face
                                            tempVector2.x = nativeUvs[trianglesIterator + 4].x + 0.375f;
                                            tempVector2.y = 1 - nativeUvs[trianglesIterator + 4].y;
                                            nativeUvs[trianglesIterator + 4] = tempVector2;

                                            tempVector2.x = nativeUvs[trianglesIterator + 5].x + 0.375f;
                                            tempVector2.y = 1 - nativeUvs[trianglesIterator + 5].y;
                                            nativeUvs[trianglesIterator + 5] = tempVector2;

                                            tempVector2.x = nativeUvs[trianglesIterator + 8].x + 0.375f;
                                            tempVector2.y = 1 - nativeUvs[trianglesIterator + 8].y;
                                            nativeUvs[trianglesIterator + 8] = tempVector2;

                                            tempVector2.x = nativeUvs[trianglesIterator + 9].x + 0.375f;
                                            tempVector2.y = 1 - nativeUvs[trianglesIterator + 9].y;
                                            nativeUvs[trianglesIterator + 9] = tempVector2;
                                        }
                                    }

                                    // Always has top as the block above using shape map to cut makes block above empty

                                    nativeTriangles.Add(trianglesIterator + 8);   // top
                                    nativeTriangles.Add(trianglesIterator + 4);
                                    nativeTriangles.Add(trianglesIterator + 5);
                                    nativeTriangles.Add(trianglesIterator + 8);
                                    nativeTriangles.Add(trianglesIterator + 5);
                                    nativeTriangles.Add(trianglesIterator + 9);
                                }
                                else 
                                {
                                    if ((int)nativeHeightMap[heightIndex] == blockHeight)
                                    {
                                        // Grass top face
                                        tempVector2.x = nativeUvs[trianglesIterator + 4].x + 0.125f;
                                        tempVector2.y = 1 - nativeUvs[trianglesIterator + 4].y;
                                        nativeUvs[trianglesIterator + 4] = tempVector2;

                                        tempVector2.x = nativeUvs[trianglesIterator + 5].x + 0.125f;
                                        tempVector2.y = 1 - nativeUvs[trianglesIterator + 5].y;
                                        nativeUvs[trianglesIterator + 5] = tempVector2;

                                        tempVector2.x = nativeUvs[trianglesIterator + 8].x + 0.125f;
                                        tempVector2.y = 1 - nativeUvs[trianglesIterator + 8].y;
                                        nativeUvs[trianglesIterator + 8] = tempVector2;

                                        tempVector2.x = nativeUvs[trianglesIterator + 9].x + 0.125f;
                                        tempVector2.y = 1 - nativeUvs[trianglesIterator + 9].y;
                                        nativeUvs[trianglesIterator + 9] = tempVector2;

                                        nativeTriangles.Add(trianglesIterator + 8);   // top
                                        nativeTriangles.Add(trianglesIterator + 4);
                                        nativeTriangles.Add(trianglesIterator + 5);
                                        nativeTriangles.Add(trianglesIterator + 8);
                                        nativeTriangles.Add(trianglesIterator + 5);
                                        nativeTriangles.Add(trianglesIterator + 9);
                                    }
                                }
                            }
                            else
                            {
                                if (shapeMap[iterator + 256] == true)
                                {
                                    // has shape above = true - No block above

                                    if ((int)nativeHeightMap[heightIndex] == blockHeight)
                                    {
                                        // Grass top face
                                        tempVector2.x = nativeUvs[trianglesIterator + 4].x + 0.125f;
                                        tempVector2.y = 1 - nativeUvs[trianglesIterator + 4].y;
                                        nativeUvs[trianglesIterator + 4] = tempVector2;

                                        tempVector2.x = nativeUvs[trianglesIterator + 5].x + 0.125f;
                                        tempVector2.y = 1 - nativeUvs[trianglesIterator + 5].y;
                                        nativeUvs[trianglesIterator + 5] = tempVector2;

                                        tempVector2.x = nativeUvs[trianglesIterator + 8].x + 0.125f;
                                        tempVector2.y = 1 - nativeUvs[trianglesIterator + 8].y;
                                        nativeUvs[trianglesIterator + 8] = tempVector2;

                                        tempVector2.x = nativeUvs[trianglesIterator + 9].x + 0.125f;
                                        tempVector2.y = 1 - nativeUvs[trianglesIterator + 9].y;
                                        nativeUvs[trianglesIterator + 9] = tempVector2;
                                    }
                                    else
                                    {
                                        if (nativeHeightMap[heightIndex] <= blockHeight + 3)
                                        {
                                            // Grass top face
                                            tempVector2.x = nativeUvs[trianglesIterator + 4].x + 0.125f;
                                            tempVector2.y = 1 - nativeUvs[trianglesIterator + 4].y;
                                            nativeUvs[trianglesIterator + 4] = tempVector2;

                                            tempVector2.x = nativeUvs[trianglesIterator + 5].x + 0.125f;
                                            tempVector2.y = 1 - nativeUvs[trianglesIterator + 5].y;
                                            nativeUvs[trianglesIterator + 5] = tempVector2;

                                            tempVector2.x = nativeUvs[trianglesIterator + 8].x + 0.125f;
                                            tempVector2.y = 1 - nativeUvs[trianglesIterator + 8].y;
                                            nativeUvs[trianglesIterator + 8] = tempVector2;

                                            tempVector2.x = nativeUvs[trianglesIterator + 9].x + 0.125f;
                                            tempVector2.y = 1 - nativeUvs[trianglesIterator + 9].y;
                                            nativeUvs[trianglesIterator + 9] = tempVector2;
                                        }
                                        else
                                        {
                                            // Stone top face
                                            tempVector2.x = nativeUvs[trianglesIterator + 4].x + 0.375f;
                                            tempVector2.y = 1 - nativeUvs[trianglesIterator + 4].y;
                                            nativeUvs[trianglesIterator + 4] = tempVector2;

                                            tempVector2.x = nativeUvs[trianglesIterator + 5].x + 0.375f;
                                            tempVector2.y = 1 - nativeUvs[trianglesIterator + 5].y;
                                            nativeUvs[trianglesIterator + 5] = tempVector2;

                                            tempVector2.x = nativeUvs[trianglesIterator + 8].x + 0.375f;
                                            tempVector2.y = 1 - nativeUvs[trianglesIterator + 8].y;
                                            nativeUvs[trianglesIterator + 8] = tempVector2;

                                            tempVector2.x = nativeUvs[trianglesIterator + 9].x + 0.375f;
                                            tempVector2.y = 1 - nativeUvs[trianglesIterator + 9].y;
                                            nativeUvs[trianglesIterator + 9] = tempVector2;
                                        }
                                    }

                                    // Always has top as the block above using shape map to cut makes block above empty

                                    nativeTriangles.Add(trianglesIterator + 8);   // top
                                    nativeTriangles.Add(trianglesIterator + 4);
                                    nativeTriangles.Add(trianglesIterator + 5);
                                    nativeTriangles.Add(trianglesIterator + 8);
                                    nativeTriangles.Add(trianglesIterator + 5);
                                    nativeTriangles.Add(trianglesIterator + 9);
                                }
                                else
                                {
                                    if ((int)nativeHeightMap[heightIndex] == blockHeight)
                                    {
                                        // Grass top face
                                        tempVector2.x = nativeUvs[trianglesIterator + 4].x + 0.125f;
                                        tempVector2.y = 1 - nativeUvs[trianglesIterator + 4].y;
                                        nativeUvs[trianglesIterator + 4] = tempVector2;

                                        tempVector2.x = nativeUvs[trianglesIterator + 5].x + 0.125f;
                                        tempVector2.y = 1 - nativeUvs[trianglesIterator + 5].y;
                                        nativeUvs[trianglesIterator + 5] = tempVector2;

                                        tempVector2.x = nativeUvs[trianglesIterator + 8].x + 0.125f;
                                        tempVector2.y = 1 - nativeUvs[trianglesIterator + 8].y;
                                        nativeUvs[trianglesIterator + 8] = tempVector2;

                                        tempVector2.x = nativeUvs[trianglesIterator + 9].x + 0.125f;
                                        tempVector2.y = 1 - nativeUvs[trianglesIterator + 9].y;
                                        nativeUvs[trianglesIterator + 9] = tempVector2;

                                        nativeTriangles.Add(trianglesIterator + 8);   // top
                                        nativeTriangles.Add(trianglesIterator + 4);
                                        nativeTriangles.Add(trianglesIterator + 5);
                                        nativeTriangles.Add(trianglesIterator + 8);
                                        nativeTriangles.Add(trianglesIterator + 5);
                                        nativeTriangles.Add(trianglesIterator + 9);
                                    }
                                }
                            }

                            if (northEdgeDetected)
                            {
                                // if north edge need to use 3d perlin noise check to see if there is a block in front. (selection is off of block map)

                                if (hasNorthNeighbour)
                                {
                                    // 3D perlin noise.
                                    tempFloat2.x = (subChunkPosition.x + x) * 0.04f;
                                    tempFloat2.y = (subChunkPosition.y + y) * 0.04f;
                                    xy = noise.snoise(tempFloat2);

                                    tempFloat2.x = (subChunkPosition.x + x) * 0.04f;
                                    tempFloat2.y = (subChunkPosition.z + z + 1) * 0.04f;
                                    xz = noise.snoise(tempFloat2);

                                    tempFloat2.x = (subChunkPosition.y + y) * 0.04f;
                                    tempFloat2.y = (subChunkPosition.z + z + 1) * 0.04f;
                                    yz = noise.snoise(tempFloat2);

                                    tempFloat2.x = (subChunkPosition.y + y) * 0.04f;
                                    tempFloat2.y = (subChunkPosition.x + x) * 0.04f;
                                    yx = noise.snoise(tempFloat2);

                                    tempFloat2.x = (subChunkPosition.z + z + 1) * 0.04f;
                                    tempFloat2.y = (subChunkPosition.x + x) * 0.04f;
                                    zx = noise.snoise(tempFloat2);

                                    tempFloat2.x = (subChunkPosition.z + z + 1) * 0.04f;
                                    tempFloat2.y = (subChunkPosition.y + y) * 0.04f;
                                    zy = noise.snoise(tempFloat2);

                                    tempResult = (xy + xz + yz + yx + zx + zy) / 6;

                                    if (tempResult > threshold)
                                    {
                                        if (shapeMap[iterator + 256] == true)
                                        {
                                            if (blockHeight == (int)nativeHeightMap[heightIndex])
                                            {
                                                // Grass dirt face
                                                tempVector2.x = 0.125f;
                                                tempVector2.y = 0.125f;
                                                nativeUvs[trianglesIterator + 0] += tempVector2;
                                                nativeUvs[trianglesIterator + 1] += tempVector2;
                                                nativeUvs[trianglesIterator + 2] += tempVector2;
                                                nativeUvs[trianglesIterator + 3] += tempVector2;
                                            }
                                            else
                                            {
                                                if (blockHeight + 3 >= nativeHeightMap[heightIndex])
                                                {
                                                    // Grass dirt face
                                                    tempVector2.x = 0.125f;
                                                    tempVector2.y = 0.125f;
                                                    nativeUvs[trianglesIterator + 0] += tempVector2;
                                                    nativeUvs[trianglesIterator + 1] += tempVector2;
                                                    nativeUvs[trianglesIterator + 2] += tempVector2;
                                                    nativeUvs[trianglesIterator + 3] += tempVector2;
                                                }
                                                else
                                                {
                                                    // Stone face default uvs
                                                    tempVector2.x = 0.125f;
                                                    tempVector2.y = 0.0f;
                                                    nativeUvs[trianglesIterator + 0] += tempVector2;
                                                    nativeUvs[trianglesIterator + 1] += tempVector2;
                                                    nativeUvs[trianglesIterator + 2] += tempVector2;
                                                    nativeUvs[trianglesIterator + 3] += tempVector2;
                                                }
                                            }

                                            if (!eastEdgeDetected
                                                 && !westEdgeDetected)
                                            {
                                                nativeTriangles.Add(trianglesIterator + 0);   //front
                                                nativeTriangles.Add(trianglesIterator + 2);
                                                nativeTriangles.Add(trianglesIterator + 3);
                                                nativeTriangles.Add(trianglesIterator + 0);
                                                nativeTriangles.Add(trianglesIterator + 3);
                                                nativeTriangles.Add(trianglesIterator + 1);
                                            }
                                            else
                                            {
                                                if (eastEdgeDetected)
                                                {
                                                    if (hasNorthEastNeighbour)
                                                    {
                                                        nativeTriangles.Add(trianglesIterator + 0);   //front
                                                        nativeTriangles.Add(trianglesIterator + 2);
                                                        nativeTriangles.Add(trianglesIterator + 3);
                                                        nativeTriangles.Add(trianglesIterator + 0);
                                                        nativeTriangles.Add(trianglesIterator + 3);
                                                        nativeTriangles.Add(trianglesIterator + 1);
                                                    }
                                                }
                                                else
                                                {
                                                    if (westEdgeDetected)
                                                    {
                                                        if (hasNorthWestNeighbour)
                                                        {
                                                            nativeTriangles.Add(trianglesIterator + 0);   //front
                                                            nativeTriangles.Add(trianglesIterator + 2);
                                                            nativeTriangles.Add(trianglesIterator + 3);
                                                            nativeTriangles.Add(trianglesIterator + 0);
                                                            nativeTriangles.Add(trianglesIterator + 3);
                                                            nativeTriangles.Add(trianglesIterator + 1);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (blockHeight == (int)nativeHeightMap[heightIndex])
                                            {
                                                // Grass dirt face
                                                tempVector2.x = 0.125f;
                                                tempVector2.y = 0.125f;
                                                nativeUvs[trianglesIterator + 0] += tempVector2;
                                                nativeUvs[trianglesIterator + 1] += tempVector2;
                                                nativeUvs[trianglesIterator + 2] += tempVector2;
                                                nativeUvs[trianglesIterator + 3] += tempVector2;

                                                nativeTriangles.Add(trianglesIterator + 0);   //front
                                                nativeTriangles.Add(trianglesIterator + 2);
                                                nativeTriangles.Add(trianglesIterator + 3);
                                                nativeTriangles.Add(trianglesIterator + 0);
                                                nativeTriangles.Add(trianglesIterator + 3);
                                                nativeTriangles.Add(trianglesIterator + 1);
                                            }
                                            else
                                            {
                                                if (blockHeight + 3 >= nativeHeightMap[heightIndex])
                                                {
                                                    // dirt face
                                                    tempVector2.x = 0.0f;
                                                    tempVector2.y = 0.125f;
                                                    nativeUvs[trianglesIterator + 0] += tempVector2;
                                                    nativeUvs[trianglesIterator + 1] += tempVector2;
                                                    nativeUvs[trianglesIterator + 2] += tempVector2;
                                                    nativeUvs[trianglesIterator + 3] += tempVector2;

                                                    nativeTriangles.Add(trianglesIterator + 0);   //front
                                                    nativeTriangles.Add(trianglesIterator + 2);
                                                    nativeTriangles.Add(trianglesIterator + 3);
                                                    nativeTriangles.Add(trianglesIterator + 0);
                                                    nativeTriangles.Add(trianglesIterator + 3);
                                                    nativeTriangles.Add(trianglesIterator + 1);
                                                }
                                                else
                                                {
                                                    // Stone face default uvs

                                                    if (!eastEdgeDetected
                                                        && !westEdgeDetected)
                                                    {
                                                        nativeTriangles.Add(trianglesIterator + 0);   //front
                                                        nativeTriangles.Add(trianglesIterator + 2);
                                                        nativeTriangles.Add(trianglesIterator + 3);
                                                        nativeTriangles.Add(trianglesIterator + 0);
                                                        nativeTriangles.Add(trianglesIterator + 3);
                                                        nativeTriangles.Add(trianglesIterator + 1);
                                                    }
                                                    else
                                                    {
                                                        if (eastEdgeDetected)
                                                        {
                                                            if (hasNorthEastNeighbour)
                                                            {
                                                                nativeTriangles.Add(trianglesIterator + 0);   //front
                                                                nativeTriangles.Add(trianglesIterator + 2);
                                                                nativeTriangles.Add(trianglesIterator + 3);
                                                                nativeTriangles.Add(trianglesIterator + 0);
                                                                nativeTriangles.Add(trianglesIterator + 3);
                                                                nativeTriangles.Add(trianglesIterator + 1); 
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (westEdgeDetected)
                                                            {
                                                                if (hasNorthWestNeighbour)
                                                                {
                                                                    nativeTriangles.Add(trianglesIterator + 0);   //front
                                                                    nativeTriangles.Add(trianglesIterator + 2);
                                                                    nativeTriangles.Add(trianglesIterator + 3);
                                                                    nativeTriangles.Add(trianglesIterator + 0);
                                                                    nativeTriangles.Add(trianglesIterator + 3);
                                                                    nativeTriangles.Add(trianglesIterator + 1);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }                       
                                    }
                                    else
                                    {
                                        if (nativeNorthHeightMap[northHeightIndex] > blockHeight)
                                        {
                                            // black face
                                            tempVector2.x = nativeUvs[trianglesIterator + 0].x;
                                            tempVector2.y = 1 - nativeUvs[trianglesIterator + 0].y;
                                            nativeUvs[trianglesIterator + 0] = tempVector2;

                                            tempVector2.x = nativeUvs[trianglesIterator + 1].x;
                                            tempVector2.y = 1 - nativeUvs[trianglesIterator + 1].y;
                                            nativeUvs[trianglesIterator + 1] = tempVector2;

                                            tempVector2.x = nativeUvs[trianglesIterator + 2].x;
                                            tempVector2.y = 1 - nativeUvs[trianglesIterator + 2].y;
                                            nativeUvs[trianglesIterator + 2] = tempVector2;

                                            tempVector2.x = nativeUvs[trianglesIterator + 3].x;
                                            tempVector2.y = 1 - nativeUvs[trianglesIterator + 3].y;
                                            nativeUvs[trianglesIterator + 3] = tempVector2;
                                            
                                             nativeTriangles.Add(trianglesIterator + 0);   //front
                                                nativeTriangles.Add(trianglesIterator + 2);
                                                nativeTriangles.Add(trianglesIterator + 3);
                                                nativeTriangles.Add(trianglesIterator + 0);
                                                nativeTriangles.Add(trianglesIterator + 3);
                                                nativeTriangles.Add(trianglesIterator + 1);
                                        }
                                        else
                                        {
                                            if (blockHeight == (int)nativeHeightMap[heightIndex])
                                            {
                                                // Grass dirt face
                                                tempVector2.x = 0.125f;
                                                tempVector2.y = 0.125f;
                                                nativeUvs[trianglesIterator + 0] += tempVector2;
                                                nativeUvs[trianglesIterator + 1] += tempVector2;
                                                nativeUvs[trianglesIterator + 2] += tempVector2;
                                                nativeUvs[trianglesIterator + 3] += tempVector2;

                                                nativeTriangles.Add(trianglesIterator + 0);   //front
                                                nativeTriangles.Add(trianglesIterator + 2);
                                                nativeTriangles.Add(trianglesIterator + 3);
                                                nativeTriangles.Add(trianglesIterator + 0);
                                                nativeTriangles.Add(trianglesIterator + 3);
                                                nativeTriangles.Add(trianglesIterator + 1);
                                            }
                                            else
                                            {
                                                if (blockHeight + 3 >= nativeHeightMap[heightIndex])
                                                {
                                                    // dirt face
                                                    tempVector2.x = 0.0f;
                                                    tempVector2.y = 0.125f;
                                                    nativeUvs[trianglesIterator + 0] += tempVector2;
                                                    nativeUvs[trianglesIterator + 1] += tempVector2;
                                                    nativeUvs[trianglesIterator + 2] += tempVector2;
                                                    nativeUvs[trianglesIterator + 3] += tempVector2;

                                                    nativeTriangles.Add(trianglesIterator + 0);   //front
                                                    nativeTriangles.Add(trianglesIterator + 2);
                                                    nativeTriangles.Add(trianglesIterator + 3);
                                                    nativeTriangles.Add(trianglesIterator + 0);
                                                    nativeTriangles.Add(trianglesIterator + 3);
                                                    nativeTriangles.Add(trianglesIterator + 1);
                                                }
                                                else
                                                {
                                                    // Stone face default uvs

                                                    nativeTriangles.Add(trianglesIterator + 0);   //front
                                                    nativeTriangles.Add(trianglesIterator + 2);
                                                    nativeTriangles.Add(trianglesIterator + 3);
                                                    nativeTriangles.Add(trianglesIterator + 0);
                                                    nativeTriangles.Add(trianglesIterator + 3);
                                                    nativeTriangles.Add(trianglesIterator + 1);
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (nativeNorthHeightMap[northHeightIndex] > blockHeight)
                                    {
                                        // black face
                                        tempVector2.x = nativeUvs[trianglesIterator + 0].x;
                                        tempVector2.y = 1 - nativeUvs[trianglesIterator + 0].y;
                                        nativeUvs[trianglesIterator + 0] = tempVector2;

                                        tempVector2.x = nativeUvs[trianglesIterator + 1].x;
                                        tempVector2.y = 1 - nativeUvs[trianglesIterator + 1].y;
                                        nativeUvs[trianglesIterator + 1] = tempVector2;

                                        tempVector2.x = nativeUvs[trianglesIterator + 2].x;
                                        tempVector2.y = 1 - nativeUvs[trianglesIterator + 2].y;
                                        nativeUvs[trianglesIterator + 2] = tempVector2;

                                        tempVector2.x = nativeUvs[trianglesIterator + 3].x;
                                        tempVector2.y = 1 - nativeUvs[trianglesIterator + 3].y;
                                        nativeUvs[trianglesIterator + 3] = tempVector2;

                                        nativeTriangles.Add(trianglesIterator + 0);   //front
                                        nativeTriangles.Add(trianglesIterator + 2);
                                        nativeTriangles.Add(trianglesIterator + 3);
                                        nativeTriangles.Add(trianglesIterator + 0);
                                        nativeTriangles.Add(trianglesIterator + 3);
                                        nativeTriangles.Add(trianglesIterator + 1);
                                    }
                                }
                            }
                            else
                            {
                                if (shapeMap[iterator + 1] == true)
                                {
                                    if (shapeMap[iterator + 256] == false)
                                    {
                                        if (blockHeight == (int)nativeHeightMap[heightIndex])
                                        {
                                            // Grass dirt face
                                            tempVector2.x = 0.125f;
                                            tempVector2.y = 0.125f;
                                            nativeUvs[trianglesIterator + 0] += tempVector2;
                                            nativeUvs[trianglesIterator + 1] += tempVector2;
                                            nativeUvs[trianglesIterator + 2] += tempVector2;
                                            nativeUvs[trianglesIterator + 3] += tempVector2;
                                        }
                                        else
                                        {
                                            if (blockHeight + 3 >= nativeHeightMap[heightIndex])
                                            {
                                                // dirt face
                                                tempVector2.x = 0.0f;
                                                tempVector2.y = 0.125f;
                                                nativeUvs[trianglesIterator + 0] += tempVector2;
                                                nativeUvs[trianglesIterator + 1] += tempVector2;
                                                nativeUvs[trianglesIterator + 2] += tempVector2;
                                                nativeUvs[trianglesIterator + 3] += tempVector2;
                                            }
                                            else
                                            {
                                                // Stone face default uvs
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (blockHeight == (int)nativeHeightMap[heightIndex])
                                        {
                                            // Grass dirt face
                                            tempVector2.x = 0.125f;
                                            tempVector2.y = 0.125f;
                                            nativeUvs[trianglesIterator + 0] += tempVector2;
                                            nativeUvs[trianglesIterator + 1] += tempVector2;
                                            nativeUvs[trianglesIterator + 2] += tempVector2;
                                            nativeUvs[trianglesIterator + 3] += tempVector2;

                                        }
                                        else
                                        {
                                            if (blockHeight + 3 >= nativeHeightMap[heightIndex])
                                            {
                                                // Grass dirt face
                                                tempVector2.x = 0.125f;
                                                tempVector2.y = 0.125f;
                                                nativeUvs[trianglesIterator + 0] += tempVector2;
                                                nativeUvs[trianglesIterator + 1] += tempVector2;
                                                nativeUvs[trianglesIterator + 2] += tempVector2;
                                                nativeUvs[trianglesIterator + 3] += tempVector2;
                                            }
                                            else
                                            {
                                                // Stone face default uvs
                                                tempVector2.x = 0.125f;
                                                tempVector2.y = 0.0f;
                                                nativeUvs[trianglesIterator + 0] += tempVector2;
                                                nativeUvs[trianglesIterator + 1] += tempVector2;
                                                nativeUvs[trianglesIterator + 2] += tempVector2;
                                                nativeUvs[trianglesIterator + 3] += tempVector2;
                                            }
                                        }
                                    }

                                    // because the is a shape map infront there will always be face north

                                    nativeTriangles.Add(trianglesIterator + 0);   //front
                                    nativeTriangles.Add(trianglesIterator + 2);
                                    nativeTriangles.Add(trianglesIterator + 3);
                                    nativeTriangles.Add(trianglesIterator + 0);
                                    nativeTriangles.Add(trianglesIterator + 3);
                                    nativeTriangles.Add(trianglesIterator + 1);
                                }
                                else    // default process
                                {
                                    if (nativeHeightMap[northHeightIndex] <= blockHeight)
                                    {
                                        nativeTriangles.Add(trianglesIterator + 0);   //front
                                        nativeTriangles.Add(trianglesIterator + 2);
                                        nativeTriangles.Add(trianglesIterator + 3);
                                        nativeTriangles.Add(trianglesIterator + 0);
                                        nativeTriangles.Add(trianglesIterator + 3);
                                        nativeTriangles.Add(trianglesIterator + 1);
                                    }

                                    if (blockHeight == (int)nativeHeightMap[heightIndex])
                                    {
                                        // Grass dirt face
                                        tempVector2.x = 0.125f;
                                        tempVector2.y = 0.125f;
                                        nativeUvs[trianglesIterator + 0] += tempVector2;
                                        nativeUvs[trianglesIterator + 1] += tempVector2;
                                        nativeUvs[trianglesIterator + 2] += tempVector2;
                                        nativeUvs[trianglesIterator + 3] += tempVector2;
                                    }
                                    else
                                    {
                                        if (blockHeight + 3 >= nativeHeightMap[heightIndex])
                                        {
                                            // dirt face
                                            tempVector2.x = 0.0f;
                                            tempVector2.y = 0.125f;
                                            nativeUvs[trianglesIterator + 0] += tempVector2;
                                            nativeUvs[trianglesIterator + 1] += tempVector2;
                                            nativeUvs[trianglesIterator + 2] += tempVector2;
                                            nativeUvs[trianglesIterator + 3] += tempVector2;
                                        }
                                        else
                                        { 
                                            // Stone face default uvs
                                        }
                                    }
                                }
                            }

                            if (southEdgeDetected)
                            {
                                // if south edge need to use 3d perlin noise check to see if there is a block in behind. (selection is off of block map)

                                if (hasSouthNeighbour)
                                {
                                    // 3D perlin noise.

                                    tempFloat2.x = (subChunkPosition.x + x) * 0.04f;
                                    tempFloat2.y = (subChunkPosition.y + y) * 0.04f;
                                    xy = noise.snoise(tempFloat2);

                                    tempFloat2.x = (subChunkPosition.x + x) * 0.04f;
                                    tempFloat2.y = (subChunkPosition.z + z - 1) * 0.04f;
                                    xz = noise.snoise(tempFloat2);

                                    tempFloat2.x = (subChunkPosition.y + y) * 0.04f;
                                    tempFloat2.y = (subChunkPosition.z + z - 1) * 0.04f;
                                    yz = noise.snoise(tempFloat2);

                                    tempFloat2.x = (subChunkPosition.y + y) * 0.04f;
                                    tempFloat2.y = (subChunkPosition.x + x) * 0.04f;
                                    yx = noise.snoise(tempFloat2);

                                    tempFloat2.x = (subChunkPosition.z + z - 1) * 0.04f;
                                    tempFloat2.y = (subChunkPosition.x + x) * 0.04f;
                                    zx = noise.snoise(tempFloat2);

                                    tempFloat2.x = (subChunkPosition.z + z - 1) * 0.04f;
                                    tempFloat2.y = (subChunkPosition.y + y) * 0.04f;
                                    zy = noise.snoise(tempFloat2);

                                    tempResult = (xy + xz + yz + yx + zx + zy) / 6;

                                    if (tempResult > threshold)
                                    {
                                        if (shapeMap[iterator + 256] == true)
                                        {
                                            if (blockHeight == (int)nativeHeightMap[heightIndex])
                                            {
                                                // Grass dirt face
                                                tempVector2.x = 0.125f;
                                                tempVector2.y = 0.125f;
                                                nativeUvs[trianglesIterator + 6] += tempVector2;
                                                nativeUvs[trianglesIterator + 7] += tempVector2;
                                                nativeUvs[trianglesIterator + 10] += tempVector2;
                                                nativeUvs[trianglesIterator + 11] += tempVector2;
                                            }
                                            else
                                            {
                                                if (blockHeight + 3 >= nativeHeightMap[heightIndex])
                                                {
                                                    // Grass dirt face
                                                    tempVector2.x = 0.125f;
                                                    tempVector2.y = 0.125f;
                                                    nativeUvs[trianglesIterator + 6] += tempVector2;
                                                    nativeUvs[trianglesIterator + 7] += tempVector2;
                                                    nativeUvs[trianglesIterator + 10] += tempVector2;
                                                    nativeUvs[trianglesIterator + 11] += tempVector2;
                                                }
                                                else
                                                {
                                                    // Stone face default uvs
                                                    tempVector2.x = 0.125f;
                                                    tempVector2.y = 0f;
                                                    nativeUvs[trianglesIterator + 6] += tempVector2;
                                                    nativeUvs[trianglesIterator + 7] += tempVector2;
                                                    nativeUvs[trianglesIterator + 10] += tempVector2;
                                                    nativeUvs[trianglesIterator + 11] += tempVector2;
                                                }
                                            }

                                            if (!eastEdgeDetected
                                            && !westEdgeDetected)
                                            {
                                                nativeTriangles.Add(trianglesIterator + 10);   // back
                                                nativeTriangles.Add(trianglesIterator + 6);
                                                nativeTriangles.Add(trianglesIterator + 7);
                                                nativeTriangles.Add(trianglesIterator + 10);
                                                nativeTriangles.Add(trianglesIterator + 7);
                                                nativeTriangles.Add(trianglesIterator + 11);
                                            }
                                            else
                                            {
                                                if (eastEdgeDetected)
                                                {
                                                    if (hasSouthEastNeighbour)
                                                    {
                                                        nativeTriangles.Add(trianglesIterator + 10);   // back
                                                        nativeTriangles.Add(trianglesIterator + 6);
                                                        nativeTriangles.Add(trianglesIterator + 7);
                                                        nativeTriangles.Add(trianglesIterator + 10);
                                                        nativeTriangles.Add(trianglesIterator + 7);
                                                        nativeTriangles.Add(trianglesIterator + 11);
                                                    }
                                                }
                                                else
                                                {
                                                    if (westEdgeDetected)
                                                    {
                                                        if (hasSouthWestNeighbour)
                                                        {
                                                            nativeTriangles.Add(trianglesIterator + 10);   // back
                                                            nativeTriangles.Add(trianglesIterator + 6);
                                                            nativeTriangles.Add(trianglesIterator + 7);
                                                            nativeTriangles.Add(trianglesIterator + 10);
                                                            nativeTriangles.Add(trianglesIterator + 7);
                                                            nativeTriangles.Add(trianglesIterator + 11);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (blockHeight == (int)nativeHeightMap[heightIndex])
                                            {
                                                // Grass dirt face
                                                tempVector2.x = 0.125f;
                                                tempVector2.y = 0.125f;
                                                nativeUvs[trianglesIterator + 6] += tempVector2;
                                                nativeUvs[trianglesIterator + 7] += tempVector2;
                                                nativeUvs[trianglesIterator + 10] += tempVector2;
                                                nativeUvs[trianglesIterator + 11] += tempVector2;

                                                nativeTriangles.Add(trianglesIterator + 10);   // back
                                                nativeTriangles.Add(trianglesIterator + 6);
                                                nativeTriangles.Add(trianglesIterator + 7);
                                                nativeTriangles.Add(trianglesIterator + 10);
                                                nativeTriangles.Add(trianglesIterator + 7);
                                                nativeTriangles.Add(trianglesIterator + 11);
                                            }
                                            else
                                            {
                                                if (blockHeight + 3 >= nativeHeightMap[heightIndex])
                                                {
                                                    // dirt face
                                                    tempVector2.x = 0f;
                                                    tempVector2.y = 0.125f;
                                                    nativeUvs[trianglesIterator + 6] += tempVector2;
                                                    nativeUvs[trianglesIterator + 7] += tempVector2;
                                                    nativeUvs[trianglesIterator + 10] += tempVector2;
                                                    nativeUvs[trianglesIterator + 11] += tempVector2;

                                                    nativeTriangles.Add(trianglesIterator + 10);   // back
                                                    nativeTriangles.Add(trianglesIterator + 6);
                                                    nativeTriangles.Add(trianglesIterator + 7);
                                                    nativeTriangles.Add(trianglesIterator + 10);
                                                    nativeTriangles.Add(trianglesIterator + 7);
                                                    nativeTriangles.Add(trianglesIterator + 11);
                                                }
                                                else
                                                {
                                                    // Stone face default uvs

                                                    if (!eastEdgeDetected
                                                        && !westEdgeDetected)
                                                    {
                                                        nativeTriangles.Add(trianglesIterator + 10);   // back
                                                        nativeTriangles.Add(trianglesIterator + 6);
                                                        nativeTriangles.Add(trianglesIterator + 7);
                                                        nativeTriangles.Add(trianglesIterator + 10);
                                                        nativeTriangles.Add(trianglesIterator + 7);
                                                        nativeTriangles.Add(trianglesIterator + 11);
                                                    }
                                                    else
                                                    {
                                                        if (eastEdgeDetected)
                                                        {
                                                            if (hasSouthEastNeighbour)
                                                            {
                                                                nativeTriangles.Add(trianglesIterator + 10);   // back
                                                                nativeTriangles.Add(trianglesIterator + 6);
                                                                nativeTriangles.Add(trianglesIterator + 7);
                                                                nativeTriangles.Add(trianglesIterator + 10);
                                                                nativeTriangles.Add(trianglesIterator + 7);
                                                                nativeTriangles.Add(trianglesIterator + 11);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (westEdgeDetected)
                                                            {
                                                                if (hasSouthWestNeighbour)
                                                                {
                                                                    nativeTriangles.Add(trianglesIterator + 10);   // back
                                                                    nativeTriangles.Add(trianglesIterator + 6);
                                                                    nativeTriangles.Add(trianglesIterator + 7);
                                                                    nativeTriangles.Add(trianglesIterator + 10);
                                                                    nativeTriangles.Add(trianglesIterator + 7);
                                                                    nativeTriangles.Add(trianglesIterator + 11);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {

                                        if (nativeSouthHeightMap[southHeightIndex] > blockHeight)
                                        {
                                            // black face
                                            tempVector2.x = nativeUvs[trianglesIterator + 6].x;
                                            tempVector2.y = 1 - nativeUvs[trianglesIterator + 6].y;
                                            nativeUvs[trianglesIterator + 6] = tempVector2;

                                            tempVector2.x = nativeUvs[trianglesIterator + 7].x;
                                            tempVector2.y = 1 - nativeUvs[trianglesIterator + 7].y;
                                            nativeUvs[trianglesIterator + 7] = tempVector2;

                                            tempVector2.x = nativeUvs[trianglesIterator + 10].x;
                                            tempVector2.y = 1 - nativeUvs[trianglesIterator + 10].y;
                                            nativeUvs[trianglesIterator + 10] = tempVector2;

                                            tempVector2.x = nativeUvs[trianglesIterator + 11].x;
                                            tempVector2.y = 1 - nativeUvs[trianglesIterator + 11].y;
                                            nativeUvs[trianglesIterator + 11] = tempVector2;

                                            nativeTriangles.Add(trianglesIterator + 10);   // back
                                                nativeTriangles.Add(trianglesIterator + 6);
                                                nativeTriangles.Add(trianglesIterator + 7);
                                                nativeTriangles.Add(trianglesIterator + 10);
                                                nativeTriangles.Add(trianglesIterator + 7);
                                                nativeTriangles.Add(trianglesIterator + 11);
                                        }
                                        else
                                        {
                                            if (blockHeight == (int)nativeHeightMap[heightIndex])
                                            {
                                                // Grass dirt face
                                                tempVector2.x = 0.125f;
                                                tempVector2.y = 0.125f;
                                                nativeUvs[trianglesIterator + 6] += tempVector2;
                                                nativeUvs[trianglesIterator + 7] += tempVector2;
                                                nativeUvs[trianglesIterator + 10] += tempVector2;
                                                nativeUvs[trianglesIterator + 11] += tempVector2;

                                                nativeTriangles.Add(trianglesIterator + 10);   // back
                                                nativeTriangles.Add(trianglesIterator + 6);
                                                nativeTriangles.Add(trianglesIterator + 7);
                                                nativeTriangles.Add(trianglesIterator + 10);
                                                nativeTriangles.Add(trianglesIterator + 7);
                                                nativeTriangles.Add(trianglesIterator + 11);
                                            }
                                            else
                                            {
                                                if (blockHeight + 3 >= nativeHeightMap[heightIndex])
                                                {
                                                    // dirt face
                                                    tempVector2.x = 0f;
                                                    tempVector2.y = 0.125f;
                                                    nativeUvs[trianglesIterator + 6] += tempVector2;
                                                    nativeUvs[trianglesIterator + 7] += tempVector2;
                                                    nativeUvs[trianglesIterator + 10] += tempVector2;
                                                    nativeUvs[trianglesIterator + 11] += tempVector2;

                                                    nativeTriangles.Add(trianglesIterator + 10);   // back
                                                    nativeTriangles.Add(trianglesIterator + 6);
                                                    nativeTriangles.Add(trianglesIterator + 7);
                                                    nativeTriangles.Add(trianglesIterator + 10);
                                                    nativeTriangles.Add(trianglesIterator + 7);
                                                    nativeTriangles.Add(trianglesIterator + 11);
                                                }
                                                else
                                                {
                                                    // Stone face default uvs

                                                    nativeTriangles.Add(trianglesIterator + 10);   // back
                                                    nativeTriangles.Add(trianglesIterator + 6);
                                                    nativeTriangles.Add(trianglesIterator + 7);
                                                    nativeTriangles.Add(trianglesIterator + 10);
                                                    nativeTriangles.Add(trianglesIterator + 7);
                                                    nativeTriangles.Add(trianglesIterator + 11);
                                                }
                                            }
                                        }
                                    }                                    
                                }
                                else
                                {
                                    if (nativeSouthHeightMap[southHeightIndex] > blockHeight)
                                    {
                                        // black face
                                        tempVector2.x = nativeUvs[trianglesIterator + 6].x;
                                        tempVector2.y = 1 - nativeUvs[trianglesIterator + 6].y;
                                        nativeUvs[trianglesIterator + 6] = tempVector2;

                                        tempVector2.x = nativeUvs[trianglesIterator + 7].x;
                                        tempVector2.y = 1 - nativeUvs[trianglesIterator + 7].y;
                                        nativeUvs[trianglesIterator + 7] = tempVector2;

                                        tempVector2.x = nativeUvs[trianglesIterator + 10].x;
                                        tempVector2.y = 1 - nativeUvs[trianglesIterator + 10].y;
                                        nativeUvs[trianglesIterator + 10] = tempVector2;

                                        tempVector2.x = nativeUvs[trianglesIterator + 11].x;
                                        tempVector2.y = 1 - nativeUvs[trianglesIterator + 11].y;
                                        nativeUvs[trianglesIterator + 11] = tempVector2;

                                        nativeTriangles.Add(trianglesIterator + 10);   // back
                                        nativeTriangles.Add(trianglesIterator + 6);
                                        nativeTriangles.Add(trianglesIterator + 7);
                                        nativeTriangles.Add(trianglesIterator + 10);
                                        nativeTriangles.Add(trianglesIterator + 7);
                                        nativeTriangles.Add(trianglesIterator + 11);
                                         
                                    }
                                }
                            }
                            else
                            {
                                if (shapeMap[iterator - 1] == true)
                                {
                                    if (shapeMap[iterator + 256] == false)
                                    {
                                        if (blockHeight == (int)nativeHeightMap[heightIndex])
                                        {
                                            // Grass dirt face
                                            tempVector2.x = 0.125f;
                                            tempVector2.y = 0.125f;
                                            nativeUvs[trianglesIterator + 6] += tempVector2;
                                            nativeUvs[trianglesIterator + 7] += tempVector2;
                                            nativeUvs[trianglesIterator + 10] += tempVector2;
                                            nativeUvs[trianglesIterator + 11] += tempVector2;
                                        }
                                        else
                                        {
                                            if (blockHeight + 3 >= nativeHeightMap[heightIndex])
                                            {
                                                // dirt face
                                                tempVector2.x = 0f;
                                                tempVector2.y = 0.125f;
                                                nativeUvs[trianglesIterator + 6] += tempVector2;
                                                nativeUvs[trianglesIterator + 7] += tempVector2;
                                                nativeUvs[trianglesIterator + 10] += tempVector2;
                                                nativeUvs[trianglesIterator + 11] += tempVector2;
                                            }
                                            else
                                            {
                                                // Stone face default uvs
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (blockHeight == (int)nativeHeightMap[heightIndex])
                                        {
                                            // Grass dirt face
                                            tempVector2.x = 0.125f;
                                            tempVector2.y = 0.125f;
                                            nativeUvs[trianglesIterator + 6] += tempVector2;
                                            nativeUvs[trianglesIterator + 7] += tempVector2;
                                            nativeUvs[trianglesIterator + 10] += tempVector2;
                                            nativeUvs[trianglesIterator + 11] += tempVector2;

                                        }
                                        else
                                        {
                                            if (blockHeight + 3 >= nativeHeightMap[heightIndex])
                                            {
                                                // Grass dirt face
                                                tempVector2.x = 0.125f;
                                                tempVector2.y = 0.125f;
                                                nativeUvs[trianglesIterator + 6] += tempVector2;
                                                nativeUvs[trianglesIterator + 7] += tempVector2;
                                                nativeUvs[trianglesIterator + 10] += tempVector2;
                                                nativeUvs[trianglesIterator + 11] += tempVector2;
                                            }
                                            else
                                            {
                                                // Stone face default uvs

                                                // Stone face default uvs
                                                tempVector2.x = 0.125f;
                                                tempVector2.y = 0f;
                                                nativeUvs[trianglesIterator + 6] += tempVector2;
                                                nativeUvs[trianglesIterator + 7] += tempVector2;
                                                nativeUvs[trianglesIterator + 10] += tempVector2;
                                                nativeUvs[trianglesIterator + 11] += tempVector2;
                                            }
                                        }
                                    }

                                    // because the is a shape map infront there will always be face north

                                    nativeTriangles.Add(trianglesIterator + 10);   // back
                                    nativeTriangles.Add(trianglesIterator + 6);
                                    nativeTriangles.Add(trianglesIterator + 7);
                                    nativeTriangles.Add(trianglesIterator + 10);
                                    nativeTriangles.Add(trianglesIterator + 7);
                                    nativeTriangles.Add(trianglesIterator + 11);
                                }
                                else    // default process
                                {
                                    if (nativeHeightMap[southHeightIndex] <= blockHeight)
                                    {
                                        nativeTriangles.Add(trianglesIterator + 10);   // back
                                        nativeTriangles.Add(trianglesIterator + 6);
                                        nativeTriangles.Add(trianglesIterator + 7);
                                        nativeTriangles.Add(trianglesIterator + 10);
                                        nativeTriangles.Add(trianglesIterator + 7);
                                        nativeTriangles.Add(trianglesIterator + 11);
                                    }

                                    if (blockHeight == (int)nativeHeightMap[heightIndex])
                                    {
                                        // Grass dirt face
                                        tempVector2.x = 0.125f;
                                        tempVector2.y = 0.125f;
                                        nativeUvs[trianglesIterator + 6] += tempVector2;
                                        nativeUvs[trianglesIterator + 7] += tempVector2;
                                        nativeUvs[trianglesIterator + 10] += tempVector2;
                                        nativeUvs[trianglesIterator + 11] += tempVector2;
                                    }
                                    else
                                    {
                                        if (blockHeight + 3 >= nativeHeightMap[heightIndex])
                                        {
                                            // dirt face
                                            tempVector2.x = 0f;
                                            tempVector2.y = 0.125f;
                                            nativeUvs[trianglesIterator + 6] += tempVector2;
                                            nativeUvs[trianglesIterator + 7] += tempVector2;
                                            nativeUvs[trianglesIterator + 10] += tempVector2;
                                            nativeUvs[trianglesIterator + 11] += tempVector2;
                                        }
                                    }
                                }
                            }

                            if (eastEdgeDetected)
                            {
                                // if south edge need to use 3d perlin noise check to see if there is a block in behind. (selection is off of block map)

                                if (hasEastNeighbour)
                                {
                                    // 3D perlin noise.
                                    tempFloat2.x = (subChunkPosition.x + x + 1) * 0.04f;
                                    tempFloat2.y = (subChunkPosition.y + y) * 0.04f;
                                    xy = noise.snoise(tempFloat2);

                                    tempFloat2.x = (subChunkPosition.x + x + 1) * 0.04f;
                                    tempFloat2.y = (subChunkPosition.z + z) * 0.04f;
                                    xz = noise.snoise(tempFloat2);

                                    tempFloat2.x = (subChunkPosition.y + y) * 0.04f;
                                    tempFloat2.y = (subChunkPosition.z + z) * 0.04f;
                                    yz = noise.snoise(tempFloat2);

                                    tempFloat2.x = (subChunkPosition.y + y) * 0.04f;
                                    tempFloat2.y = (subChunkPosition.x + x + 1) * 0.04f;
                                    yx = noise.snoise(tempFloat2);

                                    tempFloat2.x = (subChunkPosition.z + z) * 0.04f;
                                    tempFloat2.y = (subChunkPosition.x + x + 1) * 0.04f;
                                    zx = noise.snoise(tempFloat2);

                                    tempFloat2.x = (subChunkPosition.z + z) * 0.04f;
                                    tempFloat2.y = (subChunkPosition.y + y) * 0.04f;
                                    zy = noise.snoise(tempFloat2);

                                    tempResult = (xy + xz + yz + yx + zx + zy) / 6;

                                    if (tempResult > threshold)
                                    {
                                        if (shapeMap[iterator + 256] == true)
                                        {
                                            if (blockHeight == (int)nativeHeightMap[heightIndex])
                                            {
                                                // Grass dirt face
                                                tempVector2.x = 0.125f;
                                                tempVector2.y = 0.125f;
                                                nativeUvs[trianglesIterator + 20] += tempVector2;
                                                nativeUvs[trianglesIterator + 21] += tempVector2;
                                                nativeUvs[trianglesIterator + 22] += tempVector2;
                                                nativeUvs[trianglesIterator + 23] += tempVector2;
                                            }
                                            else
                                            {
                                                if (blockHeight + 3 >= nativeHeightMap[heightIndex])
                                                {
                                                    // Grass dirt face
                                                    tempVector2.x = 0.125f;
                                                    tempVector2.y = 0.125f;
                                                    nativeUvs[trianglesIterator + 20] += tempVector2;
                                                    nativeUvs[trianglesIterator + 21] += tempVector2;
                                                    nativeUvs[trianglesIterator + 22] += tempVector2;
                                                    nativeUvs[trianglesIterator + 23] += tempVector2;
                                                }
                                                else
                                                {
                                                    // Stone face default uvs
                                                    tempVector2.x = 0.125f;
                                                    tempVector2.y = 0.0f;
                                                    nativeUvs[trianglesIterator + 20] += tempVector2;
                                                    nativeUvs[trianglesIterator + 21] += tempVector2;
                                                    nativeUvs[trianglesIterator + 22] += tempVector2;
                                                    nativeUvs[trianglesIterator + 23] += tempVector2;
                                                }
                                            }

                                            if (!southEdgeDetected
                                            && !northEdgeDetected)
                                            {
                                                nativeTriangles.Add(trianglesIterator + 20);   //right
                                                nativeTriangles.Add(trianglesIterator + 21);
                                                nativeTriangles.Add(trianglesIterator + 22);
                                                nativeTriangles.Add(trianglesIterator + 20);
                                                nativeTriangles.Add(trianglesIterator + 22);
                                                nativeTriangles.Add(trianglesIterator + 23);
                                            }
                                            else
                                            {
                                                if (southEdgeDetected)
                                                {
                                                    if (hasSouthEastNeighbour)
                                                    {
                                                        nativeTriangles.Add(trianglesIterator + 20);   //right
                                                        nativeTriangles.Add(trianglesIterator + 21);
                                                        nativeTriangles.Add(trianglesIterator + 22);
                                                        nativeTriangles.Add(trianglesIterator + 20);
                                                        nativeTriangles.Add(trianglesIterator + 22);
                                                        nativeTriangles.Add(trianglesIterator + 23);
                                                    }
                                                }
                                                else
                                                {
                                                    if (northEdgeDetected)
                                                    {
                                                        if (hasNorthEastNeighbour)
                                                        {
                                                            nativeTriangles.Add(trianglesIterator + 20);   //right
                                                            nativeTriangles.Add(trianglesIterator + 21);
                                                            nativeTriangles.Add(trianglesIterator + 22);
                                                            nativeTriangles.Add(trianglesIterator + 20);
                                                            nativeTriangles.Add(trianglesIterator + 22);
                                                            nativeTriangles.Add(trianglesIterator + 23);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (blockHeight == (int)nativeHeightMap[heightIndex])
                                            {
                                                // Grass dirt face
                                                tempVector2.x = 0.125f;
                                                tempVector2.y = 0.125f;
                                                nativeUvs[trianglesIterator + 20] += tempVector2;
                                                nativeUvs[trianglesIterator + 21] += tempVector2;
                                                nativeUvs[trianglesIterator + 22] += tempVector2;
                                                nativeUvs[trianglesIterator + 23] += tempVector2;

                                                nativeTriangles.Add(trianglesIterator + 20);   //right
                                                nativeTriangles.Add(trianglesIterator + 21);
                                                nativeTriangles.Add(trianglesIterator + 22);
                                                nativeTriangles.Add(trianglesIterator + 20);
                                                nativeTriangles.Add(trianglesIterator + 22);
                                                nativeTriangles.Add(trianglesIterator + 23);
                                            }
                                            else
                                            {
                                                if (blockHeight + 3 >= nativeHeightMap[heightIndex])
                                                {
                                                    // dirt face
                                                    tempVector2.x = 0f;
                                                    tempVector2.y = 0.125f;
                                                    nativeUvs[trianglesIterator + 20] += tempVector2;
                                                    nativeUvs[trianglesIterator + 21] += tempVector2;
                                                    nativeUvs[trianglesIterator + 22] += tempVector2;
                                                    nativeUvs[trianglesIterator + 23] += tempVector2;

                                                    nativeTriangles.Add(trianglesIterator + 20);   //right
                                                    nativeTriangles.Add(trianglesIterator + 21);
                                                    nativeTriangles.Add(trianglesIterator + 22);
                                                    nativeTriangles.Add(trianglesIterator + 20);
                                                    nativeTriangles.Add(trianglesIterator + 22);
                                                    nativeTriangles.Add(trianglesIterator + 23);
                                                }
                                                else
                                                {
                                                    // Stone face default uvs

                                                    if (!southEdgeDetected
                                                        && !northEdgeDetected)
                                                    {
                                                        nativeTriangles.Add(trianglesIterator + 20);   //right
                                                        nativeTriangles.Add(trianglesIterator + 21);
                                                        nativeTriangles.Add(trianglesIterator + 22);
                                                        nativeTriangles.Add(trianglesIterator + 20);
                                                        nativeTriangles.Add(trianglesIterator + 22);
                                                        nativeTriangles.Add(trianglesIterator + 23);
                                                    }
                                                    else
                                                    {
                                                        if (southEdgeDetected)
                                                        {
                                                            if (hasSouthEastNeighbour)
                                                            {
                                                                nativeTriangles.Add(trianglesIterator + 20);   //right
                                                                nativeTriangles.Add(trianglesIterator + 21);
                                                                nativeTriangles.Add(trianglesIterator + 22);
                                                                nativeTriangles.Add(trianglesIterator + 20);
                                                                nativeTriangles.Add(trianglesIterator + 22);
                                                                nativeTriangles.Add(trianglesIterator + 23);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (northEdgeDetected)
                                                            {
                                                                if (hasNorthEastNeighbour)
                                                                {
                                                                    nativeTriangles.Add(trianglesIterator + 20);   //right
                                                                    nativeTriangles.Add(trianglesIterator + 21);
                                                                    nativeTriangles.Add(trianglesIterator + 22);
                                                                    nativeTriangles.Add(trianglesIterator + 20);
                                                                    nativeTriangles.Add(trianglesIterator + 22);
                                                                    nativeTriangles.Add(trianglesIterator + 23);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {

                                        if (nativeEastHeightMap[eastHeightIndex] > blockHeight)
                                        {
                                            // black face
                                            tempVector2.x = nativeUvs[trianglesIterator + 20].x;
                                            tempVector2.y = 1 - nativeUvs[trianglesIterator + 20].y;
                                            nativeUvs[trianglesIterator + 20] = tempVector2;

                                            tempVector2.x = nativeUvs[trianglesIterator + 21].x;
                                            tempVector2.y = 1 - nativeUvs[trianglesIterator + 21].y;
                                            nativeUvs[trianglesIterator + 21] = tempVector2;

                                            tempVector2.x = nativeUvs[trianglesIterator + 22].x;
                                            tempVector2.y = 1 - nativeUvs[trianglesIterator + 22].y;
                                            nativeUvs[trianglesIterator + 22] = tempVector2;

                                            tempVector2.x = nativeUvs[trianglesIterator + 23].x;
                                            tempVector2.y = 1 - nativeUvs[trianglesIterator + 23].y;
                                            nativeUvs[trianglesIterator + 23] = tempVector2;

                                            nativeTriangles.Add(trianglesIterator + 20);   //right
                                                nativeTriangles.Add(trianglesIterator + 21);
                                                nativeTriangles.Add(trianglesIterator + 22);
                                                nativeTriangles.Add(trianglesIterator + 20);
                                                nativeTriangles.Add(trianglesIterator + 22);
                                                nativeTriangles.Add(trianglesIterator + 23);
                                        }
                                        else
                                        {
                                            if (blockHeight == (int)nativeHeightMap[heightIndex])
                                            {
                                                // Grass dirt face
                                                tempVector2.x = 0.125f;
                                                tempVector2.y = 0.125f;
                                                nativeUvs[trianglesIterator + 20] += tempVector2;
                                                nativeUvs[trianglesIterator + 21] += tempVector2;
                                                nativeUvs[trianglesIterator + 22] += tempVector2;
                                                nativeUvs[trianglesIterator + 23] += tempVector2;

                                                nativeTriangles.Add(trianglesIterator + 20);   //right
                                                nativeTriangles.Add(trianglesIterator + 21);
                                                nativeTriangles.Add(trianglesIterator + 22);
                                                nativeTriangles.Add(trianglesIterator + 20);
                                                nativeTriangles.Add(trianglesIterator + 22);
                                                nativeTriangles.Add(trianglesIterator + 23);
                                            }
                                            else
                                            {
                                                if (blockHeight + 3 >= nativeHeightMap[heightIndex])
                                                {
                                                    // dirt face
                                                    tempVector2.x = 0f;
                                                    tempVector2.y = 0.125f;
                                                    nativeUvs[trianglesIterator + 20] += tempVector2;
                                                    nativeUvs[trianglesIterator + 21] += tempVector2;
                                                    nativeUvs[trianglesIterator + 22] += tempVector2;
                                                    nativeUvs[trianglesIterator + 23] += tempVector2;

                                                    nativeTriangles.Add(trianglesIterator + 20);   //right
                                                    nativeTriangles.Add(trianglesIterator + 21);
                                                    nativeTriangles.Add(trianglesIterator + 22);
                                                    nativeTriangles.Add(trianglesIterator + 20);
                                                    nativeTriangles.Add(trianglesIterator + 22);
                                                    nativeTriangles.Add(trianglesIterator + 23);
                                                }
                                                else
                                                {
                                                    // Stone face default uvs

                                                    nativeTriangles.Add(trianglesIterator + 20);   //right
                                                    nativeTriangles.Add(trianglesIterator + 21);
                                                    nativeTriangles.Add(trianglesIterator + 22);
                                                    nativeTriangles.Add(trianglesIterator + 20);
                                                    nativeTriangles.Add(trianglesIterator + 22);
                                                    nativeTriangles.Add(trianglesIterator + 23);
                                                }
                                            }
                                        }
                                    }

                                }
                                else
                                {
                                    if (nativeEastHeightMap[eastHeightIndex] > blockHeight)
                                    {
                                        // black face
                                        tempVector2.x = nativeUvs[trianglesIterator + 20].x;
                                        tempVector2.y = 1 - nativeUvs[trianglesIterator + 20].y;
                                        nativeUvs[trianglesIterator + 20] = tempVector2;

                                        tempVector2.x = nativeUvs[trianglesIterator + 21].x;
                                        tempVector2.y = 1 - nativeUvs[trianglesIterator + 21].y;
                                        nativeUvs[trianglesIterator + 21] = tempVector2;

                                        tempVector2.x = nativeUvs[trianglesIterator + 22].x;
                                        tempVector2.y = 1 - nativeUvs[trianglesIterator + 22].y;
                                        nativeUvs[trianglesIterator + 22] = tempVector2;

                                        tempVector2.x = nativeUvs[trianglesIterator + 23].x;
                                        tempVector2.y = 1 - nativeUvs[trianglesIterator + 23].y;
                                        nativeUvs[trianglesIterator + 23] = tempVector2;

                                        nativeTriangles.Add(trianglesIterator + 20);   //right
                                        nativeTriangles.Add(trianglesIterator + 21);
                                        nativeTriangles.Add(trianglesIterator + 22);
                                        nativeTriangles.Add(trianglesIterator + 20);
                                        nativeTriangles.Add(trianglesIterator + 22);
                                        nativeTriangles.Add(trianglesIterator + 23);
                                    }
                                }
                            }
                            else
                            {
                                if (shapeMap[iterator + 16] == true)
                                {
                                    if (shapeMap[iterator + 256] == false)
                                    {
                                        if (blockHeight == (int)nativeHeightMap[heightIndex])
                                        {
                                            // Grass dirt face
                                            tempVector2.x = 0.125f;
                                            tempVector2.y = 0.125f;
                                            nativeUvs[trianglesIterator + 20] += tempVector2;
                                            nativeUvs[trianglesIterator + 21] += tempVector2;
                                            nativeUvs[trianglesIterator + 22] += tempVector2;
                                            nativeUvs[trianglesIterator + 23] += tempVector2;
                                        }
                                        else
                                        {
                                            if (blockHeight + 3 >= nativeHeightMap[heightIndex])
                                            {
                                                // dirt face
                                                tempVector2.x = 0f;
                                                tempVector2.y = 0.125f;
                                                nativeUvs[trianglesIterator + 20] += tempVector2;
                                                nativeUvs[trianglesIterator + 21] += tempVector2;
                                                nativeUvs[trianglesIterator + 22] += tempVector2;
                                                nativeUvs[trianglesIterator + 23] += tempVector2;
                                            }
                                            else
                                            {
                                                // Stone face default uvs
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (blockHeight == (int)nativeHeightMap[heightIndex])
                                        {
                                            // Grass dirt face
                                            tempVector2.x = 0.125f;
                                            tempVector2.y = 0.125f;
                                            nativeUvs[trianglesIterator + 20] += tempVector2;
                                            nativeUvs[trianglesIterator + 21] += tempVector2;
                                            nativeUvs[trianglesIterator + 22] += tempVector2;
                                            nativeUvs[trianglesIterator + 23] += tempVector2;

                                        }
                                        else
                                        {
                                            if (blockHeight + 3 >= nativeHeightMap[heightIndex])
                                            {
                                                // Grass dirt face
                                                tempVector2.x = 0.125f;
                                                tempVector2.y = 0.125f;
                                                nativeUvs[trianglesIterator + 20] += tempVector2;
                                                nativeUvs[trianglesIterator + 21] += tempVector2;
                                                nativeUvs[trianglesIterator + 22] += tempVector2;
                                                nativeUvs[trianglesIterator + 23] += tempVector2;
                                            }
                                            else
                                            {
                                                // Stone face default uvs
                                                tempVector2.x = 0.125f;
                                                tempVector2.y = 0.0f;
                                                nativeUvs[trianglesIterator + 20] += tempVector2;
                                                nativeUvs[trianglesIterator + 21] += tempVector2;
                                                nativeUvs[trianglesIterator + 22] += tempVector2;
                                                nativeUvs[trianglesIterator + 23] += tempVector2;
                                            }
                                        }
                                    }

                                    // because the is a shape map infront there will always be face north

                                    nativeTriangles.Add(trianglesIterator + 20);   //right
                                    nativeTriangles.Add(trianglesIterator + 21);
                                    nativeTriangles.Add(trianglesIterator + 22);
                                    nativeTriangles.Add(trianglesIterator + 20);
                                    nativeTriangles.Add(trianglesIterator + 22);
                                    nativeTriangles.Add(trianglesIterator + 23);
                                }
                                else    // default process
                                {
                                    if (nativeHeightMap[eastHeightIndex] <= blockHeight)
                                    {
                                        nativeTriangles.Add(trianglesIterator + 20);   //right
                                        nativeTriangles.Add(trianglesIterator + 21);
                                        nativeTriangles.Add(trianglesIterator + 22);
                                        nativeTriangles.Add(trianglesIterator + 20);
                                        nativeTriangles.Add(trianglesIterator + 22);
                                        nativeTriangles.Add(trianglesIterator + 23);
                                    }

                                    if (blockHeight == (int)nativeHeightMap[heightIndex])
                                    {
                                        // Grass dirt face
                                        tempVector2.x = 0.125f;
                                        tempVector2.y = 0.125f;
                                        nativeUvs[trianglesIterator + 20] += tempVector2;
                                        nativeUvs[trianglesIterator + 21] += tempVector2;
                                        nativeUvs[trianglesIterator + 22] += tempVector2;
                                        nativeUvs[trianglesIterator + 23] += tempVector2;
                                    }
                                    else
                                    {
                                        if (blockHeight + 3 >= nativeHeightMap[heightIndex])
                                        {
                                            // dirt face
                                            tempVector2.x = 0f;
                                            tempVector2.y = 0.125f;
                                            nativeUvs[trianglesIterator + 20] += tempVector2;
                                            nativeUvs[trianglesIterator + 21] += tempVector2;
                                            nativeUvs[trianglesIterator + 22] += tempVector2;
                                            nativeUvs[trianglesIterator + 23] += tempVector2;
                                        }
                                    }
                                }
                            }

                            if (westEdgeDetected)
                            {
                                // if south edge need to use 3d perlin noise check to see if there is a block in behind. (selection is off of block map)

                                if (hasWestNeighbour)
                                {
                                    // 3D perlin noise.
                                    tempFloat2.x = (subChunkPosition.x + x - 1) * 0.04f;
                                    tempFloat2.y = (subChunkPosition.y + y) * 0.04f;
                                    xy = noise.snoise(tempFloat2);

                                    tempFloat2.x = (subChunkPosition.x + x - 1) * 0.04f;
                                    tempFloat2.y = (subChunkPosition.z + z) * 0.04f;
                                    xz = noise.snoise(tempFloat2);

                                    tempFloat2.x = (subChunkPosition.y + y) * 0.04f;
                                    tempFloat2.y = (subChunkPosition.z + z) * 0.04f;
                                    yz = noise.snoise(tempFloat2);

                                    tempFloat2.x = (subChunkPosition.y + y) * 0.04f;
                                    tempFloat2.y = (subChunkPosition.x + x - 1) * 0.04f;
                                    yx = noise.snoise(tempFloat2);

                                    tempFloat2.x = (subChunkPosition.z + z) * 0.04f;
                                    tempFloat2.y = (subChunkPosition.x + x - 1) * 0.04f;
                                    zx = noise.snoise(tempFloat2);

                                    tempFloat2.x = (subChunkPosition.z + z) * 0.04f;
                                    tempFloat2.y = (subChunkPosition.y + y) * 0.04f;
                                    zy = noise.snoise(tempFloat2);

                                    tempResult = (xy + xz + yz + yx + zx + zy) / 6;

                                    if (tempResult > threshold)
                                    {
                                        if (shapeMap[iterator + 256] == true)
                                        {
                                            if (blockHeight == (int)nativeHeightMap[heightIndex])
                                            {
                                                // Grass dirt face
                                                tempVector2.x = 0.125f;
                                                tempVector2.y = 0.125f;
                                                nativeUvs[trianglesIterator + 16] += tempVector2;
                                                nativeUvs[trianglesIterator + 17] += tempVector2;
                                                nativeUvs[trianglesIterator + 18] += tempVector2;
                                                nativeUvs[trianglesIterator + 19] += tempVector2;
                                            }
                                            else
                                            {
                                                if (blockHeight + 3 >= nativeHeightMap[heightIndex])
                                                {
                                                    // Grass dirt face
                                                    tempVector2.x = 0.125f;
                                                    tempVector2.y = 0.125f;
                                                    nativeUvs[trianglesIterator + 16] += tempVector2;
                                                    nativeUvs[trianglesIterator + 17] += tempVector2;
                                                    nativeUvs[trianglesIterator + 18] += tempVector2;
                                                    nativeUvs[trianglesIterator + 19] += tempVector2;
                                                }
                                                else
                                                {
                                                    // Stone face default uvs
                                                    tempVector2.x = 0.125f;
                                                    tempVector2.y = 0f;
                                                    nativeUvs[trianglesIterator + 16] += tempVector2;
                                                    nativeUvs[trianglesIterator + 17] += tempVector2;
                                                    nativeUvs[trianglesIterator + 18] += tempVector2;
                                                    nativeUvs[trianglesIterator + 19] += tempVector2;
                                                }
                                            }

                                            if (!southEdgeDetected
                                            && !northEdgeDetected)
                                            {
                                                nativeTriangles.Add(trianglesIterator + 16);   //left
                                                nativeTriangles.Add(trianglesIterator + 17);
                                                nativeTriangles.Add(trianglesIterator + 18);
                                                nativeTriangles.Add(trianglesIterator + 16);
                                                nativeTriangles.Add(trianglesIterator + 18);
                                                nativeTriangles.Add(trianglesIterator + 19);
                                            }
                                            else
                                            {
                                                if (southEdgeDetected)
                                                {
                                                    if (hasSouthWestNeighbour)
                                                    {
                                                        nativeTriangles.Add(trianglesIterator + 16);   //left
                                                        nativeTriangles.Add(trianglesIterator + 17);
                                                        nativeTriangles.Add(trianglesIterator + 18);
                                                        nativeTriangles.Add(trianglesIterator + 16);
                                                        nativeTriangles.Add(trianglesIterator + 18);
                                                        nativeTriangles.Add(trianglesIterator + 19);
                                                    }
                                                }
                                                else
                                                {
                                                    if (northEdgeDetected)
                                                    {
                                                        if (hasNorthWestNeighbour)
                                                        {
                                                            nativeTriangles.Add(trianglesIterator + 16);   //left
                                                            nativeTriangles.Add(trianglesIterator + 17);
                                                            nativeTriangles.Add(trianglesIterator + 18);
                                                            nativeTriangles.Add(trianglesIterator + 16);
                                                            nativeTriangles.Add(trianglesIterator + 18);
                                                            nativeTriangles.Add(trianglesIterator + 19);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (blockHeight == (int)nativeHeightMap[heightIndex])
                                            {
                                                // Grass dirt face
                                                tempVector2.x = 0.125f;
                                                tempVector2.y = 0.125f;
                                                nativeUvs[trianglesIterator + 16] += tempVector2;
                                                nativeUvs[trianglesIterator + 17] += tempVector2;
                                                nativeUvs[trianglesIterator + 18] += tempVector2;
                                                nativeUvs[trianglesIterator + 19] += tempVector2;

                                                nativeTriangles.Add(trianglesIterator + 16);   //left
                                                nativeTriangles.Add(trianglesIterator + 17);
                                                nativeTriangles.Add(trianglesIterator + 18);
                                                nativeTriangles.Add(trianglesIterator + 16);
                                                nativeTriangles.Add(trianglesIterator + 18);
                                                nativeTriangles.Add(trianglesIterator + 19);
                                            }
                                            else
                                            {
                                                if (blockHeight + 3 >= nativeHeightMap[heightIndex])
                                                {
                                                    // dirt face
                                                    tempVector2.x = 0f;
                                                    tempVector2.y = 0.125f;
                                                    nativeUvs[trianglesIterator + 16] += tempVector2;
                                                    nativeUvs[trianglesIterator + 17] += tempVector2;
                                                    nativeUvs[trianglesIterator + 18] += tempVector2;
                                                    nativeUvs[trianglesIterator + 19] += tempVector2;

                                                    nativeTriangles.Add(trianglesIterator + 16);   //left
                                                    nativeTriangles.Add(trianglesIterator + 17);
                                                    nativeTriangles.Add(trianglesIterator + 18);
                                                    nativeTriangles.Add(trianglesIterator + 16);
                                                    nativeTriangles.Add(trianglesIterator + 18);
                                                    nativeTriangles.Add(trianglesIterator + 19);
                                                }
                                                else
                                                {
                                                    // Stone face default uvs

                                                    if (!southEdgeDetected
                                                        && !northEdgeDetected)
                                                    {
                                                        nativeTriangles.Add(trianglesIterator + 16);   //left
                                                        nativeTriangles.Add(trianglesIterator + 17);
                                                        nativeTriangles.Add(trianglesIterator + 18);
                                                        nativeTriangles.Add(trianglesIterator + 16);
                                                        nativeTriangles.Add(trianglesIterator + 18);
                                                        nativeTriangles.Add(trianglesIterator + 19);
                                                    }
                                                    else
                                                    {
                                                        if (southEdgeDetected)
                                                        {
                                                            if (hasSouthWestNeighbour)
                                                            {
                                                                nativeTriangles.Add(trianglesIterator + 16);   //left
                                                                nativeTriangles.Add(trianglesIterator + 17);
                                                                nativeTriangles.Add(trianglesIterator + 18);
                                                                nativeTriangles.Add(trianglesIterator + 16);
                                                                nativeTriangles.Add(trianglesIterator + 18);
                                                                nativeTriangles.Add(trianglesIterator + 19);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (northEdgeDetected)
                                                            {
                                                                if (hasNorthWestNeighbour)
                                                                {
                                                                    nativeTriangles.Add(trianglesIterator + 16);   //left
                                                                    nativeTriangles.Add(trianglesIterator + 17);
                                                                    nativeTriangles.Add(trianglesIterator + 18);
                                                                    nativeTriangles.Add(trianglesIterator + 16);
                                                                    nativeTriangles.Add(trianglesIterator + 18);
                                                                    nativeTriangles.Add(trianglesIterator + 19);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {

                                        if (nativeWestHeightMap[westHeightIndex] > blockHeight)
                                        {
                                            // black face
                                            tempVector2.x = nativeUvs[trianglesIterator + 16].x;
                                            tempVector2.y = 1 - nativeUvs[trianglesIterator + 16].y;
                                            nativeUvs[trianglesIterator + 16] = tempVector2;

                                            tempVector2.x = nativeUvs[trianglesIterator + 17].x;
                                            tempVector2.y = 1 - nativeUvs[trianglesIterator + 17].y;
                                            nativeUvs[trianglesIterator + 17] = tempVector2;

                                            tempVector2.x = nativeUvs[trianglesIterator + 18].x;
                                            tempVector2.y = 1 - nativeUvs[trianglesIterator + 18].y;
                                            nativeUvs[trianglesIterator + 18] = tempVector2;

                                            tempVector2.x = nativeUvs[trianglesIterator + 19].x;
                                            tempVector2.y = 1 - nativeUvs[trianglesIterator + 19].y;
                                            nativeUvs[trianglesIterator + 19] = tempVector2;

                                            nativeTriangles.Add(trianglesIterator + 16);   //left
                                                nativeTriangles.Add(trianglesIterator + 17);
                                                nativeTriangles.Add(trianglesIterator + 18);
                                                nativeTriangles.Add(trianglesIterator + 16);
                                                nativeTriangles.Add(trianglesIterator + 18);
                                                nativeTriangles.Add(trianglesIterator + 19);
                                        }
                                        else
                                        {
                                            if (blockHeight == (int)nativeHeightMap[heightIndex])
                                            {
                                                // Grass dirt face
                                                tempVector2.x = 0.125f;
                                                tempVector2.y = 0.125f;
                                                nativeUvs[trianglesIterator + 16] += tempVector2;
                                                nativeUvs[trianglesIterator + 17] += tempVector2;
                                                nativeUvs[trianglesIterator + 18] += tempVector2;
                                                nativeUvs[trianglesIterator + 19] += tempVector2;

                                                nativeTriangles.Add(trianglesIterator + 16);   //left
                                                nativeTriangles.Add(trianglesIterator + 17);
                                                nativeTriangles.Add(trianglesIterator + 18);
                                                nativeTriangles.Add(trianglesIterator + 16);
                                                nativeTriangles.Add(trianglesIterator + 18);
                                                nativeTriangles.Add(trianglesIterator + 19);
                                            }
                                            else
                                            {
                                                if (blockHeight + 3 >= nativeHeightMap[heightIndex])
                                                {
                                                    // dirt face
                                                    tempVector2.x = 0f;
                                                    tempVector2.y = 0.125f;
                                                    nativeUvs[trianglesIterator + 16] += tempVector2;
                                                    nativeUvs[trianglesIterator + 17] += tempVector2;
                                                    nativeUvs[trianglesIterator + 18] += tempVector2;
                                                    nativeUvs[trianglesIterator + 19] += tempVector2;

                                                    nativeTriangles.Add(trianglesIterator + 16);   //left
                                                    nativeTriangles.Add(trianglesIterator + 17);
                                                    nativeTriangles.Add(trianglesIterator + 18);
                                                    nativeTriangles.Add(trianglesIterator + 16);
                                                    nativeTriangles.Add(trianglesIterator + 18);
                                                    nativeTriangles.Add(trianglesIterator + 19);
                                                }
                                                else
                                                {
                                                    // Stone face default uvs

                                                    nativeTriangles.Add(trianglesIterator + 16);   //left
                                                    nativeTriangles.Add(trianglesIterator + 17);
                                                    nativeTriangles.Add(trianglesIterator + 18);
                                                    nativeTriangles.Add(trianglesIterator + 16);
                                                    nativeTriangles.Add(trianglesIterator + 18);
                                                    nativeTriangles.Add(trianglesIterator + 19);
                                                }
                                            }
                                        }
                                    }

                                }
                                else 
                                {
                                    if (nativeWestHeightMap[westHeightIndex] > blockHeight)
                                    {
                                        // black face
                                        tempVector2.x = nativeUvs[trianglesIterator + 16].x;
                                        tempVector2.y = 1 - nativeUvs[trianglesIterator + 16].y;
                                        nativeUvs[trianglesIterator + 16] = tempVector2;

                                        tempVector2.x = nativeUvs[trianglesIterator + 17].x;
                                        tempVector2.y = 1 - nativeUvs[trianglesIterator + 17].y;
                                        nativeUvs[trianglesIterator + 17] = tempVector2;

                                        tempVector2.x = nativeUvs[trianglesIterator + 18].x;
                                        tempVector2.y = 1 - nativeUvs[trianglesIterator + 18].y;
                                        nativeUvs[trianglesIterator + 18] = tempVector2;

                                        tempVector2.x = nativeUvs[trianglesIterator + 19].x;
                                        tempVector2.y = 1 - nativeUvs[trianglesIterator + 19].y;
                                        nativeUvs[trianglesIterator + 19] = tempVector2;

                                        nativeTriangles.Add(trianglesIterator + 16);   //left
                                        nativeTriangles.Add(trianglesIterator + 17);
                                        nativeTriangles.Add(trianglesIterator + 18);
                                        nativeTriangles.Add(trianglesIterator + 16);
                                        nativeTriangles.Add(trianglesIterator + 18);
                                        nativeTriangles.Add(trianglesIterator + 19);
                                    }
                                }
                            }
                            else
                            {
                                if (shapeMap[iterator - 16] == true)
                                {
                                    if (shapeMap[iterator + 256] == false)
                                    {
                                        if (blockHeight == (int)nativeHeightMap[heightIndex])
                                        {
                                            // Grass dirt face
                                            tempVector2.x = 0.125f;
                                            tempVector2.y = 0.125f;
                                            nativeUvs[trianglesIterator + 16] += tempVector2;
                                            nativeUvs[trianglesIterator + 17] += tempVector2;
                                            nativeUvs[trianglesIterator + 18] += tempVector2;
                                            nativeUvs[trianglesIterator + 19] += tempVector2;
                                        }
                                        else
                                        {
                                            if (blockHeight + 3 >= nativeHeightMap[heightIndex])
                                            {
                                                // dirt face
                                                tempVector2.x = 0f;
                                                tempVector2.y = 0.125f;
                                                nativeUvs[trianglesIterator + 16] += tempVector2;
                                                nativeUvs[trianglesIterator + 17] += tempVector2;
                                                nativeUvs[trianglesIterator + 18] += tempVector2;
                                                nativeUvs[trianglesIterator + 19] += tempVector2;
                                            }
                                            else
                                            {
                                                // Stone face default uvs
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (blockHeight == (int)nativeHeightMap[heightIndex])
                                        {
                                            // Grass dirt face
                                            tempVector2.x = 0.125f;
                                            tempVector2.y = 0.125f;
                                            nativeUvs[trianglesIterator + 16] += tempVector2;
                                            nativeUvs[trianglesIterator + 17] += tempVector2;
                                            nativeUvs[trianglesIterator + 18] += tempVector2;
                                            nativeUvs[trianglesIterator + 19] += tempVector2;

                                        }
                                        else
                                        {
                                            if (blockHeight + 3 >= nativeHeightMap[heightIndex])
                                            {
                                                // Grass dirt face
                                                tempVector2.x = 0.125f;
                                                tempVector2.y = 0.125f;
                                                nativeUvs[trianglesIterator + 16] += tempVector2;
                                                nativeUvs[trianglesIterator + 17] += tempVector2;
                                                nativeUvs[trianglesIterator + 18] += tempVector2;
                                                nativeUvs[trianglesIterator + 19] += tempVector2;
                                            }
                                            else
                                            {
                                                // Stone face default uvs
                                                tempVector2.x = 0.125f;
                                                tempVector2.y = 0f;
                                                nativeUvs[trianglesIterator + 16] += tempVector2;
                                                nativeUvs[trianglesIterator + 17] += tempVector2;
                                                nativeUvs[trianglesIterator + 18] += tempVector2;
                                                nativeUvs[trianglesIterator + 19] += tempVector2;
                                            }
                                        }
                                    }

                                    // because the is a shape map infront there will always be face north

                                    nativeTriangles.Add(trianglesIterator + 16);   //left
                                    nativeTriangles.Add(trianglesIterator + 17);
                                    nativeTriangles.Add(trianglesIterator + 18);
                                    nativeTriangles.Add(trianglesIterator + 16);
                                    nativeTriangles.Add(trianglesIterator + 18);
                                    nativeTriangles.Add(trianglesIterator + 19);
                                }
                                else    // default process
                                {
                                    if (nativeHeightMap[westHeightIndex] <= blockHeight)
                                    {
                                        nativeTriangles.Add(trianglesIterator + 16);   //left
                                        nativeTriangles.Add(trianglesIterator + 17);
                                        nativeTriangles.Add(trianglesIterator + 18);
                                        nativeTriangles.Add(trianglesIterator + 16);
                                        nativeTriangles.Add(trianglesIterator + 18);
                                        nativeTriangles.Add(trianglesIterator + 19);
                                    }

                                    if (blockHeight == (int)nativeHeightMap[heightIndex])
                                    {
                                        // Grass dirt face
                                        tempVector2.x = 0.125f;
                                        tempVector2.y = 0.125f;
                                        nativeUvs[trianglesIterator + 16] += tempVector2;
                                        nativeUvs[trianglesIterator + 17] += tempVector2;
                                        nativeUvs[trianglesIterator + 18] += tempVector2;
                                        nativeUvs[trianglesIterator + 19] += tempVector2;
                                    }
                                    else
                                    {
                                        if (blockHeight + 3 >= nativeHeightMap[heightIndex])
                                        {
                                            // dirt face
                                            tempVector2.x = 0f;
                                            tempVector2.y = 0.125f;
                                            nativeUvs[trianglesIterator + 16] += tempVector2;
                                            nativeUvs[trianglesIterator + 17] += tempVector2;
                                            nativeUvs[trianglesIterator + 18] += tempVector2;
                                            nativeUvs[trianglesIterator + 19] += tempVector2;
                                        }
                                    }
                                }
                            }


                            //-----------------------------------------------
                            // Bottom side - below

                            if (bottomEdgeDetected)
                            {
                                // if top edge need to use 3d perlin noise check to see if there is a block above. (selection is off of block map)

                                if (hasBottomNeighbour)
                                {
                                    // 3D perlin noise.
                                    tempFloat2.x = (subChunkPosition.x + x) * 0.04f;
                                    tempFloat2.y = (subChunkPosition.y + y - 1) * 0.04f;
                                    xy = noise.snoise(tempFloat2);

                                    tempFloat2.x = (subChunkPosition.x + x) * 0.04f;
                                    tempFloat2.y = (subChunkPosition.z + z) * 0.04f;
                                    xz = noise.snoise(tempFloat2);

                                    tempFloat2.x = (subChunkPosition.y + y - 1) * 0.04f;
                                    tempFloat2.y = (subChunkPosition.z + z) * 0.04f;
                                    yz = noise.snoise(tempFloat2);

                                    tempFloat2.x = (subChunkPosition.y + y - 1) * 0.04f;
                                    tempFloat2.y = (subChunkPosition.x + x) * 0.04f;
                                    yx = noise.snoise(tempFloat2);

                                    tempFloat2.x = (subChunkPosition.z + z) * 0.04f;
                                    tempFloat2.y = (subChunkPosition.x + x) * 0.04f;
                                    zx = noise.snoise(tempFloat2);

                                    tempFloat2.x = (subChunkPosition.z + z) * 0.04f;
                                    tempFloat2.y = (subChunkPosition.y + y - 1) * 0.04f;
                                    zy = noise.snoise(tempFloat2);

                                    tempResult = (xy + xz + yz + yx + zx + zy) / 6;

                                    if (tempResult > threshold)
                                    {
                                        if ((int)nativeHeightMap[heightIndex] == blockHeight
                                            || nativeHeightMap[heightIndex] <= blockHeight + 3)
                                        {
                                            // Dirt bottom face
                                            tempVector2.x = 0f;
                                            tempVector2.y = 0.125f;
                                            nativeUvs[trianglesIterator + 12] += tempVector2;
                                            nativeUvs[trianglesIterator + 13] += tempVector2;
                                            nativeUvs[trianglesIterator + 14] += tempVector2;
                                            nativeUvs[trianglesIterator + 15] += tempVector2;
                                        }
                                        else
                                        {
                                            // Stone face
                                            // DO nothing as default uvs are stone face
                                        }

                                        // should always be uvs as there is a shape map below

                                        nativeTriangles.Add(trianglesIterator + 12);   // bottom
                                        nativeTriangles.Add(trianglesIterator + 13);
                                        nativeTriangles.Add(trianglesIterator + 14);
                                        nativeTriangles.Add(trianglesIterator + 12);
                                        nativeTriangles.Add(trianglesIterator + 14);
                                        nativeTriangles.Add(trianglesIterator + 15);
                                    } 
                                }
                            }
                            else
                            {
                                if (shapeMap[iterator - 256] == true)
                                {
                                    if ((int)nativeHeightMap[heightIndex] == blockHeight
                                        || nativeHeightMap[heightIndex] <= blockHeight + 3)
                                    {
                                        // Dirt bottom face
                                        tempVector2.x = 0f;
                                        tempVector2.y = 0.125f;
                                        nativeUvs[trianglesIterator + 12] += tempVector2;
                                        nativeUvs[trianglesIterator + 13] += tempVector2;
                                        nativeUvs[trianglesIterator + 14] += tempVector2;
                                        nativeUvs[trianglesIterator + 15] += tempVector2;
                                    }
                                    else
                                    {
                                        // Stone face
                                        // DO nothing as default uvs are stone face
                                    }

                                    // should always be uvs as there is a shape map below

                                    nativeTriangles.Add(trianglesIterator + 12);   // bottom
                                    nativeTriangles.Add(trianglesIterator + 13);
                                    nativeTriangles.Add(trianglesIterator + 14);
                                    nativeTriangles.Add(trianglesIterator + 12);
                                    nativeTriangles.Add(trianglesIterator + 14);
                                    nativeTriangles.Add(trianglesIterator + 15);
                                }
                            }
                        }

                        iterator++;
                        //faceIterator += 6;          // += number of faces
                        trianglesIterator += 24;    // += number of vertices

                    }// end of z

                }// end of x

            }// End of y
        }
    }

    // REMEMBER THERE WAS EMPTY BLOCK MAPS BEING GENERATED. WHEN USING THE OLD LOADING DATA IT WAS CLEAR THERE WAS SUB CHUNK DATA GENERATED WITHOUT ANY TRIANGLES IN IT DUE TO CHOOSING WHERE TO START THE GENERATION FROM IS EMPTY(No height map stuff being used - but with shape map potential for triangle to be generated from that).
    // ADDING BLOCK MAPS WILL INCREASE
    [BurstCompile]
    public void GenerateChunkData(string chunkName)
    {
        positionTemp.x = subMapPosition.x;
        positionTemp.y = subMapPosition.z;
        //positionTemp = new Vector2(subMapPosition.x, subMapPosition.z);

        // Actual height should always exist
        // IF HEIGHT MAP DOESN'T EXIST FOR NORTH SOUTH EAST AND WEST - return.
        if (blockEngine.heightMap.subMaps.ContainsKey(positionTemp) == false
            || blockEngine.heightMap.subMaps.ContainsKey(positionTemp + new Vector2(0, blockEngine.heightMap.SUB_MAP_SIZE)) == false
            || blockEngine.heightMap.subMaps.ContainsKey(positionTemp + new Vector2(0, -blockEngine.heightMap.SUB_MAP_SIZE)) == false
            || blockEngine.heightMap.subMaps.ContainsKey(positionTemp + new Vector2(blockEngine.heightMap.SUB_MAP_SIZE, 0)) == false
            || blockEngine.heightMap.subMaps.ContainsKey(positionTemp + new Vector2(-blockEngine.heightMap.SUB_MAP_SIZE, 0)) == false)
        {
            Debug.Log("Failed to make block map. Missing a height map!");
            IncrementSubMapPosition();
            return;
        }

        // should add a check for surrounding shape maps when used.

        // =============================

        AddChunkData();

        // Check HeightMaps lowest and highest values. Generate only what is necessary. Copy the subchunk where possible.

        positionZeroForY.x = subMapPosition.x;
        positionZeroForY.y = 0;
        positionZeroForY.z = subMapPosition.z;

        // These calculation are already done inside newBlockQueue - should pass them into function rather than redoing =================================================== <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
        // blockEngine.newBlockQueue.... 

        // x = low, y = high
        difference = blockEngine.heightMap.differenceInHeightForSubMap[positionTemp];
        
        highest = (int)(difference.y);
        lowest = (int)(difference.x);

        subChunkHighest = (int)(highest / (float)SUB_CHUNK_HEIGHT) + 2; // <<<<<
        subChunkLowest = (int)(lowest / (float)SUB_CHUNK_HEIGHT) - 2;
        required = subChunkHighest - subChunkLowest;

        subChunkDepthLowest.Add(positionZeroForY, subChunkLowest);

        subChunksRequired.Add(positionZeroForY, required);
        //================================================

        // SET STARTING DEPTH

        depth = (subChunkDepthLowest[positionZeroForY] * SUB_CHUNK_HEIGHT);

        // ===============================================

        // 0 = 0
        // 1 = 8
        // 2 = 16
        // 3 = 24
        // 4 = 32

        nativeNorthHeightMaps.Add(positionZeroForY, new NativeArray<float>(blockEngine.heightMap.subMaps[(positionTemp + new Vector2(0, blockEngine.heightMap.SUB_MAP_SIZE))], Allocator.Persistent));
        nativeSouthHeightMaps.Add(positionZeroForY, new NativeArray<float>(blockEngine.heightMap.subMaps[(positionTemp + new Vector2(0, -blockEngine.heightMap.SUB_MAP_SIZE))], Allocator.Persistent));
        nativeEastHeightMaps.Add(positionZeroForY, new NativeArray<float>(blockEngine.heightMap.subMaps[(positionTemp + new Vector2(blockEngine.heightMap.SUB_MAP_SIZE, 0))], Allocator.Persistent));
        nativeWestHeightMaps.Add(positionZeroForY, new NativeArray<float>(blockEngine.heightMap.subMaps[(positionTemp + new Vector2(-blockEngine.heightMap.SUB_MAP_SIZE, 0))], Allocator.Persistent));

        nativeHeightMaps.Add(positionZeroForY, new NativeArray<float>(blockEngine.heightMap.subMaps[(new Vector2(subMapPosition.x, subMapPosition.z))], Allocator.Persistent));

        nativeTriangles.Add(positionZeroForY, new NativeList<int>[subChunksRequired[positionZeroForY]]);
        nativeUvs.Add(positionZeroForY, new NativeArray<Vector2>[subChunksRequired[positionZeroForY]]);

        jobs.Add(positionZeroForY, new BlockDataWithShapeMapForSubChunkJob[subChunksRequired[positionZeroForY]]);

        jobHandles.Add(positionZeroForY, default);

        for (int i = 0; i < required; i++)
        {
            nativeTriangles[positionZeroForY][i] = new NativeList<int>(Allocator.Persistent);
            nativeUvs[positionZeroForY][i] = new NativeArray<Vector2>(blockEngine.rawSubChunkData.uvs, Allocator.Persistent);

            //if (i != 0) hasBottomNeighbour = true;

            tempSubChunkPosition.x = positionZeroForY.x;
            tempSubChunkPosition.y = depth;
            tempSubChunkPosition.z = positionZeroForY.z;

            jobs[positionZeroForY][i] = new BlockDataWithShapeMapForSubChunkJob
            {
                depth = depth,
                hasNorthNeighbour = blockEngine.newBlockQueue.subChunkNeighbours[chunkName][i][0],
                hasSouthNeighbour = blockEngine.newBlockQueue.subChunkNeighbours[chunkName][i][1],
                hasEastNeighbour = blockEngine.newBlockQueue.subChunkNeighbours[chunkName][i][2],
                hasWestNeighbour = blockEngine.newBlockQueue.subChunkNeighbours[chunkName][i][3],
                hasBottomNeighbour = blockEngine.newBlockQueue.subChunkNeighbours[chunkName][i][4],
                hasNorthEastNeighbour = blockEngine.newBlockQueue.subChunkNeighbours[chunkName][i][5],
                hasSouthEastNeighbour = blockEngine.newBlockQueue.subChunkNeighbours[chunkName][i][6],
                hasSouthWestNeighbour = blockEngine.newBlockQueue.subChunkNeighbours[chunkName][i][7],
                hasNorthWestNeighbour = blockEngine.newBlockQueue.subChunkNeighbours[chunkName][i][8],
                nativeNorthHeightMap = nativeNorthHeightMaps[positionZeroForY],
                nativeSouthHeightMap = nativeSouthHeightMaps[positionZeroForY],
                nativeEastHeightMap = nativeEastHeightMaps[positionZeroForY],
                nativeWestHeightMap = nativeWestHeightMaps[positionZeroForY],
                nativeHeightMap = nativeHeightMaps[positionZeroForY],
                shapeMap = new NativeArray<bool>(blockEngine.shapeMap.subMaps[tempSubChunkPosition], Allocator.Persistent),
                nativeTriangles = nativeTriangles[positionZeroForY][i],
                nativeUvs = nativeUvs[positionZeroForY][i],
                threshold = blockEngine.shapeMap.threshold,
                size = SUB_CHUNK_SIZE,
                height = SUB_CHUNK_HEIGHT,

                subChunkPosition = tempSubChunkPosition,    // sloppy creating duplicate position here and in shapeMap (new new remove)
            };

            jobHandles[positionZeroForY] = jobs[positionZeroForY][i].Schedule(jobHandles[positionZeroForY]);

            depth += SUB_CHUNK_HEIGHT;
        }

        IncrementSubMapPosition();
        UpdateProgress();
        depth = 0;
    }

    [BurstCompile]
    public void BeforeGenerating()
    {
        loadingProgressionData.savedSeconds = 9999999f;

        savedSubMapPosition = subMapPosition;

        CalculateTotal();
        CalculateSubMapsRequired();
    }

    [BurstCompile]
    public void CalculateTotal()
    {
        loadingProgressionData.total = defaultMapSize * defaultMapSize * COLUMN_BLOCK_HEIGHT;
    }

    [BurstCompile]
    public void CalculateSubMapsRequired()
    {
        loadingProgressionData.required = (float)loadingProgressionData.total / (SUB_CHUNK_SIZE * SUB_CHUNK_SIZE * COLUMN_BLOCK_HEIGHT);
    }

    [BurstCompile]
    public void UpdateProgress()
    {
        loadingProgressionData.count = chunkData.Count * (SUB_CHUNK_SIZE * SUB_CHUNK_SIZE * COLUMN_BLOCK_HEIGHT);
        loadingProgressionData.progress = (float)loadingProgressionData.count / (float)loadingProgressionData.total;
        loadingProgressionData.secondsRemaining = (int)(((loadingProgressionData.total - loadingProgressionData.count) / (SUB_CHUNK_SIZE * SUB_CHUNK_SIZE * COLUMN_BLOCK_HEIGHT)) / blockEngine.main.framesPerSecond.fps);
        if (loadingProgressionData.savedSeconds > loadingProgressionData.secondsRemaining)
        {
            loadingProgressionData.savedSeconds = loadingProgressionData.secondsRemaining;
        }
        else
        {
            loadingProgressionData.secondsRemaining = loadingProgressionData.savedSeconds;
        }
    }

    [BurstCompile]
    public void AddChunkData()
    {
        tempSubMapPositon.x = subMapPosition.x;
        tempSubMapPositon.y = subMapPosition.z;

        chunkData.Add(tempSubMapPositon, false);
        subChunkPositionsByChunk.Add(tempSubMapPositon, new List<Vector3>());
    }

    [BurstCompile]
    public void IncrementSubMapPosition()
    {
        // Are we at the last sub map in the row of sub maps?

        if (subMapPosition.x == (savedSubMapPosition.x + defaultMapSize - SUB_CHUNK_SIZE))
        {
            // Set to new row
            subMapPosition.x = savedSubMapPosition.x;
            subMapPosition.z += SUB_CHUNK_SIZE;
        }
        else { subMapPosition.x += SUB_CHUNK_SIZE; }  // Incremement in the horizontal
    }
}
