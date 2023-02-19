using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst;
using UnityEngine;
using UnityEngine.UI;

// Track and display the fps of the game
[BurstCompile]
public class FramesPerSecond : MonoBehaviour
{
    public float fps = 0.0f;        // frames per second

    public float frameCount = 0f;   // Counting frames over the duration
    public float nextUpdate = 0.0f; // the end of the duration. Triggers a count when greater than
    public float updateRate = 1.0f; // (x) updates per sec.
    public int targetFrameRate = -1;

    //public Canvas canvas = null;

    public TMP_Text text = null;    // UI text used to display the fps

    [BurstCompile]
    private void OnDestroy()
    {
        // Free memory

        text = null;
    }

    [BurstCompile]
    void Awake()
    {
        Application.targetFrameRate = targetFrameRate;     // Force the maximum possible fps

        nextUpdate = Time.time;
    }

    // Called inside Main.cs
    [BurstCompile]
    public void Update()
    {
        frameCount++;
        if (Time.time > nextUpdate)
        {
            nextUpdate += 1.0f / updateRate;
            fps = frameCount * updateRate;
            frameCount = 0;

            text.text = "FPS : " + fps.ToString();
        }
    }
}
