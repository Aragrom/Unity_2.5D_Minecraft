using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Experimental.Rendering.Universal;
using Unity.Burst;

[BurstCompile]
public class Zoom : MonoBehaviour
{
    public Main main = null;

    //===============================================

    public float sliderValue = 1f;

    // ==============================================

    public Transform cameraRig = null;
    public Camera perspectiveCamera = null;
    public Camera orthographicCamera = null;
    //public PixelPerfectCamera pixelPerfectCamera = null;

    // ==============================================

    public float[] perspectiveCameraFovSizes = null;
    public int[] pixelsPerUnit = null;
    public float[] orthographicCameraSizes = null;

    [BurstCompile]
    private void OnDestroy()
    {
        // Free memory
        main = null;

        cameraRig = null;
        perspectiveCamera = null;
        orthographicCamera = null;
        //pixelPerfectCamera = null;

        perspectiveCameraFovSizes = null;
        pixelsPerUnit = null;
        orthographicCameraSizes = null;
    }

    [BurstCompile]
    private void Awake()
    {
        main = GetComponent<Main>();

        cameraRig = GameObject.Find("Camera Rig").transform;
        perspectiveCamera = cameraRig.Find("Perspective Camera").GetComponent<Camera>();
        orthographicCamera = cameraRig.Find("Orthographic Camera").GetComponent<Camera>();
        //pixelPerfectCamera = cameraRig.Find("Orthographic Camera").GetComponent<PixelPerfectCamera>();
    }

    // Start is called before the first frame update
    [BurstCompile]
    void Start()
    {
        // 1920 x 1080 hardcoded

        perspectiveCameraFovSizes = new float[]
        {
            29.5f,
            55.6f,
            93.3f
        };

        pixelsPerUnit = new int[]
        {
            64,
            32,
            16,
        };

        orthographicCameraSizes = new float[]
        {
            8.4375f,
            16.875f, //17.39f old,
            33.75f   //32.07f
        };
    }

    // Update is called once per frame
    [BurstCompile]
    void Update()
    {
        if (main.inputEngine.zoom)
        {
            main.inputEngine.zoom = false;

            Adjust();
        }
    }

    [BurstCompile]
    void Adjust()
    {
        sliderValue--;

        if (sliderValue < 0) sliderValue = perspectiveCameraFovSizes.Length - 1;

        perspectiveCamera.fieldOfView = perspectiveCameraFovSizes[(int)sliderValue];
        //pixelPerfectCamera.assetsPPU = pixelsPerUnit[(int)sliderValue];
        orthographicCamera.orthographicSize = orthographicCameraSizes[(int)sliderValue];
    }
}
