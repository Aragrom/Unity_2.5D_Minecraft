using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

[BurstCompile]
public struct ChunkComponents 
{
    public SubChunkComponents[] subChunks;
}

[BurstCompile]
public struct SubChunkComponents
{
    // could add more here...other data is in dictionaries in blockpool.cs
    // public Transform transform; // not yet used
    public MeshFilter meshFilter;
    //public Mesh mesh;     // dont need a copy of the mesh here...
    public MeshCollider meshCollider;

    public Rigidbody rigidbody;
}

[BurstCompile]
public class BlockPool : MonoBehaviour
{
    private BlockEngine blockEngine = null;
    private SurroundingCrossSectionEngine surroundingCrossSectionEngine = null;

    public GameObject chunkPrefab = null;

    // Where are the pool of "Chunks" gameobjects stored?
    public Dictionary<Vector3, Transform> chunkPool = new Dictionary<Vector3, Transform>();    // Key - position chunks GameObjects are currently located.

    public Dictionary<string, ChunkComponents> chunkComponents = new Dictionary<string, ChunkComponents>(); // to stop calling .GetComponent each time to get meshfilter/meshcollider

    public Dictionary<string, Transform[]> subChunksPool = new Dictionary<string, Transform[]>(); // key - chunk name, List of all sub chunks associated with that chunk

    // Which "Sub Chunk"s are active for each "Chunk"?
    public Dictionary<string, List<int>> activeSubChunks = new Dictionary<string, List<int>>();                 // key - chunk name, List of active sub chunks (list of indexes for "subChunkPool")
    public Dictionary<string, List<Vector3>> activeTruePosition = new Dictionary<string, List<Vector3>>();      // key - chunk name, What true positions are active on the sub chunk

    // What are the positions adjusted and managed to control the spawning of chunks?
    public Vector2 subMapPosition = new Vector2(-256, -256);
    public Vector2 savedSubMapPositon = Vector2.zero;          // used when reseting the row position for incrementing the submap postion (x != 0 .. x == savedSubMapPosition.x)

    // What are the sizes used to make the grid generate?
    public float defaultMapSize = 512;          // Grid - size of the map (size * size)
    public int SUB_MAP_SIZE = 16;        // Grid of "submaps" make up "defaultmapsize" (size * size)

    public int MAX_SIZE = 32;
    public int MIN_SIZE = 16;
    public int size = 32;             // Max to min range

    // Used to re-name "Chunk" GameObjects to reflect a unique identifier (counter.ToString()).
    public int idCounter = 0;   // stored here to cache and avoid creating many int's

    public Rect oldBoundary = new Rect(-256, -256, 240, 240);
    public Rect boundary = new Rect(-256, -256, 240, 240);

    public bool isAddingNewChunks = false;

    public List<Vector3> needNewChunk = new List<Vector3>();
    public Dictionary<Vector3, string> newChunkThatNeedTerrain = new Dictionary<Vector3, string>();

    [BurstCompile]
    private void OnDestroy()
    {
        // Free memory

        blockEngine = null;
        chunkPrefab = null;

        //foreach (Vector3 key in chunkPool.Keys)
        //{
        //    Destroy(chunkPool[key]);
        //}
        chunkComponents.Clear();
        chunkComponents = null;
        chunkPool.Clear();
        chunkPool = null;
        subChunksPool.Clear();
        subChunksPool = null;
        activeSubChunks.Clear();
        activeSubChunks = null;
        activeTruePosition.Clear();
        activeTruePosition = null;

        needNewChunk.Clear();
        needNewChunk = null;
        newChunkThatNeedTerrain.Clear();
        newChunkThatNeedTerrain = null;

        StopAllCoroutines();
    }

    [BurstCompile]
    private void Awake()
    {
        blockEngine = GetComponent<BlockEngine>();
        //GameObject.Find("Surrounding Cross Section Engine").GetComponent<SurroundingCrossSectionEngine>();
    }

    [BurstCompile]
    public void BeforeGenerating()
    {
        savedSubMapPositon = subMapPosition;
    }

    [BurstCompile]
    public void ContinueToPopulateBlockPool()
    {
        CreateChunk(new Vector3(subMapPosition.x, 0, subMapPosition.y));

        IncrementSubMapPosition();
    }

