using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;

/*
 * REQUIRES TREADMILL ENGINE TO POWER IT! (or another system implemented)
 * without the treadmill engine - only generate the data
 * treadmill engine moves the "gameobjects" around (Block Pool - Chunks) and adds to the queues
 */

// if ever adapted to use entities would have to have the containers for terrain data all the same size.
// So instead of having different triangle array lengths would need to populate it with a bunch of zeros?
// in theory all data sizes would be the same and could take advantage of the entity system? <<<<<<<<<<<<<<<<<<<<
// Colliders would also need to be the same size extra data put in place.

[BurstCompile]
public class BlockEngine : MonoBehaviour
{
    public Main main = null;

    // ============================

    public BlockEngineOptions blockEngineOptions = null;

    // ============================

    public bool on = false;
    public bool setUp = false;
    public bool checkingForNewChunks = false;
    public bool assignMeshCoroutineRunning = false;

    // ============================

    public BlockPool blockPool = null;
    public NewBlockQueue newBlockQueue = null;
    public ModifiedBlockQueue modifiedBlockQueue = null;

    // ============================

    public RawSubChunkData rawSubChunkData = null;
    public HeightMap heightMap = null;
    public ShapeMap shapeMap = null;
    public WormMap wormMap = null;
    public LightMap lightMap = null;

    public BlockMap blockMap = null;

    // ============================

    public BlockPathing blockPathing = null;

    public Material subChunkMaterial = null;

    // same object as used in Terrain Engine just cached for reference.
    // Moved using the Treadmill system
    public Transform terrain = null;

    //-------------------------------

    private LinkedListNode<string> node = null;

    [BurstCompile]
    private void OnDestroy()
    {
        // Free memory

        main = null;
        blockPool = null;
        newBlockQueue = null;
        modifiedBlockQueue = null;

        rawSubChunkData = null;
        heightMap = null;
        shapeMap = null;
        wormMap = null;
        lightMap = null;
        blockMap = null;

        blockPathing = null;

        subChunkMaterial = null;
        terrain = null;
    }

    [BurstCompile]
    public void OnApplicationQuit()
    {
        StopAllCoroutines();
    }

    [BurstCompile]
    public void Awake()
    {
        main = GameObject.Find("Main").GetComponent<Main>();

        blockEngineOptions = GameObject.Find("Option Engine").GetComponent<BlockEngineOptions>();

        blockPool = GetComponent<BlockPool>();
        newBlockQueue = GetComponent<NewBlockQueue>();
        modifiedBlockQueue = GetComponent<ModifiedBlockQueue>();

        rawSubChunkData = GetComponent<RawSubChunkData>();
        heightMap = GetComponent<HeightMap>();
        shapeMap = GetComponent<ShapeMap>();
        wormMap = GetComponent<WormMap>();
        lightMap = GetComponent<LightMap>();
        blockMap = GetComponent<BlockMap>();

        blockPathing = GetComponent<BlockPathing>();

        terrain = GameObject.Find("Terrain").transform;
    }

