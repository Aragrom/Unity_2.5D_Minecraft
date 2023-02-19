using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

// BlockEngine.cs calls alot of functions and store the data here for the queue of sub chunks to made

[BurstCompile]
public class NewBlockQueue : MonoBehaviour
{
    private BlockEngine blockEngine = null;

    public bool skipShapeMap = false;

    public enum QueueState
    {
        needsHeightMaps,
        generatingHeightMaps,
        needsSubChunkPositions,
        needsShapeMaps,
        generatingShapeMaps,
        needsBlockMap,
        generatingBlockMap,
        needsSetMesh,
        settingMesh,
        complete,
    }

    // Could combine into a structure =============

    public Dictionary<string, QueueState> states = new Dictionary<string, QueueState>();
    public Dictionary<string, Transform> chunks = new Dictionary<string, Transform>();
    
    public Dictionary<string, Vector3> positions = new Dictionary<string, Vector3>();
    public Dictionary<string, List<Vector2>> subMapPosition = new Dictionary<string, List<Vector2>>();
    public LinkedList<string> needsSetMesh = new LinkedList<string>();

    public Dictionary<string, List<Vector3>> subChunkPositions = new Dictionary<string, List<Vector3>>();
    public Dictionary<string, List<bool[]>> subChunkNeighbours = new Dictionary<string, List<bool[]>>();

    public string isSettingMesh = "";

    // ============================================

    /*public bool heightMapExists = false;
    public bool northHeightMapExists = false;
    public bool southHeightMapExists = false;
    public bool eastHeightMapExists = false;
    public bool westHeightMapExists = false;
    public bool northEastHeightMapExists = false;
    public bool southEastHeightMapExists = false;
    public bool southWestHeightMapExists = false;
    public bool northWestHeightMapExists = false;*/

    public Vector3 tempPosition3 = Vector3.zero;            // Vector3
    public Vector3 tempNorthPosition3 = Vector3.zero;
    public Vector3 tempSouthPosition3 = Vector3.zero;
    public Vector3 tempEastPosition3 = Vector3.zero;
    public Vector3 tempWestPosition3 = Vector3.zero;
    public Vector3 tempAbovePosition3 = Vector3.zero;
    public Vector3 tempBelowPosition3 = Vector3.zero;

    public Vector2 tempPosition2 = Vector2.zero;            // Vector2
    public Vector2 tempNorthPosition = Vector2.zero;
    public Vector2 tempSouthPosition = Vector2.zero;
    public Vector2 tempEastPosition = Vector2.zero;
    public Vector2 tempWestPosition = Vector2.zero;
    public Vector2 tempNorthEastPosition = Vector2.zero;
    public Vector2 tempSouthEastPosition = Vector2.zero;
    public Vector2 tempSouthWestPosition = Vector2.zero;
    public Vector2 tempNorthWestPosition = Vector2.zero;

    //---------------------------------------------

    private List<Vector3> subChunksPosition;

    private int highest = -10000;
    private int lowest = 10000;
    private int subChunksRequired;
    private int depth;

    private Transform subChunk;
    private Mesh mesh;
    private int index;

    private int SUB_CHUNK_HEIGHT = 8;
    private int SUB_CHUNK_SIZE = 16;

    [BurstCompile]
    private void OnDestroy()
    {
        // Free memory

        blockEngine = null;

        states.Clear();
        states = null;
        chunks.Clear();
        chunks = null;
        positions.Clear();
        positions = null;
        subMapPosition.Clear();
        subMapPosition = null;
        needsSetMesh.Clear();
        needsSetMesh = null;

        subChunkPositions.Clear();
        subChunkPositions = null;
        subChunkNeighbours.Clear();
        subChunkNeighbours = null;
    }

    [BurstCompile]
    // Start is called before the first frame update
    void Awake()
    {
        blockEngine = GetComponent<BlockEngine>();
    }

    [BurstCompile]
    public bool CheckIsAlreadyInQueue(string chunkName)
    {
        return chunks.ContainsKey(chunkName);
    }

