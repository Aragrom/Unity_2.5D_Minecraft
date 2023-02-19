using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Take 2D height maps and apply 3D perlin noise for more interesting terrain.
/// Binary as either block exists = 1. or doesn't = 0.
/// </summary>

[BurstCompile]
public class TerrainMap : MonoBehaviour
{
    public BlockEngine blockEngine = null;

    public bool triggerTest = false;

    public int height = 8;
    public int size = 16;

    public float thresholdA = 0f;      // Set higher to remove more of the sample found
    public float noiseFilterAX = 0.04f;   // Make smaller to make the section cut out larger
    public float noiseFilterAY = 0.04f;   // Make smaller to make the section cut out larger

    public float thresholdB = 0f;      // Set higher to remove more of the sample found
    public float noiseFilterBX = 0.04f;   // Make smaller to make the section cut out larger
    public float noiseFilterBY = 0.04f;   // Make smaller to make the section cut out larger

    public Vector3 subMapPosition = new Vector3(-256, 0, -256);
    private Vector3 savedSubMapPositon = Vector2.zero;               // used when incrementing the submap postion (x != 0 .. x == savedSubMapPosition.x)

    public Dictionary<Vector2, List<bool>> subMaps = new Dictionary<Vector2, List<bool>>();

    public int depth = 0;

    public float defaultMapSize = 512;                  // size of the height map (limit = 46,336 / √2,147,483,647 - 46,340.9500010519853390887900102 )
    public int COLUMN_BLOCK_HEIGHT = 256;        // Total vertical number of blocks - chunk
    public int SUB_CHUNKS_PER_CHUNK = 32;        // 16 sub chunks in a "chunk"
    public int SUB_CHUNK_SIZE = 16;              // Cube
    public int SUB_CHUNK_HEIGHT = 8;             // Can NOT be 16 (24 verts * 16 * 16 * 16 > 65536)

    // Task progression data for visual display -  % done, time left
    public LoadingProgressionData loadingProgressionData = new LoadingProgressionData(true);

    // ==================================================

    public List<GameObject> pool = new List<GameObject>();

    public float xy;
    public float xz;
    public float yz;
    public float yx;
    public float zx;
    public float zy;

    public float2 offset = new float2(100, 100);
    public float2 secondOffset= new float2(1000, 1000);

    public List<bool> results = new List<bool>();

    public float tempResultA = 0;
    public float tempResultB = 0;

    // ==================================================

    /*Dictionary<Vector3, NativeArray<bool>> blockMap = new Dictionary<Vector3, NativeArray<bool>>();

    Dictionary<Vector3, Perlin3DLookUpJob[]> jobs = new Dictionary<Vector3, Perlin3DLookUpJob[]>();

    public Dictionary<Vector3, JobHandle> jobHandles = new Dictionary<Vector3, JobHandle>();*/

    // ==================================================

    [BurstCompile]
    private void OnDestroy()
    {
        // Free memory

        blockEngine = null;

        subMaps.Clear();
        subMaps = null;

        pool.Clear();
        pool = null;

        results.Clear();
        results = null;
    }

    [BurstCompile]
    private void OnApplicationQuit()
    {
        // NEED TO COMPLETE CLOSING JOB PROPERLY
        // CURRENTLY .Complete() IS CALLED AFTER JOB FORCING IT ON THE MAIN THREAD
    }

    [BurstCompile]
    public void LateUpdate()
    {
        // NEED TO ADD COMPLETE JOB
    }

    [BurstCompile]
    private void Awake()
    {
        blockEngine = GetComponent<BlockEngine>();
    }

    [BurstCompile]
    public void Update()
    {
        if (triggerTest)
        {
            foreach (GameObject go in pool)
            {
                GameObject.Destroy(go);
            }

            pool.Clear();

            triggerTest = false;
            Test();
        }
    }