    // Remember making chunk is expensive and all are cached at the beginning of the game - should maybe not delete them but onlt disable the gameobjects
    [BurstCompile]
    public void Resize(int size, bool isBigger, int differenceBetweenOldSizeAndNewSize)
    {
        // is the same size return;
        //if (this.size == size) { return; }

        oldBoundary = boundary;

        this.size = size;

        if (isBigger)
        {
            // Resize boundary
            boundary.x -= differenceBetweenOldSizeAndNewSize * SUB_MAP_SIZE;
            boundary.y -= differenceBetweenOldSizeAndNewSize * SUB_MAP_SIZE;
            boundary.width += differenceBetweenOldSizeAndNewSize * SUB_MAP_SIZE;
            boundary.height += differenceBetweenOldSizeAndNewSize * SUB_MAP_SIZE;

            defaultMapSize = SUB_MAP_SIZE * size;

            if (blockEngine.main.optionEngine.inGame == false) return;

            // Creating new chunks is expensive. 
            // The mesh needs to be cached. (internally unity makes a copy of the mesh - creates the reference when chunk is created)

            // use old boundary and new boundary.
            // going through a dictionary

            // Create all positions for new boundary
            Vector3 position = new Vector3();

            for (int x = (int)boundary.x; x <= (int)boundary.width; x += SUB_MAP_SIZE)
            {
                for (int z = (int)boundary.y; z <= (int)boundary.height; z += SUB_MAP_SIZE)
                {
                    // is the chunk outside the old boundary?
                    if (x > (int)oldBoundary.width
                        || x < (int)oldBoundary.x
                        || z > (int)oldBoundary.height
                        || z < (int)oldBoundary.y)
                    {
                        position = new Vector3(x, 0, z);

                        needNewChunk.Add(position);
                    }
                }
            }

            isAddingNewChunks = true;

            blockEngine.main.loadingBar.loadingVisualizer.SetActive(true);

            // start coroutine

            StartCoroutine(GenerateNewChunks(size, isBigger, differenceBetweenOldSizeAndNewSize));
        }
        else
        {
            // Resize boundary
            boundary.x += differenceBetweenOldSizeAndNewSize * SUB_MAP_SIZE;
            boundary.y += differenceBetweenOldSizeAndNewSize * SUB_MAP_SIZE;
            boundary.width -= differenceBetweenOldSizeAndNewSize * SUB_MAP_SIZE;
            boundary.height -= differenceBetweenOldSizeAndNewSize * SUB_MAP_SIZE;

            defaultMapSize = SUB_MAP_SIZE * size;

            if (blockEngine.main.optionEngine.inGame == false) { return; }

            // Determine location of chunks no longer needed.
            // Go through each chunk and make sure its within new boundary

            List<Vector3> positionsThatNeedDestroyed = new List<Vector3>();

            // go through dictionary and remove any chunks not within the bound of the new boundary
            foreach (Vector3 key in chunkPool.Keys)
            {
                // is the chunk outside the new boundary?
                if (key.x > boundary.width
                    || key.x < boundary.x
                    || key.z > boundary.height
                    || key.z < boundary.y)
                {
                    positionsThatNeedDestroyed.Add(key);
                }
            }

            for (int i = 0; i < positionsThatNeedDestroyed.Count; i++)
            {
                string chunkName = chunkPool[positionsThatNeedDestroyed[i]].name;

                // Destroy chunk no longer needed

                /*for (int n = 0; n < chunkComponents[chunkName].subChunks.Length; n++)
                {
                    //Destroy(chunkComponents[chunkName].subChunks[n].meshFilter.mesh);
                    Destroy(chunkComponents[chunkName].subChunks[n].meshFilter);
                    Destroy(chunkComponents[chunkName].subChunks[n].meshCollider);
                }*/

                chunkComponents.Remove(chunkName);
                subChunksPool.Remove(chunkName);
                activeSubChunks.Remove(chunkName);
                activeTruePosition.Remove(chunkName);

                Destroy(chunkPool[positionsThatNeedDestroyed[i]].gameObject);

                chunkPool.Remove(positionsThatNeedDestroyed[i]);

                //Debug.Log("Trying to Remove chunk at position: " + positionsThatNeedDestroyed[i].ToString());
            }

            positionsThatNeedDestroyed.Clear();
        }
    }

    [BurstCompile]
    public IEnumerator GenerateNewChunks(int size, bool isBigger, int differenceBetweenOldSizeAndNewSize)   // params need to be passed to treamill resize
    {
        for (int i = 0; i < needNewChunk.Count; i++)
        {
            newChunkThatNeedTerrain.Add(needNewChunk[i], CreateChunk(needNewChunk[i]));

            yield return null;
        }

        yield return null;

        blockEngine.main.treadmillEngine.Resize(size, isBigger, differenceBetweenOldSizeAndNewSize);

        Vector3 savedTruePosition = blockEngine.main.treadmillEngine.truePosition;
        Vector3 adjustedPosition;

        foreach (Vector3 position in newChunkThatNeedTerrain.Keys)
        {
            adjustedPosition = position + (savedTruePosition - blockEngine.main.treadmillEngine.truePosition);

            // Check adjustedPosition is still inside the block pool boundary. 
            // (The treadmill might have moved making the chunk at a new location completely)
            if (adjustedPosition.x > boundary.width
                || adjustedPosition.x < boundary.x
                || adjustedPosition.z > boundary.height
                || adjustedPosition.z < boundary.y)     // To Do: Reverse this properly - remove the else statement.
            {
                // DO nothing
            }
            else
            {
                if (blockEngine.newBlockQueue.CheckIsAlreadyInQueue(blockEngine.blockPool.chunkPool[adjustedPosition].name) == false)
                {
                    blockEngine.newBlockQueue.AddToQueue(blockEngine.blockPool.chunkPool[adjustedPosition], adjustedPosition + blockEngine.main.treadmillEngine.truePosition);

                    yield return null;
                }
            }
        }

        newChunkThatNeedTerrain.Clear();
        needNewChunk.Clear();

        blockEngine.blockEngineOptions.size = (int)blockEngine.blockEngineOptions.newSize;

        yield return null;

        isAddingNewChunks = false;
    }