    public void Update()
    {
        if (on && setUp)
        {
            // Perform clean up when the sub chunk data is no longer needed (newBlockQueue + modifiedBlockQueue)
            if (newBlockQueue.needsSetMesh.Count == 0
                && newBlockQueue.states.Count == 0
                && blockPool.isAddingNewChunks == false)
                //&& modifiedBlockQueue.subChunksThatNeedsMeshDataAssigned.Keys.Count == 0
                //&& modifiedBlockQueue.subChunksThatNeedsNewMeshDataGenerated.Keys.Count == 0)
            {
                heightMap.CleanUp();  // Removed as using height map to navigate troops
                shapeMap.CleanUp();
                blockMap.CleanUp();

                main.loadingBar.loadingVisualizer.SetActive(false);
            }

            // Bad to start a coroutine inside another? - Seperated then.
            if (!assignMeshCoroutineRunning)
            {
                assignMeshCoroutineRunning = true;
                StartCoroutine(AssignMeshData());
            }

            // Check NewBlockQueue for new chunks to be generated.
            // Generating new sub chunks at edges (new terrain)

            if (newBlockQueue.chunks.Keys.Count > 0)
            {
                NewBlockQueue.QueueState currentStateBeingChecked;

                foreach (string key in newBlockQueue.chunks.Keys)
                {
                    currentStateBeingChecked = newBlockQueue.states[key];

                    switch (currentStateBeingChecked)
                    {
                        case NewBlockQueue.QueueState.needsHeightMaps:

                            newBlockQueue.NeedsHeigtMaps(key);
                            break;

                        case NewBlockQueue.QueueState.generatingHeightMaps:

                            newBlockQueue.GeneratingHeightMaps(key);
                            break;

                        case NewBlockQueue.QueueState.needsSubChunkPositions:

                            newBlockQueue.NeedsSubChunkPositions(key);
                            break;

                        case NewBlockQueue.QueueState.needsShapeMaps:

                            newBlockQueue.NeedsShapeMap(key);
                            break;

                        case NewBlockQueue.QueueState.generatingShapeMaps:

                            newBlockQueue.GeneratingShapeMap(key);
                            break;

                        case NewBlockQueue.QueueState.needsBlockMap:

                            newBlockQueue.NeedsBlockMap(key);
                            break;

                        case NewBlockQueue.QueueState.generatingBlockMap:

                            newBlockQueue.GeneratingBlockMap(key);
                            break;

                        case NewBlockQueue.QueueState.needsSetMesh:

                            newBlockQueue.needsSetMesh.AddLast(key);
                            newBlockQueue.states[key] = NewBlockQueue.QueueState.settingMesh;

                            break;
                    }
                } 
            }

            //===============================================================================

            // Add and Remove blocks.
            // Check if there are any new sub chunks that need triangle data generated
            /*if (modifiedBlockQueue.subChunksThatNeedsNewMeshDataGenerated.Count > 0)
            {
                foreach (Vector3 key in modifiedBlockQueue.subChunksThatNeedsNewMeshDataGenerated.Keys)
                {
                    modifiedBlockQueue.GenerateTrianglesUsingFaceMap(key, modifiedBlockQueue.subChunksThatNeedsNewMeshDataGenerated[key].faces);

                    Debug.Log("Generating sub chunk triangles");
                }

                modifiedBlockQueue.subChunksThatNeedsNewMeshDataGenerated.Clear();
            }*/
        }
    }

    // Used to control the amount of meshes that are updated per frame. 
    // Fixes issue where all sub data would be assigned as soon as its ready causing many objects updated at once and the lag it caused.
    // Both new and mofified chunks share the same "queue" (IEnumerator). Mofified > new.
    [BurstCompile]
    public IEnumerator AssignMeshData()
    {
        // Infinite loop.
        while (true)
        {
            // Modified sub chunks
            // Check if there are any new sub chunks been modified in the ModifiedBlockQueue
            /*if (modifiedBlockQueue.subChunksThatNeedsMeshDataAssigned.Count > 0)
            {
                // copy the list and foreach through it. (more will be added to this list so it must be copied
                copy = new Dictionary<Vector3, SubChunkData>(modifiedBlockQueue.subChunksThatNeedsMeshDataAssigned);
                modifiedBlockQueue.subChunksThatNeedsMeshDataAssigned.Clear();

                foreach (Vector3 key in modifiedBlockQueue.subChunksThatNeedsMeshDataAssigned.Keys)
                {
                    index = ((int)key.y) / blockMap.SUB_CHUNK_HEIGHT;

                    // check if chunk is still holding the sub chunk.
                    if (blockPool.activeSubChunks[modifiedBlockQueue.subChunksActiveChunkName[key]].Contains(index))
                    {
                        modifiedBlockQueue.NeedsSetMesh(modifiedBlockQueue.subChunksActiveChunkName[key], key);

                        Debug.Log("Adjusted sub chunk mesh");

                        yield return null;
                    }
                    else
                    {
                        Debug.Log("Sub Chunk is no longer active in the chunk");
                    }

                    modifiedBlockQueue.subChunksActiveChunkName.Remove(key);


                }

                modifiedBlockQueue.subChunksThatNeedsMeshDataAssigned.Clear();
            }*/

            // New sub chunks
            if (newBlockQueue.needsSetMesh.Count > 0)
            {
                node = newBlockQueue.needsSetMesh.First;

                newBlockQueue.isSettingMesh = node.Value; // this is used for display only. could be deleted

                // has the chunk changed since we first processed it? if so ignore/remove from last stage.

                if (newBlockQueue.states[node.Value] != NewBlockQueue.QueueState.settingMesh)
                {
                    // need to remove sub chunk for whatever reason - adjusted?
                    // although new terrain shouldn't be updated straight away? unless online?
                    newBlockQueue.needsSetMesh.RemoveFirst();
                }
                else
                {
                    newBlockQueue.NeedsSetMesh(node.Value);
                    newBlockQueue.RemoveFromQueue(node.Value);                    

                    yield return null;
                }
            }

            yield return null;
        }
    }

