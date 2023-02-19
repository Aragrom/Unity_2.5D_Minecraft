using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;
using UnityEngine.SceneManagement;

// This should be the only object in the empty scene. Is used to smooth the resetting of memory
// The "Empty scene" is loaded inbetween returning to the "title screen" and "main scene"
[BurstCompile]
public class EmptyScene : MonoBehaviour
{
    // Start is called before the first frame update
    [BurstCompile]
    void Start()
    {
        StartCoroutine(AsyncUnloadResources());
    }

    // Use a coroutine to unload all unused assets from Resource folder.
    [BurstCompile]
    IEnumerator AsyncUnloadResources()
    {
        // Unload all unused assets from Resource folder.
        AsyncOperation asyncOperation = Resources.UnloadUnusedAssets();
        while (!asyncOperation.isDone)  // waiting for complete
        {
            Debug.Log("Unloading progress: " + asyncOperation.progress);
            yield return null;
        }
        Debug.Log("Complete unloading unused assets");

        // Load the main scene again

        SceneManager.LoadScene("Main");
    }
}