    [BurstCompile]
    public void AddToQueue(Transform chunk, Vector3 position)
    {
        // store for reference and tracking.
        // The couroutine in the Block Engine will begin the update/processing/generating.
        states.Add(chunk.name, QueueState.needsHeightMaps);
        chunks.Add(chunk.name, chunk);
        positions.Add(chunk.name, position);
        subMapPosition.Add(chunk.name, new List<Vector2>());

        subChunkPositions.Add(chunk.name, new List<Vector3>());
        subChunkNeighbours.Add(chunk.name, new List<bool[]>());
    }

    [BurstCompile]
    public void ChangeQueue(Transform chunk, Vector3 position)
    {
        // THIS WILL NOT CORRECTLY GO IN AND APPLY WHILE IN COROUTINE. SO COUROUTINE COULD CHANGE

        // Change state so the current updates are stored but not further developed
        // This will become used when the system cant keep up with the Generation for the viewer/player.
        states[chunk.name] = QueueState.needsHeightMaps;
        positions[chunk.name] = position;
        subMapPosition[chunk.name] = new List<Vector2>();
        subChunkPositions[chunk.name] = new List<Vector3>();
        subChunkNeighbours[chunk.name] = new List<bool[]>();

        // 
        needsSetMesh.Remove(chunk.name);
    }

    [BurstCompile]
    public void RemoveFromQueue(string chunkName)
    {
        tempPosition3 = positions[chunkName];

        subChunkPositions[chunkName] = null;
        subChunkPositions.Remove(chunkName);
        //subChunkDepthLowest.Remove(tempPosition3);
        //subChunksRequired.Remove(tempPosition3);

        states.Remove(chunkName);
        chunks.Remove(chunkName);
        positions.Remove(chunkName);
        subMapPosition.Remove(chunkName);
        subChunkNeighbours.Remove(chunkName);
        //needsSetMesh.Remove(chunkName);
        needsSetMesh.RemoveFirst();
    }

    // ==============

