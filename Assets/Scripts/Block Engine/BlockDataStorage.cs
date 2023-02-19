using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

[System.Serializable]
[BurstCompile]
public struct ChunkData
{
    // Should be smaller than int?
    public int[] y;                     // The y value of each sub chunk
    public List<int> triangles;         // 36 x 4096
    public List<Vector2> uvs;           // 24 x 4096
}

[BurstCompile]
public class BlockDataStorage : MonoBehaviour
{
    BlockEngine blockEngine = null;

    SaveLoadEngine saveLoadEngine = null;

    public Dictionary<Vector2, string> regions; // Key position, value filepath

    public Dictionary<Vector3, bool> activeZoneSubGeneratedSubChunks;

    public bool triggerSave = false;
    public bool triggerLoad = false;

    public ChunkData loadedChunkData = new ChunkData();

    [BurstCompile]
    private void Awake()
    {
        blockEngine = GetComponent<BlockEngine>();
        saveLoadEngine = GameObject.Find("Save Load Engine").GetComponent<SaveLoadEngine>();
    }

    // Start is called before the first frame update
    [BurstCompile]
    void Start()
    {
        
    }

    // Update is called once per frame
    [BurstCompile]
    void Update()
    {
        if (triggerSave)
        {
            triggerSave = false;
            SaveChunk(Vector2.zero);
        }

        if (triggerLoad)
        {
            triggerLoad = false;
            LoadChunk(Vector2.zero);
        }
    }

    [BurstCompile]
    void SaveChunk(Vector2 chunkPosition)
    {
        // Save all sub chunk data in a chunk

        int SubChunkCount = blockEngine.blockMap.subChunkPositionsByChunk[chunkPosition].Count;

        ChunkData chunkData = new ChunkData
        {
            y = new int[SubChunkCount],
            triangles = new List<int>(new int[SubChunkCount * 36 * 4096]),
            uvs = new List<Vector2>(new Vector2[SubChunkCount * 24 * 4096])
        };

        Vector3 subChunkPosition;
        string json;
        string compressed;

        for (int i = 0; i < blockEngine.blockMap.subChunkPositionsByChunk[chunkPosition].Count; i++)
        {
            subChunkPosition = blockEngine.blockMap.subChunkPositionsByChunk[chunkPosition][i];
            chunkData.y[i] = (int)subChunkPosition.y;

            //chunkData.triangles.InsertRange(i * 36 * 4096, blockEngine.blockMap.subChunkData[subChunkPosition].triangles);
            //chunkData.uvs.InsertRange(i * 24 * 4096, blockEngine.blockMap.subChunkData[subChunkPosition].uvs);
        }

        json = saveLoadEngine.ToJson(chunkData);
        compressed = saveLoadEngine.CompressString(json);

        saveLoadEngine.SaveData(compressed, "Chunk " + chunkPosition.ToString());

        Debug.Log("Save Complete");
    }

    [BurstCompile]
    void LoadChunk(Vector2 chunkPosition)
    {
        // Save all sub chunk data in a chunk

        string compressed;
        string json;

        compressed = saveLoadEngine.LoadData("Chunk " + chunkPosition.ToString());

        json = saveLoadEngine.DecompressString(compressed);
        loadedChunkData = saveLoadEngine.FromJson<ChunkData>(json);

        Debug.Log("Load Complete");
    }
}