    [BurstCompile]
    public string CreateChunk(Vector3 position)
    {
        GameObject go = Instantiate(chunkPrefab, position, Quaternion.identity);

        go.name = "Chunk " + idCounter.ToString();

        chunkPool.Add(position, go.transform);

        ChunkComponents cc = new ChunkComponents();

        // Add a list to store all the true positons for the sub chunks active on the chunk.
        activeTruePosition.Add(go.name, new List<Vector3>());

        // Get all sub chunk gameobjects components from children as an array.
        // Use array to create a list.

        Transform[] subChunks = go.GetComponentsInChildren<Transform>();

        cc.subChunks = new SubChunkComponents[subChunks.Length];

        // note set to 1 - "Chunk 0" gameobject does not have meshfilter (somehow is returning parent object as child. scuffed)
        for (int i = 1; i < subChunks.Length; i++)
        {
            subChunks[i].gameObject.SetActive(false);

            cc.subChunks[i] = new SubChunkComponents
            {
                meshFilter = subChunks[i].GetComponent<MeshFilter>(),
                meshCollider = subChunks[i].GetComponent<MeshCollider>(),
                rigidbody = subChunks[i].GetComponent<Rigidbody>()
            };
            //switch off collision detection to improve performance.
            cc.subChunks[i].rigidbody.detectCollisions = false;

            // SUPER IMPORTANT TO FETCH NOW SO MEMORY IS NOT CREATED DURING GAMEPLAY.
            // When Mesh is called internally unity creates a copy. So the reference is created now.
            cc.subChunks[i].meshFilter.mesh = cc.subChunks[i].meshFilter.mesh;
        }

        // Add the sub chunks to the sub chunk pool

        chunkComponents.Add(go.name, cc);

        subChunksPool.Add(go.name, subChunks);

        activeSubChunks.Add(go.name, new List<int>());

        go.SetActive(true);

        // Add the large cross section to the pool in the surrounding cross section engine
        //surroundingCrossSectionEngine.largeSurroundingCrossSectionPool.Add(go.name, go.transform.Find("Large Black Cross Section"));

        idCounter++;

        return go.name;
    }

    [BurstCompile]
    public void SetSubChunksThatAreActiveFalseForChunk(string chunkName)
    {
        for (int i = 0; i < activeSubChunks[chunkName].Count; i++)
        {
            // clear any data in mesh component.
            chunkComponents[chunkName].subChunks[activeSubChunks[chunkName][i]].meshFilter.mesh.triangles = null;
            chunkComponents[chunkName].subChunks[activeSubChunks[chunkName][i]].meshFilter.mesh.uv = null;
            chunkComponents[chunkName].subChunks[activeSubChunks[chunkName][i]].meshFilter.sharedMesh.triangles = null;
            chunkComponents[chunkName].subChunks[activeSubChunks[chunkName][i]].meshFilter.sharedMesh.uv = null;
            chunkComponents[chunkName].subChunks[activeSubChunks[chunkName][i]].meshCollider.sharedMesh.triangles = null;
            chunkComponents[chunkName].subChunks[activeSubChunks[chunkName][i]].meshCollider.sharedMesh.uv = null;

            chunkComponents[chunkName].subChunks[activeSubChunks[chunkName][i]].meshFilter.mesh.Clear();
            chunkComponents[chunkName].subChunks[activeSubChunks[chunkName][i]].meshFilter.sharedMesh.Clear();
            chunkComponents[chunkName].subChunks[activeSubChunks[chunkName][i]].meshCollider.sharedMesh.Clear();

            /*chunkComponents[chunkName].subChunks[activeSubChunks[chunkName][i]].meshFilter.mesh = null;
            chunkComponents[chunkName].subChunks[activeSubChunks[chunkName][i]].meshFilter.sharedMesh = null;
            chunkComponents[chunkName].subChunks[activeSubChunks[chunkName][i]].meshCollider.sharedMesh = null;*/

            subChunksPool[chunkName][activeSubChunks[chunkName][i]].gameObject.SetActive(false);
        }

        activeTruePosition[chunkName].Clear();

        activeSubChunks[chunkName].Clear();
    }

    // 2D
    [BurstCompile]
    public void IncrementSubMapPosition()
    {
        // Are we at the last sub map in the row of sub maps?

        if (subMapPosition.x == (savedSubMapPositon.x + defaultMapSize - SUB_MAP_SIZE))
        {
            // Set to new row
            subMapPosition.x = savedSubMapPositon.x;
            subMapPosition.y += SUB_MAP_SIZE;
        }
        else { subMapPosition.x += SUB_MAP_SIZE; }  // Incremement in the horizontal
    }
}
