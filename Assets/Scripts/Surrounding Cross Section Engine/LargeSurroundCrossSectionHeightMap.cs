using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

[BurstCompile]
public class LargeSurroundCrossSectionHeightMap : MonoBehaviour
{
    public Dictionary<Vector3, int> heights = new Dictionary<Vector3, int>();

    public int GetHeight(Vector3 chunkFixedPosition)
    {
        return 0;
    }
}
