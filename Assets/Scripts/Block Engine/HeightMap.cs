using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
public class HeightMap : MonoBehaviour
{
    private BlockEngine blockEngine = null;

    public float defaultMapSize = 544;                              // size of the height map (limit = 46,336 / √2,147,483,647 - 46,340.9500010519853390887900102 )
    public int SUB_MAP_SIZE = 16;                            // grid - size * size

    public float2 offset = new float2(0, 0);                        // The amount to offset the perlin noise sample
    public float2 secondSampleOffset = new float2(1000, 1000);      // Offset for the second sample. height is lerped between first sample and second sample at the mid point.
    public float noiseFilter = 0.005f;

    // key - position, Value - height map values 'Linearized' into an array.
    public Dictionary<Vector2, float[]> subMaps = new Dictionary<Vector2, float[]>();
    public Dictionary<Vector2, Vector2> differenceInHeightForSubMap = new Dictionary<Vector2, Vector2>();

    // ==========================================

    public LoadingProgressionData loadingProgressionData = new LoadingProgressionData(true);

    // ==========================================

    Dictionary<Vector2, NoiseForHeightMapJob> jobs = new Dictionary<Vector2, NoiseForHeightMapJob>();
    public Dictionary<Vector2, JobHandle> jobHandles = new Dictionary<Vector2, JobHandle>();

    Dictionary<Vector2, NativeArray<float>> nativeHeightMaps = new Dictionary<Vector2, NativeArray<float>>();
    Dictionary<Vector2, NativeArray<float>> nativeLowestBuffers = new Dictionary<Vector2, NativeArray<float>>();
    Dictionary<Vector2, NativeArray<float>> nativeHighestBuffers = new Dictionary<Vector2, NativeArray<float>>();

    private List<Vector2> positionsToRemove = new List<Vector2>();

    //============================================
    // Created to help optimize 'new' out of the code
    private float2 tempFloat2 = float2.zero;
    private Vector2 tempDifference = Vector2.zero;

    [BurstCompile]
    private void OnDestroy()
    {
        // Free memory

        blockEngine = null;

        subMaps.Clear();
        subMaps = null;
        differenceInHeightForSubMap.Clear();
        differenceInHeightForSubMap = null;

        jobs.Clear();
        jobs = null;
        jobHandles.Clear();
        jobHandles = null;

        nativeHeightMaps.Clear();
        nativeHeightMaps = null;
        nativeLowestBuffers.Clear();
        nativeLowestBuffers = null;
        nativeHighestBuffers.Clear();
        nativeHighestBuffers = null;

        positionsToRemove.Clear();
        positionsToRemove = null;
    }

    [BurstCompile]
    public void CleanUp() 
    {
        subMaps.Clear();
        differenceInHeightForSubMap.Clear();
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

        foreach (Vector2 key in jobHandles.Keys)
        {            
            jobHandles[key].Complete();

            nativeHeightMaps[key].Dispose();
            nativeLowestBuffers[key].Dispose();
            nativeHighestBuffers[key].Dispose();
        }

        jobHandles.Clear();
    }

    [BurstCompile]
    public void LateUpdate()
    {
        if (jobHandles.Count == 0) return;

        positionsToRemove.Clear();

        foreach (Vector2 key in jobHandles.Keys)
        {
            if (jobHandles[key].IsCompleted)
            {
                jobHandles[key].Complete();

                subMaps[key] = jobs[key].heightBuffer.ToArray();
                tempDifference.x = jobs[key].lowest[0];
                tempDifference.y = jobs[key].highest[0];
                differenceInHeightForSubMap[key] = tempDifference;

                nativeHeightMaps[key].Dispose();
                nativeLowestBuffers[key].Dispose();
                nativeHighestBuffers[key].Dispose();

                positionsToRemove.Add(key);
            }
        }

        for (int i = 0; i < positionsToRemove.Count; i++)
        {
            // Remove all field dictionary elements

            nativeHeightMaps.Remove(positionsToRemove[i]);
            nativeLowestBuffers.Remove(positionsToRemove[i]);
            nativeHighestBuffers.Remove(positionsToRemove[i]);

            jobs.Remove(positionsToRemove[i]);

            jobHandles.Remove(positionsToRemove[i]);
        }
    }

    // 2D noise generation for terrain heights
    [BurstCompile]
    struct NoiseForHeightMapJob: IJob
    {
        public float2 offset;
        public float2 secondSampleOffset;
        public int size;
        public float noiseFilter;
        public NativeArray<float> heightBuffer;
        private int iterator;
        //public float tempHeight;

        //public float mapModifier;

        public NativeArray<float> highest;
        public NativeArray<float> lowest;

        //-------------------------
        // used to optimze 'new' out of code

        private float2 tempFloat2;

        private float tempX;
        private float tempY;

        private float noiseShapeA;
        private float noiseShapeB;
        private float noiseShapeC;
        private float noiseShapeD;
        private float noiseShapeE;

        private float rollingHillsA;
        private float rollingHillsB;

        private float rollingHills;

        private float bigHills;