    [BurstCompile]
    public void NeedsHeigtMaps(string chunkName)
    {
        // Begin jobs for all height maps needed.
        // Will need surrounding height maps for complete chunk.
        // Check if the/one or all height maps already exists before generating
        tempPosition3 = positions[chunkName];

        tempPosition2.x = tempPosition3.x;
        tempPosition2.y = tempPosition3.z;

        tempNorthPosition = tempPosition2 + new Vector2(0, 16);
        tempSouthPosition = tempPosition2 + new Vector2(0, -16);
        tempEastPosition = tempPosition2 + new Vector2(16, 0);
        tempWestPosition = tempPosition2 + new Vector2(-16, 0);
        tempNorthEastPosition = tempPosition2 + new Vector2(16, 16);
        tempSouthEastPosition = tempPosition2 + new Vector2(16, -16);
        tempSouthWestPosition = tempPosition2 + new Vector2(-16, -16);
        tempNorthWestPosition = tempPosition2 + new Vector2(-16, 16);

        if (blockEngine.heightMap.subMaps.ContainsKey(tempPosition2) == false)
        {
            blockEngine.heightMap.GenerateSections(tempPosition2);

            subMapPosition[chunkName].Add(tempPosition2);
        }

        // ALL OF THIS IS USELESS!!!!! GENERATING SO MUCH THAT DOESN't GET USED UNLESS THE CHUNK FOR THIS HEIGHT MAP IS BEING GENERATED. REGARDLESS EXTRA HEIGHT MAPS WILL BE THROWN AWAY.
        // NEEDED FOR EDGES ONLY - could develop edge stuff

        // NEED TO GET RID OF THIS MESS <<<<< NEED TO GET RID OF THIS MESS <<<<< NEED TO GET RID OF THIS MESS <<<<< NEED TO GET RID OF THIS MESS <<<<< NEED TO GET RID OF THIS MESS <<<<< NEED TO GET RID OF THIS MESS <<<<< 
        // NEED TO GET RID OF THIS MESS <<<<< NEED TO GET RID OF THIS MESS <<<<< NEED TO GET RID OF THIS MESS <<<<< NEED TO GET RID OF THIS MESS <<<<< NEED TO GET RID OF THIS MESS <<<<< NEED TO GET RID OF THIS MESS <<<<< 
        // DO AN AVERAGE OF CORNER SELECTIONS SHOULD NOT REQUIRE HEIGHT MAPS <<<<<<< DO AN AVERAGE OF CORNER SELECTIONS SHOULD NOT REQUIRE HEIGHT MAPS <<<<<<< DO AN AVERAGE OF CORNER SELECTIONS SHOULD NOT REQUIRE HEIGHT MAPS <<<<<<< 

        if (blockEngine.heightMap.subMaps.ContainsKey(tempNorthPosition) == false)
        {
            blockEngine.heightMap.GenerateSections(tempNorthPosition);

            subMapPosition[chunkName].Add(tempNorthPosition);
        }

        if (blockEngine.heightMap.subMaps.ContainsKey(tempSouthPosition) == false)
        {
            blockEngine.heightMap.GenerateSections(tempSouthPosition);

            subMapPosition[chunkName].Add(tempSouthPosition);
        }

        if (blockEngine.heightMap.subMaps.ContainsKey(tempEastPosition) == false)
        {
            blockEngine.heightMap.GenerateSections(tempEastPosition);

            subMapPosition[chunkName].Add(tempEastPosition);
        }

        if (blockEngine.heightMap.subMaps.ContainsKey(tempWestPosition) == false)
        {
            blockEngine.heightMap.GenerateSections(tempWestPosition);

            subMapPosition[chunkName].Add(tempWestPosition);
        }

        // EXTRA STUPID GENERATION OF HEIGHT MAPS ONLY SO THE EDGES ARE GENERATED CORRECTLY.
        // NEED TO KNOW IF THE NEIGHBOURS OF NEIGHBOURS EXIST. need to develop neighbour edge stuff

        // NEED TO GET RID OF THIS MESS <<<<< NEED TO GET RID OF THIS MESS <<<<< NEED TO GET RID OF THIS MESS <<<<< NEED TO GET RID OF THIS MESS <<<<< NEED TO GET RID OF THIS MESS <<<<< NEED TO GET RID OF THIS MESS <<<<< 
        // NEED TO GET RID OF THIS MESS <<<<< NEED TO GET RID OF THIS MESS <<<<< NEED TO GET RID OF THIS MESS <<<<< NEED TO GET RID OF THIS MESS <<<<< NEED TO GET RID OF THIS MESS <<<<< NEED TO GET RID OF THIS MESS <<<<< 
        // DO AN AVERAGE OF CORNER SELECTIONS SHOULD NOT REQUIRE HEIGHT MAPS <<<<<<< DO AN AVERAGE OF CORNER SELECTIONS SHOULD NOT REQUIRE HEIGHT MAPS <<<<<<< DO AN AVERAGE OF CORNER SELECTIONS SHOULD NOT REQUIRE HEIGHT MAPS <<<<<<< 

        if (blockEngine.heightMap.subMaps.ContainsKey(tempNorthEastPosition) == false)
        {
            blockEngine.heightMap.GenerateSections(tempNorthEastPosition);

            subMapPosition[chunkName].Add(tempNorthEastPosition);
        }

        if (blockEngine.heightMap.subMaps.ContainsKey(tempSouthEastPosition) == false)
        {
            blockEngine.heightMap.GenerateSections(tempSouthEastPosition);

            subMapPosition[chunkName].Add(tempSouthEastPosition);
        }
        
        if (blockEngine.heightMap.subMaps.ContainsKey(tempSouthWestPosition) == false)
        {
            blockEngine.heightMap.GenerateSections(tempSouthWestPosition);

            subMapPosition[chunkName].Add(tempSouthWestPosition);
        }
        
        if (blockEngine.heightMap.subMaps.ContainsKey(tempNorthWestPosition) == false)
        {
            blockEngine.heightMap.GenerateSections(tempNorthWestPosition);

            subMapPosition[chunkName].Add(tempNorthWestPosition);
        }

        states[chunkName] = QueueState.generatingHeightMaps;
    }

