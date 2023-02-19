using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Generate and store "Height Map" for the "Terrain" GameObject
/// </summary>
[BurstCompile]
public class TerrainHeightMap : MonoBehaviour
{
    private TerrainEngine terrainEngine = null;

    public Vector2 subMapPosition = new Vector2(-2048, -2048);
    public Vector2 savedSubMapPosition = new Vector2(-2048, -2048);  // used when incrementing the submap postion (x != 0 .. x == savedSubMapPosition.x)

    public float defaultMapSize = 255;                              // size of the height map (limit = 46,336 / √2,147,483,647 - 46,340.9500010519853390887900102 )
    public int SUB_MAP_SIZE = 255;                           // grid - size * size

    public float2 offset = new float2(0, 0);                        // The amount to offset the perlin noise sample
    public float2 secondSampleOffset = new float2(1000, 1000);      // Offset for the second sample. height is lerped between first sample and second sample at the mid point.
    public float noiseFilter = 0.005f;

    //public Dictionary<Vector2, List<float>> subMaps = new Dictionary<Vector2, List<float>>();

    public Dictionary<Vector2, float> heights = new Dictionary<Vector2, float>();

    public float[,] correctFormatValues = new float[0, 0];

    public LoadingProgressionData loadingProgressionData = new LoadingProgressionData(true);

    // Generate height parameters for job =======

    //Dictionary<Vector2, NativeArray<float>> heightMaps = new Dictionary<Vector2, NativeArray<float>>();
    Dictionary<Vector2, NoiseForTerrainHeightMapJob> jobs = new Dictionary<Vector2, NoiseForTerrainHeightMapJob>();
    public Dictionary<Vector2, JobHandle> jobHandlesGenerateHeights = new Dictionary<Vector2, JobHandle>();
    public Dictionary<Vector2, Vector2> jobGridSize = new Dictionary<Vector2, Vector2>();

    //===========================================

    public Dictionary<Vector2, GetPositionsFromHashMapJob> getHeightsFromHashMapJob = new Dictionary<Vector2, GetPositionsFromHashMapJob>();
    public Dictionary<Vector2, JobHandle> jobHandlesGetFromHashMap = new Dictionary<Vector2, JobHandle>();
    public NativeHashMap<float2, float> hashMap;
    public Vector3[] sortedPositions = new Vector3[65536];

    [BurstCompile]
    private void OnDestroy()
    {
        // Free memory

        terrainEngine = null;

        heights.Clear();
        heights = null;
        correctFormatValues = null;
        jobs.Clear();
        jobs = null;
        jobHandlesGenerateHeights.Clear();
        jobHandlesGenerateHeights = null;

        jobGridSize.Clear();
        jobGridSize = null;
        getHeightsFromHashMapJob.Clear();
        getHeightsFromHashMapJob = null;
        jobHandlesGetFromHashMap.Clear();
        jobHandlesGetFromHashMap = null;
        //hashMap.Clear();
        sortedPositions = null;
    }

    /*[BurstCompile]
    public void CleanUp()
    {
        heights.Clear();
    }*/

    [BurstCompile]
    private void Awake()
    {
        terrainEngine = GetComponent<TerrainEngine>();
        hashMap = new NativeHashMap<float2, float>(65336 * 10, Allocator.Persistent);
    }

    [BurstCompile]
    public void OnApplicationQuit()
    {
        foreach (Vector2 key in terrainEngine.terrainHeightMap.jobHandlesGenerateHeights.Keys)
        {
            terrainEngine.terrainHeightMap.jobHandlesGenerateHeights[key].Complete();
        }

        foreach (Vector2 key in terrainEngine.terrainHeightMap.jobHandlesGetFromHashMap.Keys)
        {
            terrainEngine.terrainHeightMap.jobHandlesGetFromHashMap[key].Complete();

            terrainEngine.terrainHeightMap.getHeightsFromHashMapJob[key].result.Dispose();
        }

        terrainEngine.terrainHeightMap.jobHandlesGenerateHeights.Clear();
        terrainEngine.terrainHeightMap.jobHandlesGetFromHashMap.Clear();

        jobHandlesGenerateHeights.Clear();
        jobHandlesGetFromHashMap.Clear();

        hashMap.Clear();
        hashMap.Dispose();
    }