    [BurstCompile]
    public void StartCoroutineFresh()
    {
        // switch off v-sync for faster loading
        // Frame rate controls IEnumerator call rate.
        if (main.optionEngine.graphicOptions.vsync) QualitySettings.vSyncCount = 0;

        on = true;  // Show block engine to be "on"

        assignMeshCoroutineRunning = true;
        StartCoroutine(AssignMeshData()); // <<<<<<<<<<<<<<<<<<<<<< Remove if using GenerateFresh()

        //StartCoroutine(GenerateFresh());
        StartCoroutine(GenerateFreshAuto());
    }

    [BurstCompile]
    public IEnumerator GenerateFreshAuto()
    {
        // enable the loading bar and set the intial method

        on = true;
        setUp = true;

        main.loadingBar.Activate();

        yield return null;

        main.loadingBar.message = "Generating Sub Chunk shared reference mesh data.";

        rawSubChunkData.GenerateSubChunkData();

        // Waiting on raw sub chunk data

        while (rawSubChunkData.jobActive)
        {
            yield return null;
        }

        // ============================

        main.loadingBar.message = "Generating Chunk Pool...";

        blockPool.BeforeGenerating();

        int counter = 1;    // avoid / by zero!

        // could be fixed to use boundary - adjust blockbool.IncrementPosition aswell
        blockPool.subMapPosition = blockPool.boundary.position;
        blockPool.savedSubMapPositon = blockPool.subMapPosition;
        int limit = (blockPool.size * blockPool.size) + 1;

        while (counter < limit)
        {
            main.loadingBar.description = "Block Pool count = " + counter.ToString();
            main.loadingBar.progress = counter / (float)limit;     // avoid / by zero!

            blockPool.ContinueToPopulateBlockPool();

            counter++;

            yield return null;
        }

        yield return null;

        //--------------------------------

        counter = 1;

        limit = blockPool.chunkPool.Count + 1;

        foreach (Vector3 key in blockPool.chunkPool.Keys)
        {
            newBlockQueue.AddToQueue(blockPool.chunkPool[key], blockPool.chunkPool[key].transform.position);

            main.loadingBar.description = "Generating Chunk " + blockPool.chunkPool[key].transform.position.ToString();
            main.loadingBar.progress = counter / (float)limit;

            counter++;

            yield return null;
        }

        while (newBlockQueue.chunks.Count > 0)
        {
            main.loadingBar.description = "Waiting on setting meshes....";
            yield return null;
        }

        //--------------------------------

        main.loadingBar.Deactivate();

        main.playerEngine.gameObject.SetActive(true);

        main.playerEngine.playerAnimation.spriteRenderer.enabled = true;

        main.playerEngine.playerAnimation.headGameObject.SetActive(true);

        if (main.optionEngine.graphicOptions.vsync) QualitySettings.vSyncCount = main.optionEngine.graphicOptions.vSyncCount;

        EventSystem.current.firstSelectedGameObject = main.optionEngine.firstSelected;

        // get set sky box

        //RenderSettings.skybox.SetColor("_SkyColor1", new Color(15f/255f, 151f/255f, 250f/255f, 0));
        //RenderSettings.skybox.SetColor("_SkyColor2", new Color(15f / 255f, 151f / 255f, 250f / 255f, 0));
        //RenderSettings.skybox.SetColor("_SkyColor3", new Color(15f / 255f, 151f / 255f, 250f / 255f, 0));

        //RenderSettings.skybox = main.skybox_default;
        DynamicGI.UpdateEnvironment();
    }
}



