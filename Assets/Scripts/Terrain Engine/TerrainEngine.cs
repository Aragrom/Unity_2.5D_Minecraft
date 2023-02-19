using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;
using UnityEngine.EventSystems;

[BurstCompile]
public class TerrainEngine : MonoBehaviour
{
    public Main main = null;

    public NewTerrainQueue newTerrainQueue = null;

    public TerrainMesh terrainMesh = null;              // Generate the mesh data for the terrain (vertices, triangles)
    public TerrainHeightMap terrainHeightMap = null;    // Generate the height map used on the terrain
    //public TerrainMap terrainMap = null;

    // The GameObject representing the large/far "Terrain" - !! this is not unity terrain - it is a generated mesh
    // 256 x 256 = 65536 = max vertices
    public Transform terrain = null;
    //public Transform terrainBackface = null;
    public Vector3 terrainDefaultPosition = new Vector3(0, 128, 0);

    public bool on = false;
    public bool setUp = false;
    public bool checkingForNewTerrain = false;

    [BurstCompile]
    private void OnDestroy()
    {
        // Free memory
        main = null;
        newTerrainQueue = null;
        terrainMesh = null;
        terrainHeightMap = null;
        //terrainMap = null;
        terrain = null;
        //terrainBackface = null;
    }

    // Start is called before the first frame update
    [BurstCompile]
    void Awake()
    {
        main = GameObject.Find("Main").GetComponent<Main>();

        newTerrainQueue = GetComponent<NewTerrainQueue>();

        terrainMesh = GetComponent<TerrainMesh>();
        terrainHeightMap = GetComponent<TerrainHeightMap>();
        //terrainMap = GetComponent<TerrainMap>();

        terrain = GameObject.Find("Terrain").transform;
        //terrainBackface = terrain.Find("Terrain Backface");
    }

    [BurstCompile]
    public void Update()
    {
        // NEED TO CREATE BETTER RELATIONSHIP AND REMOVE FROM UPDATE. <<<<<<<

        if (!on && main.blockEngine.setUp)
        {
            StartCoroutineFreshWorld();
        }

        if (on && setUp)
        {
            while (terrainHeightMap.jobHandlesGenerateHeights.Count == 0
                && terrainHeightMap.jobHandlesGetFromHashMap.Count == 0
                && newTerrainQueue.newTerrainLocations.Count != 0)
            {
                newTerrainQueue.ProcessTheFirstLocationInTheQueue();
                return;
            }

            if (newTerrainQueue.hasNewTerrain)
            {
                newTerrainQueue.hasNewTerrain = false;

                Vector3 tempPosition = main.treadmillEngine.truePosition
                    + new Vector3(-2048, 0, -2048);

                terrainHeightMap.GetHeightsFromHashMap(tempPosition.x, tempPosition.z);
                return;
            }
            
            // Is there newSortedTerrain? Is there no other (jobs) terrain update to be done?
            if (newTerrainQueue.newSortedTerrain
                && main.blockEngine.newBlockQueue.needsSetMesh.Count == 0) // <<< checking blockEngine.
            {
                newTerrainQueue.newSortedTerrain = false;

                terrainMesh.SetVertices(terrainHeightMap.sortedPositions);
                return;
            }
        }
    }

    [BurstCompile]
    public void OnApplicationQuit()
    {
        StopAllCoroutines();
    }

    [BurstCompile]
    public void StartCoroutineFreshWorld()
    {
        on = true;  // Show terrain engine to be "on"

        StartCoroutine(GenerateFreshTerrain());
    }

    /*[BurstCompile]
    public void StartCoroutineGenerateTerrainHeights()
    {
        checkingForNewTerrain = true;  // Show terrain engine to be "searching for when to generate new terrain heights"

        StartCoroutine(GenerateTerrainHeights());
    }*/

    [BurstCompile]
    public IEnumerator GenerateFreshTerrain()
    {
        // Generate terrain heights ============

        
        main.loadingBar.message = "Generating terrain height map...";

        terrainHeightMap.BeforeGenerating();

        terrainHeightMap.GenerateSections(terrainHeightMap.SUB_MAP_SIZE + 2, terrainHeightMap.SUB_MAP_SIZE + 2);

        main.loadingBar.description = terrainHeightMap.loadingProgressionData.count.ToString() + "/" + terrainHeightMap.loadingProgressionData.total.ToString();
        main.loadingBar.progress = terrainHeightMap.loadingProgressionData.progress;
        main.loadingBar.secondsRemaining = (int)terrainHeightMap.loadingProgressionData.secondsRemaining;

        terrainHeightMap.AfterGenerating();

        yield return null;

        main.loadingBar.message = "WAITING on terrain height map...";

        while (terrainHeightMap.jobHandlesGenerateHeights.Count > 0)
        {
            main.loadingBar.description = "Job Count = " + terrainHeightMap.jobHandlesGenerateHeights.Count.ToString();

            yield return null;
        }

        yield return null;

        // Set heights =======================

        main.loadingBar.message = "Getting height map from hash map...";

        main.loadingBar.progress = 1.0f;

        terrainHeightMap.GetHeightsFromHashMap(terrainHeightMap.subMapPosition.x, terrainHeightMap.subMapPosition.y);

        // Wait until terrain Heights have been sorted (return vertices[])

        while (terrainHeightMap.jobHandlesGetFromHashMap.Count > 0)
        {
            yield return null;
        }

        //terrainMesh.SetVertices(terrainHeightMap.sortedPositions);

        //=======================================

        main.loadingBar.message = "Generating terrain mesh...";

        terrainMesh.GenerateTerrainTriangleData();

        while (terrainMesh.jobActive)
        {
            yield return null;
        }

        terrainMesh.CreateMesh(terrainHeightMap.sortedPositions, terrainMesh.triangles, terrainMesh.uvs);

        // Every time terrainHeightMap runs sets hasNewTerrain/newSortedTerrain. used in set up so forcing it off.

        newTerrainQueue.newSortedTerrain = false;
        newTerrainQueue.hasNewTerrain = false;

        setUp = true;

        yield return null;

        // ===============================================

        /*main.loadingBar.Deactivate();

        main.playerEngine.gameObject.SetActive(true);

        main.playerEngine.playerMovement.player.Find("Camera Rig/Player Sprite").GetComponent<SpriteRenderer>().enabled = true;

        if(main.optionEngine.graphicOptions.vsync) QualitySettings.vSyncCount = main.optionEngine.graphicOptions.vSyncCount;

        EventSystem.current.firstSelectedGameObject = main.optionEngine.firstSelected;

        // get set sky box

        //RenderSettings.skybox.SetColor("_SkyColor1", new Color(15f/255f, 151f/255f, 250f/255f, 0));
        //RenderSettings.skybox.SetColor("_SkyColor2", new Color(15f / 255f, 151f / 255f, 250f / 255f, 0));
        //RenderSettings.skybox.SetColor("_SkyColor3", new Color(15f / 255f, 151f / 255f, 250f / 255f, 0));

        RenderSettings.skybox = main.skybox_default;

        DynamicGI.UpdateEnvironment();

        yield return null;*/
    }
}