    [BurstCompile]
    public void GeneratingHeightMaps(string chunkName)
    {
        // Check jobs is complete - cant.
        // Instead check if there is a range/count of each height map.
        // .Count > 0
        for (int i = 0; i < subMapPosition[chunkName].Count; i++)
        {
            if (blockEngine.heightMap.subMaps[subMapPosition[chunkName][i]] != null)
            {
                // if this position is complete. Remove it from the list to be searched.
                subMapPosition[chunkName].Remove(subMapPosition[chunkName][i]);
            }
        }

        // is the list empty?
        // Have all height maps been generated?

        if (subMapPosition[chunkName].Count == 0)
        {
            // ALL are complete
            //states[chunkName] = QueueState.needsBlockMap;
            states[chunkName] = QueueState.needsSubChunkPositions;
        }
    }

    // ==============

    [BurstCompile]
    public void NeedsSubChunkPositions(string chunkName)
    {
        tempPosition3 = positions[chunkName];

        // NEED TO GET RID OF THIS MESS <<<<< NEED TO GET RID OF THIS MESS <<<<< NEED TO GET RID OF THIS MESS <<<<< NEED TO GET RID OF THIS MESS <<<<< NEED TO GET RID OF THIS MESS <<<<< NEED TO GET RID OF THIS MESS <<<<< 
        // NEED TO GET RID OF THIS MESS <<<<< NEED TO GET RID OF THIS MESS <<<<< NEED TO GET RID OF THIS MESS <<<<< NEED TO GET RID OF THIS MESS <<<<< NEED TO GET RID OF THIS MESS <<<<< NEED TO GET RID OF THIS MESS <<<<< 
        // DO AN AVERAGE OF CORNER SELECTIONS OR CENTER SHOULD NOT REQUIRE HEIGHT MAPS <<<<<<< DO AN AVERAGE OF CORNER SELECTIONS SHOULD NOT REQUIRE HEIGHT MAPS <<<<<<< DO AN AVERAGE OF CORNER SELECTIONS SHOULD NOT REQUIRE HEIGHT MAPS <<<<<<< 

        //------------------------------------------------------------------------
        // Main chunk being processed

        tempPosition2 = new Vector2(tempPosition3.x, tempPosition3.z);

        // Check HeightMaps lowest and highest values. Generate only what is necessary. Copy the subchunk where possible.

        // x = low, y = high
        Vector2 difference = blockEngine.heightMap.differenceInHeightForSubMap[tempPosition2];
        Vector2 differenceNorth = blockEngine.heightMap.differenceInHeightForSubMap[tempPosition2 + new Vector2(0, SUB_CHUNK_SIZE)];
        Vector2 differenceSouth = blockEngine.heightMap.differenceInHeightForSubMap[tempPosition2 + new Vector2(0, -SUB_CHUNK_SIZE)];
        Vector2 differenceEast = blockEngine.heightMap.differenceInHeightForSubMap[tempPosition2 + new Vector2(SUB_CHUNK_SIZE, 0)];
        Vector2 differenceWest = blockEngine.heightMap.differenceInHeightForSubMap[tempPosition2 + new Vector2(-SUB_CHUNK_SIZE, 0)];

        Vector2 differenceNorthEast = blockEngine.heightMap.differenceInHeightForSubMap[tempPosition2 + new Vector2(SUB_CHUNK_SIZE, SUB_CHUNK_SIZE)];
        Vector2 differenceSouthEast = blockEngine.heightMap.differenceInHeightForSubMap[tempPosition2 + new Vector2(SUB_CHUNK_SIZE, -SUB_CHUNK_SIZE)];
        Vector2 differenceSouhtWest = blockEngine.heightMap.differenceInHeightForSubMap[tempPosition2 + new Vector2(-SUB_CHUNK_SIZE, -SUB_CHUNK_SIZE)];
        Vector2 differenceNorthWest = blockEngine.heightMap.differenceInHeightForSubMap[tempPosition2 + new Vector2(-SUB_CHUNK_SIZE, SUB_CHUNK_SIZE)];

        int lowestNorth = (int)(differenceNorth.x / (float)SUB_CHUNK_HEIGHT) - 2;
        int lowestSouth = (int)(differenceSouth.x / (float)SUB_CHUNK_HEIGHT) - 2;
        int lowestEast = (int)(differenceEast.x / (float)SUB_CHUNK_HEIGHT) - 2;
        int lowestWest = (int)(differenceWest.x / (float)SUB_CHUNK_HEIGHT) - 2;

        int lowestNorthEast = (int)(differenceNorthEast.x / (float)SUB_CHUNK_HEIGHT) - 2;
        int lowestSouthEast = (int)(differenceSouthEast.x / (float)SUB_CHUNK_HEIGHT) - 2;
        int lowestSouthWest = (int)(differenceSouhtWest.x / (float)SUB_CHUNK_HEIGHT) - 2;
        int lowestNorthWest = (int)(differenceNorthWest.x / (float)SUB_CHUNK_HEIGHT) - 2;

        highest = (int)(difference.y);
        lowest = (int)(difference.x);

        highest = (int)(highest / (float)SUB_CHUNK_HEIGHT) + 2;
        lowest = (int)(lowest / (float)SUB_CHUNK_HEIGHT) - 2;

        subChunksRequired = highest - lowest;

        // SET STARTING DEPTH

        depth = lowest * SUB_CHUNK_HEIGHT;

        //---------------------------------------------------------------------

        // For the main chunk being processed
        for (int i = 0; i < subChunksRequired; i++)     // can be made more effecient take out 
        {
            bool[] neighbours = new bool[9];

            // is bottom chunk - no bottom neighbour
            if (i != 0) { neighbours[4] = true; }
            if (lowestNorth <= lowest + i) { neighbours[0] = true; }
            if (lowestSouth <= lowest + i) { neighbours[1] = true; }
            if (lowestEast <= lowest + i) { neighbours[2] = true; }
            if (lowestWest <= lowest + i) { neighbours[3] = true; }

            if (lowestNorthEast <= lowest + i) { neighbours[5] = true; }
            if (lowestSouthEast <= lowest + i) { neighbours[6] = true; }
            if (lowestSouthWest <= lowest + i) { neighbours[7] = true; }
            if (lowestNorthWest <= lowest + i) { neighbours[8] = true; }

            subChunkPositions[chunkName].Add(new Vector3(tempPosition3.x, depth, tempPosition3.z));

            subChunkNeighbours[chunkName].Add(neighbours);

            depth += SUB_CHUNK_HEIGHT;
        }

        //---------------------------------------------------------
        // Finished

        states[chunkName] = QueueState.needsShapeMaps;
    }