    [BurstCompile]
    public void LateUpdate()
    {
        // NEED TO ADD CHECK AND CLEAN UP hashMap. SIZE manage. <<<<

        if (jobHandlesGenerateHeights.Count != 0)
        {
            List<Vector2> positionsToRemove = new List<Vector2>();

            foreach (Vector2 key in jobHandlesGenerateHeights.Keys)
            {
                if (jobHandlesGenerateHeights[key].IsCompleted)
                {
                    jobHandlesGenerateHeights[key].Complete();

                    hashMap = jobs[key].hashMap;

                    positionsToRemove.Add(key);

                    terrainEngine.newTerrainQueue.hasNewTerrain = true;
                }
            }

            for (int i = 0; i < positionsToRemove.Count; i++)
            {
                // Remove all field dictionary elements

                jobs.Remove(positionsToRemove[i]);
                jobHandlesGenerateHeights.Remove(positionsToRemove[i]);
                jobGridSize.Remove(positionsToRemove[i]);
            }
        }

        // ==================

        if (jobHandlesGetFromHashMap.Count != 0)
        {
            List<Vector2> positionsToRemove = new List<Vector2>();

            foreach (Vector2 key in jobHandlesGetFromHashMap.Keys)
            {
                if (jobHandlesGetFromHashMap[key].IsCompleted)
                {
                    jobHandlesGetFromHashMap[key].Complete();

                    sortedPositions = getHeightsFromHashMapJob[key].result.ToArray();
                                      
                    getHeightsFromHashMapJob[key].result.Dispose();

                    positionsToRemove.Add(key);

                    // This could be set true by multiple jobs 
                    // but due to the logic being set in a coroutine 
                    // will limit how many time the actual terrain mesh
                    // is updated.

                    terrainEngine.newTerrainQueue.newSortedTerrain = true;
                }
            }

            for (int i = 0; i < positionsToRemove.Count; i++)
            {
                // Remove all field dictionary elements

                getHeightsFromHashMapJob.Remove(positionsToRemove[i]);
                jobHandlesGetFromHashMap.Remove(positionsToRemove[i]);
            }
        }
    }

    [BurstCompile]
    public struct GetPositionsFromHashMapJob : IJob
    {
        public NativeHashMap<float2, float> hashMap;

        public NativeArray<Vector3> result;

        public float startPositionX;
        public float startPositionY;

        public float lowMask;
        public float highMask;

        public float size;

        public float step;

        public int iterator;

        float2 tempPosition;

        [BurstCompile]
        public void Execute()
        {
            iterator = 0;

            Vector3 offSet = new Vector3(startPositionX + 2048, 0, startPositionY + 2048);

            for (float y = startPositionY; y < startPositionY + (size * step); y += step)   // x = 16. offset to sample height at the center of "chunk"
            {
                for (float x = startPositionX; x < startPositionX + (size * step); x += step)
                {
                    tempPosition = new float2(x, y);

                    //if (hashMap.ContainsKey(tempPosition))
                    //{
                        if ((x - startPositionX < lowMask
                        || x - startPositionX > highMask)
                        || (y - startPositionY < lowMask
                        || y - startPositionY> highMask))
                        {
                            result[iterator] = new Vector3(x, hashMap[tempPosition] * 256, y);
                        }
                        else
                        {
                            //result[iterator] = new Vector3(x, hashMap[tempPosition] * 256, y);
                            result[iterator] = new Vector3(x, -128, y);  // pushes terrain down and round the "block engine"/chunks/sub chunks etc range around 0,0,0
                        }
                    //}
                    //else
                    //{
                        // Showing error - force into the sky
                        //result[iterator] = new Vector3(x, 2048, y);
                    //}

                    // Reset the position of each result.
                    // So that the object doesn't need to be moved as each "position" is the actual position of the vertice
                    // JUST THINK ABOUT IT!

                    result[iterator] -= offSet;

                    iterator++;
                }
            }
        }
    }

    [BurstCompile]
    struct NoiseForTerrainHeightMapJob : IJob
    {
        public int step;

        public float2 offset;
        public float2 secondSampleOffset;
        public int xSize;
        public int zSize;
        public float noiseFilter;
        //public NativeArray<float> heightBuffer;
        public NativeHashMap<float2, float> hashMap;
        public int iterator;
        //public float tempHeight;
        public float tempX;
        public float tempY;

