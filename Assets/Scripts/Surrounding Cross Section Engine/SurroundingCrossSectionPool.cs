using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

[BurstCompile]
public class SurroundingCrossSectionPool : MonoBehaviour
{
    // This is filled inside BlockPool.CreateChunk at the point the chunk is created - it is added.
    public Dictionary<string, Transform[]> chunkCrossSections = new Dictionary<string, Transform[]>();

    [BurstCompile]
    private void OnDestroy()
    {
        chunkCrossSections.Clear();
        chunkCrossSections = null;
    }

    [BurstCompile]
    void Add(string chunkName, Transform[] chunkCrossSections)
    {
        this.chunkCrossSections.Add(chunkName, chunkCrossSections);
    }
}