    [BurstCompile]
    public void NeedsShapeMap(string chunkName)
    {
        for (int i = 0; i < subChunkPositions[chunkName].Count; i++)
        {
            tempPosition3 = subChunkPositions[chunkName][i];

            if (blockEngine.shapeMap.subMaps.ContainsKey(tempPosition3) == false)
            {
                blockEngine.shapeMap.Generate(tempPosition3, subChunkNeighbours[chunkName][i][0], subChunkNeighbours[chunkName][i][1], subChunkNeighbours[chunkName][i][2], subChunkNeighbours[chunkName][i][3], subChunkNeighbours[chunkName][i][4]);
                subChunkPositions[chunkName].Add(tempPosition3);
            }
        }
        
        states[chunkName] = QueueState.generatingShapeMaps;
    }

    [BurstCompile]
    public void GeneratingShapeMap(string chunkName)
    {
        // Check jobs is complete - cant?
        // Instead check if there is a range/count of each height map.
        // .Count > 0
        for (int i = 0; i < subChunkPositions[chunkName].Count; i++)
        {
            if (blockEngine.shapeMap.subMaps[subChunkPositions[chunkName][i]] != null)
            {
                // if this position is complete. Remove it from the list to be searched.
                subChunkPositions[chunkName].Remove(subChunkPositions[chunkName][i]);
            }
        }

        // is the list empty?
        // Have all shape maps been generated?
        if (subChunkPositions[chunkName].Count == 0)
        {
            // ALL are complete
            states[chunkName] = QueueState.needsBlockMap;
        }
    }