        [BurstCompile]
        public void Execute()
        {
            iterator = 0;
            highest[0] = -99999;
            lowest[0] = 99999;

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    tempX = x;
                    tempY = y;

                    // BIG HILLS ONTOP OF ROLLING HILLS - noise shape used - above 0

                    tempFloat2.x = (tempX + offset.x) * 0.0125f;
                    tempFloat2.y = (tempY + offset.y) * 0.0125f;

                    noiseShapeA = noise.snoise(tempFloat2);

                    if (noiseShapeA > 0.0f)
                    {
                        // BIG HILLS

                        tempFloat2.x = (tempX + offset.x + secondSampleOffset.x) * 0.05f;
                        tempFloat2.y = (tempY + offset.y + secondSampleOffset.y) * 0.05f;

                        noiseShapeB = noise.snoise(tempFloat2);

                        bigHills = math.lerp(noiseShapeA,
                                        noiseShapeB,
                                        0.5f) / 4;

                        // ROLLING HILLS

                        tempFloat2.x = (tempX + offset.x) * 0.00125f;
                        tempFloat2.y = (tempY + offset.y) * 0.00125f;

                        noiseShapeC = noise.snoise(tempFloat2);

                        tempFloat2.x = (tempX + offset.x + secondSampleOffset.x) * 0.0025f;
                        tempFloat2.y = (tempY + offset.y + secondSampleOffset.y) * 0.0025f;

                        noiseShapeD = noise.snoise(tempFloat2);

                        rollingHillsA = math.lerp(noiseShapeC,
                            noiseShapeD,
                            0.5f) / 4;

                        tempFloat2.x = (tempX + offset.x + secondSampleOffset.x * 2) * 0.0025f;
                        tempFloat2.y = (tempY + offset.y + secondSampleOffset.y * 2) * 0.0025f;

                        noiseShapeE = noise.snoise(tempFloat2);

                        rollingHillsB = math.lerp(noiseShapeA,
                            noiseShapeE,
                            0.5f) / 4;

                        rollingHills = math.lerp(rollingHillsA,
                            rollingHillsB,
                            0.5f);

                        // Combine

                        heightBuffer[iterator] = math.lerp(rollingHills,
                            rollingHills + bigHills,
                            0.75f);

                        // Merge mountain towards rolling hills original position.

                        if (noiseShapeA > 0.5f
                            && noiseShapeA < 0.75f)
                        {
                            heightBuffer[iterator] = math.lerp(heightBuffer[iterator],
                                rollingHills,
                                0.7f);
                        }
                        else
                        {
                            if (noiseShapeA > 0.25f
                            && noiseShapeA < 0.5f)
                            {
                                heightBuffer[iterator] = math.lerp(heightBuffer[iterator],
                                rollingHills,
                                0.8f);
                            }
                            else
                            {
                                if (noiseShapeA < 0.25f)
                                {
                                    heightBuffer[iterator] = math.lerp(heightBuffer[iterator],
                                    rollingHills,
                                    0.9f);
                                }
                                else
                                {
                                    heightBuffer[iterator] = math.lerp(heightBuffer[iterator],
                                    rollingHills,
                                    0.75f);
                                }
                            }
                        }

                        heightBuffer[iterator] = heightBuffer[iterator] * 256 + 256 + 1;
                    }
                    else
                    {
                        // ROLLING HILLS

                        tempFloat2.x = (tempX + offset.x) * 0.00125f;
                        tempFloat2.y = (tempY + offset.y) * 0.00125f;

                        noiseShapeB = noise.snoise(tempFloat2);

                        tempFloat2.x = (tempX + offset.x + secondSampleOffset.x) * 0.0025f;
                        tempFloat2.y = (tempY + offset.y + secondSampleOffset.y) * 0.0025f;

                        noiseShapeC = noise.snoise(tempFloat2);

                        rollingHillsA = math.lerp(noiseShapeB,
                            noiseShapeC,
                            0.5f) / 4;

                        
                        tempFloat2.x = (tempX + offset.x + secondSampleOffset.x * 2) * 0.0025f;
                        tempFloat2.y = (tempY + offset.y + secondSampleOffset.y * 2) * 0.0025f;

                        noiseShapeD = noise.snoise(tempFloat2);

                        rollingHillsB = math.lerp(noiseShapeA,
                            noiseShapeD,
                            0.5f) / 4;

                        // Combine

                        rollingHills = math.lerp(rollingHillsA,
                            rollingHillsB,
                            0.5f);

                        // Assign

                        heightBuffer[iterator] = rollingHills;

                        heightBuffer[iterator] = heightBuffer[iterator] * 256 + 256;
                    }

                    // Track heights so can more efficently find edges. And edge could expose a face on this height map

                    if (heightBuffer[iterator] > highest[0]) highest[0] = heightBuffer[iterator];
                    if (heightBuffer[iterator] < lowest[0]) lowest[0] = heightBuffer[iterator];

                    iterator++;
                }
            }
        }
    }

    [BurstCompile]
    public void GenerateSections(Vector3 position)
    {
        subMaps.Add(position, null);                                // New Block Queue.cs checks for null value before processing further.
        differenceInHeightForSubMap.Add(position, Vector2.zero);    // was new Vector2

        // Native arrays/containers should generate no garbage. ('new' - remains)
        nativeHeightMaps.Add(position, new NativeArray<float>(SUB_MAP_SIZE * SUB_MAP_SIZE, Allocator.Persistent));
        nativeHighestBuffers.Add(position, new NativeArray<float>(1, Allocator.Persistent));
        nativeLowestBuffers.Add(position, new NativeArray<float>(1, Allocator.Persistent));

        // used to avoid creating new float 2 each time
        tempFloat2.x = position.x + offset.x;
        tempFloat2.y = position.y + offset.y;

        // Create Job
        jobs.Add(position, new NoiseForHeightMapJob
        {
            heightBuffer = nativeHeightMaps[position],
            size = SUB_MAP_SIZE,
            offset = tempFloat2,
            secondSampleOffset = secondSampleOffset,
            noiseFilter = noiseFilter,
            lowest = nativeLowestBuffers[position],
            highest = nativeHighestBuffers[position]
        });

        // Create job handle
        jobHandles.Add(position, jobs[position].Schedule(default));
    }
}