        public float tempValue;
        public float2 tempPosition;

        public int startPositionX;
        public int startPositionY;

        public float noiseShape;        

        public float rollingHillsA;
        public float rollingHillsB;

        public float rollingHills;

        public float bigHills;

        //public float mapModifier;

        [BurstCompile]
        public void Execute()
        {
            iterator = 0;

            for (int x = startPositionX; x < startPositionX + (xSize * step); x += step)   // x = 16. offset to sample height at the center of "chunk"
            {
                for (int y = startPositionY; y < startPositionY + (zSize * step); y += step)
                {
                    tempX = x;
                    tempY = y;

                    tempPosition = new float2(tempX, tempY);

                    if (!hashMap.ContainsKey(tempPosition))
                    {
                        tempValue = 0;

                        // BIG HILLS ONTOP OF ROLLING HILLS - noise shape used - above 0

                        noiseShape = noise.snoise(new float2((tempX + offset.x) * 0.0125f, (tempY + offset.y) * 0.0125f));

                        if (noiseShape > 0.0)
                        {
                            // BIG HILLS

                            bigHills = math.lerp(noise.snoise(new float2((tempX + offset.x) * 0.0125f, (tempY + offset.y) * 0.0125f)),
                                            noise.snoise(new float2((tempX + offset.x + secondSampleOffset.x) * 0.05f, (tempY + offset.y + secondSampleOffset.y) * 0.05f)),
                                            0.5f) / 4;

                            // ROLLING HILLS

                            rollingHillsA = math.lerp(noise.snoise(new float2((tempX + offset.x) * 0.00125f, (tempY + offset.y) * 0.00125f)),
                                noise.snoise(new float2((tempX + offset.x + secondSampleOffset.x) * 0.0025f, (tempY + offset.y + secondSampleOffset.y) * 0.0025f)),
                                0.5f) / 4;

                            rollingHillsB = math.lerp(noise.snoise(new float2((tempX + offset.x) * 0.0125f, (tempY + offset.y) * 0.0125f)),
                                noise.snoise(new float2((tempX + offset.x + secondSampleOffset.x * 2) * 0.0025f, (tempY + offset.y + secondSampleOffset.y * 2) * 0.0025f)),
                                0.5f) / 4;

                            rollingHills = math.lerp(rollingHillsA,
                                rollingHillsB,
                                0.5f);

                            // Combine

                            tempValue = math.lerp(rollingHills,
                                rollingHills + bigHills,
                                0.75f);

                            // Merge

                            if (noiseShape > 0.25f
                                && noiseShape < 0.5f)
                            {
                                tempValue = math.lerp(tempValue,
                                rollingHills,
                                0.64f);
                            }
                            else
                            {
                                if (noiseShape < 0.25f)
                                {
                                    tempValue = math.lerp(tempValue,
                                    rollingHills,
                                    0.5f);
                                }
                                else
                                {
                                    tempValue = math.lerp(tempValue,
                                    rollingHills,
                                    0.9f);
                                }
                            }
                        }
                        else
                        {
                            // ROLLING HILLS

                            rollingHillsA = math.lerp(noise.snoise(new float2((tempX + offset.x) * 0.00125f, (tempY + offset.y) * 0.00125f)),
                                noise.snoise(new float2((tempX + offset.x + secondSampleOffset.x) * 0.0025f, (tempY + offset.y + secondSampleOffset.y) * 0.0025f)),
                                0.5f) / 4;

                            rollingHillsB = math.lerp(noise.snoise(new float2((tempX + offset.x) * 0.0125f, (tempY + offset.y) * 0.0125f)),
                                noise.snoise(new float2((tempX + offset.x + secondSampleOffset.x * 2) * 0.0025f, (tempY + offset.y + secondSampleOffset.y * 2) * 0.0025f)),
                                0.5f) / 4;

                            // Combine

                            rollingHills = math.lerp(rollingHillsA,
                                rollingHillsB,
                                0.5f);

                            // Assign

                            tempValue = rollingHills;
                        }

                        tempValue += 0.5f;

                        hashMap.Add(tempPosition, tempValue);
                    }
                    iterator++;
                }
            }
        }
    }