    // ==============

    [BurstCompile]
    public void NeedsBlockMap(string chunkName)
    {
        tempPosition3 = positions[chunkName];

        tempPosition2 = new Vector2(tempPosition3.x, tempPosition3.z);

        // Check if block map already exists

        if (blockEngine.blockMap.chunkData.ContainsKey(tempPosition2) == false)
        {
            blockEngine.blockMap.subMapPosition = tempPosition3;

            blockEngine.blockMap.GenerateChunkData(chunkName);

            states[chunkName] = QueueState.generatingBlockMap;
        }
        else
        {
            // Block map already exists

            states[chunkName] = QueueState.generatingBlockMap;
        }
    }

    [BurstCompile]
    public void GeneratingBlockMap(string chunkName)
    {
        // check if job exists for this position. if it doesn't then it will be complete.
        // OR check if subChunkData exists <<<<

        tempPosition3 = positions[chunkName];

        tempPosition2 = new Vector2(tempPosition3.x, tempPosition3.z);

        if (blockEngine.blockMap.chunkData[tempPosition2] == true)
        {
            states[chunkName] = QueueState.needsSetMesh;
        }
    }

    // ==============

    [BurstCompile]
    public void NeedsSetMesh(string chunkName)
    {
        tempPosition3 = positions[chunkName];

        tempPosition2 = new Vector2(tempPosition3.x, tempPosition3.z);

        subChunksPosition = blockEngine.blockMap.subChunkPositionsByChunk[tempPosition2];

        for (int i = 0; i < subChunksPosition.Count; i++)
        {
            index = ((int)subChunksPosition[i].y) / blockEngine.blockMap.SUB_CHUNK_HEIGHT;

            subChunk = blockEngine.blockPool.subChunksPool[chunkName][index];

            blockEngine.blockPool.chunkComponents[chunkName].subChunks[index].rigidbody.detectCollisions = true; // <<==========

            blockEngine.blockPool.chunkComponents[chunkName].subChunks[index].meshFilter.sharedMesh.vertices = blockEngine.rawSubChunkData.vertices;
            blockEngine.blockPool.chunkComponents[chunkName].subChunks[index].meshFilter.sharedMesh.triangles = blockEngine.blockMap.subChunkData[subChunksPosition[i]].triangles;
            blockEngine.blockPool.chunkComponents[chunkName].subChunks[index].meshFilter.sharedMesh.uv = blockEngine.blockMap.subChunkData[subChunksPosition[i]].uvs;
            blockEngine.blockPool.chunkComponents[chunkName].subChunks[index].meshFilter.sharedMesh.uv2 = blockEngine.rawSubChunkData.uvs2;

            //blockEngine.blockPool.chunkComponents[chunkName].subChunks[index].meshFilter.mesh.RecalculateBounds();
            blockEngine.blockPool.chunkComponents[chunkName].subChunks[index].meshFilter.sharedMesh.RecalculateNormals();
            blockEngine.blockPool.chunkComponents[chunkName].subChunks[index].meshFilter.sharedMesh.Optimize();

            //blockEngine.blockPool.chunkComponents[chunkName].subChunks[index].meshFilter.sharedMesh = blockEngine.blockPool.chunkComponents[chunkName].subChunks[index].meshFilter.mesh;
            blockEngine.blockPool.chunkComponents[chunkName].subChunks[index].meshCollider.sharedMesh = blockEngine.blockPool.chunkComponents[chunkName].subChunks[index].meshFilter.sharedMesh;

            subChunk.gameObject.SetActive(true);

            // Take note of the "Sub Chunk" set active
            blockEngine.blockPool.activeSubChunks[chunkName].Add(index);
            blockEngine.blockPool.activeTruePosition[chunkName].Add(subChunksPosition[i]);

            // Remove sub chunk data
            blockEngine.blockMap.subChunkData.Remove(subChunksPosition[i]);
        }
    }
}
