using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

[BurstCompile]
public struct NewTerrainLocation
{
    public Vector2 position;                        // what position should the new row of terrain be generated out from.
    public float xSize;
    public float zSize;

    public NewTerrainLocation(Vector2 position, float xSize, float zSize)
    {
        this.position = position;
        this.xSize = xSize;
        this.zSize = zSize;
    }
}

[BurstCompile]
public class NewTerrainQueue : MonoBehaviour
{
    private TerrainEngine terrainEngine = null;

    public bool hasNewTerrain = false;
    public bool newSortedTerrain = false;

    public LinkedList<NewTerrainLocation> newTerrainLocations = new LinkedList<NewTerrainLocation>();

    [BurstCompile]
    private void OnDestroy()
    {
        // Free memory
        terrainEngine = null;

        newTerrainLocations.Clear();
        newTerrainLocations = null;
    }

    [BurstCompile]
    private void Awake()
    {
        terrainEngine = GetComponent<TerrainEngine>();
    }

    [BurstCompile]
    public void AddToQueue(Vector2 position, float xSize, float zSize)
    {
        // last - in

        newTerrainLocations.AddLast(new NewTerrainLocation(position, xSize, zSize));
    }

    [BurstCompile]
    public void RemoveFromQueue()
    {
        // first - out

        newTerrainLocations.RemoveFirst();
    }

    [BurstCompile]
    public void ProcessTheFirstLocationInTheQueue()
    {
        // Get first in the queue

        NewTerrainLocation newTerrainLocation = newTerrainLocations.First.Value;

        // set sub map

        terrainEngine.terrainHeightMap.subMapPosition = newTerrainLocation.position;

        // Generate sections job

        terrainEngine.terrainHeightMap.GenerateSections((int)newTerrainLocation.xSize, (int)newTerrainLocation.zSize);

        // Remove the first location from the queue.

        RemoveFromQueue();
    }
}
