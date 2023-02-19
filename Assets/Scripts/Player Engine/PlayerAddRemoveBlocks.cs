using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

[BurstCompile]
public class PlayerAddRemoveBlocks : MonoBehaviour
{
    public ModifiedBlockQueue modifiedBlockQueue = null;
    public bool hasBlock = false;

    // =============

    // FOR DEBUGGING BLOCK INDEX ONLY
    public bool addBlock = false;
    public bool removeBlock = false;
    public Vector3 digPlaceDirection = Vector3.zero;
    public Vector3 actorPositionDebugging = Vector3.zero;
    public Vector3 subChunkPositionDebugging = Vector3.zero;
    public Transform selectedChunkForTesting = null;

    [BurstCompile]
    private void OnDestroy()
    {
        // Free memory

        modifiedBlockQueue = null;
        selectedChunkForTesting = null;
    }

    /*[BurstCompile]
    public void Update()
    {
        if (addBlock)
        {
            addBlock = false;
            int index = CalculateIndexForBlockInSubChunk(actorPositionDebugging, digPlaceDirection);
            AddBlock(selectedChunkForTesting, subChunkPositionDebugging, index, RawSubChunkData.BlockType.Stone, false);
        }

        if (removeBlock)
        {
            removeBlock = false;
            int index = CalculateIndexForBlockInSubChunk(actorPositionDebugging, digPlaceDirection);
            RemoveBlock(selectedChunkForTesting, subChunkPositionDebugging, index, false);
        }
    }*/

    // Start is called before the first frame update
    [BurstCompile]
    void Awake()
    {
        modifiedBlockQueue = GameObject.Find("Block Engine").GetComponent<ModifiedBlockQueue>();
    }

    [BurstCompile]
    public void AddBlock()
    {
        //modifiedBlockQueue.AddBlock();
    }

    [BurstCompile]
    public void RemoveBlock()
    {
        //modifiedBlockQueue.RemoveBlock();
    }
}
