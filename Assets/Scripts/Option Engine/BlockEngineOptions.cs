using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst;
using UnityEngine;
using UnityEngine.UI;

[BurstCompile]
public class BlockEngineOptions : MonoBehaviour
{
    public BlockEngine blockEngine = null;
    public TreadmillEngine treadmillEngine = null;

    public GameObject firstSelected = null; // ui

    public TMP_Text renderDistanceResultText = null;
    public TMP_Text filterModeResultText = null;
    public TMP_Text mipMapResultText = null;
    public TMP_Text anisoLevelResultText = null;

    public Slider renderDistanceSlider = null;
    public Slider filterModeSlider = null;
    public Slider mipMapBiasSlider = null;
    public Slider anisoLevelSlider = null;

    public GameObject canvas = null;

    public Texture subChunkTexture = null;

    public int MAX_SIZE = 32;
    public int MIN_SIZE = 16;
    public int size = 32;
    public float newSize = 32;

    [BurstCompile]
    public void OnApplicationQuit()
    {
        SaveSettings();
    }

    [BurstCompile]
    public void OnDestroy()
    {
        SaveSettings();

        blockEngine = null;
        treadmillEngine = null;

        firstSelected = null; // ui

        renderDistanceResultText = null;
        filterModeResultText = null;
        mipMapResultText = null;
        anisoLevelResultText = null;

        renderDistanceSlider = null;
        filterModeSlider = null;
        mipMapBiasSlider = null;
        anisoLevelSlider = null;

        canvas = null;

        subChunkTexture = null;
    }

    [BurstCompile]
    private void Awake()
    {
        blockEngine = GameObject.Find("Block Engine").GetComponent<BlockEngine>();
        treadmillEngine = GameObject.Find("Treadmill Engine").GetComponent<TreadmillEngine>();
    }

    // Start is called before the first frame update
    [BurstCompile]
    void Start()
    {
        LoadSettings();
    }

    [BurstCompile]
    public void SaveSettings()
    {
        // Block Options
        // save chunk render size.
        PlayerPrefs.SetInt("ChunkRenderSize", size);

        PlayerPrefs.SetFloat("FilterMode", filterModeSlider.value);
        PlayerPrefs.SetFloat("MipMapBias", mipMapBiasSlider.value);
        PlayerPrefs.SetFloat("AnisoLevel", anisoLevelSlider.value);
    }

    [BurstCompile]
    public void LoadSettings()
    {
        // if this key doesn't exist then no save has ever be done (first time playing)
        if (PlayerPrefs.HasKey("ChunkRenderSize") == false) return;

        // Block Options
        // Load and set chunk render size.
        newSize = PlayerPrefs.GetInt("ChunkRenderSize");
        renderDistanceSlider.value = newSize / 2;
        renderDistanceResultText.text = newSize.ToString();

        //SetNewSize(newSize);
        SetRenderDistance();

        filterModeSlider.value = PlayerPrefs.GetFloat("FilterMode");
        mipMapBiasSlider.value = PlayerPrefs.GetFloat("MipMapBias");
        anisoLevelSlider.value = PlayerPrefs.GetFloat("AnisoLevel");

        // Set UI to reflect loaded settings
        // Have to set result back from loading
        SetFilterMode(filterModeSlider.value);
        SetMipMapBias(mipMapBiasSlider.value);
        SetAnisoLevel(anisoLevelSlider.value);
    }

    [BurstCompile]
    public void SetNewSize(float size)
    {
        if (blockEngine.blockPool.needNewChunk.Count != 0
            || blockEngine.blockPool.newChunkThatNeedTerrain.Count != 0)
        {
            return;
        }

        newSize = (int)(size * 2);

        renderDistanceResultText.text = newSize.ToString();
    }

    [BurstCompile]
    public void SetRenderDistance()
    {
        if (size == newSize) { return; }

        // This should only be called when block engine is not actively generating/moving.
        if (blockEngine.newBlockQueue.needsSetMesh.Count != 0
            || blockEngine.newBlockQueue.states.Count != 0
            || blockEngine.blockPool.needNewChunk.Count != 0
            || blockEngine.blockPool.newChunkThatNeedTerrain.Count != 0)
        //|| modifiedBlockQueue.subChunksThatNeedsMeshDataAssigned.Keys.Count != 0
        //|| modifiedBlockQueue.subChunksThatNeedsNewMeshDataGenerated.Keys.Count != 0)
        { return; }

        bool isBigger = false;      // else wil stay false so does not need set. (isBigger already false)

        // new size is bigger
        if (size < newSize) { isBigger = true; }

        int differenceBetweenOldSizeAndNewSize;
        if (isBigger) { differenceBetweenOldSizeAndNewSize = (((int)newSize) - size) / 2; }   // /2 as slider range 8-16 which gets multiplied by 2 to reflect choices 16*32 and go up and down by 2. 16 > 18 > 20... 16 > 14 > 12
        else { differenceBetweenOldSizeAndNewSize = (size - ((int)newSize)) / 2; }

        if (isBigger)
        {
            // Update chunk pool objects and size (Create new chunk if larger. Destroy chunks if smaller)
            blockEngine.blockPool.Resize((int)newSize, isBigger, differenceBetweenOldSizeAndNewSize);

            if (blockEngine.main.optionEngine.inGame == false)
            {// Configure treadmill to move correctly
                treadmillEngine.Resize((int)newSize, isBigger, differenceBetweenOldSizeAndNewSize);
            }
        }
        else
        {
            // Update chunk pool objects and size (Create new chunk if larger. Destroy chunks if smaller)
            blockEngine.blockPool.Resize((int)newSize, isBigger, differenceBetweenOldSizeAndNewSize);

            // Configure treadmill to move correctly
            treadmillEngine.Resize((int)newSize, isBigger, differenceBetweenOldSizeAndNewSize);
        }

        size = (int)newSize;
    }

    [BurstCompile]
    public void SetFilterMode(float sliderValue)
    {
        FilterMode filterMode = FilterMode.Point;

        if (sliderValue == 0)
        {
            filterMode = FilterMode.Point;
            filterModeResultText.text = "Point";
        }
        else
        {
            if (sliderValue == 1)
            {
                filterMode = FilterMode.Bilinear;
                filterModeResultText.text = "Bilinear";
            }
            else
            {
                if (sliderValue == 2)
                {
                    filterMode = FilterMode.Trilinear;
                    filterModeResultText.text = "Trilinear";
                }
            }
        }

        subChunkTexture.filterMode = filterMode;
    }

    [BurstCompile]
    public void SetMipMapBias(float value)
    {
        if (value < 0.1f && value > -0.1f) { mipMapResultText.text = "0"; }
        else { mipMapResultText.text = value.ToString(); }

        subChunkTexture.mipMapBias = value;
    }

    [BurstCompile]
    public void SetAnisoLevel(float value)
    {
        if (value > 0.05f) { anisoLevelResultText.text = value.ToString(); }
        else { anisoLevelResultText.text = value.ToString(); }

        subChunkTexture.anisoLevel = (int)value;
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
