using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

//THE WRAP AROUNDS ARE HORRIBLE!!!WRAPPING AROUND WHEN DETECTING - WRAPPING AROUND WHEN 
public class WormMap : MonoBehaviour
{
    /*public BlockEngine blockEngine = null;

    public float defaultMapSize = 512;                  // size of the height map (limit = 46,336 / √2,147,483,647 - 46,340.9500010519853390887900102 )
    public int COLUMN_BLOCK_HEIGHT = 256;        // Total vertical number of blocks - chunk
    public int SUB_CHUNKS_PER_CHUNK = 32;        // 16 sub chunks in a "chunk"
    public int SUB_CHUNK_SIZE = 16;              // Cube
    public int SUB_CHUNK_HEIGHT = 8;             // Can NOT be 16 (24 verts * 16 * 16 * 16 > 65536)

    // Task progression data for visual display -  % done, time left
    public LoadingProgressionData loadingProgressionData = new LoadingProgressionData(true);

    // 0 = no cave. 1 = cave
    // if positions are out of scope must be removed
    public Dictionary<Vector3, int[]> maps = new Dictionary<Vector3, int[]>();

    // something that can be adjusted and looped through to create a shape/brush
    int[] brushNS = new int[] 
    {
        1008, 1024, 1040,                               // 0 0 1 1 1 0 0
        480, 496, 512, 528, 544,                        // 0 1 1 1 1 1 0
        208, 224, 240, 256, 272, 288, 304,              // 1 1 1 1 1 1 1
        -48, -32, -16, 0, 16, 32, 48,                   // 1 1 1 1 1 1 1
        -208, -224, -240, -256, -272, -288, -304,       // 1 1 1 1 1 1 1
        -480, -496, -512, -528, -544,                   // 0 1 1 1 1 1 0
        -1008, -1024, -1040                             // 0 0 1 1 1 0 0
    };

    int[] brushEW = new int[]
    {   // east       //west
        1023, 1024, 1025,                               // 0 0 1 1 1 0 0
        510, 511, 512, 513, 514,                        // 0 1 1 1 1 1 0
        253, 254, 255, 256, 257, 258, 259,              // 1 1 1 1 1 1 1
        3, 2, 1, 0, -1, -2, -3,                         // 1 1 1 1 1 1 1
        -253, -254, -255, -256, -257, -258, -259,       // 1 1 1 1 1 1 1
        -510, -511, -512, -513, -514,                   // 0 1 1 1 1 1 0
        -1023, -1024, -1025,                            // 0 0 1 1 1 0 0
    };

    //----------------------------------------------------

    public bool trigger = false; // testing
    public bool isGenerating = false;
    public bool jobComplete = false;    // testing
    public List<GameObject> testGameObjects = new List<GameObject>();
    public Vector3 startingPositon = Vector3.zero;
    public Vector3 startingChunkPosition = Vector3.zero;

    public PerlinWormJob job = new PerlinWormJob();
    public JobHandle jobHandle = new JobHandle();

    public NativeHashMap<Vector3, NativeArray<int>> nativeMaps = new NativeHashMap<Vector3, NativeArray<int>>();

    //----------------------------------------------------

    [BurstCompile]
    private void Update()
    {
        // for testing only.
        if (trigger)
        {
            for (int i = 0; i < testGameObjects.Count; i++)
            {
                Destroy(testGameObjects[i]);
            }

            testGameObjects.Clear();

            trigger = false;
            Generate(startingPositon, startingChunkPosition);
        }

        if (jobComplete)
        {
            jobComplete = false;
            foreach (Vector3 key in maps.Keys)
            {
                int z = 0;
                int x = 0;
                int y = 0;

                for (int i = 0; i < maps[key].Length; i++)
                {
                    if (maps[key][i] == 1)
                    {
                        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        go.transform.position = new Vector3(x, y, z);
                    }

                    z++;

                    if (z == SUB_CHUNK_SIZE - 1) 
                    {
                        z = 0;
                        x++;

                        if (x == SUB_CHUNK_SIZE - 1)
                        {
                            x = 0;
                            y++;
                        }
                    }
                }
            }

            maps.Clear();
        }
    }

    [BurstCompile]
    private void OnDestroy()
    {
        // Free memory
        blockEngine = null;

        if (!isGenerating) return;

        jobHandle.Complete();

        foreach (Vector3 key in nativeMaps.GetKeyArray(Allocator.Temp))
        {            
            nativeMaps[key].Dispose();
        }

        nativeMaps.Dispose();
        job.brushNS.Dispose();
        job.brushEW.Dispose();
    }

    [BurstCompile]
    public void CleanUp()
    {

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
    }

    [BurstCompile]
    public void LateUpdate()
    {
        // ADD COMPLETE JOB
        if (!isGenerating) return;

        if (jobHandle.IsCompleted)
        {
            jobHandle.Complete();

            NativeArray<Vector3> keys = job.nativeMaps.GetKeyArray(Allocator.Temp);

            foreach (Vector3 key in keys)
            {
                maps.Add(key, job.nativeMaps[key].ToArray());

                job.nativeMaps[key].Dispose();
            }

            keys.Dispose();

            job.nativeMaps.Dispose();

            jobComplete = true;
        }
    }

    // Worms path will be used when generating surface chunks. 
    // After which go through left over worm chunks and generate them!
    // Use perlin noise to guide. Perlin noise will make the generating of caves consistant.
    [BurstCompile]
    public struct PerlinWormJob : IJob
    {
        // The world position
        public Vector3 offset;

        public NativeHashMap<Vector3, NativeArray<int>> nativeMaps;

        public int numberOfWorms;
        public int wormLength;

        // local position
        public Vector3 startPosition;
        public Vector3 startingChunkPosition;

        public NativeArray<int> brushNS;
        public NativeArray<int> brushEW;

        public int startingIndex;

        public int chunkSize;
        public int chunkHeight;

        public int maxWormLocalIndex;

        [BurstCompile]
        public void Execute()
        {
            // insert start position chunk into array
            // all values will be set to 0 on new.
            NativeArray<int> currentChunk = new NativeArray<int>(chunkSize * chunkSize * chunkHeight, Allocator.Persistent);
            Vector3 currentChunkPosition = startingChunkPosition;
            Vector3 nextChunkPosition = startingChunkPosition;

            // Set initial starting position to 1 to represent the start of the cave.
            // all worm positions after will also be marked as 1.
            // Leaving a trail (cave) of 1's.
            nativeMaps.Add(startPosition, currentChunk);

            currentChunk[startingIndex] = 1;

            Vector3 currentPosition = startPosition;

            // north = 0, east 1, south 2, west 3
            int oldHorizontalDirection = 0;

            //int newDirection = 0;
            int horizontalDirection = 0;

            float horizontalValue = 0;
            float verticalValue = 0;

            int localWormIndex = startingIndex;
            int worldWormIndex = startingIndex;

            int indexWithoutY = 0;
            int y = 0;
            int x = 0;
            int z = 0;

            bool topEdge = false;
            bool bottomEdge = false;
            bool northEdge = false;
            bool eastEdge = false;
            bool southEdge = false;
            bool westEdge = false;

            for (int n = 0; n < numberOfWorms; n++)
            {
                // z > x > y

                for (int i = 0; i < wormLength; i++)
                {
                    // needed for wrap around - simplified
                    indexWithoutY = (int) localWormIndex % 256;
                    y = localWormIndex / 256;
                    x = indexWithoutY / 16;
                    z = indexWithoutY % 16;

                    //----------------------------------------------
                    // Reset wrap around check.

                    topEdge = false;
                    bottomEdge = false;
                    northEdge = false;
                    eastEdge = false;
                    southEdge = false;
                    westEdge = false;

                    //----------------------------------------------
                    // Detect edge for wrap around.

                    if (localWormIndex % 256 <= 15) { eastEdge = true; }                                        // "Left" 
                    if (localWormIndex % 256 >= 240 && localWormIndex % 256 <= 255) { westEdge = true; }        // "Right"
                    if (localWormIndex % 256 % 15 == 0) { northEdge = true; }                                   // "front"
                    if (localWormIndex % 256 % 16 == 0) { southEdge = true; }                                   // "back"
                    if (localWormIndex <= 255) { bottomEdge = true; }                                           // "Bottom"
                    if (localWormIndex >= 1792) { topEdge = true; }                                             //"Top"

                    //-----------------------------------------------

                    // use currentPosition in decision of next HORIZONTAL direction of perlin noise.
                    // value will be between -1 and 1; 
                    // < -0.5f left.
                    // > 0.5f right. 
                    // else forward.

                    horizontalValue = noise.snoise(new float2(currentPosition.x * 0.0125f, currentPosition.z * 0.0125f));

                    // use currentPosition + an offset in decision of next VERTICAL direction of perlin noise.
                    // < 0f down
                    // > 0.85f up
                    // Note down is a larger range than up. More likely to go down.
                    // Has a small range in which nothing will happen.

                    verticalValue = noise.snoise(new float2(currentPosition.x * 0.0125f, currentPosition.y * 0.0125f));

                    //-----------------------------------------------
                    // Horizontal
                    if (horizontalValue < -0.5f)
                    {
                        //-----------------------------------------------
                        // left
                        // facing north
                        if (oldHorizontalDirection == 0)
                        {
                            if (!westEdge) { localWormIndex -= chunkSize; }                                 // going west
                            else
                            {
                                localWormIndex = (localWormIndex - indexWithoutY) + (256 - (15 - z));       // wrap around west to east edge
                                nextChunkPosition += new Vector3(-16, 0, 0);
                            }
                        }
                        else
                        {
                            // facing east
                            if (oldHorizontalDirection == 1)
                            {
                                if (!northEdge) { localWormIndex += 1; }        // going north
                                else 
                                {
                                    localWormIndex -= (chunkSize - 1);          // wrap around north to south edge
                                    nextChunkPosition += new Vector3(0, 0, 16);
                                }
                            }
                            else
                            {
                                // facing south
                                if (oldHorizontalDirection == 2)
                                {
                                    if (!eastEdge) { localWormIndex += chunkSize; }                         // going east
                                    else 
                                    { 
                                        localWormIndex = (localWormIndex - indexWithoutY) + z;              // wrap around east to west
                                        nextChunkPosition += new Vector3(16, 0, 0);
                                    }
                                }
                                else
                                {
                                    // facing west
                                    if (oldHorizontalDirection == 3)
                                    {
                                        if (!southEdge) localWormIndex -= 1;                // going south
                                        else 
                                        { 
                                            localWormIndex += (chunkSize - 1);              // wrap around south to north edge
                                            nextChunkPosition += new Vector3(0, 0, -16);
                                        }
                                    }
                                }
                            }
                        }

                        //-----------------------------------------------
                        // limit 0-3 wrap around direction
                        if (horizontalDirection > 0)
                        {
                            horizontalDirection--;
                        }
                        else
                        {
                            horizontalDirection = 3;
                        }
                    }
                    else
                    {
                        //-----------------------------------------------
                        // Horizontal
                        if (horizontalValue > 0.5f)
                        {
                            //-----------------------------------------------
                            // right
                            // facing north
                            if (oldHorizontalDirection == 0)
                            {
                                if (!eastEdge) { localWormIndex += chunkSize; }                 // going east
                                else
                                {
                                    localWormIndex = (localWormIndex - indexWithoutY) + z;      // wrap around east to west
                                    nextChunkPosition += new Vector3(16, 0, 0);
                                }
                            }
                            else
                            {
                                // facing east
                                if (oldHorizontalDirection == 1)
                                {
                                    if (!southEdge) { localWormIndex -= 1; }    // going south
                                    else
                                    { 
                                        localWormIndex += (chunkSize - 1);      // wrap around south to north edge
                                        nextChunkPosition += new Vector3(0, 0, -16);
                                    }
                                }
                                else
                                {
                                    // facing south
                                    if (oldHorizontalDirection == 2)
                                    {
                                        if (!westEdge) { localWormIndex -= chunkSize; }                                 // going west
                                        else 
                                        { 
                                            localWormIndex = (localWormIndex - indexWithoutY) + (256 - (15 - z));       // wrap around west to east edge
                                            nextChunkPosition += new Vector3(-16, 0, 0);
                                        }
                                    }
                                    else
                                    {
                                        // facing west
                                        if (oldHorizontalDirection == 3)
                                        {
                                            if (!northEdge) { localWormIndex += 1; }        // going north
                                            else 
                                            {
                                                localWormIndex -= (chunkSize - 1);          // wrap around north to south edge
                                                nextChunkPosition += new Vector3(0, 0, 16);
                                            }
                                        }
                                    }
                                }
                            }

                            //-----------------------------------------------
                            // limit 0-3 wrap around
                            if (horizontalDirection < 3)
                            {
                                horizontalDirection++;
                            }
                            else
                            {
                                horizontalDirection = 0;
                            }
                        }
                        else
                        {
                            //-----------------------------------------------
                            // Horizontal
                            // Forward
                            // Facing north
                            if (oldHorizontalDirection == 0)
                            {
                                if (!northEdge) { localWormIndex += 1; }    // going north
                                else 
                                {
                                    localWormIndex -= (chunkSize - 1);      // wrap around north to south edge
                                    nextChunkPosition += new Vector3(0, 0, 16);
                                }
                            }
                            else
                            {
                                // Facing east
                                if (oldHorizontalDirection == 1)
                                {
                                    if (!eastEdge) { localWormIndex += chunkSize; }                 // going east
                                    else 
                                    { 
                                        localWormIndex = (localWormIndex - indexWithoutY) + z;      // wrap around east to west
                                        nextChunkPosition += new Vector3(16, 0, 0);
                                    }
                                }
                                else
                                {
                                    // facing south
                                    if (oldHorizontalDirection == 2)
                                    {
                                        if (!southEdge) { localWormIndex -= 1; }    // going south
                                        else 
                                        { 
                                            localWormIndex += (chunkSize - 1);      // wrap around south to north edge
                                            nextChunkPosition += new Vector3(0, 0, -16);
                                        }
                                    }
                                    else
                                    {
                                        // facing west
                                        if (oldHorizontalDirection == 3)
                                        {
                                            if (!westEdge) { localWormIndex -= chunkSize; }                             // going west
                                            else 
                                            { 
                                                localWormIndex = (localWormIndex - indexWithoutY) + (256 - (15 - z));   // wrap around west to east edge
                                                nextChunkPosition += new Vector3(-16, 0, 0);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    //--------------------------------------------
                    // Vertical
                    if (verticalValue < 0f)
                    {
                        // down
                        if (!bottomEdge) { localWormIndex -= 256; }
                        else 
                        { 
                            localWormIndex = 2047 - indexWithoutY;
                            nextChunkPosition += new Vector3(0, -16, 0);
                        }
                    }
                    else
                    {
                        if (verticalValue > 0.85f)
                        {
                            // up
                            if (!topEdge) { localWormIndex += 256; }
                            else 
                            { 
                                localWormIndex = indexWithoutY;
                                nextChunkPosition += new Vector3(0, 16, 0);
                            }
                        }
                    }

                    if (currentChunkPosition != nextChunkPosition)
                    {
                        // Check if next chunk position is a new one

                        if (!nativeMaps.ContainsKey(nextChunkPosition))
                        {
                            currentChunk = new NativeArray<int>(chunkSize * chunkSize * chunkHeight, Allocator.Persistent);
                            nativeMaps.Add(nextChunkPosition, currentChunk);
                        }

                        currentChunkPosition = nextChunkPosition;
                    }

                    // if horizontal direction is north/south
                    if (horizontalDirection == 0
                        || horizontalDirection == 2)
                    {
                        for (int b = 0; b < brushNS.Length; b++)
                        {
                            currentChunk[brushNS[b] + localWormIndex] = 1;
                        }
                    }
                    else    // if horizontal direction is east/west
                    {
                        for (int b = 0; b < brushEW.Length; b++)
                        {
                            currentChunk[brushEW[b] + localWormIndex] = 1;
                        }
                    }

                    //---------------------------------------------
                    // Set horizontal for next iteration
                    oldHorizontalDirection = horizontalDirection;
                } 
            }
        }
    }

    [BurstCompile]
    public void Generate(Vector3 startPosition, Vector3 startingChunkPosition)
    {
        nativeMaps = new NativeHashMap<Vector3, NativeArray<int>>(6, Allocator.Persistent);

        //jobHandle = default;

        job = new PerlinWormJob
        {
            brushNS = new NativeArray<int>(brushNS, Allocator.Persistent),
            brushEW = new NativeArray<int>(brushEW, Allocator.Persistent),
            nativeMaps = nativeMaps,
            wormLength = 32,
            numberOfWorms = 1,
            startPosition = startPosition,
            startingChunkPosition = startingChunkPosition,
            startingIndex = 0,
            maxWormLocalIndex = SUB_CHUNK_SIZE * SUB_CHUNK_SIZE * SUB_CHUNK_HEIGHT,
            chunkSize = SUB_CHUNK_SIZE,
            chunkHeight = SUB_CHUNK_HEIGHT,
            offset = Vector3.zero,
        };

        jobHandle = job.Schedule(jobHandle);

        isGenerating = true;
    }*/
}
