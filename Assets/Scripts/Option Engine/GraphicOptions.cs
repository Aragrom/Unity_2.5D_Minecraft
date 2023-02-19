using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[BurstCompile]
public class GraphicOptions : MonoBehaviour
{
    public int targetFramesPerSecond = -1;
    public bool displayFramesPerSecond = true;
    public int vSyncCount = 0;      // 0 - no vsync, or 60/30/20/15 = vsync
    public bool vsync = false;

    public GameObject canvas = null;
    public Slider slider = null;
    public TMP_Text vsyncButtonText = null;

    public GameObject firstSelected = null; // ui

    //================================

    // Display results for the graphics options so players know what is being set
    public TMP_Text vsyncResultText = null;

    [BurstCompile]
    public void OnApplicationQuit()
    {
        SaveSettings();
    }

    [BurstCompile]
    private void OnDestroy()
    {
        // Free memory

        SaveSettings();

        canvas = null;
        slider = null;
        vsyncButtonText = null;
        firstSelected = null;

        vsyncResultText = null;
    }

    [BurstCompile]
    private void Start()
    {
        LoadSettings();
    }

    [BurstCompile]
    public void SaveSettings()
    {
        // Graphic Options
        // save v sync value (slider)
        PlayerPrefs.SetInt("VSyncCount", vSyncCount);

        // Save v sync on/off
        int vsync;
        if (this.vsync) vsync = 1;
        else vsync = 0;
        PlayerPrefs.SetInt("VSync", vsync);
    }

    [BurstCompile]
    public void LoadSettings()
    {
        // Graphic Options
        // if this key doesn't exist then no save has ever be done (first time playing)
        if (PlayerPrefs.HasKey("VSyncCount") == false) return;

        // Load and set v sync value (slider)
        vSyncCount = PlayerPrefs.GetInt("VSyncCount");

        // Load and set v sync on/off
        int vsync;
        vsync = PlayerPrefs.GetInt("VSync");
        if (vsync == 1) this.vsync = true;
        else this.vsync = false;

        // need to be set in Start()!! to avoid missing the loadingSettings in OptionsEngine.cs
        // need to set the "loaded" values back onto the ui to reflect the values
        if (this.vsync)
        {
            QualitySettings.vSyncCount = vSyncCount;
            vsyncButtonText.text = "vsync on";
        }
        else vsyncButtonText.text = "vsync off";

        // Only set it when its not = zero. zero represent no vsync. which isn't an option in the slider.
        // 0 vsync off, 60(1)fps, 30(2)fps, 20(1)fps, 15fps(4).
        SetVSyncResult(vSyncCount);
        slider.value = vSyncCount;
    }

    [BurstCompile]
    public void SetVSyncResult(float value)
    {
        if (value == 1)
        {
            vsyncResultText.text = "60";
        }
        else
        {
            if (value == 2)
            {
                vsyncResultText.text = "30";
            }
            else
            {
                if (value == 3)
                {
                    vsyncResultText.text = "20";
                }
                else
                {
                    if (value == 4)
                    {
                        vsyncResultText.text = "15";
                    }
                }
            }
        }
    }

    [BurstCompile]
    public void ToggleVsync()
    {
        // User interface management - when its be clicked deselect it so it does not stay "lit" (visuals)
        //EventSystem.current.SetSelectedGameObject(null);

        // value should already be clamped in canvas slider settings
        vSyncCount = (int)slider.value;
        vsync = !vsync;
        if (vsync) { TurnOnVSync(vSyncCount); }
        else { TurnOffVSync(); }
    }

    // The number of VSyncs that should pass between each frame. 
    // Use 'Don't Sync' (0) to not wait for VSync. 
    // Value must be 0, 1, 2, 3, or 4.
    [BurstCompile]
    void TurnOnVSync(int count) 
    {
        //count = Mathf.Clamp(count, 0, 4);
        vsyncButtonText.text = "vsync on";

        QualitySettings.vSyncCount = count;
    }

    /*void TurnOnFreeSync() 
    {
        Application.targetFrameRate = -1;
        QualitySettings.vSyncCount = 0;
        Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
    }*/

    // Use 'Don't Sync' (0) to not wait for VSync.
    [BurstCompile]
    void TurnOffVSync()
    {
        vsyncButtonText.text = "vsync off";

        QualitySettings.vSyncCount = 0;
    }

    [BurstCompile]
    public void ToggleFullScreen()
    {
        // Toggle fullscreen
        Screen.fullScreen = !Screen.fullScreen;
    }

    [BurstCompile]
    public void EnableCanvas()
    {
        canvas.gameObject.SetActive(true);
    }

    [BurstCompile]
    public void DisableCanvas()
    {
        canvas.gameObject.SetActive(false);
    }
}