/*[BurstCompile]
    public IEnumerator GenerateFresh()
    {
        // enable the loading bar and set the intial method

        main.loadingBar.Activate();

        yield return null;

        main.loadingBar.message = "Generating Sub Chunk shared reference mesh data.";

        rawSubChunkData.GenerateSubChunkData();

        // Waiting on raw sub chunk data

        while (rawSubChunkData.jobActive)
        {
            yield return null;
        }

        // ============================

        main.loadingBar.message = "Generating Chunk Pool...";

        blockPool.BeforeGenerating();

        int counter = 1;    // avoid / by zero!

        int limit = (32 * 32) + 1;

        while (counter < limit)
        {
            main.loadingBar.description = "Block Pool count = " + counter.ToString();
            main.loadingBar.progress = counter / (float)limit;     // avoid / by zero!

            blockPool.ContinueToPopulateBlockPool();

            counter++;

            yield return null;
        }

        yield return null;

        // =============================

        yield return null;

        main.loadingBar.message = "Generating height map...";

        heightMap.BeforeGenerating();

        yield return null;

        counter = 0;

        while (counter < heightMap.loadingProgressionData.required)
        {
            heightMap.GenerateSections();

            counter += 1;

            main.loadingBar.description = heightMap.loadingProgressionData.count.ToString() + "/" + heightMap.loadingProgressionData.total.ToString();
            main.loadingBar.progress = heightMap.loadingProgressionData.progress;
            main.loadingBar.secondsRemaining = (int)heightMap.loadingProgressionData.secondsRemaining;

            yield return null;
        }

        main.loadingBar.message = "WAITING on height map...";

        while (heightMap.jobHandles.Count > 0)
        {
            main.loadingBar.description = "Job Count = " + heightMap.jobHandles.Count.ToString();

            yield return null;
        }*/

// ================================= SHOULD BE COMMENTED OUT

/*main.loadingBar.message = "Generating shape map...";

shapeMap.BeforeGenerating();

yield return null;

counter = 0;

while (counter < shapeMap.loadingProgressionData.required)
{
    shapeMap.GenerateSubChunks(16);

    counter += 16;

    main.loadingBar.description = shapeMap.loadingProgressionData.count.ToString() + "/" + shapeMap.loadingProgressionData.total.ToString();
    main.loadingBar.progress = shapeMap.loadingProgressionData.progress;
    main.loadingBar.secondsRemaining = (int)shapeMap.loadingProgressionData.secondsRemaining;

    yield return null;
}

main.loadingBar.message = "WAITING on shape map...";

while (shapeMap.jobHandles.Count > 0)
{
    main.loadingBar.description = "Job Count = " + shapeMap.jobHandles.Count.ToString();

    yield return null;
}*/

// ================================ SHOULD BE COMMENTED OUT