    [BurstCompile]
    private void Test()
    {
        results = new List<bool>();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < size; x++)
            {
                for (int z = 0; z < size; z++)
                {
                    // 2 layers of 3D noise.
                    // A ==============

                    xy = noise.snoise(new float2((offset.x + x) * noiseFilterAX, (offset.y + y) * noiseFilterAY));
                    xz = noise.snoise(new float2((offset.x + x) * noiseFilterAX, (offset.y + z) * noiseFilterAY));
                    yz = noise.snoise(new float2((offset.x + y) * noiseFilterAX, (offset.y + z) * noiseFilterAY));
                    yx = noise.snoise(new float2((offset.x + y) * noiseFilterAX, (offset.y + x) * noiseFilterAY));
                    zx = noise.snoise(new float2((offset.x + z) * noiseFilterAX, (offset.y + z) * noiseFilterAY));
                    zy = noise.snoise(new float2((offset.x + z) * noiseFilterAX, (offset.y + y) * noiseFilterAY));

                    tempResultA = (xy + xz + yz + yx + zx + zy) / 6;

                    // B ==============

                    xy = noise.snoise(new float2((secondOffset.x + x) * noiseFilterBX, (secondOffset.y + y) * noiseFilterBY));
                    xz = noise.snoise(new float2((secondOffset.x + x) * noiseFilterBX, (secondOffset.y + z) * noiseFilterBY));
                    yz = noise.snoise(new float2((secondOffset.x + y) * noiseFilterBX, (secondOffset.y + z) * noiseFilterBY));
                    yx = noise.snoise(new float2((secondOffset.x + y) * noiseFilterBX, (secondOffset.y + x) * noiseFilterBY));
                    zx = noise.snoise(new float2((secondOffset.x + z) * noiseFilterBX, (secondOffset.y + z) * noiseFilterBY));
                    zy = noise.snoise(new float2((secondOffset.x + z) * noiseFilterBX, (secondOffset.y + y) * noiseFilterBY));

                    tempResultB = (xy + xz + yz + yx + zx + zy) / 6;

                    // ================

                    if (tempResultA > thresholdA
                        && tempResultB > thresholdB)
                    {
                        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        cube.transform.position = new Vector3(x, y, z);

                        pool.Add(cube);

                        results.Add(false);
                    }
                    else
                    {
                        results.Add(true);
                    }
                }
            }
        }
    }

    // A SECTION OF 3D NOISE COULD BE USED TO DECIDE WHERE 3D PERLIN NOISE SHOULD BE. VERY EXPERNSIVE.

    // Raw Data
    [BurstCompile]
    public struct NoiseForTerrainMapJob : IJob
    {
        public NativeArray<bool> shapeBuffer;

        public int size;
        public int height;

        public float2 offset;
        public float2 secondOffset;

        public float thresholdA;
        public float thresholdB;
        public float noiseFilterA;
        public float noiseFilterB;

        public float tempResultA;
        public float tempResultB;

        public int depth;   // can be across several chunks. 0 - 8 - 16 - 24 - 32

        public float xy;
        public float xz;
        public float yz;
        public float yx;
        public float zx;
        public float zy;

        public int iterator;

        [BurstCompile]
        public void Execute()
        {
            iterator = 0;

            // Y > X > Z

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    for (int z = 0; z < size; z++)
                    {
                        xy = noise.snoise(new float2((offset.x + x) * 0.0125f, (offset.y + y) * 0.0125f));
                        xz = noise.snoise(new float2((offset.x + x) * 0.0125f, (offset.y + z) * 0.0125f));
                        yz = noise.snoise(new float2((offset.x + y) * 0.0125f, (offset.y + z) * 0.0125f));
                        yx = noise.snoise(new float2((offset.x + y) * 0.0125f, (offset.y + x) * 0.0125f));
                        zx = noise.snoise(new float2((offset.x + z) * 0.0125f, (offset.y + z) * 0.0125f));
                        zy = noise.snoise(new float2((offset.x + z) * 0.0125f, (offset.y + y) * 0.0125f));

                        tempResultA = (xy + xz + yz + yx + zx + zy) / 6;

                        xy = noise.snoise(new float2((offset.x + x) * 0.0125f, (offset.y + y) * 0.0125f));
                        xz = noise.snoise(new float2((offset.x + x) * 0.0125f, (offset.y + z) * 0.0125f));
                        yz = noise.snoise(new float2((offset.x + y) * 0.0125f, (offset.y + z) * 0.0125f));
                        yx = noise.snoise(new float2((offset.x + y) * 0.0125f, (offset.y + x) * 0.0125f));
                        zx = noise.snoise(new float2((offset.x + z) * 0.0125f, (offset.y + z) * 0.0125f));
                        zy = noise.snoise(new float2((offset.x + z) * 0.0125f, (offset.y + y) * 0.0125f));

                        tempResultB = (xy + xz + yz + yx + zx + zy) / 6;

                        if (tempResultA > thresholdA
                            && tempResultB > thresholdB)
                        {
                            shapeBuffer[iterator] = false;
                        }
                        else
                        {
                            shapeBuffer[iterator] = true;
                        }

                        iterator++;
                    }
                }
            }
        }
    }

    [BurstCompile]
    public void GenerateLocationsForShapeMaps(int numberOfSections)
    {
        NativeArray<bool>[] shapeBuffer = new NativeArray<bool>[numberOfSections];

        NoiseForTerrainMapJob[] jobs = new NoiseForTerrainMapJob[numberOfSections];

        Vector2[] subMapPositions = new Vector2[numberOfSections];

        JobHandle jobHandle = default;

        for (int i = 0; i < numberOfSections; i++)
        {
            shapeBuffer[i] = new NativeArray<bool>(size * size * height, Allocator.TempJob);

            subMapPositions[i] = new float2(subMapPosition.x, subMapPosition.y);

            jobs[i] = new NoiseForTerrainMapJob
            {
                shapeBuffer = shapeBuffer[i],
                size = size,
                height = height,
                offset = new float2(subMapPosition.x, subMapPosition.y) + offset,
                secondOffset = secondOffset,
                //noiseFilterA = noiseFilterA,
                //noiseFilterB = noiseFilterB,
                thresholdA = thresholdA,
                thresholdB = thresholdB,
            };
            jobHandle = jobs[i].Schedule(jobHandle);

            AddSubMap();
            IncrementSubMapPosition();
        }

        jobHandle.Complete();

        for (int i = 0; i < numberOfSections; i++)
        {
            subMaps[subMapPositions[i]].AddRange(jobs[i].shapeBuffer.ToArray());
            shapeBuffer[i].Dispose();
        }

        UpdateProgress();
    }

    [BurstCompile]
    public void BeforeGenerating()
    {
        loadingProgressionData.savedSeconds = 9999999f;

        savedSubMapPositon = subMapPosition;

        CalculateTotal();
        CalculateSubMapsRequired();
    }

    [BurstCompile]
    public void CalculateTotal()
    {
        //loadingProgressionData.total = defaultMapSize * defaultMapSize * COLUMN_BLOCK_HEIGHT;
    }

    [BurstCompile]
    public void CalculateSubMapsRequired()
    {
        //loadingProgressionData.required = (float)loadingProgressionData.total / (SUB_CHUNK_SIZE * SUB_CHUNK_SIZE * COLUMN_BLOCK_HEIGHT);
    }

    [BurstCompile]
    public void UpdateProgress()
    {
        /*loadingProgressionData.count = chunkData.Count * (SUB_CHUNK_SIZE * SUB_CHUNK_SIZE * COLUMN_BLOCK_HEIGHT);
        loadingProgressionData.progress = (float)loadingProgressionData.count / (float)loadingProgressionData.total;
        loadingProgressionData.secondsRemaining = (int)(((loadingProgressionData.total - loadingProgressionData.count) / (SUB_CHUNK_SIZE * SUB_CHUNK_SIZE * COLUMN_BLOCK_HEIGHT)) / blockEngine.main.framesPerSecond.fps);
        if (loadingProgressionData.savedSeconds > loadingProgressionData.secondsRemaining)
        {
            loadingProgressionData.savedSeconds = loadingProgressionData.secondsRemaining;
        }
        else
        {
            loadingProgressionData.secondsRemaining = loadingProgressionData.savedSeconds;
        }*/
    }

    [BurstCompile]
    public void AddSubMap()
    {
        subMaps.Add(new Vector2(subMapPosition.x, subMapPosition.z), new List<bool>());
    }

    [BurstCompile]
    public void IncrementSubMapPosition()
    {
        // Are we at the last sub map in the row of sub maps?

        if (subMapPosition.x == (savedSubMapPositon.x + defaultMapSize - SUB_CHUNK_SIZE))
        {
            // Set to new row
            subMapPosition.x = savedSubMapPositon.x;
            subMapPosition.z += SUB_CHUNK_SIZE;
        }
        else { subMapPosition.x += SUB_CHUNK_SIZE; }  // Incremement in the horizontal
    }
}
