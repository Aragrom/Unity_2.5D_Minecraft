using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst;
using UnityEngine;
using UnityEngine.UI;

[BurstCompile]
public struct LoadingProgressionData
{
    [Range(0, 1)]
    public float progress;                      // Complete %?
    public float secondsRemaining;
    public float savedSeconds;
    public double total;                        // Total number of heights required to be created?
    public float required;
    public double count;

    public LoadingProgressionData(bool result)
    {
        progress = 0f;                      // Complete %?
        secondsRemaining = 0f;
        savedSeconds = 9999999f;
        total = 0;                          // Total number of heights required to be created?
        required = 0f;
        count = 0;
    }
}

[BurstCompile]
public class LoadingBar : MonoBehaviour
{
    public bool expand = true;

    public string message = "";

    public string description = "";

    [Range(0.0f, 1.0f)]
    public float progress = 0.0f;   // Value between 0 and 1 to show

    public int secondsRemaining = 0;

    public float width = 300.0f;
    public float height = 30.0f;

    public bool isLoading = true;

    public GameObject canvas = null;

    public Slider sliderLoadingBar = null;
    //public Transform executeButton = null;
    public TMP_Text tmpMessage = null;
    public TMP_Text tmpDescription = null;
    public TMP_Text tmpTime = null;

    public GameObject loadingVisualizer = null;

    [BurstCompile]
    private void OnDestroy()
    {
        // Free memory

        canvas = null;
        sliderLoadingBar = null;
        tmpMessage = null;
        tmpDescription = null;
        tmpTime = null;
        loadingVisualizer = null;
    }

    /*[BurstCompile]
    public void Awake()
    {
        GameObject go = GameObject.Find("Canvas Loading Bar");

        executeButton = go.transform.Find("Button Generate World");
        sliderLoadingBar = go.transform.Find("Slider Loading Bar").GetComponent<Slider>();
        tmpMessage = go.transform.Find("Text Message").GetComponent<TMP_Text>();
        tmpDescription = go.transform.Find("Text Description").GetComponent<TMP_Text>();
        tmpTime = go.transform.Find("Text Time").GetComponent<TMP_Text>();

        sliderLoadingBar.transform.gameObject.SetActive(false);
    }*/

    [BurstCompile]
    public void Activate()
    {
        this.enabled = true;

        canvas.SetActive(true);

        isLoading = true;

        //executeButton.gameObject.SetActive(false);
        sliderLoadingBar.transform.gameObject.SetActive(true);
        sliderLoadingBar.enabled = true;
        tmpMessage.enabled = true;
        tmpDescription.enabled = true;
        tmpTime.enabled = true;

        loadingVisualizer.SetActive(true);
    }

    [BurstCompile]
    public void Deactivate()
    {
        canvas.SetActive(true);

        isLoading = false;

        //sliderLoadingBar.enabled = false;
        sliderLoadingBar.transform.gameObject.SetActive(false);
        sliderLoadingBar.enabled = false;
        tmpMessage.enabled = false;
        tmpDescription.enabled = false;
        tmpTime.enabled = false;

        loadingVisualizer.SetActive(false);

        this.enabled = false;
    }

    [BurstCompile]
    public void Update()
    {
        if (isLoading)
        {
            ApplyUpdateToUI();
        }
    }

    [BurstCompile]
    public void ApplyUpdateToUI()
    {
        tmpMessage.text = message;
        tmpDescription.text = description;
        tmpTime.text = TimeSpan.FromSeconds(secondsRemaining).ToString();

        sliderLoadingBar.value = progress;
    }

    /*[BurstCompile]
    private void OnGUI()
    {
        if (expand)
        {
            GUI.Label(new Rect(Screen.width / 2 - width / 2, Screen.height / 2 - height / 2 - 20, width, height), message);
            GUI.DrawTexture(new Rect(Screen.width / 2 - width / 2, Screen.height / 2 - height / 2, width, height), backgroundTexture);
            GUI.DrawTexture(new Rect(Screen.width / 2 - width / 2, Screen.height / 2 - height / 2, width * progress, height), loadingTexture);
            GUI.Label(new Rect(Screen.width / 2 - width / 2, Screen.height / 2 - height / 2 + 30, width * 5, height), description);
            GUI.Label(new Rect(Screen.width / 2 - width / 2, Screen.height / 2 - height / 2 + 45, width * 5, height), TimeSpan.FromSeconds(secondsRemaining).ToString());
        }
        else
        {
            GUI.DrawTexture(new Rect(10, Screen.height - 40, 150, 10), backgroundTexture);
            GUI.DrawTexture(new Rect(10, Screen.height - 40, 150 * progress, 10), loadingTexture);
        }
    }*/
}
