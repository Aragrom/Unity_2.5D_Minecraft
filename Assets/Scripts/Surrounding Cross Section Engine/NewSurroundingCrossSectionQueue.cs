using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

[BurstCompile]
public class NewSurroundingCrossSectionQueue : MonoBehaviour
{
    // Could combine into a structure =============

    public Dictionary<string, QueueState> states = new Dictionary<string, QueueState>();

    public Dictionary<string, Transform> chunks = new Dictionary<string, Transform>();

    public Dictionary<string, Vector3> positions = new Dictionary<string, Vector3>();

    public LinkedList<string> needsSetMesh = new LinkedList<string>();

    public string isSettingMesh = "";

    // ============================================

    public Vector3 tempPosition3 = Vector3.zero;

    public Vector2 tempPosition2 = Vector2.zero;

    public enum QueueState
    {
        needsCrossSectionMap,
        generatingCrossSectionMap,
        needsSetMesh,
        settingMesh
    }

    [BurstCompile]
    private void OnDestroy()
    {
        // Free memory

        states.Clear();
        states = null;
        chunks.Clear();
        chunks = null;
        positions.Clear();
        positions = null;
        needsSetMesh.Clear();
        needsSetMesh = null;
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
        states.Add(chunk.name, QueueState.needsCrossSectionMap);
        chunks.Add(chunk.name, chunk);
        positions.Add(chunk.name, position);
    }

    [BurstCompile]
    public void ChangeQueue(Transform chunk, Vector3 position)
    {
        // Change state so the current updates are stored but not further developed
        // This will become used when the system cant keep up with the Generation for the player.
        states[chunk.name] = QueueState.needsCrossSectionMap;
        positions[chunk.name] = position;
    }

    [BurstCompile]
    public void RemoveFromQueue(string chunkName)
    {
        states.Remove(chunkName);
        chunks.Remove(chunkName);
        positions.Remove(chunkName);
        needsSetMesh.Remove(chunkName);
    }

    /*
    // Assume block maps will always exists for the necessary cross section
    [BurstCompile]
    public void NeedsCrossSectionMap(string name)
    {
        tempPosition3 = positions[name];

        tempPosition2 = new Vector2(tempPosition3.x, tempPosition3.z);

        // Check if cross section map already exists

        if (crossSectionEngine.crossSectionMap.triangles.ContainsKey(tempPosition2) == false)
        {
            crossSectionEngine.crossSectionMap.subMapPosition = tempPosition3;

            crossSectionEngine.crossSectionMap.GenerateCrossSection(tempPosition3);

            states[name] = QueueState.generatingCrossSectionMap;
        }
        else
        {
            // Cross section map already exists

            states[name] = QueueState.generatingCrossSectionMap;
        }
    }

    [BurstCompile]
    public void GeneratingCrossSectionMap(string name)
    {
        // check if job exists for this position. if it doesn't then it will be complete.
        // OR check if subChunkData exists <<<<

        tempPosition3 = positions[name];

        tempPosition2 = new Vector2(tempPosition3.x, tempPosition3.z);

        if (crossSectionEngine.crossSectionMap.chunkData[tempPosition2] == true)
        {
            states[name] = QueueState.needsSetMesh;
        }
    }

    // ==============

    [BurstCompile]
    public void NeedsSetMesh(string name)
    {
        tempPosition3 = positions[name];

        tempPosition2 = new Vector2(tempPosition3.x, tempPosition3.z);

        List<Vector3> subChunksPosition = crossSectionEngine.crossSectionMap.subChunkPositionsByChunk[tempPosition2];

        for (int i = 0; i < subChunksPosition.Count; i++)
        {
            int index = ((int)subChunksPosition[i].y) / BlockMap.SUB_CHUNK_HEIGHT;

            Transform subChunk = crossSectionEngine.crossSectionPool.subChunksPool[name][index];

            // Get the mesh component of the "Sub Chunk"

            Mesh mesh = subChunk.GetComponent<MeshFilter>().mesh;

            mesh.vertices = crossSectionEngine.rawCrossSectionData.vertices;
            mesh.triangles = crossSectionEngine.crossSectionMap.triangles[subChunksPosition[i]];
            mesh.uv = crossSectionEngine.rawCrossSectionData.uvs;

            mesh.Optimize();
            //mesh.RecalculateNormals();

            subChunk.GetComponent<MeshCollider>().sharedMesh = mesh;

            subChunk.gameObject.SetActive(true);

            // Take note of the "Sub Chunk" set active

            crossSectionEngine.crossSectionPool.activeSubChunkCrossSectionsEditable[name].Add(index);
        }
    }
    */
}
