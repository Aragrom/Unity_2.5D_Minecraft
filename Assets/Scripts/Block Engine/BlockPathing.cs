using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Jobs;
using UnityEngine;

[BurstCompile]
public class BlockPathing : MonoBehaviour
{
    // Start is called before the first frame update
    [BurstCompile]
    void Start()
    {
        
    }

    // Update is called once per frame
    [BurstCompile]
    void Update()
    {
        
    }

    [BurstCompile]
    private void LateUpdate()
    {
        
    }

    [BurstCompile]
    public void OnApplicationQuit()
    {

    }

    [BurstCompile]
    public void CleanUp()
    {

    }

    struct BlockDataWithShapeMapForSubChunkJob : IJob
    {
        public void Execute()
        {

        }
    }
}
