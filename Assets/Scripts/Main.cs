using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;

// Game backbone. Root of all components.
[BurstCompile]
public class Main : MonoBehaviour
{
    public InputEngine inputEngine = null;

    public PlayerEngine playerEngine = null;            // Player data/logic/input handling system
    public ActorEngine actorEngine = null;

    public TreadmillEngine treadmillEngine;
    public BlockEngine blockEngine = null;              // Handle block generation and management
    public TerrainEngine terrainEngine = null;

    public SaveLoadEngine saveLoadEngine = null;        // Have one save load system to avoid reading/writing as much as possible. Do it in one go.

    public TitleScreenEngine titleScreenEngine = null;
    public OptionEngine optionEngine = null;

    public MemoryProfiling memoryProfiling = null;
    public LoadingBar loadingBar = null;                // For displaying several elements loading progression
    public Window window = null;
    public FramesPerSecond framesPerSecond = null;      // Calculates the frames per second of the game
    public Zoom zoom = null;
    public Look look = null;

    public GameObject cameraRig = null;                 // Camera rig attached to the player - only cameras orthographic overlaying perspective

    public Material skybox_black = null;
    public Material skybox_default = null;

    [BurstCompile]
    private void OnDestroy()
    {
        // free memory

        inputEngine = null;

        playerEngine = null;
        actorEngine = null;

        treadmillEngine = null;
        blockEngine = null;
        terrainEngine = null;

        saveLoadEngine = null;

        titleScreenEngine = null;
        optionEngine = null;

        memoryProfiling = null;
        loadingBar = null;
        window = null;
        framesPerSecond = null;
        zoom = null;
        look = null;

        cameraRig = null;

        skybox_black = null;
        skybox_default = null;
    }

    [BurstCompile]
    private void Awake()
    {
        // Initialize the fields with the required components

        inputEngine = GameObject.Find("Input Engine").GetComponent<InputEngine>();
        playerEngine = GameObject.Find("Player Engine").GetComponent<PlayerEngine>();
        actorEngine = GameObject.Find("Actor Engine").GetComponent<ActorEngine>();

        treadmillEngine = GameObject.Find("Treadmill Engine").GetComponent<TreadmillEngine>();
        blockEngine = GameObject.Find("Block Engine").GetComponent<BlockEngine>();
        terrainEngine = GameObject.Find("Terrain Engine").GetComponent<TerrainEngine>();

        saveLoadEngine = GameObject.Find("Save Load Engine").GetComponent<SaveLoadEngine>();

        titleScreenEngine = GameObject.Find("Title Screen Engine").GetComponent<TitleScreenEngine>();
        optionEngine = GameObject.Find("Option Engine").GetComponent<OptionEngine>();

        memoryProfiling = GetComponent<MemoryProfiling>();
        loadingBar = GetComponent<LoadingBar>();
        window = GetComponent<Window>();
        framesPerSecond = GetComponent<FramesPerSecond>();
        zoom = GetComponent<Zoom>();
        look = GetComponent<Look>();

        //RenderSettings.skybox = skybox_black;
        DynamicGI.UpdateEnvironment();  // required to update skybox

        StartCoroutine(AsyncUnloadResources());

        //==================================

        //Physics.autoSyncTransforms = true;    // Attempty to not have sub chunks with rigidbodies.
        //UnityEditor.EditorPrefs.SetBool("DeveloperMode", true);   // Toggle on/off developer mode in the unity editor

        //PlayerPrefs.DeleteAll();
    }

    // Unload unused assets
    [BurstCompile]
    IEnumerator AsyncUnloadResources()
    {
        AsyncOperation asyncOperation = Resources.UnloadUnusedAssets();

        while (!asyncOperation.isDone)
        {
            Debug.Log("Unloading progress: " + asyncOperation.progress);
            yield return null;
        }
        Debug.Log("Complete unloading unused assets");
    }

    // Title screen
    [BurstCompile]
    public void Exit()
    {
        Application.Quit();
    }

    // Return to title screen
    [BurstCompile]
    public void ExitToMainMenu()
    {
        //optionEngine.blockEngineOptions.SaveSettings();
        //optionEngine.graphicOptions.SaveSettings();

        // MUST WAIT ON JOBS TO COMPLETE!!! - OnApplicationQuit() - handles this
        // All classes with jobs should be called to finish all jobs as if closing before returning to main menu
        terrainEngine.terrainHeightMap.OnApplicationQuit();
        terrainEngine.terrainMesh.OnApplicationQuit();
        //terrainEngine.terrainMap.OnApplicationQuit();

        blockEngine.heightMap.OnApplicationQuit();
        blockEngine.shapeMap.OnApplicationQuit();
        blockEngine.blockMap.OnApplicationQuit();
        blockEngine.rawSubChunkData.OnApplicationQuit();

        // NEED TO ADD CROSS SECTION IN WHEN IT IS USED
        //crossSectionEngine.rawCrossSectionData.OnApplicationQuit();
        //crossSectionEngine.crossSectionMap.OnApplicationQuit();

        SceneManager.LoadScene("Empty");
    }

    // Exit from game and close window
    [BurstCompile]
    public void QuitGame()
    {
        Application.Quit();
    }
}