    [BurstCompile]
    public void GetHeightsFromHashMap(float x, float y)
    {
        Vector2 tempPosition = new Vector2(x, y);

        JobHandle jobHandle = default;

        getHeightsFromHashMapJob.Add(tempPosition, new GetPositionsFromHashMapJob
        {
            result = new NativeArray<Vector3>((int)(defaultMapSize * defaultMapSize), Allocator.Persistent),
            size = SUB_MAP_SIZE,
            lowMask = terrainEngine.terrainMesh.lowMask * 16,
            highMask = terrainEngine.terrainMesh.highMask * 16,
            startPositionX = x,
            startPositionY = y,
            hashMap = hashMap,
            step = 16
        });

        jobHandlesGetFromHashMap[tempPosition] = getHeightsFromHashMapJob[tempPosition].Schedule(jobHandle);

    }

    [BurstCompile]
    public void GenerateSections(int xSize, int zSize)
    {
        jobGridSize.Add(subMapPosition, new Vector2(xSize, zSize));

        JobHandle jobHandle = default;

        jobs.Add(subMapPosition, new NoiseForTerrainHeightMapJob
        {
            //heightBuffer = heightMaps[subMapPosition],
            xSize = xSize,
            zSize = zSize,
            startPositionX = (int)subMapPosition.x,
            startPositionY = (int)subMapPosition.y,
            hashMap = hashMap,
            step = 16,
            offset = offset,
            secondSampleOffset = secondSampleOffset,
            noiseFilter = noiseFilter
        });

        jobHandlesGenerateHeights[subMapPosition] = jobs[subMapPosition].Schedule(jobHandle);

        UpdateProgress();
    }

    // Change height format for the unity built in "Terrain"
    /*[BurstCompile]
    public float[,] ChangeFormat(List<float> values)
    {
        float[,] result = new float[(int)defaultMapSize, (int)defaultMapSize];
        int iterator = 0;

        for (int y = 0; y < defaultMapSize; y++)
        {
            for (int x = 0; x < defaultMapSize; x++)
            {
                result[x, y] = values[iterator];

                iterator++;
            }
        }

        return result;
    }*/

    /*[BurstCompile]
    public void ChangeFormatAddToHeights(float[] values, Vector2 startPosition, float xSize, float ySize)
    {
        int iterator = 0;

        for (float x = startPosition.x; x < startPosition.x + (xSize * 16); x += 16)
        {
            for (float y = startPosition.y; y < startPosition.y + (ySize * 16); y += 16)
            {
                heights.Add(new Vector2(x, y), values[iterator]);

                iterator++;
            }
        }
    }*/

    [BurstCompile]
    public void BeforeGenerating()
    {
        loadingProgressionData.savedSeconds = 9999999f;

        savedSubMapPosition = subMapPosition;

        CalculateTotal();
        CalculateSubMapsRequired();
    }

    [BurstCompile]
    public void AfterGenerating()
    {
        subMapPosition = savedSubMapPosition;
    }

    [BurstCompile]
    public void CalculateTotal()
    {
        loadingProgressionData.total = defaultMapSize * defaultMapSize;
    }

    [BurstCompile]
    public void CalculateSubMapsRequired()
    {
        loadingProgressionData.required = (float)loadingProgressionData.total / (SUB_MAP_SIZE * SUB_MAP_SIZE);
    }

    [BurstCompile]
    public void UpdateProgress()
    {
        //loadingProgressionData.count = subMaps.Count * (SUB_MAP_SIZE * SUB_MAP_SIZE);
        loadingProgressionData.progress = (float)loadingProgressionData.count / (float)loadingProgressionData.total;
        loadingProgressionData.secondsRemaining = (int)(((loadingProgressionData.total - loadingProgressionData.count) / (SUB_MAP_SIZE * SUB_MAP_SIZE)) / terrainEngine.main.framesPerSecond.fps);
        if (loadingProgressionData.savedSeconds > loadingProgressionData.secondsRemaining)
        {
            loadingProgressionData.savedSeconds = loadingProgressionData.secondsRemaining;
        }
        else
        {
            loadingProgressionData.secondsRemaining = loadingProgressionData.savedSeconds;
        }
    }
}

