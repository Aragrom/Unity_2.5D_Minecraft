using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

[BurstCompile]
public class LargeSurroundingCrossSectionPool : MonoBehaviour
{
    public Dictionary<string, Transform> largeBlackCrossSections = new Dictionary<string, Transform>();

    [BurstCompile]
    public void OnDestroy()
    {
        largeBlackCrossSections.Clear();
        largeBlackCrossSections = null;
    }

    [BurstCompile]
    public void Add(string chunkName, Transform t)
    {
        largeBlackCrossSections.Add(chunkName, t);
    }
}