/*
    main.loadingBar.message = "Generating block map...";

    blockMap.BeforeGenerating();

    yield return null;

    counter = 0;

    while (counter < blockMap.loadingProgressionData.required)
    {
        blockMap.GenerateChunkData();

        counter += 1;

        main.loadingBar.description = blockMap.loadingProgressionData.count.ToString() + "/" + blockMap.loadingProgressionData.total.ToString();
        main.loadingBar.progress = blockMap.loadingProgressionData.progress;
        main.loadingBar.secondsRemaining = (int)blockMap.loadingProgressionData.secondsRemaining;

        yield return null;
    }

    main.loadingBar.message = "WAITING on block map...";

    while (blockMap.jobHandles.Count > 0)
    {
        main.loadingBar.description = "Job Count = " + blockMap.jobHandles.Count.ToString();

        yield return null;
    }

    // finished with height maps - perform clean up
    heightMap.CleanUp();

    // ===============================================================

    main.loadingBar.message = "Editing meshes of 'Sub Chunk' GameObjects..";

    int max_sub_chunks = (int)(blockMap.defaultMapSize / blockMap.SUB_CHUNK_SIZE)
        * (int)(blockMap.defaultMapSize / blockMap.SUB_CHUNK_SIZE)
        * blockMap.SUB_CHUNKS_PER_CHUNK;

    int numberOfChunksNeeded = blockMap.subChunkData.Keys.Count;

    counter = 0;

    main.loadingBar.secondsRemaining = 0;
    //float savedSeconds = 999999f;

    int numberOfEmptySubChunks = 0;

    // Cache variables to not keep creating them. And to reduce times fetched
    Transform chunk;
    Transform subChunk;
    int index;

    foreach (Vector3 key in blockMap.subChunkData.Keys)
    {
        // Does the mesh have any triangles?

        if (blockMap.subChunkData[key].triangles.Length != 0)
        {
            // Get chunk from the pool                
            chunk = blockPool.chunkPool[new Vector3(key.x, 0, key.z)];

            index = ((int) key.y) / blockMap.SUB_CHUNK_HEIGHT;

            subChunk = blockPool.subChunksPool[chunk.name][index];

            // Get the mesh component of the "Sub Chunk"

            blockPool.chunkComponents[chunk.name].subChunks[index].meshFilter.mesh.vertices = rawSubChunkData.vertices;
            blockPool.chunkComponents[chunk.name].subChunks[index].meshFilter.mesh.triangles = blockMap.subChunkData[key].triangles;
            blockPool.chunkComponents[chunk.name].subChunks[index].meshFilter.mesh.uv = blockMap.subChunkData[key].uvs;*/

/*blockPool.chunkComponents[chunk.name].subChunks[index].meshFilter.mesh.vertices = rawSubChunkData.vertices;
blockPool.chunkComponents[chunk.name].subChunks[index].meshFilter.mesh.SetIndices(blockMap.subChunkData[key].triangles, MeshTopology.Triangles, 0);
blockPool.chunkComponents[chunk.name].subChunks[index].meshFilter.mesh.SetUVs(0, blockMap.subChunkData[key].uvs);*/

/*
blockPool.chunkComponents[chunk.name].subChunks[index].meshFilter.mesh.Optimize();

blockPool.chunkComponents[chunk.name].subChunks[index].meshCollider.sharedMesh = blockPool.chunkComponents[chunk.name].subChunks[index].meshFilter.mesh;

subChunk.gameObject.SetActive(true);

// Take note of the "Sub Chunk" set active

blockPool.activeSubChunks[chunk.name].Add(index);
blockPool.activeTruePosition[chunk.name].Add(key);

yield return null;
}
else { numberOfEmptySubChunks++; } // No triangles - Fragements are empty - Nothing to render.

counter++;

main.loadingBar.progress = counter / (float)numberOfChunksNeeded;
main.loadingBar.description = counter.ToString() + "-" + numberOfEmptySubChunks + "-/" + numberOfChunksNeeded.ToString();
main.loadingBar.secondsRemaining = (int)((numberOfChunksNeeded - counter) / main.framesPerSecond.fps);
}

yield return null;

// block map data not currently needed.
blockMap.CleanUp();

yield return null;

setUp = true;

//-------------------------------------------------
// needs moved back into terrain stuff

main.loadingBar.Deactivate();

main.playerEngine.gameObject.SetActive(true);

main.playerEngine.playerMovement.player.Find("Camera Rig/Player Sprite").GetComponent<SpriteRenderer>().enabled = true;

if (main.optionEngine.graphicOptions.vsync) QualitySettings.vSyncCount = main.optionEngine.graphicOptions.vSyncCount;

EventSystem.current.firstSelectedGameObject = main.optionEngine.firstSelected;

// get set sky box

//RenderSettings.skybox.SetColor("_SkyColor1", new Color(15f/255f, 151f/255f, 250f/255f, 0));
//RenderSettings.skybox.SetColor("_SkyColor2", new Color(15f / 255f, 151f / 255f, 250f / 255f, 0));
//RenderSettings.skybox.SetColor("_SkyColor3", new Color(15f / 255f, 151f / 255f, 250f / 255f, 0));

RenderSettings.skybox = main.skybox_default;

DynamicGI.UpdateEnvironment();

//blockMap.CleanUp();
}*/
